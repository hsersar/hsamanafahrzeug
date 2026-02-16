#!/bin/bash
# Hetzner Cloud Server Setup Script for Ubuntu 22.04 LTS
# This script sets up a production-ready server with Docker, security hardening, and monitoring

set -e

# Color output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

log_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

log_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if running as root
if [[ $EUID -ne 0 ]]; then
   log_error "This script must be run as root (use sudo)"
   exit 1
fi

log_info "Starting Hetzner Cloud Server Setup for HsaManageFahrzeug..."

# 1. Update system packages
log_info "Updating system packages..."
apt-get update
apt-get upgrade -y
apt-get autoremove -y

# 2. Install essential packages
log_info "Installing essential packages..."
apt-get install -y \
    apt-transport-https \
    ca-certificates \
    curl \
    gnupg \
    lsb-release \
    software-properties-common \
    ufw \
    fail2ban \
    unattended-upgrades \
    git \
    htop \
    vim \
    wget \
    net-tools

# 3. Configure automatic security updates
log_info "Configuring automatic security updates..."
cat > /etc/apt/apt.conf.d/50unattended-upgrades <<EOF
Unattended-Upgrade::Allowed-Origins {
    "\${distro_id}:\${distro_codename}-security";
    "\${distro_id}ESMApps:\${distro_codename}-apps-security";
    "\${distro_id}ESM:\${distro_codename}-infra-security";
};
Unattended-Upgrade::AutoFixInterruptedDpkg "true";
Unattended-Upgrade::MinimalSteps "true";
Unattended-Upgrade::Remove-Unused-Kernel-Packages "true";
Unattended-Upgrade::Remove-Unused-Dependencies "true";
Unattended-Upgrade::Automatic-Reboot "false";
EOF

cat > /etc/apt/apt.conf.d/20auto-upgrades <<EOF
APT::Periodic::Update-Package-Lists "1";
APT::Periodic::Download-Upgradeable-Packages "1";
APT::Periodic::AutocleanInterval "7";
APT::Periodic::Unattended-Upgrade "1";
EOF

# 4. Install Docker
log_info "Installing Docker..."
# Remove old versions
apt-get remove -y docker docker-engine docker.io containerd runc || true

# Add Docker's official GPG key
mkdir -p /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | gpg --dearmor -o /etc/apt/keyrings/docker.gpg

# Set up Docker repository
echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
  $(lsb_release -cs) stable" | tee /etc/apt/sources.list.d/docker.list > /dev/null

# Install Docker Engine
apt-get update
apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

# Enable Docker service
systemctl enable docker
systemctl start docker

log_info "Docker version: $(docker --version)"
log_info "Docker Compose version: $(docker compose version)"

# 5. Create application user (non-root)
log_info "Creating application user 'hsa-app'..."
if ! id -u hsa-app > /dev/null 2>&1; then
    useradd -m -s /bin/bash hsa-app
    usermod -aG docker hsa-app
    log_info "User 'hsa-app' created and added to docker group"
else
    log_warn "User 'hsa-app' already exists"
fi

# 6. Create application directory
log_info "Creating application directory..."
mkdir -p /opt/hsamanafahrzeug
chown -R hsa-app:hsa-app /opt/hsamanafahrzeug
chmod 755 /opt/hsamanafahrzeug

# 7. Configure UFW Firewall
log_info "Configuring UFW firewall..."
ufw --force reset
ufw default deny incoming
ufw default allow outgoing

# Allow SSH (port 22)
ufw allow 22/tcp comment 'SSH'

# Allow HTTP and HTTPS
ufw allow 80/tcp comment 'HTTP'
ufw allow 443/tcp comment 'HTTPS'

# Enable UFW
ufw --force enable

log_info "UFW firewall configured and enabled"
ufw status verbose

# 8. Configure Fail2ban for SSH protection
log_info "Configuring Fail2ban..."
cat > /etc/fail2ban/jail.local <<EOF
[DEFAULT]
bantime = 3600
findtime = 600
maxretry = 5
destemail = root@localhost
sendername = Fail2Ban
action = %(action_mwl)s

[sshd]
enabled = true
port = ssh
filter = sshd
logpath = /var/log/auth.log
maxretry = 3
bantime = 7200
EOF

systemctl enable fail2ban
systemctl restart fail2ban

log_info "Fail2ban configured and started"

# 9. Configure swap space (for smaller servers)
log_info "Setting up swap space (2GB)..."
if [ ! -f /swapfile ]; then
    fallocate -l 2G /swapfile
    chmod 600 /swapfile
    mkswap /swapfile
    swapon /swapfile
    echo '/swapfile none swap sw 0 0' >> /etc/fstab
    
    # Configure swappiness (10 = swap only when necessary)
    sysctl vm.swappiness=10
    echo 'vm.swappiness=10' >> /etc/sysctl.conf
    
    log_info "Swap space created and configured"
else
    log_warn "Swap file already exists"
fi

# 10. Configure Docker daemon
log_info "Configuring Docker daemon..."
mkdir -p /etc/docker
cat > /etc/docker/daemon.json <<EOF
{
  "log-driver": "json-file",
  "log-opts": {
    "max-size": "10m",
    "max-file": "3"
  },
  "live-restore": true,
  "userland-proxy": false
}
EOF

systemctl restart docker

# 11. Create Docker network
log_info "Creating Docker networks..."
docker network create hsamanafahrzeug-backend 2>/dev/null || log_warn "Network already exists"

# 12. Set up log rotation for application
log_info "Configuring log rotation..."
cat > /etc/logrotate.d/hsamanafahrzeug <<EOF
/opt/hsamanafahrzeug/logs/*.log {
    daily
    rotate 7
    compress
    delaycompress
    notifempty
    create 0640 hsa-app hsa-app
    sharedscripts
    postrotate
        /usr/bin/docker compose -f /opt/hsamanafahrzeug/docker-compose.prod.yml restart nginx
    endscript
}
EOF

# 13. Configure sysctl for better performance
log_info "Optimizing system parameters..."
cat >> /etc/sysctl.conf <<EOF

# HsaManageFahrzeug Performance Tuning
net.core.somaxconn = 1024
net.ipv4.tcp_max_syn_backlog = 2048
net.ipv4.ip_local_port_range = 1024 65535
net.ipv4.tcp_tw_reuse = 1
net.ipv4.tcp_fin_timeout = 15
fs.file-max = 65536
EOF

sysctl -p

# 14. Set up directory structure
log_info "Creating directory structure..."
mkdir -p /opt/hsamanafahrzeug/{logs,backups,data}
chown -R hsa-app:hsa-app /opt/hsamanafahrzeug

# 15. Install Docker Compose completion (optional)
log_info "Installing Docker Compose shell completion..."
curl -L https://raw.githubusercontent.com/docker/compose/master/contrib/completion/bash/docker-compose -o /etc/bash_completion.d/docker-compose 2>/dev/null || true

# 16. Configure SSH hardening (optional but recommended)
log_info "Applying SSH hardening..."
sed -i 's/#PermitRootLogin yes/PermitRootLogin no/' /etc/ssh/sshd_config
sed -i 's/#PasswordAuthentication yes/PasswordAuthentication no/' /etc/ssh/sshd_config
sed -i 's/#PubkeyAuthentication yes/PubkeyAuthentication yes/' /etc/ssh/sshd_config
systemctl restart sshd

log_info "SSH hardening applied (root login disabled, password auth disabled)"

# 17. Display summary
log_info "============================================"
log_info "Server setup completed successfully!"
log_info "============================================"
log_info ""
log_info "Summary:"
log_info "  - Docker installed: $(docker --version)"
log_info "  - Docker Compose installed: $(docker compose version)"
log_info "  - UFW Firewall: enabled (ports 22, 80, 443 open)"
log_info "  - Fail2ban: enabled (SSH protection)"
log_info "  - Automatic security updates: enabled"
log_info "  - Swap space: 2GB configured"
log_info "  - Application user: hsa-app"
log_info "  - Application directory: /opt/hsamanafahrzeug"
log_info ""
log_info "Next steps:"
log_info "  1. Copy your application files to /opt/hsamanafahrzeug"
log_info "  2. Configure .env file with your secrets"
log_info "  3. Set up SSL certificates with Let's Encrypt"
log_info "  4. Run the deployment script (deploy.sh)"
log_info ""
log_info "Security Notes:"
log_info "  - SSH root login is disabled"
log_info "  - Password authentication is disabled (use SSH keys)"
log_info "  - Fail2ban is protecting SSH (3 failed attempts = 2h ban)"
log_info "  - Automatic security updates are enabled"
log_info ""

# Reminder
log_warn "IMPORTANT: Make sure to configure your .env file before deployment!"
log_warn "IMPORTANT: Set up SSH keys before logging out!"

log_info "Rebooting in 10 seconds to apply all changes... (Ctrl+C to cancel)"
sleep 10
reboot

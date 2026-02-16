#!/bin/bash
#
# Hetzner Server Setup Script
# Prepares Ubuntu 22.04 server for Fahrzeugzulassungs-Webapp deployment
#
# Usage: ./setup-hetzner.sh
#

set -e  # Exit on error

echo "=========================================="
echo "Hetzner Server Setup for Fahrzeugzulassung"
echo "=========================================="
echo ""

# Check if running as root
if [ "$EUID" -ne 0 ]; then 
    echo "Please run as root or with sudo"
    exit 1
fi

echo "Step 1: Updating system packages..."
apt update
apt upgrade -y

echo ""
echo "Step 2: Installing essential packages..."
apt install -y \
    apt-transport-https \
    ca-certificates \
    curl \
    gnupg \
    lsb-release \
    software-properties-common \
    git \
    wget \
    ufw \
    fail2ban \
    unattended-upgrades \
    nginx \
    certbot \
    python3-certbot-nginx

echo ""
echo "Step 3: Installing Docker..."
# Add Docker's official GPG key
install -m 0755 -d /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | gpg --dearmor -o /etc/apt/keyrings/docker.gpg
chmod a+r /etc/apt/keyrings/docker.gpg

# Add Docker repository
echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
  $(lsb_release -cs) stable" | tee /etc/apt/sources.list.d/docker.list > /dev/null

# Install Docker
apt update
apt install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

# Start and enable Docker
systemctl enable docker
systemctl start docker

echo ""
echo "Step 4: Configuring firewall (UFW)..."
# Reset firewall to default
ufw --force reset

# Set default policies
ufw default deny incoming
ufw default allow outgoing

# Allow SSH (critical!)
ufw allow ssh
ufw allow 22/tcp

# Allow HTTP and HTTPS
ufw allow http
ufw allow https
ufw allow 80/tcp
ufw allow 443/tcp

# Enable firewall
ufw --force enable

echo ""
echo "Step 5: Configuring fail2ban..."
# Create jail configuration for SSH
cat > /etc/fail2ban/jail.local <<EOF
[DEFAULT]
bantime  = 3600
findtime  = 600
maxretry = 5
destemail = root@localhost
sendername = Fail2Ban
action = %(action_mwl)s

[sshd]
enabled = true
port = ssh
logpath = %(sshd_log)s
backend = %(sshd_backend)s
maxretry = 3
bantime = 7200

[nginx-http-auth]
enabled = true
filter = nginx-http-auth
port = http,https
logpath = /var/log/nginx/error.log

[nginx-limit-req]
enabled = true
filter = nginx-limit-req
port = http,https
logpath = /var/log/nginx/error.log
EOF

# Restart fail2ban
systemctl enable fail2ban
systemctl restart fail2ban

echo ""
echo "Step 6: Configuring automatic security updates..."
cat > /etc/apt/apt.conf.d/50unattended-upgrades <<EOF
Unattended-Upgrade::Allowed-Origins {
    "\${distro_id}:\${distro_codename}";
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

# Enable automatic updates
systemctl enable unattended-upgrades
systemctl start unattended-upgrades

echo ""
echo "Step 7: Creating application directories..."
mkdir -p /opt/hsamanafahrzeug/data/{postgres,uploads,logs,backups}
chmod -R 755 /opt/hsamanafahrzeug

echo ""
echo "Step 8: Setting up log rotation..."
cat > /etc/logrotate.d/docker-containers <<EOF
/opt/hsamanafahrzeug/data/logs/*.log {
    daily
    rotate 7
    compress
    delaycompress
    missingok
    notifempty
    create 0644 root root
}
EOF

echo ""
echo "Step 9: Optimizing system settings..."
# Increase file descriptors
cat >> /etc/security/limits.conf <<EOF

# Increased limits for Docker containers
* soft nofile 65536
* hard nofile 65536
root soft nofile 65536
root hard nofile 65536
EOF

# Kernel parameters for better Docker performance
cat >> /etc/sysctl.conf <<EOF

# Docker and network optimizations
net.ipv4.ip_forward=1
net.bridge.bridge-nf-call-iptables=1
net.bridge.bridge-nf-call-ip6tables=1
vm.max_map_count=262144
EOF

sysctl -p

echo ""
echo "Step 10: Setting up system monitoring..."
# Install monitoring tools
apt install -y htop iotop nethogs

echo ""
echo "=========================================="
echo "Setup Complete!"
echo "=========================================="
echo ""
echo "Next steps:"
echo "1. Clone your repository: git clone https://github.com/hsersar/hsamanafahrzeug.git /opt/hsamanafahrzeug/repo"
echo "2. Configure environment: cd /opt/hsamanafahrzeug/repo && cp .env.example .env.production"
echo "3. Edit .env.production with your settings"
echo "4. Deploy application: docker-compose -f docker-compose.prod.yml up -d"
echo "5. Setup SSL: certbot --nginx -d yourdomain.de"
echo ""
echo "Installed services:"
echo "  - Docker: $(docker --version)"
echo "  - Docker Compose: $(docker compose version)"
echo "  - Nginx: $(nginx -v 2>&1)"
echo "  - UFW: $(ufw status | head -1)"
echo "  - Fail2ban: $(fail2ban-client version)"
echo ""
echo "Security status:"
echo "  - Firewall: Enabled (ports 22, 80, 443)"
echo "  - Fail2ban: Active"
echo "  - Automatic updates: Enabled"
echo ""
echo "System ready for deployment!"

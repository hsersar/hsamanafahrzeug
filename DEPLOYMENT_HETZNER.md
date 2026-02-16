# Hetzner Cloud Deployment Guide - Fahrzeugzulassungs-Webapp

Complete guide for deploying the vehicle registration application to Hetzner Cloud.

## Überblick (Overview)

This guide walks you through deploying the Fahrzeugzulassungs-Webapp to Hetzner Cloud with:
- Docker containerization
- PostgreSQL database
- Nginx reverse proxy with SSL/TLS
- Automated backups
- Production-ready security

## Voraussetzungen (Prerequisites)

### 1. Hetzner Cloud Account
- Sign up at https://console.hetzner.cloud
- Add payment method
- Create a new project: "Fahrzeugzulassung"

### 2. Domain Name
- Register a domain (e.g., fahrzeugzulassung.de)
- Access to DNS settings

### 3. Local Tools
- SSH client
- Git

## Schritt 1: Server erstellen (Create Server)

### Via Hetzner Cloud Console

1. **Login** to Hetzner Cloud Console: https://console.hetzner.cloud
2. **Create New Server**:
   - **Location**: Nuremberg, Germany (for GDPR compliance)
   - **Image**: Ubuntu 22.04 LTS
   - **Type**: CPX21 (3 vCPU, 4 GB RAM, 80 GB SSD) - Recommended for production
   - **Networking**: 
     - Enable Public IPv4
     - Enable Private Networks (optional)
   - **SSH Key**: Add your SSH public key
   - **Name**: fahrzeugzulassung-prod
   - **Label**: production

3. **Create Server** - Wait ~60 seconds for provisioning

### Server Sizing Recommendations

| Environment | Server Type | vCPU | RAM | Storage | Monthly Cost* |
|-------------|-------------|------|-----|---------|---------------|
| Development | CX11 | 1 | 2 GB | 20 GB | ~€3.79 |
| Production (Small) | CPX21 | 3 | 4 GB | 80 GB | ~€11.39 |
| Production (Medium) | CPX31 | 4 | 8 GB | 160 GB | ~€22.79 |
| Production (Large) | CPX41 | 8 | 16 GB | 240 GB | ~€45.59 |

*Prices as of 2026, subject to change

## Schritt 2: DNS konfigurieren (Configure DNS)

Point your domain to the Hetzner server:

```
Type: A
Name: @
Value: <SERVER_IP_ADDRESS>
TTL: 3600

Type: A
Name: api
Value: <SERVER_IP_ADDRESS>
TTL: 3600

Type: A
Name: www
Value: <SERVER_IP_ADDRESS>
TTL: 3600
```

Wait 5-30 minutes for DNS propagation.

## Schritt 3: Server vorbereiten (Prepare Server)

### Connect to Server

```bash
ssh root@<SERVER_IP_ADDRESS>
```

### Run Setup Script

```bash
# Download setup script
wget https://raw.githubusercontent.com/hsersar/hsamanafahrzeug/main/scripts/setup-hetzner.sh

# Make executable
chmod +x setup-hetzner.sh

# Run setup
./setup-hetzner.sh
```

The script will:
- Update system packages
- Install Docker & Docker Compose
- Configure firewall (UFW)
- Install fail2ban for security
- Set up automatic security updates
- Configure logging

### Manual Setup (Alternative)

If you prefer manual setup:

```bash
# Update system
apt update && apt upgrade -y

# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sh get-docker.sh

# Install Docker Compose
apt install docker-compose-plugin -y

# Install additional tools
apt install -y git ufw fail2ban certbot python3-certbot-nginx nginx

# Configure firewall
ufw default deny incoming
ufw default allow outgoing
ufw allow ssh
ufw allow http
ufw allow https
ufw enable

# Start services
systemctl enable docker
systemctl start docker
systemctl enable fail2ban
systemctl start fail2ban
```

## Schritt 4: Applikation deployen (Deploy Application)

### Clone Repository

```bash
cd /opt
git clone https://github.com/hsersar/hsamanafahrzeug.git
cd hsamanafahrzeug
```

### Configure Environment

```bash
# Copy environment template
cp .env.example .env.production

# Edit production environment
nano .env.production
```

Set the following in `.env.production`:

```bash
# PostgreSQL
POSTGRES_PASSWORD=<GENERATE_STRONG_PASSWORD>

# JWT (Generate with: openssl rand -base64 32)
JWT_SECRET=<GENERATE_STRONG_SECRET>

# Domain
DOMAIN=fahrzeugzulassung.de
EMAIL=admin@fahrzeugzulassung.de

# Environment
ASPNETCORE_ENVIRONMENT=Production
```

### Deploy with Docker Compose

```bash
# Copy production compose file
cp docker-compose.prod.yml docker-compose.yml

# Start services
docker-compose up -d

# Check status
docker-compose ps

# View logs
docker-compose logs -f
```

## Schritt 5: Nginx & SSL einrichten (Setup Nginx & SSL)

### Install SSL Certificate

```bash
# Stop nginx if running
systemctl stop nginx

# Get Let's Encrypt certificate
certbot certonly --standalone -d fahrzeugzulassung.de -d api.fahrzeugzulassung.de -d www.fahrzeugzulassung.de --email admin@fahrzeugzulassung.de --agree-tos --non-interactive

# Copy nginx configuration
cp /opt/hsamanafahrzeug/nginx/fahrzeugzulassung.conf /etc/nginx/sites-available/
ln -s /etc/nginx/sites-available/fahrzeugzulassung.conf /etc/nginx/sites-enabled/

# Test configuration
nginx -t

# Start nginx
systemctl enable nginx
systemctl start nginx

# Setup automatic certificate renewal
certbot renew --dry-run
```

### Configure Nginx (Manual)

If nginx config not provided, create `/etc/nginx/sites-available/fahrzeugzulassung.conf`:

```nginx
# Redirect HTTP to HTTPS
server {
    listen 80;
    listen [::]:80;
    server_name fahrzeugzulassung.de www.fahrzeugzulassung.de api.fahrzeugzulassung.de;
    return 301 https://$server_name$request_uri;
}

# HTTPS configuration
server {
    listen 443 ssl http2;
    listen [::]:443 ssl http2;
    server_name fahrzeugzulassung.de www.fahrzeugzulassung.de;

    # SSL certificates
    ssl_certificate /etc/letsencrypt/live/fahrzeugzulassung.de/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/fahrzeugzulassung.de/privkey.pem;
    
    # SSL configuration
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;
    ssl_prefer_server_ciphers on;
    
    # Security headers
    add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
    add_header X-Frame-Options "DENY" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;

    # Proxy to backend
    location / {
        proxy_pass http://localhost:5001;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        
        # Timeouts
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
    }
}

# API subdomain
server {
    listen 443 ssl http2;
    listen [::]:443 ssl http2;
    server_name api.fahrzeugzulassung.de;

    # SSL certificates
    ssl_certificate /etc/letsencrypt/live/fahrzeugzulassung.de/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/fahrzeugzulassung.de/privkey.pem;
    
    # SSL configuration
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;
    ssl_prefer_server_ciphers on;

    # Proxy to backend
    location / {
        proxy_pass http://localhost:5001;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

## Schritt 6: Testen (Test)

### Check Application Status

```bash
# Check Docker containers
docker-compose ps

# Check application logs
docker-compose logs backend

# Check database
docker-compose exec postgres psql -U fahrzeug_user -d FahrzeugZulassung -c "\dt"
```

### Access Application

1. **Swagger UI**: https://fahrzeugzulassung.de/swagger
2. **Health Check**: https://fahrzeugzulassung.de/health
3. **API**: https://api.fahrzeugzulassung.de/api

### Login

- **Email**: admin@fahrzeugzulassung.de
- **Password**: Admin@123456789 (CHANGE IMMEDIATELY!)

## Schritt 7: Sicherheit & Wartung (Security & Maintenance)

### Change Default Password

1. Login to Swagger UI
2. Call POST `/api/auth/login` with default credentials
3. Create new SuperAdmin user
4. Delete or disable default admin account

### Database Backups

```bash
# Manual backup
docker-compose exec postgres pg_dump -U fahrzeug_user FahrzeugZulassung > backup-$(date +%Y%m%d).sql

# Automated daily backups (add to crontab)
0 2 * * * cd /opt/hsamanafahrzeug && ./scripts/backup-db.sh

# Restore from backup
docker-compose exec -T postgres psql -U fahrzeug_user FahrzeugZulassung < backup-20260216.sql
```

### Monitoring

```bash
# View application logs
docker-compose logs -f backend

# Monitor resource usage
docker stats

# Check disk space
df -h

# Monitor PostgreSQL
docker-compose exec postgres psql -U fahrzeug_user -d FahrzeugZulassung -c "SELECT * FROM pg_stat_activity;"
```

### Updates

```bash
cd /opt/hsamanafahrzeug

# Pull latest changes
git pull origin main

# Rebuild containers
docker-compose down
docker-compose build --no-cache
docker-compose up -d

# Check logs
docker-compose logs -f
```

### Firewall Rules

Current UFW rules:

```bash
# View current rules
ufw status verbose

# Required rules:
22/tcp    ALLOW IN    # SSH
80/tcp    ALLOW IN    # HTTP
443/tcp   ALLOW IN    # HTTPS
```

### Fail2ban Protection

```bash
# Check fail2ban status
fail2ban-client status

# Check SSH jail
fail2ban-client status sshd

# Unban an IP
fail2ban-client set sshd unbanip <IP_ADDRESS>
```

## Troubleshooting

### Application won't start

```bash
# Check container status
docker-compose ps

# Check logs for errors
docker-compose logs backend
docker-compose logs postgres

# Restart services
docker-compose restart
```

### Database connection errors

```bash
# Check PostgreSQL is running
docker-compose ps postgres

# Test database connection
docker-compose exec postgres psql -U fahrzeug_user -d FahrzeugZulassung -c "SELECT 1;"

# Check connection string in .env.production
```

### SSL certificate issues

```bash
# Renew certificate manually
certbot renew --force-renewal

# Check certificate expiry
certbot certificates

# Test nginx configuration
nginx -t
```

### Out of disk space

```bash
# Check disk usage
df -h

# Clean Docker images and volumes
docker system prune -a --volumes

# Remove old logs
journalctl --vacuum-time=7d
```

## Performance Optimization

### Database Tuning

Edit `/etc/postgresql/postgresql.conf` in the postgres container:

```ini
# Connections
max_connections = 100

# Memory
shared_buffers = 1GB
effective_cache_size = 3GB
work_mem = 16MB

# Checkpoints
checkpoint_completion_target = 0.9
wal_buffers = 16MB
```

### Application Scaling

For high traffic, consider:

1. **Horizontal scaling**: Run multiple backend containers
2. **Load balancer**: Use Hetzner Load Balancer
3. **Database replication**: PostgreSQL read replicas
4. **Caching**: Redis for session storage
5. **CDN**: Cloudflare for static assets

## Kosten (Costs)

### Estimated Monthly Costs

**Minimum Setup (Development/Testing)**:
- Server (CX11): €3.79
- **Total**: ~€4/month

**Recommended Production Setup**:
- Server (CPX21): €11.39
- Backup Volume (20GB): €2.00
- **Total**: ~€13/month

**Enterprise Setup**:
- Server (CPX31): €22.79
- Load Balancer: €5.39
- Backup Volume (50GB): €5.00
- **Total**: ~€33/month

Plus:
- Domain registration: ~€10-15/year
- Bandwidth: Included (20 TB outgoing per month)

## Support & Hilfe (Support & Help)

### Hetzner Support
- Community: https://community.hetzner.com
- Docs: https://docs.hetzner.com
- Support: https://console.hetzner.cloud (logged in)

### Application Support
- GitHub Issues: https://github.com/hsersar/hsamanafahrzeug/issues
- Documentation: See README.md

## Checkliste für Produktion (Production Checklist)

- [ ] Server created and SSH access working
- [ ] DNS configured and propagated
- [ ] Firewall configured (UFW)
- [ ] Docker and Docker Compose installed
- [ ] Application deployed with docker-compose
- [ ] SSL certificate installed (Let's Encrypt)
- [ ] Nginx configured and running
- [ ] Health check returns 200: https://yourdomain.de/health
- [ ] Swagger UI accessible: https://yourdomain.de/swagger
- [ ] Default admin password changed
- [ ] Database backups configured
- [ ] Monitoring setup (logs, disk space)
- [ ] Fail2ban enabled
- [ ] Automatic security updates enabled

## Nächste Schritte (Next Steps)

1. **Setup monitoring**: Configure Prometheus/Grafana
2. **Add frontend**: Deploy React frontend
3. **Email notifications**: Configure SMTP
4. **Implement real iKFZ integration**
5. **Add integration tests**
6. **Setup staging environment**

## License

This deployment guide is part of the Fahrzeugzulassungs-Webapp project.

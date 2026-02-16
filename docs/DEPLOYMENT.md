# HsaManageFahrzeug - Production Deployment Guide

Complete guide for deploying the vehicle registration application to Hetzner Cloud with Docker.

## Table of Contents

1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Server Recommendations](#server-recommendations)
4. [Initial Server Setup](#initial-server-setup)
5. [SSL Certificate Setup](#ssl-certificate-setup)
6. [Application Deployment](#application-deployment)
7. [Monitoring Setup](#monitoring-setup)
8. [Backup Configuration](#backup-configuration)
9. [Security Checklist](#security-checklist)
10. [Troubleshooting](#troubleshooting)
11. [Cost Overview](#cost-overview)

---

## Overview

This deployment stack includes:
- **Backend**: ASP.NET Core 8 API (Docker container)
- **Frontend**: React SPA served via Nginx (Docker container)
- **Database**: PostgreSQL 16 with persistent storage
- **Cache**: Redis for session and dashboard caching
- **Reverse Proxy**: Nginx with SSL/TLS (Let's Encrypt)
- **Monitoring**: Prometheus + Grafana + cAdvisor (optional)
- **Backup**: Automated PostgreSQL backups

---

## Prerequisites

### 1. Domain Setup
- A domain name (e.g., `app.example.de`)
- DNS A record pointing to your Hetzner server IP

### 2. Local Requirements
- Git installed
- SSH key pair for server access
- GitHub account (for CI/CD)

### 3. Hetzner Cloud Account
- Create account at [hetzner.com](https://www.hetzner.com/cloud)
- Add SSH key to your Hetzner project

---

## Server Recommendations

### Recommended Server Sizes

| Use Case | Server Type | vCPU | RAM | Storage | Price/Month | Users |
|----------|-------------|------|-----|---------|-------------|-------|
| **Development/Testing** | CX21 | 2 | 4 GB | 40 GB | ~€5.83 | 1-10 |
| **Small Production** | CPX21 | 3 | 4 GB | 80 GB | ~€8.21 | 10-50 |
| **Production** | CPX31 | 4 | 8 GB | 160 GB | ~€13.14 | 50-200 |
| **High Traffic** | CPX41 | 8 | 16 GB | 240 GB | ~€24.27 | 200-1000 |

**Recommendation for MVP**: CPX31 (4 vCPU, 8GB RAM) - €13.14/month

### Operating System
- **Ubuntu 22.04 LTS** (recommended and tested)
- **Debian 11** (also supported)

---

## Initial Server Setup

### Step 1: Create Hetzner Cloud Server

1. Log in to [Hetzner Cloud Console](https://console.hetzner.cloud)
2. Create new project: "HsaManageFahrzeug"
3. Add your SSH key to the project
4. Create server:
   - **Location**: Nuremberg (nbg1) or Falkenstein (fsn1)
   - **Image**: Ubuntu 22.04 LTS
   - **Type**: CPX31 (or your choice)
   - **SSH Key**: Select your key
   - **Firewall**: Create firewall rules (HTTP, HTTPS, SSH)

5. Note the server IP address

### Step 2: Configure DNS

Point your domain to the server:
```
Type: A
Name: app (or @)
Value: <YOUR_SERVER_IP>
TTL: 300
```

Wait for DNS propagation (can take up to 24 hours, usually ~5-10 minutes).

### Step 3: Initial Server Connection

```bash
ssh root@<YOUR_SERVER_IP>
```

### Step 4: Run Server Setup Script

```bash
# Download setup script
curl -o hetzner-setup.sh https://raw.githubusercontent.com/hsersar/hsamanafahrzeug/main/deploy/hetzner-setup.sh

# Make executable
chmod +x hetzner-setup.sh

# Run setup (as root)
sudo ./hetzner-setup.sh
```

This script will:
- ✅ Update all system packages
- ✅ Install Docker and Docker Compose
- ✅ Configure UFW firewall (ports 22, 80, 443)
- ✅ Set up Fail2ban for SSH protection
- ✅ Enable automatic security updates
- ✅ Create application user `hsa-app`
- ✅ Configure swap space (2GB)
- ✅ Optimize system parameters
- ✅ Harden SSH configuration

**⚠️ IMPORTANT**: The server will reboot after setup. Wait 30 seconds and reconnect.

---

## SSL Certificate Setup

### Option 1: Automatic SSL with Certbot (Recommended)

1. SSH into your server:
```bash
ssh hsa-app@<YOUR_SERVER_IP>
cd /opt/hsamanafahrzeug
```

2. Create initial Nginx configuration for HTTP (certificate challenge):
```bash
# Temporarily use HTTP-only Nginx config
docker compose -f docker-compose.prod.yml up -d nginx
```

3. Obtain SSL certificate:
```bash
docker compose -f docker-compose.prod.yml run --rm certbot certonly \
  --webroot \
  --webroot-path=/var/www/certbot \
  --email your-email@example.de \
  --agree-tos \
  --no-eff-email \
  -d app.example.de
```

4. Restart Nginx with SSL configuration:
```bash
docker compose -f docker-compose.prod.yml restart nginx
```

### Option 2: Wildcard Certificate (Advanced)

For wildcard domains (*.example.de), use DNS challenge:
```bash
docker compose -f docker-compose.prod.yml run --rm certbot certonly \
  --manual \
  --preferred-challenges dns \
  --email your-email@example.de \
  --agree-tos \
  -d *.example.de
```

Follow the instructions to add TXT record to your DNS.

### SSL Certificate Renewal

Certificates auto-renew via the Certbot container. To manually renew:
```bash
bash /opt/hsamanafahrzeug/nginx/ssl-renew.sh
```

---

## Application Deployment

### Step 1: Clone Repository

```bash
ssh hsa-app@<YOUR_SERVER_IP>
cd /opt/hsamanafahrzeug

# Clone repository
git clone https://github.com/hsersar/hsamanafahrzeug.git .
```

### Step 2: Configure Environment Variables

```bash
# Copy example environment file
cp deploy/.env.example .env

# Edit with your values
nano .env
```

**Required Configuration**:

```bash
# Application
DOMAIN_NAME=app.example.de
ASPNETCORE_ENVIRONMENT=Production

# Database
POSTGRES_USER=hsamanafahrzeug
POSTGRES_PASSWORD=<STRONG_PASSWORD>  # Generate with: openssl rand -base64 32
POSTGRES_DB=hsamanafahrzeug

# Redis
REDIS_PASSWORD=<STRONG_PASSWORD>  # Generate with: openssl rand -base64 32

# JWT (Generate 256-bit key)
JWT_KEY=<BASE64_KEY>  # Generate with: openssl rand -base64 64
JWT_ISSUER=https://app.example.de
JWT_AUDIENCE=https://app.example.de

# SSL
LETSENCRYPT_EMAIL=admin@example.de

# CORS
ALLOWED_ORIGINS=https://app.example.de

# IKFZ API (if applicable)
IKFZ_API_URL=https://api.ikfz.de/v1
IKFZ_API_KEY=<YOUR_API_KEY>
```

**Security Note**: Never commit `.env` file to version control!

### Step 3: Build and Deploy

```bash
# Run deployment script
bash deploy/deploy.sh
```

This will:
1. Create database backup
2. Pull/build Docker images
3. Start PostgreSQL and Redis
4. Run database migrations
5. Start backend and frontend
6. Start Nginx reverse proxy
7. Perform health checks

### Step 4: Verify Deployment

```bash
# Check running containers
docker compose -f docker-compose.prod.yml ps

# Check logs
docker compose -f docker-compose.prod.yml logs -f

# Test application
curl https://app.example.de/health
```

Open browser: `https://app.example.de`

---

## Monitoring Setup

### Step 1: Deploy Monitoring Stack

```bash
cd /opt/hsamanafahrzeug/deploy/monitoring

# Start monitoring services
docker compose -f docker-compose.monitoring.yml up -d
```

### Step 2: Access Grafana

1. Open browser: `http://<SERVER_IP>:3000`
2. Login:
   - Username: `admin`
   - Password: `admin` (change immediately!)

### Step 3: Add Dashboards

Import pre-built dashboards:
- **Docker Container Metrics**: Dashboard ID `10619`
- **PostgreSQL**: Dashboard ID `9628`
- **Redis**: Dashboard ID `11835`
- **Node Exporter**: Dashboard ID `1860`

---

## Backup Configuration

### Manual Backup

```bash
# Run backup script
bash /opt/hsamanafahrzeug/deploy/backup.sh
```

Backups are stored in: `/opt/hsamanafahrzeug/backups/`

### Automated Daily Backups

Set up cron job:
```bash
# Edit crontab for hsa-app user
crontab -e

# Add daily backup at 2:00 AM
0 2 * * * /opt/hsamanafahrzeug/deploy/backup.sh >> /opt/hsamanafahrzeug/logs/backup.log 2>&1
```

### Restore from Backup

```bash
# List backups
ls -lh /opt/hsamanafahrzeug/backups/

# Restore database
gunzip -c /opt/hsamanafahrzeug/backups/db_backup_YYYYMMDD_HHMMSS.sql.gz | \
  docker exec -i hsamanafahrzeug-postgres psql -U hsamanafahrzeug -d hsamanafahrzeug
```

### Offsite Backup to Hetzner Object Storage

Configure S3-compatible backup:
```bash
# Install s3cmd
sudo apt-get install s3cmd

# Configure in .env
HETZNER_S3_BUCKET=hsamanafahrzeug-backups
HETZNER_S3_ACCESS_KEY=<YOUR_KEY>
HETZNER_S3_SECRET_KEY=<YOUR_SECRET>
HETZNER_S3_ENDPOINT=fsn1.your-objectstorage.com
```

---

## Security Checklist

### ✅ Server Security

- [ ] SSH root login disabled
- [ ] SSH password authentication disabled
- [ ] SSH key-based authentication only
- [ ] UFW firewall enabled (ports 22, 80, 443)
- [ ] Fail2ban enabled (SSH protection)
- [ ] Automatic security updates enabled
- [ ] Swap space configured

### ✅ Application Security

- [ ] Strong passwords in `.env`
- [ ] `.env` file permissions: `chmod 600 .env`
- [ ] JWT key is 256-bit or longer
- [ ] SSL/TLS certificates valid (Let's Encrypt)
- [ ] All containers run as non-root users
- [ ] Docker socket not exposed
- [ ] PostgreSQL not accessible from internet
- [ ] Redis not accessible from internet

### ✅ Network Security

- [ ] HTTPS enforced (HTTP redirects to HTTPS)
- [ ] HSTS header enabled
- [ ] Security headers configured (CSP, X-Frame-Options, etc.)
- [ ] Rate limiting enabled
- [ ] CORS properly configured

### ✅ Monitoring & Logging

- [ ] Centralized logging configured
- [ ] Log rotation enabled
- [ ] Monitoring dashboards accessible
- [ ] Backup system operational
- [ ] Health checks working

---

## Troubleshooting

### Container Won't Start

```bash
# Check logs
docker compose -f docker-compose.prod.yml logs <service-name>

# Check container status
docker compose -f docker-compose.prod.yml ps

# Restart specific service
docker compose -f docker-compose.prod.yml restart <service-name>
```

### Database Connection Issues

```bash
# Check PostgreSQL logs
docker compose -f docker-compose.prod.yml logs postgres

# Connect to database manually
docker exec -it hsamanafahrzeug-postgres psql -U hsamanafahrzeug -d hsamanafahrzeug

# Check connection from backend
docker exec -it hsamanafahrzeug-backend dotnet ef database update
```

### SSL Certificate Issues

```bash
# Check certificate validity
openssl s_client -connect app.example.de:443 -servername app.example.de

# Renew certificate manually
bash /opt/hsamanafahrzeug/nginx/ssl-renew.sh

# Check Certbot logs
docker compose -f docker-compose.prod.yml logs certbot
```

### High Memory Usage

```bash
# Check memory usage
free -h
docker stats

# Restart Redis (clears cache)
docker compose -f docker-compose.prod.yml restart redis
```

### Performance Issues

```bash
# Check system resources
htop

# Check Docker stats
docker stats

# Restart all services
docker compose -f docker-compose.prod.yml restart
```

---

## Cost Overview

### Hetzner Cloud Costs (Monthly)

| Component | Server Type | Price |
|-----------|-------------|-------|
| **Server** | CPX31 (4 vCPU, 8GB RAM) | €13.14 |
| **Backup** | 10GB snapshot | €2.00 |
| **Traffic** | 20TB included | €0.00 |
| **IPv4** | 1 address | €0.00 |

**Total Monthly Cost**: ~€15-20

### Additional Costs

- **Domain**: €10-20/year (depends on TLD)
- **Object Storage** (optional): €0.0045/GB/month
- **Load Balancer** (optional): €5.68/month

### Cost Optimization Tips

1. **Start Small**: CPX21 for MVP, scale up as needed
2. **Use Snapshots**: Create snapshots before major changes
3. **Enable Backups**: Only for critical periods
4. **Monitor Resources**: Scale down if underutilized
5. **Reserved Instances**: Contact Hetzner for volume discounts

---

## GitHub Actions CI/CD Setup

### Step 1: Configure GitHub Secrets

In your GitHub repository, add these secrets:

```
Settings → Secrets and variables → Actions → New repository secret
```

Required secrets:
- `HETZNER_SSH_KEY`: Your private SSH key
- `HETZNER_HOST`: Server IP address
- `HETZNER_USER`: `hsa-app`
- `DOMAIN_NAME`: `app.example.de`
- `SLACK_WEBHOOK_URL`: (optional) For deployment notifications

### Step 2: Enable GitHub Packages

The workflow automatically pushes Docker images to GitHub Container Registry.

### Step 3: Trigger Deployment

Push to `main` branch:
```bash
git push origin main
```

GitHub Actions will:
1. Run tests
2. Build Docker images
3. Push to GitHub Container Registry
4. Deploy to Hetzner server
5. Run health checks
6. Send notifications

---

## Maintenance Tasks

### Weekly Tasks
- [ ] Review application logs
- [ ] Check monitoring dashboards
- [ ] Verify backup completion

### Monthly Tasks
- [ ] Review security updates
- [ ] Test backup restoration
- [ ] Review resource usage
- [ ] Check SSL certificate expiration

### Quarterly Tasks
- [ ] Security audit
- [ ] Performance optimization
- [ ] Update dependencies
- [ ] Review and update documentation

---

## Support & Resources

### Documentation
- [Docker Documentation](https://docs.docker.com/)
- [Nginx Documentation](https://nginx.org/en/docs/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Hetzner Cloud API](https://docs.hetzner.cloud/)

### Tools
- [SSL Labs Test](https://www.ssllabs.com/ssltest/)
- [Security Headers](https://securityheaders.com/)
- [Docker Hub](https://hub.docker.com/)

### Monitoring
- [Grafana Dashboards](https://grafana.com/grafana/dashboards/)
- [Prometheus Exporters](https://prometheus.io/docs/instrumenting/exporters/)

---

## License

This deployment configuration is part of HsaManageFahrzeug project.

---

**Last Updated**: February 2026
**Version**: 1.0.0

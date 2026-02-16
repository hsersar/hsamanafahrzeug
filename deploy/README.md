# HsaManageFahrzeug Deployment Scripts

This directory contains all deployment-related scripts and configurations for production deployment on Hetzner Cloud.

## Directory Structure

```
deploy/
├── hetzner-setup.sh          # Server initialization script
├── deploy.sh                 # Application deployment script
├── backup.sh                 # Database backup script
├── init-db.sh                # PostgreSQL initialization
├── .env.example              # Environment variables template
└── monitoring/               # Monitoring stack configuration
    ├── docker-compose.monitoring.yml
    └── prometheus.yml
```

## Scripts Overview

### hetzner-setup.sh
**Purpose**: Initial server setup and hardening  
**Run Once**: Yes, on new server  
**Requires**: Root access

Sets up Docker, firewall, Fail2ban, security updates, and creates application user.

```bash
sudo bash hetzner-setup.sh
```

### deploy.sh
**Purpose**: Deploy/update application  
**Run**: Every deployment  
**Requires**: hsa-app user

Deploys application with health checks and automatic rollback on failure.

```bash
bash deploy.sh
```

### backup.sh
**Purpose**: Backup PostgreSQL database  
**Run**: Daily via cron  
**Requires**: hsa-app user

Creates compressed database backups with configurable retention.

```bash
bash backup.sh
```

### init-db.sh
**Purpose**: Initialize PostgreSQL database  
**Run**: Automatically on first database creation  
**Requires**: Docker container

Sets up extensions, timezone, and permissions.

## Environment Variables

Copy `.env.example` to `.env` and configure all required variables before deployment:

```bash
cp .env.example ../.env
nano ../.env
```

**Required Variables**:
- `DOMAIN_NAME`: Your domain (e.g., app.example.de)
- `POSTGRES_PASSWORD`: Strong database password
- `REDIS_PASSWORD`: Strong Redis password
- `JWT_KEY`: 256-bit JWT signing key
- `LETSENCRYPT_EMAIL`: Email for SSL certificates

## Monitoring

The monitoring stack includes:
- **Prometheus**: Metrics collection
- **Grafana**: Visualization dashboards
- **cAdvisor**: Container metrics
- **Node Exporter**: System metrics
- **PostgreSQL Exporter**: Database metrics
- **Redis Exporter**: Cache metrics

Deploy monitoring:
```bash
cd monitoring
docker compose -f docker-compose.monitoring.yml up -d
```

Access Grafana: `http://<server-ip>:3000` (admin/admin)

## Backup Strategy

### Local Backups
- Retention: 7 days
- Location: `/opt/hsamanafahrzeug/backups/`
- Schedule: Daily at 2:00 AM (via cron)

### Offsite Backups (Optional)
Configure Hetzner Object Storage in `.env`:
```bash
HETZNER_S3_BUCKET=hsamanafahrzeug-backups
HETZNER_S3_ACCESS_KEY=your-access-key
HETZNER_S3_SECRET_KEY=your-secret-key
```

## Cron Jobs

Set up automated backups:
```bash
crontab -e

# Add this line:
0 2 * * * /opt/hsamanafahrzeug/deploy/backup.sh >> /opt/hsamanafahrzeug/logs/backup.log 2>&1
```

## Security Notes

1. **Never commit `.env` file** - Contains sensitive credentials
2. **Use strong passwords** - Generate with `openssl rand -base64 32`
3. **Keep scripts executable** - `chmod +x *.sh`
4. **Review logs regularly** - Check `/opt/hsamanafahrzeug/logs/`
5. **Update dependencies** - Run `apt-get update && apt-get upgrade` monthly

## Troubleshooting

### Script Permissions
```bash
chmod +x hetzner-setup.sh deploy.sh backup.sh
```

### View Deployment Logs
```bash
tail -f /opt/hsamanafahrzeug/logs/deploy.log
```

### Rollback Deployment
```bash
cd /opt/hsamanafahrzeug/backups/
ls -lt
# Restore from latest backup
```

## Support

For detailed deployment guide, see [DEPLOYMENT.md](../docs/DEPLOYMENT.md)

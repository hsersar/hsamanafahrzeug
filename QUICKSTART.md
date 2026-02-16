# Quick Start Guide - Hetzner Cloud Deployment

**5-Minute Guide to Deploy HsaManageFahrzeug**

## Prerequisites

- Hetzner Cloud account
- Domain name with DNS access
- SSH key pair

## Step-by-Step Deployment

### 1. Create Hetzner Server (2 minutes)

1. Go to [Hetzner Cloud Console](https://console.hetzner.cloud)
2. Create new project
3. Click "Add Server"
   - **Location**: Nuremberg or Falkenstein
   - **Image**: Ubuntu 22.04 LTS
   - **Type**: CPX31 (4 vCPU, 8GB RAM) - €13.14/month
   - **SSH Key**: Add your public key
4. Note the IP address

### 2. Configure DNS (1 minute)

Add DNS A record:
```
Type: A
Name: app (or your subdomain)
Value: <YOUR_SERVER_IP>
TTL: 300
```

### 3. Initial Server Setup (5 minutes)

SSH into your server:
```bash
ssh root@<YOUR_SERVER_IP>
```

Download and run setup script:
```bash
curl -o setup.sh https://raw.githubusercontent.com/hsersar/hsamanafahrzeug/main/deploy/hetzner-setup.sh
chmod +x setup.sh
sudo ./setup.sh
```

**Wait for reboot** (30 seconds)

### 4. Deploy Application (5 minutes)

SSH as app user:
```bash
ssh hsa-app@<YOUR_SERVER_IP>
cd /opt/hsamanafahrzeug
```

Clone repository:
```bash
git clone https://github.com/hsersar/hsamanafahrzeug.git .
```

Configure environment:
```bash
cp deploy/.env.example .env
nano .env
```

**Minimum required configuration**:
```bash
DOMAIN_NAME=app.yourdomain.de
POSTGRES_PASSWORD=$(openssl rand -base64 32)
REDIS_PASSWORD=$(openssl rand -base64 32)
JWT_KEY=$(openssl rand -base64 64)
LETSENCRYPT_EMAIL=your-email@example.de
```

### 5. Get SSL Certificate (2 minutes)

```bash
# Start Nginx for certificate challenge
docker compose -f docker-compose.prod.yml up -d nginx

# Obtain certificate
docker compose -f docker-compose.prod.yml run --rm certbot certonly \
  --webroot \
  --webroot-path=/var/www/certbot \
  --email your-email@example.de \
  --agree-tos \
  --no-eff-email \
  -d app.yourdomain.de

# Restart Nginx with SSL
docker compose -f docker-compose.prod.yml restart nginx
```

### 6. Deploy (3 minutes)

```bash
bash deploy/deploy.sh
```

Wait for deployment to complete. The script will:
- ✅ Build Docker images
- ✅ Start all services
- ✅ Run health checks
- ✅ Verify deployment

### 7. Verify (1 minute)

Open browser: `https://app.yourdomain.de`

Check containers:
```bash
docker compose -f docker-compose.prod.yml ps
```

View logs:
```bash
docker compose -f docker-compose.prod.yml logs -f
```

## What You Get

✅ **ASP.NET Core 8 Backend** - Running on port 8080 (internal)  
✅ **React Frontend** - Served via Nginx  
✅ **PostgreSQL 16** - With persistent storage  
✅ **Redis Cache** - For high performance  
✅ **Nginx Reverse Proxy** - With SSL/TLS (Let's Encrypt)  
✅ **Automatic Backups** - Daily at 2:00 AM  
✅ **Security Hardening** - UFW, Fail2ban, SSL, Headers  
✅ **Health Checks** - All services monitored  
✅ **Auto Updates** - Security patches applied automatically  

## Optional: Enable Monitoring (2 minutes)

```bash
cd /opt/hsamanafahrzeug/deploy/monitoring
docker compose -f docker-compose.monitoring.yml up -d
```

Access Grafana: `http://<SERVER_IP>:3000`
- Username: `admin`
- Password: `admin` (change immediately!)

## Daily Operations

**View logs**:
```bash
docker compose -f docker-compose.prod.yml logs -f
```

**Restart service**:
```bash
docker compose -f docker-compose.prod.yml restart <service-name>
```

**Backup database**:
```bash
bash /opt/hsamanafahrzeug/deploy/backup.sh
```

**Update application** (via GitHub Actions):
```bash
git push origin main  # Triggers automatic deployment
```

## Troubleshooting

**Container won't start?**
```bash
docker compose -f docker-compose.prod.yml logs <service-name>
```

**SSL certificate issues?**
```bash
bash nginx/ssl-renew.sh
```

**Database connection failed?**
```bash
docker compose -f docker-compose.prod.yml restart postgres
```

**Need to restore backup?**
```bash
gunzip -c backups/db_backup_*.sql.gz | \
  docker exec -i hsamanafahrzeug-postgres psql -U hsamanafahrzeug
```

## Cost Breakdown

| Item | Cost |
|------|------|
| Server (CPX31) | €13.14/month |
| Backup Storage | €2.00/month |
| Domain | €10-20/year |
| **Total** | **~€15-20/month** |

## Security Checklist

- [x] SSH key authentication only
- [x] Firewall enabled (ports 22, 80, 443)
- [x] Fail2ban protecting SSH
- [x] SSL/TLS with Let's Encrypt
- [x] All containers non-root
- [x] Secrets in .env file (not committed)
- [x] Automatic security updates
- [x] Daily backups enabled

## Next Steps

1. ✅ Set up automated backups to Hetzner Object Storage
2. ✅ Configure GitHub Actions for CI/CD
3. ✅ Enable monitoring (Prometheus + Grafana)
4. ✅ Add custom domain email notifications
5. ✅ Review and customize security headers

## Full Documentation

For detailed information, see [DEPLOYMENT.md](docs/DEPLOYMENT.md)

## Support

- 📧 Open an [Issue](https://github.com/hsersar/hsamanafahrzeug/issues)
- 📖 Read [DEPLOYMENT.md](docs/DEPLOYMENT.md)
- 🔍 Check [Troubleshooting](docs/DEPLOYMENT.md#troubleshooting)

---

**Total Setup Time**: ~15-20 minutes  
**Difficulty**: Beginner-friendly  
**Cost**: €15-20/month

# Production Deployment Checklist

Complete checklist for deploying HsaManageFahrzeug to production on Hetzner Cloud.

## Pre-Deployment

### Domain & DNS
- [ ] Domain registered and accessible
- [ ] DNS A record pointing to server IP
- [ ] DNS propagation completed (check with `nslookup`)
- [ ] Backup MX records configured (if using email)

### Hetzner Cloud
- [ ] Hetzner Cloud account created
- [ ] SSH key added to Hetzner project
- [ ] Server created (CPX31 or larger recommended)
- [ ] Server IP address noted
- [ ] Firewall rules configured (HTTP, HTTPS, SSH)

### Local Setup
- [ ] Git repository cloned locally
- [ ] SSH connection to server tested
- [ ] `.env` file configured with production values
- [ ] All passwords generated securely (32+ characters)
- [ ] JWT key generated (256-bit minimum)

## Server Setup

### Initial Configuration
- [ ] Connected to server as root: `ssh root@<IP>`
- [ ] System packages updated: `apt-get update && apt-get upgrade`
- [ ] `hetzner-setup.sh` script executed successfully
- [ ] Server rebooted and reconnected
- [ ] Non-root user `hsa-app` created and accessible
- [ ] Docker installed and running: `docker --version`
- [ ] Docker Compose installed: `docker compose version`

### Security Hardening
- [ ] UFW firewall enabled and configured
- [ ] Fail2ban installed and running
- [ ] SSH root login disabled
- [ ] SSH password authentication disabled
- [ ] SSH key-based authentication working
- [ ] Automatic security updates enabled
- [ ] Swap space configured (2GB)

### Application Directory
- [ ] Application directory created: `/opt/hsamanafahrzeug`
- [ ] Directory ownership set to `hsa-app`
- [ ] Repository cloned to application directory
- [ ] `.env` file created and configured
- [ ] `.env` file permissions set: `chmod 600 .env`

## SSL/TLS Setup

### Let's Encrypt Certificate
- [ ] Certbot container available
- [ ] DNS propagation verified
- [ ] HTTP challenge accessible (port 80 open)
- [ ] SSL certificate obtained successfully
- [ ] Certificate files present in `certbot/conf/live/<domain>/`
- [ ] Nginx configured with SSL certificate paths
- [ ] HTTPS accessible: `https://<domain>`
- [ ] HTTP redirects to HTTPS
- [ ] SSL Labs test passed (A+ rating): https://www.ssllabs.com/ssltest/

### Certificate Renewal
- [ ] Certbot renewal container running
- [ ] Certificate expiration date noted (90 days)
- [ ] Renewal script tested: `bash nginx/ssl-renew.sh`
- [ ] Automatic renewal scheduled

## Application Deployment

### Docker Images
- [ ] Backend Dockerfile validated
- [ ] Frontend Dockerfile validated
- [ ] Docker Compose file validated
- [ ] Environment variables verified in docker-compose
- [ ] All required secrets present in `.env`

### Database Setup
- [ ] PostgreSQL container starts successfully
- [ ] Database initialization script executed
- [ ] Database accessible from backend container
- [ ] Connection string in `.env` correct
- [ ] Database migrations applied (if applicable)
- [ ] Test data loaded (if needed)

### Redis Cache
- [ ] Redis container starts successfully
- [ ] Redis password configured
- [ ] Redis accessible from backend container
- [ ] Connection string in `.env` correct

### Backend Service
- [ ] Backend container builds successfully
- [ ] Backend container starts without errors
- [ ] Health check endpoint responding: `/health`
- [ ] API endpoints accessible via Nginx: `https://<domain>/api/`
- [ ] Logs show no errors: `docker compose logs backend`
- [ ] Database connection working
- [ ] Redis connection working

### Frontend Service
- [ ] Frontend container builds successfully
- [ ] Frontend container starts without errors
- [ ] Static files served correctly
- [ ] Application loads in browser
- [ ] API calls work correctly
- [ ] No console errors in browser

### Nginx Reverse Proxy
- [ ] Nginx container starts successfully
- [ ] SSL/TLS configuration loaded
- [ ] Security headers present in responses
- [ ] Rate limiting working
- [ ] Gzip/Brotli compression enabled
- [ ] Static file caching working
- [ ] Logs accessible: `docker compose logs nginx`

## Testing & Validation

### Functional Testing
- [ ] Application homepage loads
- [ ] User registration works
- [ ] User login works
- [ ] JWT authentication working
- [ ] CRUD operations functional
- [ ] Dashboard loads correctly
- [ ] API endpoints returning expected data
- [ ] Error handling working correctly

### Performance Testing
- [ ] Page load time acceptable (<3 seconds)
- [ ] API response time acceptable (<1 second)
- [ ] Database queries optimized
- [ ] Redis caching working
- [ ] Static assets loading quickly
- [ ] No memory leaks detected

### Security Testing
- [ ] HTTPS enforced (HTTP redirects)
- [ ] Security headers present (HSTS, CSP, etc.)
- [ ] XSS protection working
- [ ] CSRF protection enabled
- [ ] SQL injection protection verified
- [ ] Authentication required for protected routes
- [ ] JWT tokens expire correctly
- [ ] Secrets not exposed in logs or responses

### Browser Compatibility
- [ ] Chrome/Edge (latest)
- [ ] Firefox (latest)
- [ ] Safari (latest)
- [ ] Mobile browsers (iOS, Android)

## Monitoring & Logging

### Logging Configuration
- [ ] Application logs available: `docker compose logs -f`
- [ ] Log rotation configured
- [ ] Log levels appropriate (INFO in production)
- [ ] Sensitive data not logged
- [ ] Structured logging enabled (JSON)

### Monitoring (Optional)
- [ ] Prometheus deployed
- [ ] Grafana deployed
- [ ] cAdvisor deployed
- [ ] Node Exporter deployed
- [ ] PostgreSQL Exporter deployed
- [ ] Redis Exporter deployed
- [ ] Dashboards configured
- [ ] Grafana accessible and secured
- [ ] Metrics collecting correctly

### Health Checks
- [ ] All containers have health checks defined
- [ ] Health checks passing
- [ ] Uptime monitoring configured (optional)
- [ ] Alert notifications configured (optional)

## Backup & Recovery

### Backup Configuration
- [ ] Backup script tested: `bash deploy/backup.sh`
- [ ] Backup directory created: `/opt/hsamanafahrzeug/backups/`
- [ ] Backup retention policy configured (7 days)
- [ ] Cron job for daily backups configured
- [ ] Backup completion verified
- [ ] Backup file integrity checked

### Backup Testing
- [ ] Test backup created successfully
- [ ] Test restore performed successfully
- [ ] Restore time acceptable
- [ ] Data integrity verified after restore

### Offsite Backup (Optional)
- [ ] Hetzner Object Storage configured
- [ ] S3 credentials in `.env`
- [ ] Upload to Object Storage working
- [ ] Backup retention policy in Object Storage

## CI/CD Pipeline

### GitHub Actions
- [ ] GitHub repository created/updated
- [ ] GitHub Secrets configured:
  - [ ] `HETZNER_SSH_KEY`
  - [ ] `HETZNER_HOST`
  - [ ] `HETZNER_USER`
  - [ ] `DOMAIN_NAME`
  - [ ] `SLACK_WEBHOOK_URL` (optional)
- [ ] Workflow file present: `.github/workflows/deploy-hetzner.yml`
- [ ] Test job runs successfully
- [ ] Build job runs successfully
- [ ] Deploy job runs successfully
- [ ] Deployment verified after push to `main`

## Documentation

### Project Documentation
- [ ] README.md updated with project info
- [ ] DEPLOYMENT.md reviewed and accurate
- [ ] QUICKSTART.md available
- [ ] deploy/README.md available
- [ ] API documentation available (if applicable)
- [ ] Architecture diagram available (optional)

### Operational Documentation
- [ ] Server access credentials documented (securely)
- [ ] Domain/DNS configuration documented
- [ ] Backup/restore procedures documented
- [ ] Troubleshooting guide available
- [ ] Runbook for common operations
- [ ] Emergency contacts documented

## Post-Deployment

### Communication
- [ ] Stakeholders notified of deployment
- [ ] Users informed of new features (if applicable)
- [ ] Support team briefed
- [ ] Documentation shared with team

### Monitoring & Maintenance
- [ ] Monitor logs for 24 hours
- [ ] Monitor error rates
- [ ] Monitor performance metrics
- [ ] Check backup completion next day
- [ ] Verify SSL certificate renewal schedule
- [ ] Schedule regular security updates

### Final Verification
- [ ] All containers running: `docker compose ps`
- [ ] All health checks passing
- [ ] Application accessible from internet
- [ ] No errors in logs
- [ ] Performance acceptable
- [ ] Security tests passed

## Rollback Plan

### If Deployment Fails
- [ ] Rollback script prepared
- [ ] Previous backup available
- [ ] Rollback procedure tested
- [ ] Communication plan for rollback
- [ ] Post-mortem process defined

## Compliance & Security (BSI Standards)

### BSI IT-Grundschutz
- [ ] Authentication mechanisms strong
- [ ] Authorization working correctly
- [ ] Audit logging enabled
- [ ] Encryption in transit (HTTPS)
- [ ] Encryption at rest (database, backups)
- [ ] Password policy enforced
- [ ] Session management secure
- [ ] Error messages don't leak sensitive info

### GDPR Compliance (if applicable)
- [ ] Data protection measures in place
- [ ] User consent mechanisms
- [ ] Data deletion capabilities
- [ ] Privacy policy available
- [ ] Cookie consent implemented

## Sign-Off

**Deployment Date**: _____________

**Deployed By**: _____________

**Verified By**: _____________

**Production URL**: https://_____________

**Server**: _____________

**Notes**: 
_________________________________
_________________________________
_________________________________

---

**Status**: ☐ Ready for Production | ☐ Needs Attention | ☐ Blocked

**Next Review Date**: _____________

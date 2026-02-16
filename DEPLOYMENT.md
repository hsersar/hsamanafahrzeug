# Deployment Guide

This guide provides instructions for deploying the iKfz application in various environments.

## Prerequisites

- Docker and Docker Compose (recommended)
- OR .NET 10.0 SDK + Node.js 18+ for manual deployment
- OAuth2/OIDC provider (Azure AD, Auth0, Keycloak, etc.)
- SSL/TLS certificate for production

## Quick Start with Docker

The easiest way to deploy the entire application:

```bash
# 1. Clone the repository
git clone https://github.com/hsersar/hsamanafahrzeug.git
cd hsamanafahrzeug

# 2. Configure environment variables
# Edit docker-compose.yml and set your OAuth2 settings

# 3. Build and run
docker-compose up -d

# 4. Access the application
# Backend: http://localhost:7000
# Frontend: http://localhost:3000
```

## Production Deployment

### 1. Configure OAuth2/OIDC

Set up your identity provider and obtain:
- Authority URL (issuer)
- Audience (client ID for API)
- Client ID (for frontend)

### 2. Backend Deployment

#### Option A: Docker

```bash
# Build the backend image
docker build -f Dockerfile.backend -t ikfz-backend .

# Run with environment variables
docker run -d \
  -p 7000:80 \
  -e JwtSettings__Authority="https://your-idp.com" \
  -e JwtSettings__Audience="ikfz-api" \
  -e ConnectionStrings__DefaultConnection="Server=db;Database=ikfz;..." \
  ikfz-backend
```

#### Option B: Manual Deployment

```bash
cd backend/iKfz.Backend

# Set environment variables
export JwtSettings__Authority="https://your-idp.com"
export JwtSettings__Audience="ikfz-api"
export ConnectionStrings__DefaultConnection="Server=..."

# Build and publish
dotnet publish -c Release -o ./publish

# Run
cd publish
dotnet iKfz.Backend.dll
```

### 3. Frontend Deployment

#### Option A: Docker with Nginx

```bash
# Build the frontend image
docker build -f Dockerfile.frontend -t ikfz-frontend .

# Run
docker run -d -p 3000:80 ikfz-frontend
```

#### Option B: Static Hosting (Azure, AWS S3, Netlify, Vercel)

```bash
cd frontend

# Create production build
REACT_APP_API_URL="https://api.yourcompany.com" npm run build

# Deploy the 'build' folder to your static hosting service
# For example, with Azure Static Web Apps:
# az staticwebapp create --name ikfz-app --resource-group myRG --source ./build
```

### 4. Database Setup

#### Production Database (SQL Server)

1. Create database:
```sql
CREATE DATABASE iKfz;
```

2. Update connection string:
```bash
export ConnectionStrings__DefaultConnection="Server=your-server;Database=iKfz;User Id=user;Password=***;Encrypt=true;"
```

3. Run migrations:
```bash
cd backend/iKfz.Backend
dotnet ef database update
```

## Cloud Deployments

### Azure

#### Backend (Azure App Service)

```bash
# Create App Service
az webapp create --name ikfz-api --resource-group myRG --plan myPlan

# Configure app settings
az webapp config appsettings set --name ikfz-api --resource-group myRG \
  --settings \
  JwtSettings__Authority="https://login.microsoftonline.com/your-tenant" \
  JwtSettings__Audience="api://ikfz-api"

# Deploy
cd backend/iKfz.Backend
dotnet publish -c Release
az webapp deployment source config-zip --name ikfz-api --resource-group myRG --src ./publish.zip
```

#### Frontend (Azure Static Web Apps)

```bash
# Deploy via Azure Static Web Apps CLI or GitHub Actions
cd frontend
npm run build
az staticwebapp create --name ikfz-app --resource-group myRG
```

### AWS

#### Backend (Elastic Beanstalk or ECS)

```bash
# Package for deployment
cd backend/iKfz.Backend
dotnet publish -c Release

# Create deployment package
cd bin/Release/net10.0/publish
zip -r ../../../deploy.zip .

# Deploy to Elastic Beanstalk
eb init -p "64bit Amazon Linux 2023 v3.0.0 running .NET 10"
eb create ikfz-api-env
eb deploy
```

#### Frontend (S3 + CloudFront)

```bash
cd frontend
npm run build

# Upload to S3
aws s3 sync build/ s3://ikfz-frontend --delete

# Invalidate CloudFront cache
aws cloudfront create-invalidation --distribution-id YOUR_DIST_ID --paths "/*"
```

## Environment Variables Reference

### Backend

| Variable | Description | Example |
|----------|-------------|---------|
| `JwtSettings__Authority` | OAuth2 authority URL | `https://login.microsoftonline.com/tenant-id` |
| `JwtSettings__Audience` | API audience/client ID | `api://ikfz-api` |
| `JwtSettings__Issuer` | Token issuer | `https://login.microsoftonline.com/tenant-id` |
| `ConnectionStrings__DefaultConnection` | Database connection | `Server=...;Database=ikfz;...` |
| `Frontend__Url` | Frontend URL for CORS | `https://ikfz.yourcompany.com` |
| `ASPNETCORE_ENVIRONMENT` | Environment | `Production` |

### Frontend

| Variable | Description | Example |
|----------|-------------|---------|
| `REACT_APP_API_URL` | Backend API URL | `https://api.yourcompany.com/api` |
| `REACT_APP_AUTH_AUTHORITY` | OAuth2 authority | `https://login.microsoftonline.com/tenant-id` |
| `REACT_APP_AUTH_CLIENT_ID` | Frontend client ID | `client-id-from-idp` |

## SSL/TLS Configuration

### Production Requirements

- **Always use HTTPS** in production
- Obtain SSL certificate from:
  - Let's Encrypt (free, automated)
  - Your cloud provider (Azure, AWS, etc.)
  - Commercial CA

### Nginx Configuration (if using reverse proxy)

```nginx
server {
    listen 443 ssl http2;
    server_name ikfz.yourcompany.com;

    ssl_certificate /path/to/fullchain.pem;
    ssl_certificate_key /path/to/privkey.pem;
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;

    location / {
        proxy_pass http://frontend:80;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }

    location /api/ {
        proxy_pass http://backend:80;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }
}
```

## Health Monitoring

### Health Check Endpoints

- Backend: `https://api.yourcompany.com/health`
- Returns: `200 OK` if healthy

### Monitoring Setup

Configure monitoring for:
- HTTP endpoint availability
- Response time
- Error rate
- Database connectivity

Example with Azure Application Insights:
```bash
# Add Application Insights
dotnet add package Microsoft.ApplicationInsights.AspNetCore

# Configure in Program.cs
builder.Services.AddApplicationInsightsTelemetry();
```

## Scaling

### Horizontal Scaling

The application is stateless and can be horizontally scaled:

```bash
# Docker Compose with replicas
docker-compose up -d --scale backend=3 --scale frontend=2
```

### Load Balancing

Use a load balancer (Azure Load Balancer, AWS ALB, Nginx):

```nginx
upstream backend {
    server backend1:80;
    server backend2:80;
    server backend3:80;
}

server {
    location / {
        proxy_pass http://backend;
    }
}
```

## Backup & Recovery

### Database Backups

Schedule regular backups:

```bash
# SQL Server backup
BACKUP DATABASE iKfz TO DISK = '/backups/ikfz_backup.bak'

# Automated with Azure SQL
# Configure automated backups in Azure Portal
```

### Application State

- No application state stored on servers
- All data in database
- Frontend is static (stateless)

## Security Checklist

Before deploying to production:

- [ ] Configure real OAuth2/OIDC provider
- [ ] Use HTTPS with valid SSL certificate
- [ ] Store secrets in secure vault (Azure Key Vault, AWS Secrets Manager)
- [ ] Enable production logging and monitoring
- [ ] Configure rate limiting appropriately
- [ ] Set up database backups
- [ ] Configure firewall rules
- [ ] Review CORS settings
- [ ] Enable security headers
- [ ] Perform security audit
- [ ] Set up alerting for errors

## Troubleshooting

### Backend won't start

1. Check logs: `docker logs <container-id>`
2. Verify OAuth2 settings
3. Check database connectivity
4. Verify environment variables

### Frontend can't reach backend

1. Check CORS configuration in backend
2. Verify `REACT_APP_API_URL` is correct
3. Check network connectivity
4. Verify SSL certificates if using HTTPS

### Authentication fails

1. Verify OAuth2 authority URL
2. Check audience/client ID configuration
3. Verify token format and claims
4. Check token expiration

## Support

For deployment issues:
1. Check logs first
2. Review configuration
3. Consult documentation
4. Open issue on GitHub

## Additional Resources

- [ASP.NET Core Deployment](https://docs.microsoft.com/aspnet/core/host-and-deploy/)
- [React Deployment](https://create-react-app.dev/docs/deployment/)
- [Docker Documentation](https://docs.docker.com/)
- [OAuth2/OIDC Specification](https://openid.net/connect/)

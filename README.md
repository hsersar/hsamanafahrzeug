# HsaManageFahrzeug

**Fahrzeugzulassungs-Verwaltungssystem** - Vehicle Registration Management System

Modern web application for managing vehicle registrations built with ASP.NET Core 8 and React, deployed on Hetzner Cloud with Docker.

## 🚀 Features

- **Vehicle Registration Management**: Complete CRUD operations for vehicle data
- **User Authentication**: JWT-based authentication and authorization
- **IKFZ API Integration**: Integration with official vehicle registration APIs
- **Real-time Dashboard**: Vehicle statistics and reporting
- **Redis Caching**: High-performance caching for improved response times
- **Responsive UI**: Modern React-based frontend

## 🏗️ Architecture

### Technology Stack

**Backend**:
- ASP.NET Core 8
- Entity Framework Core
- PostgreSQL 16
- Redis Cache
- JWT Authentication

**Frontend**:
- React 18
- TypeScript
- Material-UI / Tailwind CSS

**Infrastructure**:
- Docker & Docker Compose
- Nginx (Reverse Proxy + SSL/TLS)
- Let's Encrypt (SSL Certificates)
- Prometheus + Grafana (Monitoring)

## 📦 Deployment

### Quick Start (Production)

1. **Create Hetzner Cloud Server**
   - Server: CPX31 (4 vCPU, 8GB RAM)
   - OS: Ubuntu 22.04 LTS
   - Add SSH key

2. **Run Server Setup**
   ```bash
   curl -o hetzner-setup.sh https://raw.githubusercontent.com/hsersar/hsamanafahrzeug/main/deploy/hetzner-setup.sh
   chmod +x hetzner-setup.sh
   sudo ./hetzner-setup.sh
   ```

3. **Configure Application**
   ```bash
   cd /opt/hsamanafahrzeug
   cp deploy/.env.example .env
   nano .env  # Add your configuration
   ```

4. **Deploy Application**
   ```bash
   bash deploy/deploy.sh
   ```

For detailed deployment instructions, see [DEPLOYMENT.md](docs/DEPLOYMENT.md)

## 🔧 Development Setup

### Prerequisites
- .NET 8 SDK
- Node.js 20+
- Docker & Docker Compose
- PostgreSQL 16
- Redis

### Local Development

**Backend**:
```bash
cd backend
dotnet restore
dotnet run
```

**Frontend**:
```bash
cd frontend
npm install
npm start
```

**Database** (via Docker):
```bash
docker compose up -d postgres redis
```

## 📁 Project Structure

```
hsamanafahrzeug/
├── backend/                      # ASP.NET Core API
│   ├── HsaManageFahrzeug.Api/
│   ├── HsaManageFahrzeug.Core/
│   └── HsaManageFahrzeug.Infrastructure/
├── frontend/                     # React SPA
│   ├── src/
│   ├── public/
│   └── package.json
├── deploy/                       # Deployment scripts
│   ├── hetzner-setup.sh
│   ├── deploy.sh
│   ├── backup.sh
│   └── monitoring/
├── nginx/                        # Nginx configuration
│   ├── nginx.conf
│   └── ssl-params.conf
├── docs/                         # Documentation
│   └── DEPLOYMENT.md
├── docker-compose.prod.yml       # Production Docker Compose
├── Dockerfile.backend.prod       # Backend production image
└── Dockerfile.frontend.prod      # Frontend production image
```

## 🔐 Security

### Security Features
- ✅ All containers run as non-root users
- ✅ Docker network isolation
- ✅ UFW Firewall (ports 22, 80, 443 only)
- ✅ Fail2ban SSH protection
- ✅ SSL/TLS with Let's Encrypt (A+ rating)
- ✅ Security headers (HSTS, CSP, X-Frame-Options, etc.)
- ✅ Rate limiting
- ✅ Automatic security updates

### Secrets Management
- Environment variables via `.env` file (never committed)
- GitHub Secrets for CI/CD
- PostgreSQL and Redis authentication
- JWT token-based authentication

## 📊 Monitoring

Monitoring stack includes:
- **Prometheus**: Metrics collection
- **Grafana**: Visualization dashboards
- **cAdvisor**: Container metrics
- **Node Exporter**: System metrics
- **PostgreSQL Exporter**: Database metrics
- **Redis Exporter**: Cache metrics

Access Grafana: `http://<server-ip>:3000`

## 💾 Backup

### Automated Backups
- Daily PostgreSQL backups (2:00 AM)
- 7-day retention policy
- Optional offsite backup to Hetzner Object Storage

### Manual Backup
```bash
bash /opt/hsamanafahrzeug/deploy/backup.sh
```

### Restore
```bash
gunzip -c backup.sql.gz | docker exec -i hsamanafahrzeug-postgres psql -U hsamanafahrzeug
```

## 🚢 CI/CD

GitHub Actions workflow automatically:
1. Runs tests (backend + frontend)
2. Builds Docker images
3. Pushes to GitHub Container Registry
4. Deploys to Hetzner Cloud
5. Performs health checks
6. Sends notifications (Slack)

Trigger: Push to `main` branch

## 📝 Environment Variables

Required environment variables:

| Variable | Description | Example |
|----------|-------------|---------|
| `DOMAIN_NAME` | Application domain | `app.example.de` |
| `POSTGRES_PASSWORD` | Database password | `strong_password` |
| `REDIS_PASSWORD` | Redis password | `strong_password` |
| `JWT_KEY` | JWT signing key (256-bit) | `base64_encoded_key` |
| `LETSENCRYPT_EMAIL` | SSL certificate email | `admin@example.de` |
| `IKFZ_API_KEY` | IKFZ API key | `your_api_key` |

See [.env.example](deploy/.env.example) for complete list.

## 💰 Cost Estimate

**Monthly Hosting Costs** (Hetzner Cloud):
- Server (CPX31): €13.14
- Backups: €2.00
- Domain: ~€1.00

**Total: ~€15-20/month**

## 📚 Documentation

- [Deployment Guide](docs/DEPLOYMENT.md) - Complete production deployment guide
- [Deploy Scripts README](deploy/README.md) - Deployment scripts documentation

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License.

## 🆘 Support

For issues and questions:
- Open an [Issue](https://github.com/hsersar/hsamanafahrzeug/issues)
- Check [Deployment Guide](docs/DEPLOYMENT.md)
- Review [Troubleshooting](docs/DEPLOYMENT.md#troubleshooting)

## 🎯 Roadmap

- [ ] Add automated testing suite
- [ ] Implement advanced search and filtering
- [ ] Add PDF export functionality
- [ ] Multi-language support
- [ ] Mobile app (React Native)
- [ ] API documentation (Swagger)
- [ ] Role-based access control (RBAC)

---

**Built with ❤️ for efficient vehicle registration management**
# Fahrzeugzulassungs-Webapp

Complete vehicle registration web application with ASP.NET Core 8 backend and Docker support.

## ✅ Project Status

### Backend: COMPLETE ✅
The complete ASP.NET Core 8 backend is fully implemented with Clean Architecture and ready for deployment.

### Docker: COMPLETE ✅
Full Docker containerization with PostgreSQL database.

### Migrations: COMPLETE ✅
EF Core migrations created and auto-apply on startup.

## Features

### Architecture
- **Clean Architecture** with 4 layers: Domain, Infrastructure, Application, API
- **Domain Layer**: 8 entities, 5 enums
- **Infrastructure Layer**: EF Core, PostgreSQL, file storage, iKFZ integration
- **Application Layer**: 25+ DTOs, 10 services, 9 validators, AutoMapper
- **API Layer**: 8 REST controllers, 2 middleware components

### Security (BSI Compliant)
- ✅ JWT authentication (15min access tokens, 7-day refresh tokens)
- ✅ Strong password policy (min 12 chars, uppercase, lowercase, digit, special char)
- ✅ Account lockout (5 failed attempts = 15min lockout)
- ✅ Security headers (CSP, HSTS, X-Frame-Options, X-Content-Type-Options, etc.)
- ✅ Rate limiting (100 requests/minute per IP)
- ✅ CORS protection
- ✅ Audit logging for all data operations
- ✅ Role-based authorization (SuperAdmin, StandortAdmin, Mitarbeiter, Kunde)

### API Endpoints
- **Auth**: Login, Register, Refresh Token, Logout
- **Standorte**: Full CRUD (SuperAdmin only)
- **Mitarbeiter**: Full CRUD, Activate/Deactivate (StandortAdmin)
- **Kunden**: Full CRUD (Mitarbeiter)
- **Aufträge**: CRUD, Status updates, iKFZ submission, wizard support
- **Rechnungen**: Create, View, Payment processing
- **Dokumente**: Upload, Download, Delete
- **Dashboard**: KPIs, recent orders, monthly revenue

## Quick Start

### Prerequisites
- Docker and Docker Compose
- OR .NET 8 SDK + PostgreSQL 15+

### Option 1: Docker (Recommended)

1. Clone the repository:
```bash
git clone https://github.com/hsersar/hsamanafahrzeug.git
cd hsamanafahrzeug
```

2. Configure environment variables:
```bash
cp .env.example .env
# Edit .env and set strong passwords
```

3. Start the services:
```bash
docker-compose up -d
```

4. Wait for services to start (check logs):
```bash
docker-compose logs -f backend
```

5. Access the API:
- **Swagger UI**: http://localhost:5001/swagger
- **Health Check**: http://localhost:5001/health
- **API Base URL**: http://localhost:5001/api

6. Login with default credentials:
- **Email**: admin@fahrzeugzulassung.de
- **Password**: Admin@123456789

### Option 2: Local Development

1. Install .NET 8 SDK and PostgreSQL

2. Update connection string in `backend/FahrzeugZulassung.API/appsettings.Development.json`

3. Run the application:
```bash
cd backend/FahrzeugZulassung.API
dotnet run
```

4. Access Swagger UI: https://localhost:7001/swagger

## Docker Commands

### Start services
```bash
docker-compose up -d
```

### View logs
```bash
docker-compose logs -f backend
docker-compose logs -f postgres
```

### Stop services
```bash
docker-compose down
```

### Rebuild after code changes
```bash
docker-compose up -d --build
```

### Remove all data (including database)
```bash
docker-compose down -v
```

## Database

### Migrations

Migrations are automatically applied on application startup. To manage migrations manually:

```bash
# Create new migration
cd backend/FahrzeugZulassung.API
dotnet ef migrations add MigrationName --project ../FahrzeugZulassung.Infrastructure

# Apply migrations
dotnet ef database update

# Rollback migration
dotnet ef database update PreviousMigrationName

# Remove last migration (if not applied)
dotnet ef migrations remove
```

### Default Data

The application seeds the following data on first run:
- SuperAdmin user (admin@fahrzeugzulassung.de / Admin@123456789)
- Default roles (SuperAdmin, StandortAdmin, Mitarbeiter, Kunde)
- Default Standort (Hauptstandort in München)

## API Documentation

### Authentication Flow

1. **Register** (POST /api/auth/register):
```json
{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!",
  "vorname": "Max",
  "nachname": "Mustermann",
  "telefon": "+49 123 456789"
}
```

2. **Login** (POST /api/auth/login):
```json
{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

Response:
```json
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "abc123...",
  "expiresIn": 900,
  "userId": "guid",
  "email": "user@example.com",
  "rolle": "Kunde",
  "vorname": "Max",
  "nachname": "Mustermann"
}
```

3. **Use Bearer Token**:
```
Authorization: Bearer {accessToken}
```

4. **Refresh Token** (POST /api/auth/refresh-token):
```json
{
  "refreshToken": "abc123...",
  "accessToken": "eyJhbGc..."
}
```

### Wizard Flow (Vehicle Registration)

1. **Create Order** (POST /api/auftraege):
```json
{
  "step1": {
    "typ": "Anmeldung",
    "vorname": "Max",
    "nachname": "Mustermann",
    "strasse": "Musterstraße",
    "hausnummer": "123",
    "plz": "80331",
    "ort": "München",
    "email": "max@example.com",
    "telefon": "+49 89 123456"
  },
  "step2": {
    "fin": "WBA12345678901234",
    "kennzeichen": "M AB 1234",
    "marke": "BMW",
    "modell": "3er",
    "erstzulassung": "2020-01-15"
  },
  "step3": {
    "agbAkzeptiert": true,
    "datenschutzAkzeptiert": true
  }
}
```

2. **Upload Documents** (POST /api/dokumente/upload)

3. **Submit to iKFZ** (POST /api/auftraege/{id}/submit-ikfz)

## Configuration

### Environment Variables

#### Database
- `ConnectionStrings__DefaultConnection`: PostgreSQL connection string

#### JWT
- `JwtSettings__Secret`: Secret key (min 32 chars)
- `JwtSettings__Issuer`: Token issuer
- `JwtSettings__Audience`: Token audience
- `JwtSettings__AccessTokenExpiration`: Minutes (default: 15)
- `JwtSettings__RefreshTokenExpiration`: Minutes (default: 10080 = 7 days)

#### CORS
- `CorsSettings__AllowedOrigins__0`: Frontend URL

#### File Storage
- `FileStorage__Path`: Upload directory path

#### Rate Limiting
- `IpRateLimiting__GeneralRules__0__Limit`: Requests per period

### Production Deployment

1. **Generate secure secrets**:
```bash
# JWT Secret
openssl rand -base64 32

# PostgreSQL Password
openssl rand -base64 24
```

2. **Update .env file** with production values

3. **Use HTTPS**: Configure reverse proxy (nginx/traefik) for SSL/TLS

4. **Set ASPNETCORE_ENVIRONMENT=Production**

5. **Backup strategy**: Regular PostgreSQL backups
```bash
docker exec fahrzeugzulassung-db pg_dump -U fahrzeug_user FahrzeugZulassung > backup.sql
```

## Technical Stack

### Backend
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- AutoMapper
- FluentValidation
- Serilog
- Swagger/OpenAPI
- JWT Authentication
- AspNetCoreRateLimit

### Infrastructure
- Docker
- Docker Compose
- PostgreSQL 15

## Project Structure

```
/
├── backend/
│   ├── FahrzeugZulassung.API/          # Web API layer
│   ├── FahrzeugZulassung.Application/  # Business logic
│   ├── FahrzeugZulassung.Domain/       # Domain entities
│   └── FahrzeugZulassung.Infrastructure/ # Data access
├── Dockerfile.backend                   # Backend Docker image
├── docker-compose.yml                   # Multi-container orchestration
├── .env.example                         # Environment variables template
└── README.md                            # This file
```

## Troubleshooting

### Port already in use
```bash
# Change ports in docker-compose.yml
ports:
  - "5002:8080"  # Instead of 5001:8080
```

### Database connection failed
```bash
# Check PostgreSQL is running
docker-compose ps

# Check logs
docker-compose logs postgres

# Verify connection string in .env
```

### Migration errors
```bash
# Reset database (CAUTION: deletes all data)
docker-compose down -v
docker-compose up -d
```

## License

Proprietary

## Support

For issues and questions, please open an issue on GitHub.

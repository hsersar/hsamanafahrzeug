# Fahrzeugzulassungs-Webapp

Complete vehicle registration web application with ASP.NET Core 8 backend.

## ✅ Backend Status: COMPLETE

The complete ASP.NET Core 8 backend is fully implemented with Clean Architecture and ready for deployment.

### Features Implemented

#### Architecture
- **Clean Architecture** with 4 layers: Domain, Infrastructure, Application, API
- **Domain Layer**: 8 entities, 5 enums
- **Infrastructure Layer**: EF Core, PostgreSQL, file storage, iKFZ integration
- **Application Layer**: 25+ DTOs, 10 services, 9 validators, AutoMapper
- **API Layer**: 8 REST controllers, 2 middleware components

#### Security (BSI Compliant)
- ✅ JWT authentication (15min access tokens, 7-day refresh tokens)
- ✅ Strong password policy (min 12 chars, uppercase, lowercase, digit, special char)
- ✅ Account lockout (5 failed attempts = 15min lockout)
- ✅ Security headers (CSP, HSTS, X-Frame-Options, X-Content-Type-Options, etc.)
- ✅ Rate limiting (100 requests/minute per IP)
- ✅ CORS protection
- ✅ Audit logging for all data operations
- ✅ Role-based authorization (SuperAdmin, StandortAdmin, Mitarbeiter, Kunde)

#### API Endpoints
- **Auth**: Login, Register, Refresh Token, Logout
- **Standorte**: Full CRUD (SuperAdmin only)
- **Mitarbeiter**: Full CRUD, Activate/Deactivate (StandortAdmin)
- **Kunden**: Full CRUD (Mitarbeiter)
- **Aufträge**: CRUD, Status updates, iKFZ submission, wizard support
- **Rechnungen**: Create, View, Payment processing
- **Dokumente**: Upload, Download, Delete
- **Dashboard**: KPIs, recent orders, monthly revenue

#### Technical Stack
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- AutoMapper
- FluentValidation
- Serilog
- Swagger/OpenAPI
- JWT Bearer Authentication
- AspNetCoreRateLimit

### Build Status
✅ **0 errors, 0 warnings**

### Getting Started

#### Prerequisites
- .NET 8 SDK
- PostgreSQL 15+

#### Configuration

1. Update `appsettings.json` with your database connection string:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=FahrzeugZulassung;Username=your_user;Password=your_password"
}
```

2. Update JWT secret (production):
```json
"JWT": {
  "Secret": "your-secure-secret-key-min-32-chars"
}
```

#### Run

```bash
cd backend/src/FahrzeugZulassung.API
dotnet run
```

API will be available at:
- HTTPS: https://localhost:7001
- HTTP: http://localhost:5001
- Swagger: https://localhost:7001/swagger

#### Default Credentials

**SuperAdmin**:
- Email: admin@fahrzeugzulassung.de
- Password: Admin@123456789

### Next Steps (Not Yet Implemented)

- [ ] React frontend application
- [ ] Docker configuration
- [ ] GitHub Actions CI/CD
- [ ] Unit tests
- [ ] Complete documentation

## License

Proprietary
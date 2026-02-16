# Fahrzeug Zulassung API

BSI-compliant API for vehicle registration management.

## Configuration

### Security Requirements

**⚠️ IMPORTANT: The following configuration values MUST be externalized for production:**

1. **Database Connection String** - Set via environment variable:
   ```bash
   export ConnectionStrings__DefaultConnection="Host=prod-server;Port=5432;Database=fahrzeugzulassung;Username=app_user;Password=<secure-password>"
   ```

2. **JWT Secret** - Must be a cryptographically secure random string (minimum 32 characters):
   ```bash
   export JwtSettings__Secret="<generate-secure-random-string>"
   ```

### Environment Variables

For production deployment, configure these environment variables:

- `ConnectionStrings__DefaultConnection`: PostgreSQL connection string
- `JwtSettings__Secret`: JWT signing key (minimum 32 characters)
- `JwtSettings__Issuer`: Token issuer (default: FahrzeugZulassungAPI)
- `JwtSettings__Audience`: Token audience (default: FahrzeugZulassungClient)
- `JwtSettings__AccessTokenExpiration`: Access token lifetime in minutes (default: 15)
- `JwtSettings__RefreshTokenExpiration`: Refresh token lifetime in minutes (default: 43200 / 30 days)
- `CorsSettings__AllowedOrigins__0`: Frontend origin URL

### Development Setup

1. Update `appsettings.Development.json` with your local database credentials
2. Run migrations:
   ```bash
   dotnet ef database update
   ```
3. Run the application:
   ```bash
   dotnet run
   ```

## Security Features

### BSI-Compliant Security Headers
- Content-Security-Policy: default-src 'self'
- X-Frame-Options: DENY
- X-Content-Type-Options: nosniff
- Referrer-Policy: strict-origin-when-cross-origin
- Permissions-Policy: geolocation=(), microphone=(), camera=()
- Strict-Transport-Security: max-age=31536000; includeSubDomains

### Authentication & Authorization
- JWT Bearer token authentication
- Short-lived access tokens (15 minutes)
- Long-lived refresh tokens (30 days)
- Strong password policy:
  - Minimum 12 characters
  - Requires uppercase letter
  - Requires lowercase letter
  - Requires digit
  - Requires special character

### Rate Limiting
- 100 requests per minute per IP address
- Configurable per-endpoint rules
- Returns HTTP 429 (Too Many Requests) when exceeded

### Audit Logging
- All POST, PUT, DELETE operations are logged
- Captures: User ID, IP address, User-Agent, timestamp
- Stored in database via IAuditLogService

## Middleware Pipeline

The middleware is executed in the following order:

1. Serilog Request Logging
2. HTTPS Redirection
3. CORS
4. Rate Limiting
5. Authentication
6. Authorization
7. Security Headers
8. Audit Logging
9. Controllers

## API Documentation

Swagger UI is available at `/swagger` in development mode.

Bearer token authentication is required for protected endpoints.

## Logging

Logs are written to:
- Console (structured JSON in production)
- File: `logs/fahrzeug-zulassung-YYYY-MM-DD.log` (rolling daily)

Log retention: 30 days (configurable in appsettings.json)

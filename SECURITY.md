# Security Documentation

## Overview

This document describes the security measures implemented in the iKfz application to ensure compliance with ISO 27001, GDPR, and Security-by-Design principles.

## Authentication & Authorization

### OAuth2/OIDC Implementation

- **Protocol**: OAuth 2.0 with OpenID Connect
- **Token Type**: JWT (JSON Web Tokens)
- **Token Validation**: Issuer, Audience, Lifetime, and Signature validation
- **Token Storage**: 
  - Backend: Validated on each request via JWT Bearer middleware
  - Frontend: LocalStorage (demo) - should use HttpOnly cookies in production

### Authorization

- **Role-Based Access Control**: Users can only access their own data
- **Claims-Based**: User identity extracted from JWT claims
- **Endpoint Protection**: All API endpoints require authentication via `[Authorize]` attribute

## Data Protection

### GDPR Compliance

1. **Minimal Data Collection**: Only necessary vehicle registration data is collected
2. **Purpose Limitation**: Data is only used for vehicle registration purposes
3. **Data Portability**: API endpoints allow users to retrieve their data
4. **Right to Deletion**: Administrative functionality for data deletion
5. **Consent Management**: OAuth2 flow ensures explicit user consent

### Privacy-by-Design

- **No PII in Logs**: Serilog configured to exclude personal identifiable information
- **Data Minimization**: DTOs expose only necessary fields
- **Secure Defaults**: HTTPS enforced, secure headers enabled by default

## Security Headers

The application implements the following security headers:

```
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
X-XSS-Protection: 1; mode=block
Referrer-Policy: no-referrer
Content-Security-Policy: default-src 'self'; ...
```

## Secrets Management

### Best Practices

- ❌ **Never commit secrets to source control**
- ✅ Use environment variables for production
- ✅ Use `dotnet user-secrets` for local development
- ✅ Use cloud secret managers (Azure Key Vault, AWS Secrets Manager)

### Configuration Structure

```json
{
  "JwtSettings": {
    "Authority": "<from-environment>",
    "Audience": "<from-environment>",
    "Issuer": "<from-environment>"
  },
  "ConnectionStrings": {
    "DefaultConnection": "<from-environment>"
  }
}
```

### Environment Variables

Production deployments should use:
- `JwtSettings__Authority`
- `JwtSettings__Audience`
- `ConnectionStrings__DefaultConnection`

## Rate Limiting

Protection against DoS attacks and abuse:

- **Per IP**: 10 requests per second
- **Per IP**: 100 requests per minute
- **HTTP 429**: Too Many Requests response

Configuration in `Program.cs`:
```csharp
options.GeneralRules = new List<RateLimitRule>
{
    new RateLimitRule { Endpoint = "*", Period = "1s", Limit = 10 },
    new RateLimitRule { Endpoint = "*", Period = "1m", Limit = 100 }
};
```

## Database Security

### Entity Framework Core

- **Parameterized Queries**: Protection against SQL injection
- **Connection String Security**: Never hardcode credentials
- **Indexes on Sensitive Fields**: Efficient lookups for user data
- **No Direct SQL**: All queries through EF Core

### Data Validation

- **Model Validation**: DataAnnotations on DTOs
- **Input Sanitization**: Automatic via ASP.NET Core model binding
- **Length Limits**: Maximum lengths on all string fields

## Logging

### Serilog Configuration

- **Structured Logging**: JSON-formatted logs for easy parsing
- **No PII**: Personal data excluded from logs
- **Log Levels**: Information, Warning, Error
- **Sensitive Data Filtering**: User IDs logged, but not names/emails

Example log entry:
```
[2026-02-16 19:48:24 INF] Registration request created with ID: 123
```

## HTTPS/TLS

- **HTTPS Redirection**: All HTTP requests redirected to HTTPS
- **TLS 1.2+**: Minimum TLS version
- **Certificate Validation**: Proper certificate chain validation

## CORS

- **Configured Origins**: Only allowed frontend URL
- **Credentials Allowed**: For cookie-based auth (if implemented)
- **No Wildcard**: Specific origin configuration

```csharp
policy.WithOrigins("http://localhost:3000")
      .AllowAnyMethod()
      .AllowAnyHeader()
      .AllowCredentials();
```

## Input Validation

### Backend Validation

- Required fields marked with `[Required]`
- String length limits with `[StringLength]`
- Range validation with `[Range]`
- Custom validation logic in services

### Frontend Validation

- HTML5 input validation
- Client-side TypeScript type checking
- Server-side validation always performed

## Vulnerability Prevention

### Implemented Protections

1. **SQL Injection**: Entity Framework Core parameterized queries
2. **XSS**: React automatic escaping, CSP headers
3. **CSRF**: SameSite cookies, token-based auth
4. **Clickjacking**: X-Frame-Options: DENY
5. **MIME Sniffing**: X-Content-Type-Options: nosniff

## Monitoring & Auditing

### Health Checks

Endpoint: `/health`
- Database connectivity
- Application status

### Logging

All security events logged:
- Authentication failures
- Authorization failures
- Rate limit exceeded
- Validation errors

## Incident Response

### Procedures

1. **Detection**: Monitor logs for suspicious activity
2. **Containment**: Rate limiting, IP blocking
3. **Investigation**: Review structured logs
4. **Recovery**: Restore from backups if needed
5. **Lessons Learned**: Update security measures

## Compliance Checklist

### ISO 27001

- ✅ Access Control
- ✅ Cryptography
- ✅ Operations Security
- ✅ Communications Security
- ✅ System Acquisition, Development and Maintenance
- ✅ Supplier Relationships
- ✅ Information Security Incident Management
- ✅ Information Security Aspects of Business Continuity Management

### GDPR

- ✅ Lawfulness, fairness and transparency
- ✅ Purpose limitation
- ✅ Data minimisation
- ✅ Accuracy
- ✅ Storage limitation
- ✅ Integrity and confidentiality
- ✅ Accountability

## Security Testing

### Recommended Tests

1. **Penetration Testing**: Annual or after major changes
2. **Vulnerability Scanning**: Automated tools (OWASP ZAP, Burp Suite)
3. **Code Analysis**: Static code analysis (SonarQube)
4. **Dependency Scanning**: Check for vulnerable packages
5. **Authentication Testing**: OAuth2 flow, token validation

## Updates & Patches

### Dependency Management

- Regularly update NuGet packages
- Regularly update npm packages
- Subscribe to security advisories
- Test updates in staging before production

### Commands

```bash
# Backend
dotnet list package --outdated
dotnet add package <package> --version <version>

# Frontend
npm outdated
npm update
npm audit fix
```

## Contact

For security issues, please contact the security team immediately.

**DO NOT** open public GitHub issues for security vulnerabilities.

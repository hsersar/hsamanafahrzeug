# iKfz - Internet-based Vehicle Registration System

A secure, scalable, and GDPR-compliant web application for vehicle registration built with C# .NET backend and React TypeScript frontend.

## 🔒 Security & Compliance Features

- **OAuth2/OIDC Authentication**: Secure user authentication using industry-standard protocols
- **ISO 27001 Compliant**: Following information security management best practices
- **GDPR Compliant**: No personal data in logs, privacy-by-design principles
- **Security-by-Design**: Built with security as a foundational requirement
- **No Secrets in Code**: All sensitive configuration externalized
- **Security Headers**: X-Content-Type-Options, X-Frame-Options, CSP, etc.
- **Rate Limiting**: Protection against abuse and DoS attacks
- **HTTPS Enforcement**: Encrypted communication in production

## ⚡ Performance Features

- **Async Operations**: All database operations are asynchronous
- **Caching**: Memory cache implementation for frequently accessed data
- **Efficient DB Access**: Entity Framework Core with optimized queries
- **Scalable**: Supports ≥100 parallel requests
- **Health Checks**: Built-in health monitoring endpoints

## 🏗️ Architecture

### Backend (C# .NET 10.0)
- **ASP.NET Core Web API**: RESTful API design
- **Entity Framework Core**: ORM with SQLite (demo) / SQL Server (production)
- **Repository Pattern**: Clean separation of concerns
- **Service Layer**: Business logic with caching
- **Dependency Injection**: Built-in DI container
- **Serilog**: Structured logging without PII
- **Swagger/OpenAPI**: API documentation

### Frontend (React + TypeScript)
- **React 18**: Modern UI library
- **TypeScript**: Type-safe development
- **React Router**: Client-side routing
- **Axios**: HTTP client with interceptors
- **Responsive Design**: Mobile-friendly interface

## 📁 Project Structure

```
hsamanafahrzeug/
├── backend/
│   └── iKfz.Backend/
│       ├── Controllers/        # API endpoints
│       ├── Models/             # Domain entities
│       ├── DTOs/               # Data transfer objects
│       ├── Data/               # Database context
│       ├── Repositories/       # Data access layer
│       ├── Services/           # Business logic
│       └── Program.cs          # Application configuration
└── frontend/
    └── src/
        ├── components/         # React components
        ├── pages/              # Page components
        ├── services/           # API client
        └── types/              # TypeScript types
```

## 🚀 Getting Started

### Prerequisites

- .NET 10.0 SDK
- Node.js 18+ and npm
- (Optional) SQL Server for production

### Backend Setup

1. Navigate to backend directory:
```bash
cd backend/iKfz.Backend
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Configure settings (create `appsettings.Development.json`):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=ikfz.db"
  },
  "JwtSettings": {
    "Authority": "https://your-identity-provider.com",
    "Audience": "ikfz-api"
  }
}
```

4. Run the backend:
```bash
dotnet run
```

The API will be available at `https://localhost:7000` (or configured port).

### Frontend Setup

1. Navigate to frontend directory:
```bash
cd frontend
```

2. Install dependencies:
```bash
npm install
```

3. Create `.env.local` file:
```
REACT_APP_API_URL=https://localhost:7000/api
```

4. Run the development server:
```bash
npm start
```

The application will be available at `http://localhost:3000`.

## 🔐 Security Configuration

### OAuth2/OIDC Setup

The application is designed to work with any OAuth2/OIDC provider (e.g., Azure AD, Auth0, Keycloak).

**Backend Configuration** (`appsettings.json`):
```json
{
  "JwtSettings": {
    "Authority": "https://your-identity-provider.com",
    "Audience": "ikfz-api",
    "Issuer": "https://your-identity-provider.com"
  }
}
```

**Frontend Configuration** (`.env.local`):
```
REACT_APP_AUTH_AUTHORITY=https://your-identity-provider.com
REACT_APP_AUTH_CLIENT_ID=your-client-id
REACT_APP_AUTH_REDIRECT_URI=http://localhost:3000/callback
```

### Secrets Management

**IMPORTANT**: Never commit secrets to version control!

- Use environment variables for production
- Use Azure Key Vault, AWS Secrets Manager, or similar in cloud environments
- Use `dotnet user-secrets` for local development:
  ```bash
  dotnet user-secrets set "JwtSettings:Authority" "your-value"
  ```

## 📊 Database Migrations

```bash
cd backend/iKfz.Backend
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## 🧪 Testing

### Backend Tests
```bash
cd backend/iKfz.Backend
dotnet test
```

### Frontend Tests
```bash
cd frontend
npm test
```

## 🌐 API Endpoints

### Registration
- `POST /api/registration` - Submit new registration request
- `GET /api/registration/{id}` - Get registration request details
- `GET /api/registration/my-requests` - Get all user's requests

### Vehicles
- `GET /api/vehicles/my-vehicles` - Get all user's vehicles
- `GET /api/vehicles/{id}` - Get vehicle details

### Health
- `GET /health` - Health check endpoint

## 🛡️ GDPR Compliance

The system is designed with privacy-by-design principles:

1. **Minimal Data Collection**: Only necessary data is collected
2. **No PII in Logs**: Logging configuration excludes personal identifiable information
3. **User Consent**: OAuth2 flow ensures proper user authorization
4. **Data Portability**: API endpoints allow users to export their data
5. **Right to Deletion**: Administrative endpoints for data deletion (to be implemented)

## 📈 Scalability

The application is designed to handle high load:

- **Async/Await**: All I/O operations are non-blocking
- **Caching**: 10-minute cache for frequently accessed data
- **Rate Limiting**: 100 requests per minute per IP
- **Database Indexing**: Optimized queries with proper indexes
- **Stateless API**: Can be horizontally scaled

## 🔧 Configuration Options

### Rate Limiting
Edit `Program.cs` to adjust rate limits:
```csharp
options.GeneralRules = new List<RateLimitRule>
{
    new RateLimitRule { Endpoint = "*", Period = "1s", Limit = 10 },
    new RateLimitRule { Endpoint = "*", Period = "1m", Limit = 100 }
};
```

### Caching
Adjust cache expiration in `VehicleService.cs`:
```csharp
private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(10);
```

## 📝 License

This project is created as a demonstration of secure, scalable web application development.

## 🤝 Contributing

This is a demonstration project. For production use:
1. Implement comprehensive unit and integration tests
2. Set up CI/CD pipeline
3. Configure production-grade OAuth2/OIDC provider
4. Use production database (SQL Server, PostgreSQL)
5. Implement comprehensive error handling
6. Add monitoring and alerting
7. Perform security audit and penetration testing

## 📞 Support

For questions or issues, please open an issue in the repository.
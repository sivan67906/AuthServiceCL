# AuthService - Enterprise Authentication Microservice

A comprehensive .NET 9 WebAPI microservice for authentication and authorization, built with Vertical Slice Architecture, CQRS pattern, and dual database support.

## Features

### Authentication & Authorization
- User Registration with Email Confirmation
- Login with JWT Access & Refresh Tokens
- Password Management (Forgot/Reset/Change)
- Two-Factor Authentication (2FA) with QR Code
- External Authentication (Google, Microsoft)
- Role-Based Authorization
- Token Refresh & Revocation

### User Management
- User Profile Management
- User Address CRUD Operations
- Account Management

## Technology Stack

- **.NET 9** - Latest framework
- **ASP.NET Core Web API** - RESTful API
- **Entity Framework Core 9.0** - ORM
- **SQL Server** - Command database
- **PostgreSQL** - Query database
- **MediatR 12.4** - CQRS implementation
- **FluentValidation 11.9** - Input validation
- **Mapster 7.4** - Object mapping
- **Serilog 4.1** - Structured logging
- **JWT Bearer** - Authentication
- **xUnit** - Unit testing
- **.NET Aspire 9.1** - Orchestration

## Architecture

The solution follows **Vertical Slice Architecture** with clear separation of concerns:

- **Domain Layer**: Entities, Value Objects, Domain Interfaces
- **Application Layer**: CQRS Handlers, DTOs, Validators, Application Logic
- **Infrastructure Layer**: Database Contexts, Repositories, External Services
- **API Layer**: Controllers, Middleware, Configuration

### Dual Database Strategy
- **SQL Server**: Command database for all write operations
- **PostgreSQL**: Query database for read operations

## Getting Started

### Prerequisites
- .NET 9 SDK
- SQL Server (LocalDB or Full)
- PostgreSQL
- Visual Studio 2022 or Rider

### Configuration

Update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "CommandDatabase": "Server=(localdb)\\mssqllocaldb;Database=AuthServiceCommand;Trusted_Connection=true;",
    "QueryDatabase": "Host=localhost;Port=5432;Database=AuthServiceQuery;Username=postgres;Password=yourpassword;"
  },
  "JwtSettings": {
    "SecretKey": "Your-Minimum-32-Character-Secret-Key",
    "Issuer": "AuthService",
    "Audience": "AuthServiceClients",
    "AccessTokenExpirationMinutes": 60
  },
  "Authentication": {
    "Google": {
      "ClientId": "your-google-client-id",
      "ClientSecret": "your-google-client-secret"
    },
    "Microsoft": {
      "ClientId": "your-microsoft-client-id",
      "ClientSecret": "your-microsoft-client-secret"
    }
  }
}
```

### Running the Application

1. **Restore packages**:
   ```bash
   dotnet restore
   ```

2. **Apply database migrations**:
   ```bash
   cd src/AuthService.API
   dotnet ef database update --context CommandDbContext
   dotnet ef database update --context QueryDbContext
   ```

3. **Run the API**:
   ```bash
   dotnet run --project src/AuthService.API
   ```

4. **Access Swagger UI**:
   ```
   https://localhost:5001/swagger
   ```

### Using .NET Aspire

```bash
dotnet run --project orchestration/AuthService.AppHost
```

### Default Admin Account

- Email: `admin@authservice.com`
- Password: `Admin@123456`

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/verify-email` - Verify email
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh-token` - Refresh access token
- `POST /api/auth/forgot-password` - Request password reset
- `POST /api/auth/reset-password` - Reset password
- `POST /api/auth/change-password` - Change password (requires authentication)

### User Profile
- `GET /api/user/profile` - Get user profile
- `PUT /api/user/profile` - Update user profile

### Two-Factor Authentication
- `POST /api/twofactor/enable` - Enable 2FA
- `POST /api/twofactor/verify` - Verify 2FA code
- `GET /api/twofactor/qrcode` - Get QR code

### Health Check
- `GET /health` - Application health status

## Database Migrations

### Create Migration
```bash
# Command Database (SQL Server)
dotnet ef migrations add MigrationName --project src/AuthService.Infrastructure --startup-project src/AuthService.API --context CommandDbContext

# Query Database (PostgreSQL)
dotnet ef migrations add MigrationName --project src/AuthService.Infrastructure --startup-project src/AuthService.API --context QueryDbContext
```

### Apply Migrations
```bash
dotnet ef database update --project src/AuthService.Infrastructure --startup-project src/AuthService.API --context CommandDbContext
dotnet ef database update --project src/AuthService.Infrastructure --startup-project src/AuthService.API --context QueryDbContext
```

## Testing

```bash
dotnet test
```

## Docker Support

### Build and Run
```bash
docker-compose up --build
```

## Security Features

- Password hashing with Identity
- JWT token-based authentication
- Refresh token rotation
- Account lockout protection
- Email confirmation required
- Two-factor authentication
- Role-based authorization

## License

MIT License

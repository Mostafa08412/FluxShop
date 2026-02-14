# FluxStore API

A .NET 10 Web API built with **Clean Architecture**, **CQRS** (MediatR), and **JWT Authentication**.

---

## 🏗️ Project Structure

```
src/
├── FluxStore.API/                  # Presentation Layer
│   ├── Controllers/v1/             # Versioned API controllers
│   ├── Contracts/                  # API routes, response models
│   ├── Infrastructure/             # Base controller, status codes, response helpers
│   └── Middleware/                  # Exception handling, authentication error handling
│
├── FluxStore.Application/          # Application Layer
│   ├── Auth/                       # Authentication feature (commands & handlers)
│   │   ├── Login/                  # Email & password login
│   │   ├── Register/               # User registration
│   │   ├── GoogleLogin/            # OAuth2 Google login
│   │   ├── ChangePassword/         # Change password (authenticated)
│   │   ├── ForgetPassword/         # Request password reset OTP
│   │   ├── VerifyResetPasswordOtp/ # Verify OTP & get reset token
│   │   ├── ResetPassword/          # Reset password with token
│   │   ├── RefreshToken/           # JWT token refresh
│   │   ├── Logout/                 # Revoke refresh token
│   │   ├── SendEmail/              # Generic email sending
│   │   └── Common/                 # Shared DTOs (AuthenticationResponse, tokens)
│   ├── Users/                      # User management (CRUD, activate, deactivate)
│   ├── Common/                     # Cross-cutting concerns
│   │   ├── Interfaces/             # IIdentityService, IEmailService, IBackgroundJobWorker
│   │   ├── Behaviors/              # MediatR pipeline (validation, UoW transaction)
│   │   └── Errors/                 # Application-level error definitions
│   └── Contracts/                  # Identity DTOs, CSV models
│
├── FluxStore.Domain/               # Domain Layer
│   ├── Core/Primitives/            # Result, Result<T>, Error, ValueObject
│   ├── Core/Errors/                # Domain error definitions
│   ├── Enums/                      # Roles (User, Manager, Admin)
│   └── Abstractions/               # Domain interfaces
│
└── FluxStore.Infrastructure/       # Infrastructure Layer
    ├── Authentication/             # IdentityService (ASP.NET Identity + Google OAuth)
    ├── Persistence/                # EF Core DbContext, repositories, background jobs
    ├── Tokens/                     # JWT access token & refresh token generation
    ├── EmailServices/              # FluentEmail SMTP integration
    └── FileManager/                # File management services

tests/
└── Domain.UnitTests/               # Domain layer unit tests
```

---

## 🔐 Auth Endpoints

Base URL: `api/v1/auth`

### Overview

| Method | Endpoint                    | Auth       | Description                                  |
|--------|-----------------------------|------------|----------------------------------------------|
| POST   | `/login`                    | Anonymous  | Login with email & password                  |
| POST   | `/register`                 | Anonymous  | Register a new user                          |
| POST   | `/google-login`             | Anonymous  | Login/register with Google OAuth             |
| POST   | `/refresh-token`            | Anonymous  | Refresh an expired access token              |
| POST   | `/forget-password`          | Anonymous  | Request a password reset OTP via email       |
| POST   | `/verify-reset-password-otp`| Anonymous  | Verify OTP and receive a reset token         |
| POST   | `/reset-password`           | Anonymous  | Reset password using the reset token         |
| POST   | `/change-password`          | Authorized | Change password for the logged-in user       |
| POST   | `/logout`                   | Authorized | Revoke the active refresh token              |

---

### `POST /login`

Authenticates a user with email and password.

**Request Body:**
```json
{
  "emailAddress": "user@example.com",
  "password": "YourPassword123!"
}
```

**Success Response** `200`:
```json
{
  "isSuccess": true,
  "message": "",
  "errorCode": "",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/login",
  "traceId": "...",
  "data": {
    "user": {
      "id": "user-id",
      "firstName": "John",
      "lastName": "Doe",
      "fullName": "John Doe",
      "email": "user@example.com",
      "userName": "user@example.com",
      "roles": ["User"]
    },
    "accessToken": {
      "token": "eyJhbGciOiJIUzI1NiIs..."
    },
    "refreshToken": {
      "token": "random-refresh-token-string"
    }
  }
}
```

---

### `POST /register`

Creates a new user account.

**Request Body:**
```json
{
  "name": "John Doe",
  "emailAddress": "user@example.com",
  "password": "YourPassword123!"
}
```

**Success Response** `200`:
```json
{
  "isSuccess": true,
  "data": null
}
```

---

### `POST /google-login`

Authenticates using a Google ID token. If the user doesn't exist, a new account is created automatically with the `User` role.

**Request Body:**
```json
{
  "idToken": "eyJhbGciOiJSUzI1NiIs..."
}
```

**Success Response** `200`:
```json
{
  "isSuccess": true,
  "data": {
    "user": {
      "id": "user-id",
      "firstName": "John",
      "lastName": "Doe",
      "fullName": "John Doe",
      "email": "user@example.com",
      "userName": "user@example.com",
      "roles": ["User"]
    },
    "accessToken": {
      "token": "eyJhbGciOiJIUzI1NiIs..."
    },
    "refreshToken": {
      "token": "random-refresh-token-string"
    }
  }
}
```

---

### `POST /refresh-token`

Obtains a new JWT access token using a valid refresh token.

**Request Body:**
```json
{
  "refreshToken": "existing-refresh-token"
}
```

**Success Response** `200`:
```json
{
  "isSuccess": true,
  "data": {
    "user": { ... },
    "accessToken": { "token": "new-access-token" },
    "refreshToken": { "token": "existing-refresh-token" }
  }
}
```

---

### `POST /forget-password`

Generates a one-time password (OTP) and sends it to the user's email via a **Hangfire background job**.

**Request Body:**
```json
{
  "emailAddress": "user@example.com"
}
```

**Success Response** `200`:
```json
{
  "isSuccess": true
}
```

---

### `POST /verify-reset-password-otp`

Validates the OTP and returns a password reset token.

**Request Body:**
```json
{
  "emailAddress": "user@example.com",
  "otp": "123456"
}
```

**Success Response** `200`:
```json
{
  "isSuccess": true,
  "data": {
    "resetPasswordToken": "CfDJ8N..."
  }
}
```

---

### `POST /reset-password`

Resets the user's password using the token obtained from OTP verification.

**Request Body:**
```json
{
  "emailAddress": "user@example.com",
  "resetPasswordToken": "CfDJ8N...",
  "newPassword": "NewPassword123!"
}
```

**Success Response** `200`:
```json
{
  "isSuccess": true
}
```

---

### `POST /change-password` 🔒

Changes the password for the currently authenticated user.

**Headers:** `Authorization: Bearer <access-token>`

**Request Body:**
```json
{
  "currentPassword": "OldPassword123!",
  "newPassword": "NewPassword456!",
  "confirmNewPassword": "NewPassword456!"
}
```

**Success Response** `200`:
```json
{
  "isSuccess": true
}
```

---

### `POST /logout` 🔒

Revokes the user's active refresh token.

**Headers:** `Authorization: Bearer <access-token>`

**Request Body:**
```json
{}
```

**Success Response** `200`:
```json
{
  "isSuccess": true
}
```

---

## ❌ Error Responses

All endpoints return a consistent error format:

**Validation Error** `400`:
```json
{
  "isSuccess": false,
  "message": "One or more validation errors have occurred.",
  "errorCode": "VALIDATION_ERROR",
  "validationErrors": {
    "EmailAddress": "The Email Address field is required."
  },
  "instance": "/api/v1/auth/login",
  "traceId": "..."
}
```

**Authentication Error** `401`:
```json
{
  "isSuccess": false,
  "message": "The provided credentials are invalid.",
  "errorCode": "Identity.InvalidCredentials",
  "validationErrors": {},
  "instance": "/api/v1/auth/login",
  "traceId": "..."
}
```

**Not Found** `404`:
```json
{
  "isSuccess": false,
  "message": "No user was found with email 'user@example.com'.",
  "errorCode": "Identity.UserNotFound.Email",
  "validationErrors": {},
  "instance": "/api/v1/auth/forget-password",
  "traceId": "..."
}
```

---

## 🔄 Password Reset Flow

```
Client                          API                         Hangfire
  │                              │                             │
  ├─ POST /forget-password ─────►│                             │
  │                              ├─ Generate OTP               │
  │                              ├─ Enqueue email job ─────────►│
  │◄─────── 200 OK ─────────────┤                             │
  │                              │                 Send email ─►│
  │                              │                             │
  ├─ POST /verify-reset-otp ────►│                             │
  │                              ├─ Verify OTP                 │
  │                              ├─ Generate reset token       │
  │◄─── 200 { resetToken } ─────┤                             │
  │                              │                             │
  ├─ POST /reset-password ──────►│                             │
  │                              ├─ Reset password             │
  │◄─────── 200 OK ─────────────┤                             │
```

---

## 🔑 Google OAuth Flow

```
Client                          API                       Google
  │                              │                           │
  │─ Google Sign-In ────────────────────────────────────────►│
  │◄──────────── ID Token ──────────────────────────────────┤
  │                              │                           │
  ├─ POST /google-login ────────►│                           │
  │                              ├─ Validate ID token ──────►│
  │                              │◄─ Payload (name, email) ──┤
  │                              ├─ Find or create user      │
  │                              ├─ Issue JWT + refresh token │
  │◄── 200 { user, tokens } ────┤                           │
```

---

## ⚙️ Tech Stack

| Layer            | Technology                                              |
|------------------|---------------------------------------------------------|
| Runtime          | .NET 10 / C# 14                                        |
| API              | ASP.NET Core, API Versioning, Swagger/Swashbuckle       |
| Architecture     | Clean Architecture, CQRS                                |
| Mediator         | MediatR                                                 |
| Validation       | FluentValidation (pipeline behavior)                    |
| Authentication   | JWT Bearer, ASP.NET Identity, Google OAuth              |
| Database         | SQL Server (LocalDB), Entity Framework Core             |
| Background Jobs  | Hangfire (SQL Server storage)                           |
| Email            | FluentEmail (SMTP)                                      |
| Caching          | IMemoryCache (OTP throttling)                           |
| Logging          | Serilog (Console, File, Seq)                            |
| Containers       | Docker, Docker Compose                                  |

---

## 🐳 Run with Docker Compose

### Prerequisites

- [Docker](https://docs.docker.com/get-docker/) installed and running

### Quick Start

**1. Create a `docker-compose.yml`** file in your project root:

```yaml
version: '3.8'

services:
  backend:
    container_name: fluxstore-service
    image: mostafa0841/fluxstore:v1.0
    environment:
      - ConnectionStrings__DefaultConnection=Server=sqlserver,1433;Database=FluxStoreDb;User Id=SA;Password=Admin#123;TrustServerCertificate=True
      # Token Settings
      - TokenSettings__SecretKey=Yj6pr{WJ+c}WL:Zmc%v364$$jkOi}O3HM_ExtraLongKey123
      - TokenSettings__Issuer=http://localhost:5089
      - TokenSettings__Audience=http://localhost:5089
      - TokenSettings__AccessTokenExpiryMinutes=1440
      - TokenSettings__RefreshTokenExpiryMinutes=43200
      - ASPNETCORE_URLS=http://+:5089
      - ASPNETCORE_ENVIRONMENT=Production
      # Database Initialization
      - InitializeDatabase__ResetDatabase=false
      - InitializeDatabase__InitializeDatabase=true
      - InitializeDatabase__SeedData=true
      # Google OAuth
      - Authentication__Google__ClientId=159007714113-l1nka853t90rqrej8eu4h1sc0lg2kjiv.apps.googleusercontent.com
      # SMTP Settings
      - SmtpSettings__SmtpHost=smtp4dev
      - SmtpSettings__SmtpPort=25
      - SmtpSettings__UseSSL=false
      - SmtpSettings__FromEmail=FluxStore@dev.com
      - SmtpSettings__Password=hfgclegezffgqvus
      # CORS Settings
      - CorsSettings__PolicyName=DevCorsPolicy
      - CorsSettings__AllowedOrigins__0=http://localhost:5341
      - CorsSettings__AllowedOrigins__1=http://localhost:3000
      # File Manager
      - FileManager__TempCsvPath=/Temp
      # SignalR Hub Settings
      - HubSettings__ImportProducts__Status=/hubs/import-status
      - HubSettings__ImportProducts__OnPreviewReady=OnPreviewReady
      - HubSettings__ImportProducts__OnImportCompleted=OnImportCompleted
      - HubSettings__ImportProducts__OnJobFailed=OnJobFailed
      - HubSettings__ImportProducts__OnProgress=OnProgress
      # Serilog
      - Serilog__Using__0=Serilog.Sinks.Console
      - Serilog__Using__1=Serilog.Sinks.File
      - Serilog__Using__2=Serilog.Sinks.Seq
      - Serilog__MinimumLevel__Default=Information
      - Serilog__MinimumLevel__Override__Microsoft=Warning
      - Serilog__MinimumLevel__Override__Microsoft.Hosting.Lifetime=Information
      - Serilog__MinimumLevel__Override__Microsoft.EntityFrameworkCore=Warning
      - Serilog__MinimumLevel__Override__System=Warning
      - Serilog__WriteTo__0__Name=Console
      - Serilog__WriteTo__0__Args__formatter=Serilog.Formatting.Json.JsonFormatter, Serilog
      - Serilog__WriteTo__1__Name=File
      - Serilog__WriteTo__1__Args__path=/app/logs/FluxStore-.json
      - Serilog__WriteTo__1__Args__rollingInterval=Day
      - Serilog__WriteTo__1__Args__retainedFileCountLimit=7
      - Serilog__WriteTo__1__Args__formatter=Serilog.Formatting.Json.JsonFormatter, Serilog
      - Serilog__WriteTo__2__Name=Seq
      - Serilog__WriteTo__2__Args__serverUrl=http://seqserver:5341
      - Serilog__Enrich__0=FromLogContext
      - Serilog__Enrich__1=WithMachineName
      - Serilog__Enrich__2=WithThreadId
      - Serilog__Enrich__3=WithEnvironmentName
      - Serilog__Properties__Application=FluxStore
    ports:
      - "5089:5089"
    depends_on:
      - sqlserver
      - seqserver
    networks:
      - backend-network
    volumes:
      - appdata:/app/EmailServices/EmailTemplates/

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: sqlserver
    environment:
      ACCEPT_EULA: "Y"
      MSSQL_SA_PASSWORD: "Admin#123"
    ports:
      - "1433:1433"
    volumes:
      - sqlserverdata:/var/opt/mssql
    networks:
      - backend-network

  seqserver:
    image: datalust/seq
    container_name: seqserver
    ports:
      - "5341:80"
    environment:
      - ACCEPT_EULA=Y
      - SEQ_FIRSTRUN_ADMINPASSWORD=Admin#123
    networks:
      - backend-network

  smtp4dev:
    image: rnwood/smtp4dev
    container_name: smtpserver
    ports:
      - "2525:25"
      - "3000:80"
    networks:
      - backend-network

volumes:
  sqlserverdata:
  appdata:

networks:
  backend-network:
```

**2. Start all services:**

```bash
docker compose up -d
```

**3. Verify everything is running:**

```bash
docker compose ps
```

### Services & Ports

| Service        | Container          | Port(s)              | Description                         |
|----------------|--------------------|----------------------|-------------------------------------|
| **Backend**    | fluxstore-service  | `5089`               | FluxStore API                       |
| **SQL Server** | sqlserver          | `1433`               | Database (SA password: `Admin#123`) |
| **Seq**        | seqserver          | `5341`               | Structured log viewer               |
| **smtp4dev**   | smtpserver         | `3000` (UI), `2525`  | Dev SMTP server & web inbox         |

### Access Points

| Resource             | URL                           |
|----------------------|-------------------------------|
| API                  | http://localhost:5089         |
| Seq Dashboard        | http://localhost:5341          |
| smtp4dev Inbox       | http://localhost:3000          |

### Useful Commands

```bash
# Stop all services
docker compose down

# Stop and remove volumes (reset database)
docker compose down -v

# View backend logs
docker compose logs -f backend

# Restart only the backend
docker compose restart backend
```

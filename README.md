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

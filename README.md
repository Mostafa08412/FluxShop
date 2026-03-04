# FluxStore API

A .NET 10 Web API using **Vertical Slice Architecture**, **CQRS** (MediatR), and **JWT Authentication**.

## Table of Contents

- [Overview](#fluxstore-api)
- [Project Structure](#-project-structure)
- [Auth Endpoints](#-auth-endpoints)
  - Register, Login, Refresh, Change Password, Logout
  - Forget Password → Verify OTP → Reset Password
  - Google Login
- [Password Reset Flow](#-password-reset-flow)
- [Google OAuth Flow](#-google-oauth-flow)
- [Tech Stack](#-tech-stack)
- [Docker Compose Setup](#-run-with-docker-compose)
  - Quick Start
  - Services & Ports
  - Access URLs
  - Useful Commands

---



## 🏗️ Project Structure

```
src/
└── FluxStore.Api/                  # Single Project Architecture (VSA)
    ├── Domain/                     # Enterprise logic, Entities, ValueObjects, Enums, Exceptions
    ├── Features/                   # Vertical Slices organized by module
    │   └── Authentication/         # Authentication Feature
    │       ├── Login/
    │       ├── Register/
    │       ├── GoogleLogin/
    │       ├── ChangePassword/
    │       ├── ForgetPassword/
    │       ├── VerifyOTP/
    │       ├── ResetPassword/
    │       ├── Refresh/
    │       └── Logout/
    ├── Infrastructure/             # Persistence (DbContext, Configurations) and external Services
    └── Shared/                     # Cross-cutting concerns
        ├── Abstractions/           # Interfaces and Base classes
        ├── Behaviors/              # MediatR pipeline behaviors
        ├── Errors/                 # Application-level error definitions
        ├── Extensions/             # Extension methods
        ├── Middlewares/            # Global Exception Handling
        └── Settings/               # Configuration settings classes
```

---

## 🔐 Auth Endpoints

Base URL: `api/v1/auth`

| Method | Endpoint                    | Auth       | Description                                  |
|--------|-----------------------------|------------|----------------------------------------------|
| POST | `/register` | Anonymous | Registers a new user account and returns JWT + refresh tokens. |
| POST | `/login` | Anonymous | Authenticates a user and returns JWT + refresh tokens. |
| POST | `/refresh-token` | Anonymous | Refreshes the JWT token using a valid refresh token. |
| POST | `/change-password` | Anonymous | Changes password for the authenticated user. |
| POST | `/logout` | Anonymous | Logs out the user by revoking the active refresh token. |
| POST | `/forget-password` | Anonymous | Sends a password reset OTP to the user's email. |
| POST | `/verify-reset-password-otp` | Anonymous | Verifies the OTP and returns a reset password token. |
| POST | `/reset-password` | Anonymous | Resets the user's password using the token obtained from OTP verification. |
| POST | `/google-login` | Anonymous | Authenticates an existing user using a valid Google ID token. |

---

### `POST /register`

Registers a new user account and returns JWT + refresh tokens.

**Request Body:**
```json
{
  "name": "John Doe",
  "emailAddress": "john.doe@example.com",
  "password": "StrongP@ssw0rd!",
  "confirmPassword" : "StrongP@ssw0rd!"
}
```

**Register - Success**:
*200 - Registration Successful* `200`:
```json
{
    "data": {
        "userId": "6933d253-8584-40cc-0777-08de76dc9e7a",
        "email": "john.doe@example.com",
        "userName": "john.doe@example.com",
        "firstName": "John",
        "lastName": "Doe",
        "roles": [
            "User"
        ],
        "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjY5MzNkMjUzLTg1ODQtNDBjYy0wNzc3LTA4ZGU3NmRjOWU3YSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6ImpvaG4uZG9lQGV4YW1wbGUuY29tIiwiRmlyc3ROYW1lIjoiSm9obiIsIkxhc3ROYW1lIjoiRG9lIiwic3ViIjoiNjkzM2QyNTMtODU4NC00MGNjLTA3NzctMDhkZTc2ZGM5ZTdhIiwianRpIjoiZTQ4N2NlODEtN2U2MS00MmExLWIxYzktZWEzZTNiY2VhYmYzIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiVXNlciIsImV4cCI6MTc3MjMwMTUwNiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MTE3IiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MTE3In0.Fx9FOjYjGI-bsocOPHanw_1sASsXH5KNvXGpYUZ_KYY",
        "refreshToken": "F3on13NHpd0lWql9HJtSiz9IjkBvYk4HBskVS2n2Hsni7tBgzW6U+Vzs6a4DmKknaX9tKb6Y4mRjOhNk7B/ikg=="
    },
    "isSuccess": true,
    "message": "",
    "errorCode": "",
    "statusCode": 200,
    "validationErrors": {},
    "meta": {},
    "instance": "/api/v1/auth/register",
    "traceId": "0HNJMRIEUNQ7N:00000001"
}
```

*400 - Validation Error (all fields empty)* `400`:
```json
{
    "data": null,
    "isSuccess": false,
    "message": "One or more validation errors occurred.",
    "errorCode": "VALIDATION_ERROR",
    "statusCode": 400,
    "validationErrors": {
        "name": "Name is required.",
        "emailAddress": "Email is required",
        "password": "Password is required.",
        "confirmPassword": "Confirm new password is required."
    },
    "meta": {},
    "instance": "/api/v1/auth/register",
    "traceId": "0HNJMRIEUNQ7P:00000002"
}
```

*400 - Validation Error (invalid email format)* `400`:
```json
{
    "data": null,
    "isSuccess": false,
    "message": "One or more validation errors occurred.",
    "errorCode": "VALIDATION_ERROR",
    "statusCode": 400,
    "validationErrors": {
        "emailAddress": "The email format is invalid. Please enter a valid email address."
    },
    "meta": {},
    "instance": "/api/v1/auth/register",
    "traceId": "0HNJMRIEUNQ7P:00000003"
}
```

*400 - Validation Error (invalid name format)* `400`:
```json
{
    "data": null,
    "isSuccess": false,
    "message": "One or more validation errors occurred.",
    "errorCode": "VALIDATION_ERROR",
    "statusCode": 400,
    "validationErrors": {
        "name": "Name must consist of first name and last name separated by a space."
    },
    "meta": {},
    "instance": "/api/v1/auth/register",
    "traceId": "0HNJMRIEUNQ7R:00000001"
}
```

*400 - Validation Error (Weak Password)* `400`:
```json
{
    "data": null,
    "isSuccess": false,
    "message": "One or more validation errors occurred.",
    "errorCode": "VALIDATION_ERROR",
    "statusCode": 400,
    "validationErrors": {
        "Password": "The password must contain at least one non-alphanumeric character."
    },
    "meta": {},
    "instance": "/api/v1/auth/register",
    "traceId": "0HNJMRIEUNQ7R:00000007"
}
```

*409 - Email Already Exists* `409`:
```json
{
    "data": null,
    "isSuccess": false,
    "message": "This email address is already registered. Please use a different email or sign in.",
    "errorCode": "Identity_EmailAlreadyExists",
    "statusCode": 409,
    "validationErrors": {},
    "meta": {},
    "instance": "/api/v1/auth/register",
    "traceId": "0HNJMRIEUNQ7T:00000007"
}
```

**Register - Empty Fields**:
**Register - Invalid Email Format**:
**Register - Invalid Name (single word)**:
**Register - Invalid Name (three words)**:
**Register - Weak Password**:
**Register - Duplicate Email**:
### `POST /login`

Authenticates a user and returns JWT + refresh tokens.

**Request Body:**
```json
{
  "emailAddress": "john.doe@example.com",
  "password": "StrongP@ssw0rd!"
}
```

**Login - Success**:
*200 - Login Successful* `200`:
```json
{
  "isSuccess": true,
  "message": "",
  "errorCode": "",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/login",
  "traceId": "0HN8A1B2C3D4E:00000001",
  "data": {
    "user": {
      "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "firstName": "John",
      "lastName": "Doe",
      "fullName": "John Doe",
      "email": "john.doe@example.com",
      "userName": "john.doe@example.com",
      "roles": ["User"]
    },
    "accessToken": {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    },
    "refreshToken": {
      "token": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4..."
    }
  }
}
```

*400 - Validation Error (empty fields)* `400`:
```json
{
  "isSuccess": false,
  "message": "One or more validation errors have occurred.",
  "errorCode": "VALIDATION_ERROR",
  "validationErrors": {
    "emailAddress": "Email address is required.",
    "password": "Password is required."
  },
  "meta": {},
  "instance": "/api/v1/auth/login",
  "traceId": "0HN8A1B2C3D4E:00000002"
}
```

*401 - Invalid Credentials (wrong email or password)* `401`:
```json
{
  "isSuccess": false,
  "message": "Invalid email or password. Please check your credentials and try again.",
  "errorCode": "Identity_InvalidCredentials",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/login",
  "traceId": "0HN8A1B2C3D4E:00000003"
}
```

*403 - Account Locked Out* `403`:
```json
{
  "isSuccess": false,
  "message": "This account has been temporarily locked due to multiple failed login attempts.",
  "errorCode": "Identity_UserLockedOut",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/login",
  "traceId": "0HN8A1B2C3D4E:00000004"
}
```

**Login - Empty Fields**:
**Login - Invalid Credentials**:
**Login - Non-existent Email**:
### `POST /refresh-token`

Refreshes the JWT token using a valid refresh token.

**Request Body:**
```json
{
  "refreshToken": "{{refreshToken}}"
}
```

**Refresh Token - Success**:
*200 - Token Refreshed* `200`:
```json
{
    "data": {
        "userId": "e54ca29b-06f2-4be5-a5a2-08de76dea993",
        "email": "john.doe@example.com",
        "userName": "john.doe@example.com",
        "firstName": "John",
        "lastName": "Doe",
        "roles": [
            "User"
        ],
        "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImU1NGNhMjliLTA2ZjItNGJlNS1hNWEyLTA4ZGU3NmRlYTk5MyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6ImpvaG4uZG9lQGV4YW1wbGUuY29tIiwiRmlyc3ROYW1lIjoiSm9obiIsIkxhc3ROYW1lIjoiRG9lIiwic3ViIjoiZTU0Y2EyOWItMDZmMi00YmU1LWE1YTItMDhkZTc2ZGVhOTkzIiwianRpIjoiOTEwZjU4ZjEtYzcwMi00OWQ4LWI3ZDktYzE2YzIwMzVhOTBkIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiVXNlciIsImV4cCI6MTc3MjMwMjM5NiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MTE3IiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MTE3In0.5ASMS_5sKgzFFs9T4dPZ55LHOWcfsLhTGgUB2Wk6Kuw",
        "refreshToken": "ZVB9AGAqizi5BqFK0QP2vqRurg8jNjGSy8L+4sfhmFmPLAHEasfd7MWj1p3ptEzWg9TY4OKxcvxQ9kCAkK0OxQ=="
    },
    "isSuccess": true,
    "message": "",
    "errorCode": "",
    "statusCode": 200,
    "validationErrors": {},
    "meta": {},
    "instance": "/api/v1/auth/refresh-token",
    "traceId": "0HNJMRQKFBHQV:00000002"
}
```

*400 - Validation Error (empty token)* `400`:
```json
{
  "isSuccess": false,
  "message": "One or more validation errors have occurred.",
  "errorCode": "VALIDATION_ERROR",
  "validationErrors": {
    "refreshToken": "Refresh token is required."
  },
  "meta": {},
  "instance": "/api/v1/auth/refresh-token",
  "traceId": "0HN8A1B2C3D4E:00000002"
}
```

*401 - Invalid Token* `401`:
```json
{
    "data": null,
    "isSuccess": false,
    "message": "رمز المصادقة المقدم غير صالح أو مشوه.",
    "errorCode": "Identity_InvalidToken",
    "statusCode": 401,
    "validationErrors": {},
    "meta": {},
    "instance": "/api/v1/auth/refresh-token",
    "traceId": "0HNJMRU3F43TN:00000011"
}
```

**Refresh Token - Empty Token**:
**Refresh Token - Invalid Token**:
### `POST /change-password`

Changes password for the authenticated user.

**Request Body:**
```json
{
  "currentPassword": "StrongP@ssw0rd!",
  "newPassword": "NewStr0ngP@ss!",
  "confirmNewPassword": "NewStr0ngP@ss!"
}
```

**Change Password - Success**:
*200 - Password Changed* `200`:
```json
{
  "isSuccess": true,
  "message": "",
  "errorCode": "",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/change-password",
  "traceId": "0HN8A1B2C3D4E:00000001"
}
```

*400 - Validation Error (all fields empty)* `400`:
```json
{
  "isSuccess": false,
  "message": "One or more validation errors have occurred.",
  "errorCode": "VALIDATION_ERROR",
  "validationErrors": {
    "currentPassword": "Current password is required.",
    "newPassword": "New password is required.",
    "confirmNewPassword": "Confirm new password is required."
  },
  "meta": {},
  "instance": "/api/v1/auth/change-password",
  "traceId": "0HN8A1B2C3D4E:00000002"
}
```

*400 - Validation Error (confirm password mismatch)* `400`:
```json
{
  "isSuccess": false,
  "message": "One or more validation errors have occurred.",
  "errorCode": "VALIDATION_ERROR",
  "validationErrors": {
    "confirmNewPassword": "New password and confirm password do not match."
  },
  "meta": {},
  "instance": "/api/v1/auth/change-password",
  "traceId": "0HN8A1B2C3D4E:00000003"
}
```

*400 - Weak New Password (from Identity)* `400`:
```json
{
  "isSuccess": false,
  "message": "One or more validation errors have occurred.",
  "errorCode": "VALIDATION_ERROR",
  "validationErrors": {
    "password": "Password does not meet the minimum security requirements (complexity/length)."
  },
  "meta": {},
  "instance": "/api/v1/auth/change-password",
  "traceId": "0HN8A1B2C3D4E:00000004"
}
```

*400 - Password Reuse Not Allowed (from Identity)* `400`:
```json
{
  "isSuccess": false,
  "message": "One or more validation errors have occurred.",
  "errorCode": "VALIDATION_ERROR",
  "validationErrors": {
    "password": "For security reasons, you cannot reuse a previously used password."
  },
  "meta": {},
  "instance": "/api/v1/auth/change-password",
  "traceId": "0HN8A1B2C3D4E:00000005"
}
```

*401 - Wrong Current Password (PasswordMismatch from Identity)* `401`:
```json
{
  "isSuccess": false,
  "message": "The password provided is incorrect. Please try again.",
  "errorCode": "Identity_InvalidPassword",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/change-password",
  "traceId": "0HN8A1B2C3D4E:00000006"
}
```

*401 - Not Authenticated (missing or invalid bearer token)* `401`:
```json
{
  "isSuccess": false,
  "message": "Authentication token is missing. Please provide a valid Authorization header.",
  "errorCode": "Identity_MissingToken",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/change-password",
  "traceId": "0HN8A1B2C3D4E:00000007"
}
```

*404 - User Not Found* `404`:
```json
{
  "isSuccess": false,
  "message": "The requested user profile does not exist.",
  "errorCode": "Identity_UserNotFound",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/change-password",
  "traceId": "0HN8A1B2C3D4E:00000008"
}
```

**Change Password - Empty Fields**:
**Change Password - Confirm Mismatch**:
**Change Password - Wrong Current Password**:
**Change Password - Weak New Password**:
**Change Password - No Auth Token**:
### `POST /logout`

Logs out the user by revoking the active refresh token.

**Request Body:**
```json
{}
```

**Logout - Success**:
*200 - Logged Out* `200`:
```json
{
  "isSuccess": true,
  "message": "",
  "errorCode": "",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/logout",
  "traceId": "0HN8A1B2C3D4E:00000001"
}
```

*401 - Not Authenticated (missing token)* `401`:
```json
{
  "isSuccess": false,
  "message": "Authentication token is missing. Please provide a valid Authorization header.",
  "errorCode": "Identity_MissingToken",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/logout",
  "traceId": "0HN8A1B2C3D4E:00000002"
}
```

*401 - Expired Access Token* `401`:
```json
{
  "isSuccess": false,
  "message": "Your session has expired. Please log in again to continue.",
  "errorCode": "Identity_ExpiredToken",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/logout",
  "traceId": "0HN8A1B2C3D4E:00000003"
}
```

*404 - User Not Found (userId from token not in DB)* `404`:
```json
{
  "isSuccess": false,
  "message": "The identifier '00000000-0000-0000-0000-000000000000' does not match any existing user record.",
  "errorCode": "Identity_UserNotFound__Id",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/logout",
  "traceId": "0HN8A1B2C3D4E:00000004"
}
```

**Logout - No Auth Token**:
**Logout - Expired Token**:
### `POST /forget-password`

Sends a password reset OTP to the user's email.

**Request Body:**
```json
{
  "emailAddress": "john.doe@example.com"
}
```

**Forget Password - Success**:
*200 - OTP Sent* `200`:
```json
{
  "isSuccess": true,
  "message": "",
  "errorCode": "",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/forget-password",
  "traceId": "0HN8A1B2C3D4E:00000001"
}
```

*400 - Validation Error (empty email)* `400`:
```json
{
  "isSuccess": false,
  "message": "One or more validation errors have occurred.",
  "errorCode": "VALIDATION_ERROR",
  "validationErrors": {
    "emailAddress": "Email address is required."
  },
  "meta": {},
  "instance": "/api/v1/auth/forget-password",
  "traceId": "0HN8A1B2C3D4E:00000002"
}
```

*400 - Validation Error (invalid email format)* `400`:
```json
{
  "isSuccess": false,
  "message": "One or more validation errors have occurred.",
  "errorCode": "VALIDATION_ERROR",
  "validationErrors": {
    "emailAddress": "The email format is invalid. Please enter a valid email address."
  },
  "meta": {},
  "instance": "/api/v1/auth/forget-password",
  "traceId": "0HN8A1B2C3D4E:00000003"
}
```

*400 - OTP Cooldown (request within 30s)* `400`:
```json
{
  "isSuccess": false,
  "message": "One or more validation errors have occurred.",
  "errorCode": "VALIDATION_ERROR",
  "validationErrors": {
    "otp": "Please wait 30 seconds before requesting another OTP."
  },
  "meta": {},
  "instance": "/api/v1/auth/forget-password",
  "traceId": "0HN8A1B2C3D4E:00000004"
}
```

*403 - Account Locked* `403`:
```json
{
  "isSuccess": false,
  "message": "This account has been temporarily locked due to multiple failed login attempts.",
  "errorCode": "Identity_UserLockedOut",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/forget-password",
  "traceId": "0HN8A1B2C3D4E:00000005"
}
```

*404 - User Not Found* `404`:
```json
{
  "isSuccess": false,
  "message": "The requested user profile does not exist.",
  "errorCode": "Identity_UserNotFound",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/forget-password",
  "traceId": "0HN8A1B2C3D4E:00000006"
}
```

**Forget Password - Empty Email**:
**Forget Password - Invalid Email Format**:
**Forget Password - Non-existent Email**:
**Forget Password - OTP Cooldown**:
**Forget Password - Locked Account**:
### `POST /verify-reset-password-otp`

Verifies the OTP and returns a reset password token.

**Request Body:**
```json
{
  "emailAddress": "john.doe@example.com",
  "otp": "008803"
}
```

**Verify OTP - Success**:
*200 - OTP Verified* `200`:
```json
{
  "isSuccess": true,
  "message": "",
  "errorCode": "",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/verify-reset-password-otp",
  "traceId": "0HN8A1B2C3D4E:00000001",
  "data": {
    "resetPasswordToken": "CfDJ8NrFh3x5z9K2mW7vQ1pL..."
  }
}
```

*400 - Validation Error (empty fields)* `400`:
```json
{
  "isSuccess": false,
  "message": "One or more validation errors have occurred.",
  "errorCode": "VALIDATION_ERROR",
  "validationErrors": {
    "emailAddress": "Email address is required.",
    "otp": "OTP code is required."
  },
  "meta": {},
  "instance": "/api/v1/auth/verify-reset-password-otp",
  "traceId": "0HN8A1B2C3D4E:00000002"
}
```

*400 - Validation Error (invalid email format)* `400`:
```json
{
  "isSuccess": false,
  "message": "One or more validation errors have occurred.",
  "errorCode": "VALIDATION_ERROR",
  "validationErrors": {
    "emailAddress": "The email format is invalid. Please enter a valid email address."
  },
  "meta": {},
  "instance": "/api/v1/auth/verify-reset-password-otp",
  "traceId": "0HN8A1B2C3D4E:00000003"
}
```

*400 - Invalid OTP* `400`:
```json
{
  "isSuccess": false,
  "message": "One or more validation errors have occurred.",
  "errorCode": "VALIDATION_ERROR",
  "validationErrors": {
    "otp": "The OTP code entered is invalid or has expired."
  },
  "meta": {},
  "instance": "/api/v1/auth/verify-reset-password-otp",
  "traceId": "0HN8A1B2C3D4E:00000004"
}
```

*403 - Account Locked* `403`:
```json
{
  "isSuccess": false,
  "message": "This account has been temporarily locked due to multiple failed login attempts.",
  "errorCode": "Identity_UserLockedOut",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/verify-reset-password-otp",
  "traceId": "0HN8A1B2C3D4E:00000005"
}
```

*404 - User Not Found* `404`:
```json
{
  "isSuccess": false,
  "message": "No account found associated with the email address.",
  "errorCode": "Identity_UserNotFound__Email",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/verify-reset-password-otp",
  "traceId": "0HN8A1B2C3D4E:00000006"
}
```

**Verify OTP - Empty Fields**:
**Verify OTP - Invalid Email Format**:
**Verify OTP - Invalid Code**:
**Verify OTP - Non-existent Email**:
**Verify OTP - Locked Account**:
### `POST /reset-password`

Resets the user's password using the token obtained from OTP verification.

**Request Body:**
```json
{
  "emailAddress": "john.doe2@example.com",
  "resetPasswordToken": "CfDJ8KrIJX4+RJRArddXhkU8y1104+LYUmqnv+mQFy7rops0PgkCXtALR2p9oqGOwb42n0bu/nS9rEII6UU1/tP44Axpu3Rd+vUbJUN5A/QborNeZatbnI2MKL/U3T5Ew7gy02tlRByvVWHXJw4JqoihTac6SAcMZVwQtH7jaxIRwo06FsbvmfPX9j3pnqdfmcm9jpuE+5i0EobyGp1f/1WyqD2rLnHuFAnUq632rUabdMRJ",
  "newPassword": "MyNewStr0ngP@ss!"
}
```

**Reset Password - Success**:
*200 - Password Reset Successful* `200`:
```json
{
  "isSuccess": true,
  "message": "",
  "errorCode": "",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/reset-password",
  "traceId": "0HN8A1B2C3D4E:00000001"
}
```

*400 - Validation Error (all fields empty)* `400`:
```json
{
  "isSuccess": false,
  "message": "One or more validation errors have occurred.",
  "errorCode": "VALIDATION_ERROR",
  "validationErrors": {
    "emailAddress": "Email address is required.",
    "resetPasswordToken": "Reset password token is required.",
    "newPassword": "New password is required."
  },
  "meta": {},
  "instance": "/api/v1/auth/reset-password",
  "traceId": "0HN8A1B2C3D4E:00000002"
}
```

*400 - Validation Error (invalid email format)* `400`:
```json
{
  "isSuccess": false,
  "message": "One or more validation errors have occurred.",
  "errorCode": "VALIDATION_ERROR",
  "validationErrors": {
    "emailAddress": "The email format is invalid. Please enter a valid email address."
  },
  "meta": {},
  "instance": "/api/v1/auth/reset-password",
  "traceId": "0HN8A1B2C3D4E:00000003"
}
```

*400 - Invalid/Expired Reset Token* `400`:
```json
{
  "isSuccess": false,
  "message": "One or more validation errors have occurred.",
  "errorCode": "VALIDATION_ERROR",
  "validationErrors": {
    "resetPasswordToken": "The password reset token is invalid or has expired."
  },
  "meta": {},
  "instance": "/api/v1/auth/reset-password",
  "traceId": "0HN8A1B2C3D4E:00000004"
}
```

*404 - User Not Found* `404`:
```json
{
  "isSuccess": false,
  "message": "No account found associated with the email address.",
  "errorCode": "Identity_UserNotFound__Email",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/reset-password",
  "traceId": "0HN8A1B2C3D4E:00000005"
}
```

**Reset Password - Empty Fields**:
**Reset Password - Invalid Email Format**:
**Reset Password - Invalid Token**:
**Reset Password - Non-existent Email**:
### `POST /google-login`

Authenticates an existing user using a valid Google ID token.

**Request Body:**
```json
{
  "idToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6Ij...<valid Google ID Token for existing user>..."
}
```

**Google Login - Success (existing user)**:
*200 - Google Login Successful (existing user)* `200`:
```json
{
  "isSuccess": true,
  "message": "",
  "errorCode": "",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/google-login",
  "traceId": "0HN8A1B2C3D4E:00000001",
  "data": {
    "user": {
      "id": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
      "firstName": "Jane",
      "lastName": "Smith",
      "fullName": "Jane Smith",
      "email": "jane.smith@gmail.com",
      "userName": "jane.smith@gmail.com",
      "roles": ["User"]
    },
    "accessToken": {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    },
    "refreshToken": {
      "token": "Z29vZ2xlIHJlZnJlc2ggdG9rZW4..."
    }
  }
}
```

*200 - Google Login Successful (new user auto-created)* `200`:
```json
{
  "isSuccess": true,
  "message": "",
  "errorCode": "",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/google-login",
  "traceId": "0HN8A1B2C3D4E:00000002",
  "data": {
    "user": {
      "id": "c3d4e5f6-a7b8-9012-cdef-123456789012",
      "firstName": "New",
      "lastName": "User",
      "fullName": "New User",
      "email": "new.user@gmail.com",
      "userName": "new.user@gmail.com",
      "roles": ["User"]
    },
    "accessToken": {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    },
    "refreshToken": {
      "token": "bmV3IHVzZXIgcmVmcmVzaCB0b2tlbg..."
    }
  }
}
```

*400 - Validation Error (empty token)* `400`:
```json
{
  "isSuccess": false,
  "message": "One or more validation errors have occurred.",
  "errorCode": "VALIDATION_ERROR",
  "validationErrors": {
    "idToken": "Google ID token is required."
  },
  "meta": {},
  "instance": "/api/v1/auth/google-login",
  "traceId": "0HN8A1B2C3D4E:00000003"
}
```

*401 - Invalid Google ID Token* `401`:
```json
{
  "isSuccess": false,
  "message": "The Google authentication token is invalid or has expired.",
  "errorCode": "Identity_InvalidGoogleToken",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/google-login",
  "traceId": "0HN8A1B2C3D4E:00000004"
}
```

*403 - Account Locked* `403`:
```json
{
  "isSuccess": false,
  "message": "This account has been temporarily locked due to multiple failed login attempts.",
  "errorCode": "Identity_UserLockedOut",
  "validationErrors": {},
  "meta": {},
  "instance": "/api/v1/auth/google-login",
  "traceId": "0HN8A1B2C3D4E:00000005"
}
```


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
| Runtime          | .NET 10 / C# 14                                         |
| API              | ASP.NET Core, API Versioning, Swagger/Swashbuckle       |
| Architecture     | Vertical Slice Architecture, CQRS                       |
| Mediator         | MediatR                                                 |
| Validation       | FluentValidation (pipeline behavior)                    |
| Authentication   | JWT Bearer, ASP.NET Identity, Google OAuth              |
| Database         | SQL Server (LocalDB), Entity Framework Core             |
| Background Jobs  | Hangfire (SQL Server storage)                           |
| Email            | FluentEmail (SMTP)                                      |
| Caching          | HybridCache (OTP throttling)                           |
| Logging          | Serilog (Console, Seq)                            |
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
    build: .
    container_name: fluxstore-service
    image: mostafa0841/fluxstore-vsa:latest
    restart: on-failure # Important!
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:5089
      
      # Connection Strings
      - ConnectionStrings__DefaultConnection=Server=sqlserver,1433;Database=FluxStoreDb;User Id=SA;Password=Admin#123;TrustServerCertificate=True
      
# --- Token Settings ---
      - TokenSettings__SecretKey=Yj6pr{WJ+c}WL:Zmc%v364$$jkOi}O3HM
      - TokenSettings__Issuer=http://localhost:5089
      - TokenSettings__Audience=http://localhost:5089
      - TokenSettings__AccessTokenExpiryInMinutes=160
      - TokenSettings__RefreshTokenExpiryInMinutes=260

      # --- SMTP Settings (Update 'mailhog' to your mail service name) ---
      - SmtpSettings__Name=FluxStore
      - SmtpSettings__Server=smtp4dev
      - SmtpSettings__Port=25
      - SmtpSettings__User=support@fluxstore
      - SmtpSettings__Password=your-email-password
      - SmtpSettings__UseSsl=false
      - SmtpSettings__RequiresAuthentication=false

      # --- Database Setup ---
      - DatabaseSetupSettings__ResetOnStartup=true
      - DatabaseSetupSettings__EnsureCreated=true
      - DatabaseSetupSettings__SeedInitialData=true

      # --- CORS (Note the index for arrays) ---
      - CorsSettings__PolicyName=AllowSpecificOrigins
      - CorsSettings__AllowedOrigins__0=http://localhost:8080
      - CorsSettings__AllowedOrigins__1=http://localhost:8081
      - CorsSettings__AllowedOrigins__2=http://localhost:3000
      - CorsSettings__AllowCredentials=true

      # --- External Auth ---
      - ExternalAuthenticationSettings__Google__ClientId=159007714113-l1nka853t90rqrej8eu4h1sc0lg2kjiv.apps.googleusercontent.com

      # Serilog Logging Configuration
      - Serilog__Using__0=Serilog.Sinks.Console
      - Serilog__Using__2=Serilog.Sinks.Seq
      - Serilog__MinimumLevel__Default=Information
      - Serilog__MinimumLevel__Override__Microsoft=Warning
      - Serilog__WriteTo__0__Name=Console
      - Serilog__WriteTo__1__Name=Seq
      - Serilog__WriteTo__1__Args__serverUrl=http://seqserver:80
      - Serilog__Enrich__0=FromLogContext
      - Serilog__Properties__Application=FluxStore.API
    volumes:
      - ./src/FluxStore.Api/Shared:/app/Shared
    ports:
      - "5089:5089"
    networks:
      - backend-network
    depends_on:
      - sqlserver
      - seqserver
      - smtp4dev

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
    environment:
      - ACCEPT_EULA=Y
      - SEQ_FIRSTRUN_ADMINPASSWORD=ABC#123
    ports:
      - "5341:80"
    networks:
      - backend-network

  smtp4dev:
    image: rnwood/smtp4dev
    container_name: smtpserver
    ports:
      - "8082:80"
    networks:
      - backend-network

volumes:
  sqlserverdata:
  appdata:
  templates_data:
  resources_data:

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

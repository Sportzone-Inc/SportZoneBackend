# Authentication Feature

## Overview

This feature adds JWT-based authentication to the SportZone API. Users can authenticate using username and password credentials, and receive a JWT token for accessing protected endpoints.

## Default Credentials

A default admin user is automatically created when the application starts:

- **Username**: `admin`
- **Password**: `passtimadmin`

## Endpoints

### POST /api/auth/login

Authenticates a user and returns a JWT token.

**Request Body:**
```json
{
  "username": "admin",
  "password": "passtimadmin"
}
```

**Success Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "expiresAt": "2025-10-15T13:48:07Z"
}
```

**Error Response (401 Unauthorized):**
```json
{
  "message": "Ongeldige gebruikersnaam of wachtwoord"
}
```

## Using the JWT Token

Once you have received a JWT token, include it in the `Authorization` header of subsequent requests:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Configuration

JWT settings can be configured in `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "SportZone",
    "Audience": "SportZoneUsers",
    "ExpiryMinutes": "60"
  }
}
```

## User Model Changes

The `User` model has been extended with authentication fields:

- `Username`: Unique username for login
- `PasswordHash`: Hashed password (never store plain text passwords)
- `FirstName`: User's first name
- `LastName`: User's last name

## Security Features

1. **Password Hashing**: All passwords are hashed using BCrypt before storage
2. **JWT Tokens**: Secure token-based authentication
3. **Token Expiration**: Tokens expire after the configured time (default: 60 minutes)
4. **Swagger Integration**: JWT authentication is integrated with Swagger UI for easy testing

## Testing

### Unit Tests

Three unit tests have been created in `SportZone.Tests/Services/AuthenticationServiceTests.cs`:

1. **AuthenticateAsync_WithValidCredentials_ReturnsTrue**: Tests successful authentication
2. **AuthenticateAsync_WithInvalidUsername_ReturnsFalse**: Tests authentication failure with invalid username
3. **AuthenticateAsync_WithInvalidPassword_ReturnsFalse**: Tests authentication failure with invalid password

Run tests with:
```bash
dotnet test
```

### Testing with Swagger

1. Navigate to the Swagger UI (root URL in development)
2. Click on the "Authorize" button at the top right
3. Use the `/api/auth/login` endpoint to get a token
4. Enter the token in the format: `Bearer <your-token>`
5. Click "Authorize" to apply the token to all requests

### Testing with cURL

```bash
# Login
curl -X POST "https://localhost:7xxx/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"passtimadmin"}'

# Use token in subsequent requests
curl -X GET "https://localhost:7xxx/api/users" \
  -H "Authorization: Bearer <your-token>"
```

## Database Seeding

The application automatically seeds the database with the default admin user on startup. This is handled by the `DatabaseSeeder` service, which:

1. Checks if an admin user exists
2. If not, creates one with the default credentials
3. Logs the result

This ensures there's always at least one user who can authenticate to the system.

## Architecture

### Services

- **IAuthenticationService / AuthenticationService**: Handles user authentication and JWT token generation
- **DatabaseSeeder**: Seeds initial data (default admin user)

### Controllers

- **AuthController**: Exposes the login endpoint

### DTOs

- **LoginRequestDto**: Login request with username and password
- **LoginResponseDto**: Login response with JWT token and expiration

### Extensions

- **DatabaseSeederExtensions**: Extension method for seeding the database during application startup

## Dependencies

New NuGet packages added:

- `Microsoft.AspNetCore.Authentication.JwtBearer` (9.0.0)
- `System.IdentityModel.Tokens.Jwt` (8.2.1)

# JWT Authentication in ASP.NET Core Minimal API: A Developer's Guide

This repository contains the source code for a tutorial on implementing JWT (JSON Web Token) authentication in ASP.NET Core Minimal API. The project demonstrates how to implement token-based authentication, helping developers understand the fundamentals of JWT while showcasing both learning-focused and production-ready approaches.

## Project Description

This tutorial provides a hands-on approach to understanding JWT authentication by:
- Implementing token generation and validation
- Securing API endpoints
- Managing authentication configurations
- Understanding different authentication approaches (self-managed vs. third-party providers)

## Features in the Project

- **JWT Token Generation**: Learn how to generate secure JWT tokens with customizable claims
- **Token Validation**: Implement proper token validation and authorization
- **Configuration Management**: Handle JWT settings through application configuration
- **Protected Endpoints**: Secure API endpoints using JWT authentication
- **Swagger Integration**: Test the authenticated endpoints through Swagger UI

## How to Run the Project

1. Clone the repository:
```bash
git clone https://github.com/[your-username]/SecureWeatherApi.git
```

2. Navigate to the project directory:
```bash
cd SecureWeatherApi
```

3. Update the JWT settings in `appsettings.json`:
```json
{
  "Jwt": {
    "Key": "your-secure-key",
    "Issuer": "your-issuer",
    "Audience": "your-audience",
    "ExpirationSeconds": 3600
  }
}
```

4. Build and run the project:
```bash
dotnet restore
dotnet run
```

5. Access Swagger UI at `https://localhost:[port]/swagger`

## Testing the API

1. Use the `/login` endpoint with the following credentials:
   - Username: `demo`
   - Password: `password`

2. Copy the received JWT token

3. Use the token to access the protected `/weatherforecast` endpoint

## Links to Tutorial

For a detailed walkthrough, check out the complete tutorial on my blog:
[JWT Authentication in ASP.NET Core Minimal API](https://www.ottorinobruni.com/how-to-implement-jwt-authentication-in-asp-net-core-minimal-api/)

## Contents

- `Program.cs`: Main application with JWT configuration and endpoints
- `appsettings.json`: Application configuration including JWT settings
- `SecureWeatherApi.http`: HTTP request file for testing the API
- `README.md`: Project documentation

## Contributions

Contributions are welcome! Feel free to:
- Open issues
- Submit pull requests
- Suggest improvements
- Report bugs

## License

This project is licensed under the MIT License. See the LICENSE file for details.

## Example Usage

The API includes a protected weather forecast endpoint that requires a valid JWT token for access. This demonstrates:
- Token-based authentication flow
- Protected resource access
- JWT validation in action

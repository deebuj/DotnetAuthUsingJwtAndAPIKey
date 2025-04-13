# .NET API Authentication Demo

This project demonstrates implementation of multiple authentication methods in a .NET 8 Web API, specifically:
- API Key Authentication
- Firebase JWT Authentication

## Features

- Dual authentication support (API Key or Firebase JWT)
- Swagger UI integration for API documentation
- Sample WeatherForecast endpoint with authentication
- Configuration support for different environments

## Prerequisites

- .NET 8.0 SDK
- Firebase project (for JWT authentication)
- REST Client extension for VS Code (to run .http files)

## Configuration

The application uses the following configuration structure in `appsettings.json`:

```json
{
  "Authentication": {
    "ApiKey": "",
    "Firebase": {
      "ProjectId": ""
    }
  }
}
```

Configuration values should be set in `appsettings.local.json` for local development:

```json
{
  "Authentication": {
    "ApiKey": "your-api-key",
    "Firebase": {
      "ProjectId": "your-firebase-project-id"
    }
  }
}
```

## Running the Application

1. Clone the repository
2. Create an `appsettings.local.json` file with your configuration values
3. Run the application using:
   ```bash
   dotnet run
   ```
   
The application will start at:
- http://localhost:5018 (HTTP)
- https://localhost:7133 (HTTPS)

## Testing the API

The project includes a `TestAuth.http` file that can be used to test the API endpoints. To use it:

1. Install the REST Client extension in VS Code
2. Open `TestAuth.http`
3. Click "Send Request" above each request to test:
   - No authentication (will return 401)
   - API Key authentication (using X-API-Key header)
   - Firebase JWT authentication (using Bearer token)

### Available Test Endpoints

```http
### Test without authentication
GET http://localhost:5018/WeatherForecast

### Test with API Key
GET http://localhost:5018/WeatherForecast
X-API-Key: your-api-key

### Test with Firebase JWT
GET http://localhost:5018/WeatherForecast
Authorization: Bearer your-firebase-token
```

## Authentication Methods

### Global Authentication
The application uses global authentication, which means all endpoints require authentication by default. This is configured in `Program.cs` using a fallback policy:

```csharp
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes("ApiKeyOrJwt")
        .Build();
});
```

This means that:
1. All endpoints require authentication by default
2. You don't need to add [Authorize] attributes to your controllers or actions
3. The only way to make an endpoint public is to explicitly mark it with [AllowAnonymous]

Here's an example:

```csharp
[ApiController]
[Route("[controller]")]
public class SomeController : ControllerBase
{
    [AllowAnonymous]  // This endpoint is explicitly made public
    [HttpGet]
    public IActionResult PublicEndpoint()
    {
        return Ok("Hello World! Anyone can access this.");
    }

    [HttpGet("protected")]  // No [AllowAnonymous], so this is automatically protected
    public IActionResult ProtectedEndpoint()
    {
        // This endpoint requires authentication because of the global FallbackPolicy
        // No need for [Authorize] attribute - it's protected by default
        return Ok("Hello Authenticated User!");
    }
}
```

### API Key Authentication
- Add the `X-API-Key` header to your requests
- The API key should match the one in your configuration

### Firebase JWT Authentication
- Add the `Authorization: Bearer <token>` header
- Token should be a valid Firebase ID token
- Firebase project ID must match the one in your configuration

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
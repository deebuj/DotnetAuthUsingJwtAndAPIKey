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

### API Key Authentication
- Add the `X-API-Key` header to your requests
- The API key should match the one in your configuration

### Firebase JWT Authentication
- Add the `Authorization: Bearer <token>` header
- Token should be a valid Firebase ID token
- Firebase project ID must match the one in your configuration

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
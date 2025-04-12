using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using TestAuth.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(options => 
{
    options.DefaultAuthenticateScheme = "ApiKeyOrJwt";
    options.DefaultChallengeScheme = "ApiKeyOrJwt";
})
.AddJwtBearer(options =>
{
    var firebaseProjectId = builder.Configuration["Authentication:Firebase:ProjectId"];
    options.Authority = $"https://securetoken.google.com/{firebaseProjectId}";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = $"https://securetoken.google.com/{firebaseProjectId}",
        ValidateAudience = true,
        ValidAudience = firebaseProjectId,
        ValidateLifetime = true
    };
    
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var identity = context.Principal?.Identity as ClaimsIdentity;
            if (identity != null)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, "FirebaseUser"));
            }
            return Task.CompletedTask;
        }
    };
})
.AddScheme<ApiKeyAuthOptions, ApiKeyAuthHandler>("ApiKey", options =>
{
    options.ApiKey = builder.Configuration["Authentication:ApiKey"] ?? "your-api-key-here";
})
.AddPolicyScheme("ApiKeyOrJwt", "ApiKey or JWT", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        if (context.Request.Headers.ContainsKey("X-API-Key"))
            return "ApiKey";
        return JwtBearerDefaults.AuthenticationScheme;
    };
});

// Configure authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiKeyOrFirebase", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == System.Security.Claims.ClaimTypes.Role && 
                (c.Value == "ApiUser" || c.Value == "FirebaseUser"))
        ));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

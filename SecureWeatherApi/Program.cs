using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

// Key: RefreshToken, Value: Username
var refreshTokens = new Dictionary<string, string>();

var builder = WebApplication.CreateBuilder(args);

// Add CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Bind JWT settings from configuration
var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>();

// Configure Authentication and JWT Bearer.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });

builder.Services.AddAuthorization();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseStaticFiles();

// Enable CORS
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/login", (UserCredentials credentials) =>
{
    if (credentials.Username == "username" && credentials.Password == "password")
    {
        var token = GenerateAccessToken(credentials.Username, jwtSettings);
        var refreshToken = Guid.NewGuid().ToString();
        refreshTokens[refreshToken] = credentials.Username; 

        return Results.Ok(new { AccessToken = token, RefreshToken = refreshToken });
    }
    return Results.Unauthorized();
})
.WithName("Login")
.WithOpenApi();

app.MapPost("/refresh", (RefreshRequest request) => {
    if (refreshTokens.TryGetValue(request.RefreshToken, out var username))
    {
        var newAccessToken = GenerateAccessToken(username, jwtSettings);
        return Results.Ok(new { AccessToken = newAccessToken });
    }

    return Results.BadRequest("Invalid refresh token");
})
.WithName("Refresh")
.WithOpenApi();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.RequireAuthorization()
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

// Helper method for token generation
string GenerateAccessToken(string username, JwtSettings jwtSettings)
{
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, username)
    };

    var securityKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(jwtSettings.Key));

    var credentials = new SigningCredentials(
        securityKey, 
        SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: jwtSettings.Issuer,
        audience: jwtSettings.Audience,
        claims: claims,
        expires: DateTime.UtcNow.AddSeconds(jwtSettings.ExpirationSeconds),
        signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);    
}

//Models
record UserCredentials(string Username, string Password);
record RefreshRequest(string RefreshToken);

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationSeconds { get; set; }
}
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TaskList_Server.Data;

var builder = WebApplication.CreateBuilder(args);

// Configure SQL Server DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<Tasklist25Context>(options =>
    options.UseSqlServer(connectionString));

// Add Controllers
builder.Services.AddControllers();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Authorization
builder.Services.AddAuthorization();

// CORS for React App
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger / OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Tasklist API",
        Version = "v1",
        Description = "API for task management with JWT authentication"
    });

    // JWT Bearer support in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Enable Swagger
app.UseSwagger(); // remove c.SerializeAsV2
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tasklist API v1");
    c.RoutePrefix = string.Empty;
});

// Middleware pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AllowReactApp");

app.UseAuthentication(); // Must come before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();

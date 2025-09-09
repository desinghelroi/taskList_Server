using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TaskList_Server.Data;
using System.Threading.RateLimiting;
using TaskList_Server.Interface;
using TaskList_Server.Service;

var builder = WebApplication.CreateBuilder(args);


#region Add Rate Limiter ---> General limiter and Strict limiter for sensitive endpoints
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("GeneralLimiter", config =>
    {
        config.PermitLimit = 10;
        config.Window = TimeSpan.FromSeconds(10);
        config.QueueLimit = 2;
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.AddFixedWindowLimiter("WriteLimiter", config =>
    {
        config.PermitLimit = 5;
        config.Window = TimeSpan.FromSeconds(10);
        config.QueueLimit = 1;
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

#endregion

#region Connection to SQL Server

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<Tasklist25Context>(options =>
    options.UseSqlServer(connectionString));

#endregion


builder.Services.AddControllers();

#region DI for Services

builder.Services.AddScoped<ITaskListService, TaskListService>();
builder.Services.AddScoped<IAuthService, AuthService>();

#endregion

#region jwt authentication and authorization

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? ""))
    };
});

builder.Services.AddAuthorization();

#endregion

#region cors for react app

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

#endregion

#region swagger 
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Tasklist API",
        Version = "v1",
        Description = "API for task management with JWT authentication"
    });

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

#endregion

var app = builder.Build();

app.UseRateLimiter();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tasklist API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AllowReactApp");

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();

using System.Reflection;
using System.Text;
using CoreBanking.Api;
using CoreBanking.API.Middleware;
using CoreBanking.Application.Commands.Auth;
using CoreBanking.Application.Interfaces;
using CoreBanking.Application.Validators;
using CoreBanking.Infrastructure.Persistence;
using CoreBanking.Infrastructure.Repositories;
using CoreBanking.Infrastructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// Make Program accessible for integration tests

public partial class Program {
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Render injects PORT env var — only override when deployed (not in local dev)
        var port = Environment.GetEnvironmentVariable("PORT");
        if (port is not null)
        {
            builder.WebHost.UseUrls($"http://+:{port}");
        }

        builder.Services.AddControllers();
        builder.Services.AddHealthChecks();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IJwtService, JwtService>();

        builder.Services.AddMediatR(typeof(RegisterUserCommand).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserValidator).Assembly);

        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var blacklistRepo = context.HttpContext.RequestServices
                            .GetRequiredService<ITokenBlacklistRepository>();

                        var token = context.SecurityToken as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;

                        if (token != null)
                        {
                            var rawToken = context.Request.Headers["Authorization"]
                                .ToString()
                                .Replace("Bearer ", "");

                            var isRevoked = await blacklistRepo.IsTokenRevokedAsync(rawToken);

                            if (isRevoked)
                            {
                                context.Fail("Token has been revoked");
                            }
                        }
                    }
                };
            });

        builder.Services.AddAuthorization();

        builder.Services.AddSwaggerGen(static options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CoreBanking API",
                Version = "v1",
                Description = "Core Banking System Web API",
                Contact = new OpenApiContact
                {
                    Name = "CoreBanking Team"
                }
            });

            // Load XML comments for endpoint documentation
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            // JWT Bearer auth — ready for when you add authentication
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
            });
        });


        builder.Services.AddHttpContextAccessor();

        builder.Services.AddAppDI();

        var app = builder.Build();

        // Apply pending EF Core migrations automatically on startup
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }

        // Swagger available in all environments (useful for testing on Render)
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "CoreBanking API v1");
            options.DocumentTitle = "CoreBanking API";
        });

        // Render handles HTTPS at the load balancer — skip in-container redirect
        if (!app.Environment.IsProduction())
        {
            app.UseHttpsRedirection();
        }
        app.UseMiddleware<ExceptionMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHealthChecks("/health");

        app.Run();
    }
}

// Make Program accessible for integration tests
public partial class Program { }

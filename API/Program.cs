using System.Text;
using Application;
using Application.Common.Exceptions;
using FluentValidation;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ──────────────────────────────────────────────────────────────
// Controllers
// ──────────────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// ──────────────────────────────────────────────────────────────
// Swagger / OpenAPI
// ──────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ЗнаниеЗаЗнание API",
        Version = "v1",
        Description = "REST API для платформы образовательного бартера"
    });

    // Поддержка JWT в Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ──────────────────────────────────────────────────────────────
// JWT Authentication
// ──────────────────────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// ──────────────────────────────────────────────────────────────
// CORS
// ──────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ──────────────────────────────────────────────────────────────
// Application + Infrastructure layers
// ──────────────────────────────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// IMemoryCache для хранения OTP-сессий (2FA)
builder.Services.AddMemoryCache();

// Static files (для загрузки Proofs)
builder.Services.AddDirectoryBrowser();

// ──────────────────────────────────────────────────────────────
var app = builder.Build();

// ──────────────────────────────────────────────────────────────
// Global exception handling
// ──────────────────────────────────────────────────────────────
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        context.Response.ContentType = "application/json";

        switch (exception)
        {
            case ValidationException validationException:
                context.Response.StatusCode = 400;
                var errors = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                await context.Response.WriteAsJsonAsync(new { message = "Validation failed", errors });
                break;

            case NotFoundException notFoundException:
                context.Response.StatusCode = 404;
                await context.Response.WriteAsJsonAsync(new { message = notFoundException.Message });
                break;

            case UnauthorizedAccessException unauthorizedException:
                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(new { message = unauthorizedException.Message });
                break;

            case InvalidOperationException invalidOpException:
                context.Response.StatusCode = 409;
                await context.Response.WriteAsJsonAsync(new { message = invalidOpException.Message });
                break;

            // Нарушение уникального ограничения БД → 409
            case DbUpdateException dbEx
                when dbEx.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true
                  || dbEx.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true
                  || dbEx.InnerException?.Message.Contains("23505") == true:
                context.Response.StatusCode = 409;
                await context.Response.WriteAsJsonAsync(new { message = "Запись с такими данными уже существует." });
                break;

            default:
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new { message = "Внутренняя ошибка сервера." });
                break;
        }
    });
});

// ──────────────────────────────────────────────────────────────
// Middleware pipeline
// ──────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ЗнаниеЗаЗнание API v1");
        c.RoutePrefix = string.Empty; // Swagger на /
    });
}

app.UseStaticFiles(); // Раздача файлов из wwwroot / uploads
app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

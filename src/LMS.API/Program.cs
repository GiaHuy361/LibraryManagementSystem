using LMS.API.Middleware;
using LMS.Application.Interfaces;
using LMS.Application.Services;
using LMS.Application.Strategies;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using LMS.Infrastructure.Repositories;
using LMS.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using LMS.Application.Validators;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────
// 1. DATABASE — Entity Framework Core + SQL Server
// ─────────────────────────────────────────────────────────
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ─────────────────────────────────────────────────────────
// 2. REPOSITORIES & UNIT OF WORK (Infrastructure)
// ─────────────────────────────────────────────────────────
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ─────────────────────────────────────────────────────────
// 3. EXTERNAL SERVICES (Infrastructure → Application interfaces)
// ─────────────────────────────────────────────────────────
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

// ─────────────────────────────────────────────────────────
// 4. FINE CALCULATION STRATEGY (Strategy Pattern)
//    Switch strategy here without touching any other code.
// ─────────────────────────────────────────────────────────
var fineRatePerDay = builder.Configuration.GetValue<decimal>("FineSettings:RatePerDay", 5000m);
builder.Services.AddSingleton<IFineCalculationStrategy>(new DailyFineStrategy(fineRatePerDay));
// To switch to progressive fines, replace above line with:
// builder.Services.AddSingleton<IFineCalculationStrategy>(new ProgressiveFineStrategy(5000m, 10000m));

// ─────────────────────────────────────────────────────────
// 5. APPLICATION SERVICES (Internal Business Logic)
// ─────────────────────────────────────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IBorrowService, BorrowService>();
builder.Services.AddScoped<IFineService, FineService>();
builder.Services.AddScoped<IReportService, ReportService>();

// ─────────────────────────────────────────────────────────
// 6. JWT AUTHENTICATION
// ─────────────────────────────────────────────────────────
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]
    ?? throw new InvalidOperationException("JWT SecretKey is not configured in appsettings.json.");

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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// ─────────────────────────────────────────────────────────
// 7. AUTHORIZATION POLICIES
// ─────────────────────────────────────────────────────────
builder.Services.AddAuthorization();

// ─────────────────────────────────────────────────────────
// 8. CORS
// ─────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendOnly", policy =>
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

// ─────────────────────────────────────────────────────────
// 9. SWAGGER / OPENAPI WITH JWT SUPPORT
// ─────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Library Management System API",
        Version = "v1",
        Description = "RESTful API for managing library books, users, and borrow operations."
    });

    // Add JWT Bearer support in Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token below. Example: \"Bearer eyJhbGciOiJIUzI1NiIs...\""
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

// ─────────────────────────────────────────────────────────
// 10. INPUT VALIDATION (FluentValidation)
// ─────────────────────────────────────────────────────────
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();

builder.Services.AddControllers();

var app = builder.Build();

// ─────────────────────────────────────────────────────────
// MIDDLEWARE PIPELINE
// ─────────────────────────────────────────────────────────
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LMS API v1");
    c.RoutePrefix = string.Empty; // Serve Swagger at root URL
});

app.UseHttpsRedirection();
app.UseCors("FrontendOnly");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ─────────────────────────────────────────────────────────
// AUTO-MIGRATE & SEED on startup (runs in all environments)
// ─────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    db.Database.Migrate();
}

app.Run();

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyId.Application.Interface;
using MyId.Application.Services;
using MyId.Domain.Entities;
using MyId.Persistence.Data;
using MyId.Persistence.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────
// CORS — allow all origins (Swagger UI + any frontend)
// ─────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ─────────────────────────────────────────────────────────────
// Forward headers — required for Render's reverse proxy
// (safe to include locally; has no effect when no proxy is present)
// ─────────────────────────────────────────────────────────────
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// ─────────────────────────────────────────────────────────────
// DbContext — supports both environments:
//   LOCAL:      appsettings.Development.json → SQL Server
//   RENDER:     DATABASE_URL env var         → PostgreSQL
//
// Render automatically sets DATABASE_URL as a postgres:// URI.
// Locally, appsettings.Development.json provides the SQL Server
// connection string via ConnectionStrings:DefaultConnection.
// ─────────────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Render sets DATABASE_URL automatically for linked PostgreSQL databases
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

    // Fall back to appsettings (used locally with SQL Server)
    var connectionString = !string.IsNullOrEmpty(databaseUrl)
        ? databaseUrl
        : builder.Configuration.GetConnectionString("DefaultConnection");

    var isPostgres = !string.IsNullOrEmpty(connectionString) &&
                     (connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) ||
                      connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase));

    if (isPostgres)
    {
        // PostgreSQL — Render production
        options.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.MigrationsAssembly("MyId.Persistence");
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null
            );
        });
    }
    else
    {
        // SQL Server — local development
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.MigrationsAssembly("MyId.Persistence");
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null
            );
        });
    }
});

// ─────────────────────────────────────────────────────────────
// Identity
// ─────────────────────────────────────────────────────────────
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// ─────────────────────────────────────────────────────────────
// JWT Authentication
//   LOCAL:  Jwt:Key comes from appsettings.Development.json
//   RENDER: Set environment variables Jwt__Key, Jwt__Issuer, Jwt__Audience
//           (double underscore = colon separator in Render env vars)
// ─────────────────────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? "SUPER_SECRET_KEY_FOR_JWT_TOKEN_AUTHENTICATION_12345!"; // fallback for safety

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
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "IdPicture",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "IdPictureUsers",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// ─────────────────────────────────────────────────────────────
// Dependency Injection — Repositories
// ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<IEducationRepository, EducationRepository>();
builder.Services.AddScoped<IExperienceRepository, ExperienceRepository>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<IGalleryRepository, GalleryRepositories>();
builder.Services.AddScoped<ITaxCalculatorRepository, TaxCalculatorRepository>();

// ─────────────────────────────────────────────────────────────
// Dependency Injection — Application Services
// ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<IEducationService, EducationService>();
builder.Services.AddScoped<IExperienceService, ExperienceService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IGalleryService, GalleryService>();
builder.Services.AddScoped<ITaxCalculatorService, TaxCalculatorService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ─────────────────────────────────────────────────────────────
// Swagger with Bearer Auth
// ─────────────────────────────────────────────────────────────
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "IdPicture API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization: paste your token below (without 'Bearer ' prefix).",
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

var app = builder.Build();

// ─────────────────────────────────────────────────────────────
// Auto-Apply Migrations & Seed Admin User on Startup
//
// Strategy:
//   • PostgreSQL (Render): MigrateAsync() — applies EF migrations
//   • SQL Server  (local): EnsureCreated() — creates schema without
//     migrations, preserving existing local data on restart
// ─────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var providerName = context.Database.ProviderName ?? string.Empty;

        if (providerName.Contains("Npgsql", StringComparison.OrdinalIgnoreCase))
        {
            // Production (Render) — run EF Core migrations
            await context.Database.MigrateAsync();
        }
        else
        {
            // Local development (SQL Server) — create schema if it doesn't exist
            await context.Database.EnsureCreatedAsync();
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await SeedAdminUserAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

// ─────────────────────────────────────────────────────────────
// Middleware Pipeline
// ─────────────────────────────────────────────────────────────

// Must come first — resolves X-Forwarded-Proto from Render's load balancer
app.UseForwardedHeaders();

// Swagger — enabled in all environments (local + Render)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdPicture API v1");
    c.RoutePrefix = "swagger";
});

// ⚠ UseHttpsRedirection intentionally removed:
// Render terminates TLS at its edge — the container only receives HTTP on port 10000.
// Adding HTTPS redirect causes redirect loops and breaks Swagger on Render.
// Local dev: HTTPS profile in launchSettings.json handles this natively.

app.UseCors("AllowAll");

app.UseStaticFiles();

// Gallery uploads — path outside wwwroot for persistence
var persistentUploadsPath = Path.GetFullPath(
    Path.Combine(app.Environment.ContentRootPath, "..", "gallery_uploads"));

if (!Directory.Exists(persistentUploadsPath))
{
    Directory.CreateDirectory(persistentUploadsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(persistentUploadsPath),
    RequestPath = "/gallery_uploads"
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// ─────────────────────────────────────────────────────────────
// Helper: Seed Admin User
// ─────────────────────────────────────────────────────────────
static async Task SeedAdminUserAsync(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager)
{
    string[] roles = { "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    string adminEmail = "admin@idpicture.com";
    string adminPassword = "AdminSecurePass2026!";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "System Administrator",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
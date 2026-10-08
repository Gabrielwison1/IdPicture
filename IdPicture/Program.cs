
using Microsoft.AspNetCore.Authentication.JwtBearer;
        using Microsoft.AspNetCore.Identity;
        using Microsoft.EntityFrameworkCore;
        using Microsoft.IdentityModel.Tokens;
        using Microsoft.OpenApi.Models;
        using MyId.Application.Interface;
        using MyId.Application.Services;
        using MyId.Domain.Entities;
        using MyId.Persistence.Data;
        using MyId.Persistence.Repositories;
        using System.Text;

        var builder = WebApplication.CreateBuilder(args);

        // DbContext with explicit MigrationsAssembly & Transient Error Resiliency for Azure SQL
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("MyId.Persistence");
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                    );
                }
            ));

        // Identity Configuration
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        // JWT Authentication Setup
        var jwtKey = builder.Configuration["Jwt:Key"] ?? "SUPER_SECRET_KEY_FOR_JWT_TOKEN_AUTHENTICATION_12345!";
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

        // Dependency Injection - Repositories
        builder.Services.AddScoped<IEducationRepository, EducationRepository>();
        builder.Services.AddScoped<IExperienceRepository, ExperienceRepository>();
        builder.Services.AddScoped<ISkillRepository, SkillRepository>();
        builder.Services.AddScoped<IGalleryRepository, GalleryRepositories>();
        builder.Services.AddScoped<ITaxCalculatorRepository, TaxCalculatorRepository>();

        // Dependency Injection - Application Services
        builder.Services.AddScoped<IEducationService, EducationService>();
        builder.Services.AddScoped<IExperienceService, ExperienceService>();
        builder.Services.AddScoped<ISkillService, SkillService>();
        builder.Services.AddScoped<IGalleryService, GalleryService>();
        builder.Services.AddScoped<ITaxCalculatorService, TaxCalculatorService>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        // Swagger Generator Configuration with Bearer Auth
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "IdPicture API", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Enter your JWT token directly below without typing 'Bearer'.",
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

        // Enable Swagger UI in Development AND Production (Azure App Service)
        if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdPicture API v1");
                c.RoutePrefix = string.Empty; // Serves Swagger UI at the root domain URL
            });
        }

        app.UseHttpsRedirection();

        // Enable Static Files for Image Upload Access (wwwroot/uploads)
        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
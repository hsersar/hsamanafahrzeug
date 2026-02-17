using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using AspNetCoreRateLimit;
using iKfz.Backend.Data;
using iKfz.Backend.Repositories;
using iKfz.Backend.Services;
using iKfz.Backend.Auth;

// Configure Serilog for logging without PII
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Use Serilog
builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Configure Database (SQLite for demo, use SQL Server in production)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=ikfz.db"));

// Configure Memory Cache
builder.Services.AddMemoryCache();

// Configure Rate Limiting for scalability (≥100 parallel requests)
builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.HttpStatusCode = 429;
    options.RealIpHeader = "X-Real-IP";
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "*",
            Period = "1s",
            Limit = 10
        },
        new RateLimitRule
        {
            Endpoint = "*",
            Period = "1m",
            Limit = 100
        }
    };
});
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

// Configure CORS – restrict to specific methods and headers (F-07)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                builder.Configuration.GetValue<string>("Frontend:Url") ?? "http://localhost:3000")
              .WithMethods("GET", "POST", "PUT", "DELETE")
              .WithHeaders("Content-Type", "Authorization", "X-Mock-Role")
              .AllowCredentials();
    });
});

// Configure Authentication (OAuth2/OIDC in production, dev bypass locally)
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddAuthentication(DevAuthHandler.SchemeName)
        .AddScheme<AuthenticationSchemeOptions, DevAuthHandler>(DevAuthHandler.SchemeName, _ => { });
}
else
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = jwtSettings.GetValue<string>("Authority");
            options.Audience = jwtSettings.GetValue<string>("Audience");
            options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    // Log without PII
                    Log.Warning("Authentication failed");
                    return Task.CompletedTask;
                }
            };
        });
}

// Authorization policies for role-based access (F-03)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireMitarbeiter", policy =>
        policy.RequireRole("Mitarbeiter", "StandortAdmin", "SuperAdmin"));
    options.AddPolicy("RequireStandortAdmin", policy =>
        policy.RequireRole("StandortAdmin", "SuperAdmin"));
    options.AddPolicy("RequireSuperAdmin", policy =>
        policy.RequireRole("SuperAdmin"));
});

// Register repositories and services
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IRegistrationRequestRepository, RegistrationRequestRepository>();
builder.Services.AddScoped<IPersonalProfileRepository, PersonalProfileRepository>();
builder.Services.AddScoped<ICompanyProfileRepository, CompanyProfileRepository>();
builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "iKfz API", Version = "v1" });
});

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

// Suppress Kestrel Server header (F-30)
builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
});

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global exception handler – prevents stack trace leaks (F-27)
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        Log.Error("Unhandled exception on {Method} {Path}",
            context.Request.Method, context.Request.Path);
        await context.Response.WriteAsJsonAsync(new { error = "Ein interner Fehler ist aufgetreten." });
    });
});

// Security Headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "no-referrer");
    context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");
    context.Response.Headers.Append("Content-Security-Policy", 
        "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'; img-src 'self' data: blob:;");
    await next();
});

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

// Rate limiting
app.UseIpRateLimiting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

Log.Information("iKfz Backend starting...");

try
{
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

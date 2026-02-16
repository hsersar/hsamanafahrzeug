using FahrzeugZulassung.Application.Interfaces;
using FahrzeugZulassung.Infrastructure.Data;
using FahrzeugZulassung.Infrastructure.Services;
using FahrzeugZulassung.API.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ?? 
        "Server=(localdb)\\mssqllocaldb;Database=FahrzeugZulassungDb;Trusted_Connection=True;MultipleActiveResultSets=true"));

// SignalR
builder.Services.AddSignalR();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:3000" })
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Application Services
builder.Services.AddScoped<ITrackingService, TrackingService>();
builder.Services.AddScoped<IPaymentService, StripePaymentService>();
builder.Services.AddScoped<IBenachrichtigungService, BenachrichtigungService>();

// Authentication (simplified for now)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Kunde", policy => policy.RequireAuthenticatedUser());
    options.AddPolicy("Mitarbeiter", policy => policy.RequireAuthenticatedUser());
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthorization();

app.MapControllers();
app.MapHub<TrackingHub>("/hubs/tracking");

app.Run();

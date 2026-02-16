using FahrzeugZulassung.Application.Interfaces;
using FahrzeugZulassung.Infrastructure.Persistence.Data;
using FahrzeugZulassung.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("FahrzeugZulassungDb")); // Using InMemory for demo

// Register Application Services
builder.Services.AddScoped<IProvisionsService, ProvisionsService>();
builder.Services.AddScoped<IMonatsabrechnungService, MonatsabrechnungService>();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await SeedData(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();

async Task SeedData(ApplicationDbContext context)
{
    // Create demo Standorte
    if (!context.Standorte.Any())
    {
        var standorte = new[]
        {
            new FahrzeugZulassung.Domain.Entities.Standort
            {
                Id = Guid.NewGuid(),
                Name = "Standort Berlin",
                Adresse = "Berliner Str. 1, 10115 Berlin",
                IstAktiv = true
            },
            new FahrzeugZulassung.Domain.Entities.Standort
            {
                Id = Guid.NewGuid(),
                Name = "Standort München",
                Adresse = "Münchner Str. 1, 80331 München",
                IstAktiv = true
            },
            new FahrzeugZulassung.Domain.Entities.Standort
            {
                Id = Guid.NewGuid(),
                Name = "Standort Hamburg",
                Adresse = "Hamburger Str. 1, 20095 Hamburg",
                IstAktiv = true
            }
        };
        
        context.Standorte.AddRange(standorte);
        await context.SaveChangesAsync();
    }
    
    // Create Standard-Provisionsmodell
    if (!context.ProvisionsModelle.Any(p => p.IstStandard))
    {
        var standardModell = new FahrzeugZulassung.Domain.Entities.ProvisionsModell
        {
            Id = Guid.NewGuid(),
            IstStandard = true,
            StandortId = null,
            MonatlicheGrundgebuehr = 199.00m,
            ProvisionsProzentsatz = 2.0m,
            GueltigAb = DateTime.UtcNow,
            IstAktiv = true,
            ErstelltAm = DateTime.UtcNow,
            ErstelltVon = "System",
            Aenderungsgrund = "Initial standard commission model"
        };
        
        context.ProvisionsModelle.Add(standardModell);
        await context.SaveChangesAsync();
    }
}


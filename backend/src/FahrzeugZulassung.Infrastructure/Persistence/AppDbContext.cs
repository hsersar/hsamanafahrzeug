using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FahrzeugZulassung.Domain.Entities;
using FahrzeugZulassung.Infrastructure.Persistence.Configurations;

namespace FahrzeugZulassung.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<Benutzer, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Standort> Standorte => Set<Standort>();
    public DbSet<Kunde> Kunden => Set<Kunde>();
    public DbSet<Fahrzeug> Fahrzeuge => Set<Fahrzeug>();
    public DbSet<Auftrag> Auftraege => Set<Auftrag>();
    public DbSet<Rechnung> Rechnungen => Set<Rechnung>();
    public DbSet<Dokument> Dokumente => Set<Dokument>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new BenutzerConfiguration());
        builder.ApplyConfiguration(new StandortConfiguration());
        builder.ApplyConfiguration(new KundeConfiguration());
        builder.ApplyConfiguration(new FahrzeugConfiguration());
        builder.ApplyConfiguration(new AuftragConfiguration());
        builder.ApplyConfiguration(new RechnungConfiguration());
        builder.ApplyConfiguration(new DokumentConfiguration());
        builder.ApplyConfiguration(new AuditLogConfiguration());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified)
            .ToList();

        foreach (var entry in entries)
        {
            if (entry.Entity.GetType().GetProperty("GeaendertAm") != null)
            {
                entry.Property("GeaendertAm").CurrentValue = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}

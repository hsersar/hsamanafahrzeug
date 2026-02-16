using FahrzeugZulassung.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FahrzeugZulassung.Infrastructure.Data;

public class FahrzeugZulassungDbContext : DbContext
{
    public FahrzeugZulassungDbContext(DbContextOptions<FahrzeugZulassungDbContext> options)
        : base(options)
    {
    }

    public DbSet<Standort> Standorte { get; set; }
    public DbSet<Kunde> Kunden { get; set; }
    public DbSet<Rechnung> Rechnungen { get; set; }
    public DbSet<RechnungsPosition> RechnungsPositionen { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Standort
        modelBuilder.Entity<Standort>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Firmenname).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Strasse).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PLZ).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Ort).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UStID).HasMaxLength(50);
            entity.Property(e => e.IBAN).HasMaxLength(34);
            entity.Property(e => e.BIC).HasMaxLength(11);
        });

        // Kunde
        modelBuilder.Entity<Kunde>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Vorname).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Nachname).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Strasse).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PLZ).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Ort).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UStID).HasMaxLength(50);
        });

        // Rechnung
        modelBuilder.Entity<Rechnung>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RechnungsNummer).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Nettobetrag).HasPrecision(18, 2);
            entity.Property(e => e.Steuerbetrag).HasPrecision(18, 2);
            entity.Property(e => e.Bruttobetrag).HasPrecision(18, 2);
            
            entity.HasOne(e => e.Kunde)
                .WithMany(k => k.Rechnungen)
                .HasForeignKey(e => e.KundeId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Standort)
                .WithMany()
                .HasForeignKey(e => e.StandortId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasIndex(e => e.RechnungsNummer).IsUnique();
        });

        // RechnungsPosition
        modelBuilder.Entity<RechnungsPosition>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Beschreibung).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Einheit).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Einzelpreis).HasPrecision(18, 2);
            entity.Property(e => e.Nettobetrag).HasPrecision(18, 2);
            entity.Property(e => e.Steuersatz).HasPrecision(5, 2);
            entity.Property(e => e.Steuerbetrag).HasPrecision(18, 2);
            entity.Property(e => e.Bruttobetrag).HasPrecision(18, 2);
            
            entity.HasOne(e => e.Rechnung)
                .WithMany(r => r.Positionen)
                .HasForeignKey(e => e.RechnungId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

using FahrzeugZulassung.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FahrzeugZulassung.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Auftrag> Auftraege { get; set; } = null!;
    public DbSet<Rechnung> Rechnungen { get; set; } = null!;
    public DbSet<Zahlung> Zahlungen { get; set; } = null!;
    public DbSet<AuftragTracking> AuftragTrackings { get; set; } = null!;
    public DbSet<AuftragStatusHistorie> AuftragStatusHistorien { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Auftrag
        modelBuilder.Entity<Auftrag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Antragsnummer).IsRequired().HasMaxLength(50);
            entity.Property(e => e.AuftragTyp).IsRequired().HasMaxLength(50);
            entity.Property(e => e.KundenEmail).IsRequired().HasMaxLength(255);
            entity.Property(e => e.KundenTelefon).HasMaxLength(50);
            entity.Property(e => e.Kennzeichen).HasMaxLength(20);

            entity.HasOne(e => e.Rechnung)
                .WithOne(r => r.Auftrag)
                .HasForeignKey<Rechnung>(r => r.AuftragId);

            entity.HasOne(e => e.Tracking)
                .WithOne(t => t.Auftrag)
                .HasForeignKey<AuftragTracking>(t => t.AuftragId);

            entity.HasMany(e => e.StatusHistorie)
                .WithOne(h => h.Auftrag)
                .HasForeignKey(h => h.AuftragId);
        });

        // Rechnung
        modelBuilder.Entity<Rechnung>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Rechnungsnummer).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Betrag).HasPrecision(18, 2);
            entity.Property(e => e.Waehrung).HasMaxLength(3);

            entity.HasMany(e => e.Zahlungen)
                .WithOne(z => z.Rechnung)
                .HasForeignKey(z => z.RechnungId);
        });

        // Zahlung
        modelBuilder.Entity<Zahlung>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Betrag).HasPrecision(18, 2);
            entity.Property(e => e.Waehrung).HasMaxLength(3);
            entity.Property(e => e.StripePaymentIntentId).HasMaxLength(255);
            entity.Property(e => e.StripeClientSecret).HasMaxLength(500);
            entity.Property(e => e.PayPalOrderId).HasMaxLength(255);
            entity.Property(e => e.PayPalApprovalUrl).HasMaxLength(500);
            entity.Property(e => e.UeberweisungsReferenz).HasMaxLength(100);
            entity.Property(e => e.EmpfaengerIBAN).HasMaxLength(34);
            entity.Property(e => e.EmpfaengerBIC).HasMaxLength(11);
            entity.Property(e => e.EmpfaengerBank).HasMaxLength(200);
            entity.Property(e => e.Verwendungszweck).HasMaxLength(140);
        });

        // AuftragTracking
        modelBuilder.Entity<AuftragTracking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TrackingCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TrackingToken).IsRequired().HasMaxLength(64);
            entity.Property(e => e.QRCodeUrl).HasMaxLength(500);

            entity.HasIndex(e => e.TrackingCode).IsUnique();
            entity.HasIndex(e => new { e.TrackingCode, e.TrackingToken });
        });

        // AuftragStatusHistorie
        modelBuilder.Entity<AuftragStatusHistorie>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Kommentar).HasMaxLength(1000);
            entity.Property(e => e.BearbeitetVon).HasMaxLength(255);
        });
    }
}

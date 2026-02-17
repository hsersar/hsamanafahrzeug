using Microsoft.EntityFrameworkCore;
using iKfz.Backend.Models;

namespace iKfz.Backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<RegistrationRequest> RegistrationRequests { get; set; }
    public DbSet<PersonalProfile> PersonalProfiles { get; set; }
    public DbSet<CompanyProfile> CompanyProfiles { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Announcement> Announcements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Vehicle entity
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.VIN).IsUnique();
            entity.HasIndex(e => e.LicensePlate).IsUnique();
            entity.Property(e => e.VIN).IsRequired().HasMaxLength(17);
            entity.Property(e => e.LicensePlate).IsRequired().HasMaxLength(15);
            entity.Property(e => e.Brand).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Model).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Color).IsRequired().HasMaxLength(50);
            entity.Property(e => e.OwnerId).IsRequired().HasMaxLength(256);
            entity.HasIndex(e => e.OwnerId);
        });

        // Configure RegistrationRequest entity
        modelBuilder.Entity<RegistrationRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.VIN);
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(256);
            entity.Property(e => e.VIN).IsRequired().HasMaxLength(17);
            entity.Property(e => e.RequestedLicensePlate).IsRequired().HasMaxLength(15);
            entity.Property(e => e.Brand).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Model).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Color).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ProcessedBy).HasMaxLength(256);
            entity.Property(e => e.RejectionReason).HasMaxLength(500);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Status);
        });

        // Configure PersonalProfile entity
        modelBuilder.Entity<PersonalProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Anrede).HasMaxLength(20);
            entity.Property(e => e.Titel).HasMaxLength(50);
            entity.Property(e => e.Vorname).HasMaxLength(100);
            entity.Property(e => e.Nachname).HasMaxLength(100);
            entity.Property(e => e.Strasse).HasMaxLength(200);
            entity.Property(e => e.Hausnummer).HasMaxLength(20);
            entity.Property(e => e.Plz).HasMaxLength(10);
            entity.Property(e => e.Ort).HasMaxLength(100);
            entity.Property(e => e.Telefon).HasMaxLength(30);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.ProfilbildPfad).HasMaxLength(500);
        });

        // Configure CompanyProfile entity
        modelBuilder.Entity<CompanyProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Firmenname).HasMaxLength(200);
            entity.Property(e => e.Rechtsform).HasMaxLength(50);
            entity.Property(e => e.Handelsregisternummer).HasMaxLength(50);
            entity.Property(e => e.UstIdNr).HasMaxLength(30);
            entity.Property(e => e.Strasse).HasMaxLength(200);
            entity.Property(e => e.Hausnummer).HasMaxLength(20);
            entity.Property(e => e.Plz).HasMaxLength(10);
            entity.Property(e => e.Ort).HasMaxLength(100);
            entity.Property(e => e.Telefon).HasMaxLength(30);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Website).HasMaxLength(256);
            entity.Property(e => e.AnsprechpartnerAnrede).HasMaxLength(20);
            entity.Property(e => e.AnsprechpartnerVorname).HasMaxLength(100);
            entity.Property(e => e.AnsprechpartnerNachname).HasMaxLength(100);
            entity.Property(e => e.AnsprechpartnerTelefon).HasMaxLength(30);
            entity.Property(e => e.AnsprechpartnerEmail).HasMaxLength(256);
            entity.Property(e => e.AnsprechpartnerPosition).HasMaxLength(100);
            entity.Property(e => e.LogoPfad).HasMaxLength(500);
        });

        // Configure PaymentMethod entity
        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Typ).HasMaxLength(30);
            entity.Property(e => e.Bezeichnung).HasMaxLength(100);
            entity.Property(e => e.Kontoinhaber).HasMaxLength(200);
            entity.Property(e => e.IBAN).HasMaxLength(34);
            entity.Property(e => e.BIC).HasMaxLength(11);
            entity.Property(e => e.Bankname).HasMaxLength(200);
            entity.Property(e => e.KartenNummer).HasMaxLength(4);
            entity.Property(e => e.KartenInhaber).HasMaxLength(200);
            entity.Property(e => e.GueltigBis).HasMaxLength(5);
            entity.Property(e => e.PaypalEmail).HasMaxLength(256);
        });

        // Configure Invoice entity
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Rechnungsnummer).IsUnique();
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Rechnungsnummer).IsRequired().HasMaxLength(30);
            entity.Property(e => e.Beschreibung).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Nettobetrag).HasPrecision(10, 2);
            entity.Property(e => e.MwstSatz).HasPrecision(5, 2);
            entity.Property(e => e.MwstBetrag).HasPrecision(10, 2);
            entity.Property(e => e.Bruttobetrag).HasPrecision(10, 2);
            entity.Property(e => e.Zahlungsmethode).HasMaxLength(50);
            entity.HasIndex(e => e.Status);
        });

        // Configure Announcement entity
        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titel).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Nachricht).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Typ).HasMaxLength(20);
        });
    }
}

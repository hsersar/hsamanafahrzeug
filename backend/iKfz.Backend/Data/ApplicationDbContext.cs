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
    }
}

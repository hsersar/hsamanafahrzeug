using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FahrzeugZulassung.Domain.Entities;

namespace FahrzeugZulassung.Infrastructure.Persistence.Configurations;

public class StandortConfiguration : IEntityTypeConfiguration<Standort>
{
    public void Configure(EntityTypeBuilder<Standort> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Strasse)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Hausnummer)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(s => s.PLZ)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(s => s.Ort)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Telefon)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.IstAktiv)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.ErstelltAm)
            .IsRequired();

        builder.HasMany(s => s.Mitarbeiter)
            .WithOne(b => b.Standort)
            .HasForeignKey(b => b.StandortId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(s => s.Auftraege)
            .WithOne(a => a.Standort)
            .HasForeignKey(a => a.StandortId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.Name);
        builder.HasIndex(s => s.PLZ);
        builder.HasIndex(s => s.Email);
        builder.HasIndex(s => s.IstAktiv);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FahrzeugZulassung.Domain.Entities;

namespace FahrzeugZulassung.Infrastructure.Persistence.Configurations;

public class DokumentConfiguration : IEntityTypeConfiguration<Dokument>
{
    public void Configure(EntityTypeBuilder<Dokument> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Typ)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(d => d.DateiName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(d => d.DateiPfad)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.DateiGroesse)
            .IsRequired();

        builder.Property(d => d.HochgeladenAm)
            .IsRequired();

        builder.HasOne(d => d.Auftrag)
            .WithMany(a => a.Dokumente)
            .HasForeignKey(d => d.AuftragId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.HochgeladenVon)
            .WithMany()
            .HasForeignKey(d => d.HochgeladenVonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => d.AuftragId);
        builder.HasIndex(d => d.Typ);
        builder.HasIndex(d => d.HochgeladenAm);
    }
}

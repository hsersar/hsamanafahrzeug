using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FahrzeugZulassung.Domain.Entities;

namespace FahrzeugZulassung.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Aktion)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Entitaet)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.EntitaetId)
            .HasMaxLength(50);

        builder.Property(a => a.AlteWerte)
            .HasColumnType("text");

        builder.Property(a => a.NeueWerte)
            .HasColumnType("text");

        builder.Property(a => a.IPAdresse)
            .IsRequired()
            .HasMaxLength(45);

        builder.Property(a => a.UserAgent)
            .HasMaxLength(500);

        builder.Property(a => a.Zeitstempel)
            .IsRequired();

        builder.HasOne(a => a.Benutzer)
            .WithMany(b => b.AuditLogs)
            .HasForeignKey(a => a.BenutzerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(a => a.BenutzerId);
        builder.HasIndex(a => a.Aktion);
        builder.HasIndex(a => a.Entitaet);
        builder.HasIndex(a => a.EntitaetId);
        builder.HasIndex(a => a.Zeitstempel);
        builder.HasIndex(a => new { a.Entitaet, a.EntitaetId });
    }
}

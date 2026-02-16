using Microsoft.EntityFrameworkCore;
using FahrzeugZulassung.Domain.Entities;

namespace FahrzeugZulassung.Infrastructure.Persistence.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Standort> Standorte { get; set; } = null!;
    public DbSet<ProvisionsModell> ProvisionsModelle { get; set; } = null!;
    public DbSet<ProvisionsStaffel> ProvisionsStaffeln { get; set; } = null!;
    public DbSet<Monatsabrechnung> Monatsabrechnungen { get; set; } = null!;
    public DbSet<MonatsabrechnungPosition> MonatsabrechnungPositionen { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}

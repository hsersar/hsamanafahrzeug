using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FahrzeugZulassung.Domain.Entities;
using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task InitializeAsync(
        AppDbContext context,
        UserManager<Benutzer> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        // Ensure database is created
        await context.Database.MigrateAsync();

        // Seed roles
        await SeedRolesAsync(roleManager);

        // Seed default standort
        var standort = await SeedStandortAsync(context);

        // Seed admin user
        await SeedAdminUserAsync(userManager, standort);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        string[] roles = { "SuperAdmin", "StandortAdmin", "Mitarbeiter", "Kunde" };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                });
            }
        }
    }

    private static async Task<Standort> SeedStandortAsync(AppDbContext context)
    {
        if (await context.Standorte.AnyAsync())
        {
            return await context.Standorte.FirstAsync();
        }

        var standort = new Standort
        {
            Id = Guid.NewGuid(),
            Name = "Hauptstandort",
            Strasse = "Musterstraße",
            Hausnummer = "1",
            PLZ = "80331",
            Ort = "München",
            Telefon = "+49 89 12345678",
            Email = "hauptstandort@fahrzeugzulassung.de",
            IstAktiv = true,
            ErstelltAm = DateTime.UtcNow
        };

        context.Standorte.Add(standort);
        await context.SaveChangesAsync();

        return standort;
    }

    private static async Task SeedAdminUserAsync(
        UserManager<Benutzer> userManager,
        Standort standort)
    {
        var adminEmail = "admin@fahrzeugzulassung.de";

        if (await userManager.FindByEmailAsync(adminEmail) != null)
        {
            return;
        }

        var adminUser = new Benutzer
        {
            Id = Guid.NewGuid(),
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            Vorname = "Super",
            Nachname = "Admin",
            Rolle = BenutzerRolle.SuperAdmin,
            StandortId = standort.Id,
            IstAktiv = true,
            ErstelltAm = DateTime.UtcNow,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await userManager.CreateAsync(adminUser, "Admin@123456789");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
        }
    }
}

using FahrzeugZulassung.Domain.Entities;
using FahrzeugZulassung.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FahrzeugZulassung.Infrastructure;

public static class DbSeeder
{
    public static void SeedData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FahrzeugZulassungDbContext>();

        // Prüfen ob bereits Daten vorhanden sind
        if (context.Standorte.Any() || context.Kunden.Any())
            return;

        // Standort erstellen
        var standort = new Standort
        {
            Id = Guid.NewGuid(),
            Name = "Hauptstelle München",
            Firmenname = "KFZ-Zulassungsstelle München GmbH",
            Strasse = "Maximilianstraße 45",
            PLZ = "80538",
            Ort = "München",
            UStID = "DE123456789",
            Steuernummer = "143/815/08154",
            IBAN = "DE89370400440532013000",
            BIC = "COBADEFFXXX",
            Bankname = "Commerzbank München",
            HandelsregisterNr = "HRB 123456",
            Geschaeftsfuehrer = "Max Mustermann"
        };

        // Kunden erstellen
        var kunde1 = new Kunde
        {
            Id = Guid.NewGuid(),
            Vorname = "Anna",
            Nachname = "Schmidt",
            Email = "anna.schmidt@email.de",
            Telefon = "+49 89 12345678",
            Strasse = "Leopoldstraße 123",
            PLZ = "80802",
            Ort = "München"
        };

        var kunde2 = new Kunde
        {
            Id = Guid.NewGuid(),
            Vorname = "Michael",
            Nachname = "Weber",
            Email = "m.weber@firma.de",
            Telefon = "+49 89 98765432",
            Strasse = "Sendlinger Straße 50",
            PLZ = "80331",
            Ort = "München",
            Firmenname = "Weber Transporte GmbH",
            UStID = "DE987654321"
        };

        context.Standorte.Add(standort);
        context.Kunden.AddRange(kunde1, kunde2);
        context.SaveChanges();

        Console.WriteLine("✓ Seed-Daten erfolgreich eingefügt:");
        Console.WriteLine($"  - 1 Standort: {standort.Firmenname}");
        Console.WriteLine($"  - 2 Kunden: {kunde1.Vorname} {kunde1.Nachname}, {kunde2.Vorname} {kunde2.Nachname}");
    }
}

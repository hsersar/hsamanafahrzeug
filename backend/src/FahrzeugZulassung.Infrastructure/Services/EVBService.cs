using FahrzeugZulassung.Application.Interfaces;
using FahrzeugZulassung.Domain.Enums;
using System.Text.RegularExpressions;

namespace FahrzeugZulassung.Infrastructure.Services;

public class EVBService : IEVBService
{
    private static readonly string[] VerboteneZeichen = { "I", "O", "Q" };
    private static readonly Regex EvbFormatRegex = new Regex("^[A-HJ-NPR-Z0-9]{7}$", RegexOptions.Compiled);

    public bool IstFormatGueltig(string evbNummer)
    {
        if (string.IsNullOrWhiteSpace(evbNummer))
            return false;

        // Entferne Bindestriche und Leerzeichen
        evbNummer = evbNummer.Replace("-", "").Replace(" ", "").ToUpperInvariant();

        // Prüfe Länge
        if (evbNummer.Length != 7)
            return false;

        // Prüfe Format (A-Z, 0-9, aber ohne I, O, Q)
        if (!EvbFormatRegex.IsMatch(evbNummer))
            return false;

        // Prüfe verbotene Zeichen
        foreach (var zeichen in VerboteneZeichen)
        {
            if (evbNummer.Contains(zeichen))
                return false;
        }

        return true;
    }

    public async Task<EVBValidierungResult> ValidiereEVBNummerAsync(string evbNummer)
    {
        evbNummer = evbNummer.Replace("-", "").Replace(" ", "").ToUpperInvariant();

        if (!IstFormatGueltig(evbNummer))
        {
            return new EVBValidierungResult
            {
                IstGueltig = false,
                Fehlermeldung = "Die eVB-Nummer muss genau 7 Zeichen lang sein und darf nur Großbuchstaben (außer I, O, Q) und Ziffern enthalten."
            };
        }

        // In einer echten Implementierung würde hier eine GDV-API-Abfrage erfolgen
        await Task.CompletedTask;

        return new EVBValidierungResult
        {
            IstGueltig = true,
            GueltigBis = DateTime.UtcNow.AddMonths(6) // Normalerweise 6 Monate gültig
        };
    }

    public async Task<EVBAbfrageResult> PruefeBeimGDVAsync(string evbNummer)
    {
        // Vorbereitet für zukünftige GDV-API-Anbindung
        // Aktuell nur Mock-Implementierung
        var validierung = await ValidiereEVBNummerAsync(evbNummer);

        return new EVBAbfrageResult
        {
            Erfolgreich = validierung.IstGueltig,
            Fehlermeldung = validierung.Fehlermeldung,
            Validierung = validierung
        };
    }

    public async Task<EVBNummer> SpeichereEVBAsync(Guid auftragId, EVBEingabeDto dto)
    {
        // In einer echten Implementierung würde hier die eVB-Nummer in der Datenbank gespeichert
        await Task.CompletedTask;

        return new EVBNummer
        {
            Id = Guid.NewGuid(),
            Nummer = dto.Nummer.Replace("-", "").Replace(" ", "").ToUpperInvariant(),
            Versicherungsgesellschaft = dto.Versicherungsgesellschaft
        };
    }
}

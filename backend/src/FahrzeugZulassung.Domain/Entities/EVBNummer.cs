using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Domain.Entities;

public class EVBNummer
{
    public Guid Id { get; set; }
    public Guid AuftragId { get; set; }
    public string Nummer { get; set; } = string.Empty;          // 7-stellig, alphanumerisch
    public string? Versicherungsgesellschaft { get; set; }
    public string? Versicherungsnummer { get; set; }
    public EVBStatus Status { get; set; } = EVBStatus.Eingegeben;
    public EVBVerwendungszweck Verwendungszweck { get; set; }
    
    // Fahrzeugdaten (von der Versicherung)
    public string? FahrzeugIdentNr { get; set; }                // FIN
    public string? Kennzeichen { get; set; }
    public DateTime? GueltigBis { get; set; }                   // Gültigkeit (meist 6 Monate)
    
    // GDV-Abfrage (für zukünftige API-Anbindung)
    public bool GDVGeprueft { get; set; } = false;
    public DateTime? GDVPruefungAm { get; set; }
    public string? GDVAntwort { get; set; }                     // JSON Response
    
    // Timestamps
    public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public Auftrag Auftrag { get; set; } = null!;
}

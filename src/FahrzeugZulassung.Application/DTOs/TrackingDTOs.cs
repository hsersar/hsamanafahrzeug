using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Application.DTOs;

public class TrackingInfo
{
    public string TrackingCode { get; set; } = string.Empty;
    public string TrackingUrl { get; set; } = string.Empty;
    public byte[] QRCodeBytes { get; set; } = Array.Empty<byte>();
    public string QRCodeBase64 { get; set; } = string.Empty;
}

public class TrackingStatusResponse
{
    public string TrackingCode { get; set; } = string.Empty;
    public string AuftragTyp { get; set; } = string.Empty;       // Anmeldung/Abmeldung/Ummeldung
    public string AktuellerStatus { get; set; } = string.Empty;
    public string StatusBeschreibung { get; set; } = string.Empty;
    public int FortschrittProzent { get; set; }                   // 0-100%
    public DateTime ErstelltAm { get; set; }
    public DateTime? LetzteAktualisierung { get; set; }
    public string? Kennzeichen { get; set; }                      // Teilweise maskiert: "B-**1234"
    public bool ZahlungErforderlich { get; set; }
    public bool ZahlungErfolgt { get; set; }
    public List<StatusHistorieEintrag> Historie { get; set; } = new();
}

public class StatusHistorieEintrag
{
    public string Status { get; set; } = string.Empty;
    public string Beschreibung { get; set; } = string.Empty;
    public DateTime Zeitpunkt { get; set; }
    public bool IstAktuell { get; set; }
}

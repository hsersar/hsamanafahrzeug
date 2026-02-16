namespace FahrzeugZulassung.Application.DTOs.Signatur;

public class SignaturAnforderungResult
{
    public bool Erfolg { get; set; }
    public Guid? SignaturId { get; set; }
    public string? RedirectUrl { get; set; }        // URL wohin der Kunde weitergeleitet wird
    public string? SessionId { get; set; }
    public string? FehlerNachricht { get; set; }
    public int? SessionGueltigSekunden { get; set; }  // Wie lange die Session gültig ist
}

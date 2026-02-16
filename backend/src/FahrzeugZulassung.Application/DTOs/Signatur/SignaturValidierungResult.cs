namespace FahrzeugZulassung.Application.DTOs.Signatur;

public class SignaturValidierungResult
{
    public bool IstGueltig { get; set; }
    public bool ZertifikatGueltig { get; set; }
    public bool ZeitstempelGueltig { get; set; }
    public bool DokumentUnveraendert { get; set; }
    public string? SigniererName { get; set; }
    public string? ZertifikatAussteller { get; set; }
    public DateTime? SigniertAm { get; set; }
    public List<string> Warnungen { get; set; } = new();
    public List<string> Fehler { get; set; } = new();
}

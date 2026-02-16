using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Application.DTOs.Signatur;

public class SignaturStatusResult
{
    public Guid SignaturId { get; set; }
    public SignaturStatus Status { get; set; }
    public string StatusBeschreibung { get; set; } = string.Empty;
    public string? SigniererName { get; set; }
    public DateTime? SigniertAm { get; set; }
    public string? ZertifikatInfo { get; set; }
    public bool DokumentVerfuegbar { get; set; }
    public string? FehlerNachricht { get; set; }
}

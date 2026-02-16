using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Application.DTOs.Signatur;

public class SignaturAnforderungRequest
{
    public Guid AuftragId { get; set; }
    public SignaturTyp Typ { get; set; }
    public SignaturLevel Level { get; set; } = SignaturLevel.QES;
    public string SigniererEmail { get; set; } = string.Empty;
    public string SigniererName { get; set; } = string.Empty;
    public string? SigniererTelefon { get; set; }          // Für SMS-TAN
    public SignaturAuthMethode BevorzugteAuthMethode { get; set; } = SignaturAuthMethode.SMS_TAN;
    public string? RedirectUrlNachSignatur { get; set; }    // URL nach erfolgreicher Signatur
    public byte[]? DokumentBytes { get; set; }              // Das zu signierende PDF
    public string? DokumentName { get; set; }
}

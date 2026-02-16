namespace FahrzeugZulassung.Infrastructure.Services;

public class ZUGFeRDResult
{
    public string Xml { get; set; } = string.Empty;
    public byte[] PdfBytes { get; set; } = Array.Empty<byte>();
    public string DateiName { get; set; } = string.Empty;
    public bool IstGueltig { get; set; }
    public List<string> Validierungsfehler { get; set; } = new();
}

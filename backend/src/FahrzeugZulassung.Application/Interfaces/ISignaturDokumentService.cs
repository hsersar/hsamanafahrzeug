using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Application.Interfaces;

public interface ISignaturDokumentService
{
    /// <summary>
    /// Erstellt ein PDF-Dokument das signiert werden soll (z.B. Vollmacht, Antrag)
    /// </summary>
    Task<byte[]> ErstelleSignaturDokumentAsync(Guid auftragId, SignaturTyp typ);
    
    /// <summary>
    /// Bettet die Signatur in das PDF ein (PAdES-Format)
    /// </summary>
    Task<byte[]> BetteSignaturEinAsync(byte[] originalPdf, byte[] signaturBytes);
}

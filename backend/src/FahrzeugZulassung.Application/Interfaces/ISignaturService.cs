using FahrzeugZulassung.Application.DTOs.Signatur;

namespace FahrzeugZulassung.Application.Interfaces;

public interface ISignaturService
{
    /// <summary>
    /// Erstellt eine Signatur-Anforderung und leitet den QES-Prozess ein.
    /// </summary>
    Task<SignaturAnforderungResult> SignaturAnfordernAsync(SignaturAnforderungRequest request);
    
    /// <summary>
    /// Prüft den Status einer laufenden Signatur-Session beim Provider.
    /// </summary>
    Task<SignaturStatusResult> StatusPruefenAsync(Guid signaturId);
    
    /// <summary>
    /// Verarbeitet den Callback vom QES-Provider nach Authentifizierung.
    /// </summary>
    Task<SignaturCallbackResult> CallbackVerarbeitenAsync(SignaturCallbackRequest request);
    
    /// <summary>
    /// Ruft das signierte Dokument vom Provider ab.
    /// </summary>
    Task<byte[]> SigniertesDokumentAbrufenAsync(Guid signaturId);
    
    /// <summary>
    /// Validiert eine bestehende Signatur (Zertifikatskette, Zeitstempel, etc.)
    /// </summary>
    Task<SignaturValidierungResult> SignaturValidierenAsync(Guid signaturId);
    
    /// <summary>
    /// Storniert eine laufende Signatur-Anforderung.
    /// </summary>
    Task<bool> StornierenAsync(Guid signaturId);
}

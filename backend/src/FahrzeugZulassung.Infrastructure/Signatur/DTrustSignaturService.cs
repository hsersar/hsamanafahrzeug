using FahrzeugZulassung.Application.DTOs.Signatur;
using FahrzeugZulassung.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace FahrzeugZulassung.Infrastructure.Signatur;

/// <summary>
/// D-Trust / sign-me Integration (Bundesdruckerei)
/// REST API Integration für qualifizierte elektronische Signaturen
/// </summary>
public class DTrustSignaturService : ISignaturService
{
    private readonly HttpClient _httpClient;
    private readonly DTrustConfiguration _config;
    private readonly ILogger<DTrustSignaturService> _logger;

    public DTrustSignaturService(
        HttpClient httpClient,
        DTrustConfiguration config,
        ILogger<DTrustSignaturService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    public async Task<SignaturAnforderungResult> SignaturAnfordernAsync(SignaturAnforderungRequest request)
    {
        // D-Trust sign-me REST API Integration
        // Ablauf:
        // 1. POST /api/v1/signing-sessions → Session erstellen
        // 2. Dokument hochladen als PDF
        // 3. Redirect-URL erhalten → Kunde wird zur D-Trust Auth-Seite weitergeleitet
        // 4. Kunde authentifiziert sich (SMS-TAN, eID, Video)
        // 5. Callback an unsere App → Signatur abschließen
        // 6. Signiertes Dokument (PAdES PDF) herunterladen
        
        _logger.LogWarning("D-Trust Integration nicht implementiert. Benötigt API-Zugangsdaten.");
        throw new NotImplementedException("Implementierung benötigt D-Trust API-Zugangsdaten");
    }

    public Task<SignaturStatusResult> StatusPruefenAsync(Guid signaturId)
    {
        throw new NotImplementedException("D-Trust API-Integration ausstehend");
    }

    public Task<SignaturCallbackResult> CallbackVerarbeitenAsync(SignaturCallbackRequest request)
    {
        throw new NotImplementedException("D-Trust API-Integration ausstehend");
    }

    public Task<byte[]> SigniertesDokumentAbrufenAsync(Guid signaturId)
    {
        throw new NotImplementedException("D-Trust API-Integration ausstehend");
    }

    public Task<SignaturValidierungResult> SignaturValidierenAsync(Guid signaturId)
    {
        throw new NotImplementedException("D-Trust API-Integration ausstehend");
    }

    public Task<bool> StornierenAsync(Guid signaturId)
    {
        throw new NotImplementedException("D-Trust API-Integration ausstehend");
    }
}

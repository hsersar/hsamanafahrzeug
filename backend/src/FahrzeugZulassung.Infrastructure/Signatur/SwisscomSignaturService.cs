using FahrzeugZulassung.Application.DTOs.Signatur;
using FahrzeugZulassung.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace FahrzeugZulassung.Infrastructure.Signatur;

/// <summary>
/// Swisscom Trust Services All-in Signing Service Integration
/// REST API Integration mit Unterstützung für SMS-TAN, Mobile ID
/// PAdES (PDF) und CAdES Signatur, Batch-Signatur möglich
/// </summary>
public class SwisscomSignaturService : ISignaturService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SwisscomSignaturService> _logger;

    public SwisscomSignaturService(
        HttpClient httpClient,
        ILogger<SwisscomSignaturService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public Task<SignaturAnforderungResult> SignaturAnfordernAsync(SignaturAnforderungRequest request)
    {
        _logger.LogWarning("Swisscom Integration nicht implementiert. Benötigt API-Zugangsdaten.");
        throw new NotImplementedException("Swisscom API-Integration ausstehend");
    }

    public Task<SignaturStatusResult> StatusPruefenAsync(Guid signaturId)
    {
        throw new NotImplementedException("Swisscom API-Integration ausstehend");
    }

    public Task<SignaturCallbackResult> CallbackVerarbeitenAsync(SignaturCallbackRequest request)
    {
        throw new NotImplementedException("Swisscom API-Integration ausstehend");
    }

    public Task<byte[]> SigniertesDokumentAbrufenAsync(Guid signaturId)
    {
        throw new NotImplementedException("Swisscom API-Integration ausstehend");
    }

    public Task<SignaturValidierungResult> SignaturValidierenAsync(Guid signaturId)
    {
        throw new NotImplementedException("Swisscom API-Integration ausstehend");
    }

    public Task<bool> StornierenAsync(Guid signaturId)
    {
        throw new NotImplementedException("Swisscom API-Integration ausstehend");
    }
}

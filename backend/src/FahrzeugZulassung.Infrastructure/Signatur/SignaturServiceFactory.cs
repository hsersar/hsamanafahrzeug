using FahrzeugZulassung.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace FahrzeugZulassung.Infrastructure.Signatur;

public interface ISignaturServiceFactory
{
    ISignaturService Create(string? provider = null);
}

/// <summary>
/// Factory Pattern: Wählt den richtigen QES-Provider basierend auf Konfiguration
/// </summary>
public class SignaturServiceFactory : ISignaturServiceFactory
{
    private readonly ILogger<SignaturServiceFactory> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly DTrustConfiguration _dTrustConfig;
    private readonly string _defaultProvider;
    private readonly string _callbackBaseUrl;

    public SignaturServiceFactory(
        ILogger<SignaturServiceFactory> logger,
        IHttpClientFactory httpClientFactory,
        DTrustConfiguration dTrustConfig,
        string defaultProvider = "Mock",
        string callbackBaseUrl = "http://localhost:5000")
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _dTrustConfig = dTrustConfig;
        _defaultProvider = defaultProvider;
        _callbackBaseUrl = callbackBaseUrl;
    }

    public ISignaturService Create(string? provider = null)
    {
        var selectedProvider = provider ?? _defaultProvider;
        
        _logger.LogInformation("Erstelle Signatur-Service für Provider: {Provider}", selectedProvider);

        return selectedProvider.ToUpperInvariant() switch
        {
            "MOCK" => CreateMockService(),
            "DTRUST" => CreateDTrustService(),
            "SWISSCOM" => CreateSwisscomService(),
            _ => throw new ArgumentException($"Unbekannter Signatur-Provider: {selectedProvider}", nameof(provider))
        };
    }

    private ISignaturService CreateMockService()
    {
        var logger = _logger as ILogger<MockSignaturService> 
            ?? throw new InvalidOperationException("Logger factory required");
        return new MockSignaturService(logger, _callbackBaseUrl);
    }

    private ISignaturService CreateDTrustService()
    {
        var httpClient = _httpClientFactory.CreateClient("DTrust");
        var logger = _logger as ILogger<DTrustSignaturService> 
            ?? throw new InvalidOperationException("Logger factory required");
        return new DTrustSignaturService(httpClient, _dTrustConfig, logger);
    }

    private ISignaturService CreateSwisscomService()
    {
        var httpClient = _httpClientFactory.CreateClient("Swisscom");
        var logger = _logger as ILogger<SwisscomSignaturService> 
            ?? throw new InvalidOperationException("Logger factory required");
        return new SwisscomSignaturService(httpClient, logger);
    }
}

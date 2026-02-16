using FahrzeugZulassung.Application.Interfaces;

namespace FahrzeugZulassung.API.Services;

/// <summary>
/// Mock implementation of IIKfzService for development and testing.
/// In production, this should be replaced with actual iKFZ API integration.
/// </summary>
public class MockIKfzService : IIKfzService
{
    private readonly ILogger<MockIKfzService> _logger;

    public MockIKfzService(ILogger<MockIKfzService> logger)
    {
        _logger = logger;
    }

    public async Task<string> SubmitAnmeldungAsync(Guid auftragId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Submitting Anmeldung for Auftrag {AuftragId}", auftragId);
        
        // Simulate API call delay
        await Task.Delay(100, cancellationToken);
        
        // Return mock transaction ID
        var transactionId = $"IKFZ-ANM-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        
        _logger.LogInformation("Mock: Anmeldung submitted with Transaction ID {TransactionId}", transactionId);
        
        return transactionId;
    }

    public async Task<string> SubmitAbmeldungAsync(Guid auftragId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Submitting Abmeldung for Auftrag {AuftragId}", auftragId);
        
        // Simulate API call delay
        await Task.Delay(100, cancellationToken);
        
        // Return mock transaction ID
        var transactionId = $"IKFZ-ABM-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        
        _logger.LogInformation("Mock: Abmeldung submitted with Transaction ID {TransactionId}", transactionId);
        
        return transactionId;
    }

    public async Task<string> SubmitUmmeldungAsync(Guid auftragId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Submitting Ummeldung for Auftrag {AuftragId}", auftragId);
        
        // Simulate API call delay
        await Task.Delay(100, cancellationToken);
        
        // Return mock transaction ID
        var transactionId = $"IKFZ-UMM-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        
        _logger.LogInformation("Mock: Ummeldung submitted with Transaction ID {TransactionId}", transactionId);
        
        return transactionId;
    }

    public async Task<IKfzStatusDto> GetStatusAsync(string transactionId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Getting status for Transaction {TransactionId}", transactionId);
        
        // Simulate API call delay
        await Task.Delay(50, cancellationToken);
        
        // Return mock status
        return new IKfzStatusDto
        {
            TransactionId = transactionId,
            Status = "In Bearbeitung",
            Beschreibung = "Der Antrag wird derzeit bearbeitet (Mock-Status)",
            LastUpdated = DateTime.UtcNow
        };
    }
}

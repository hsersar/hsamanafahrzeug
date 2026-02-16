namespace FahrzeugZulassung.Application.Interfaces;

public interface IIKfzService
{
    Task<string> SubmitAnmeldungAsync(Guid auftragId, CancellationToken cancellationToken = default);
    Task<string> SubmitAbmeldungAsync(Guid auftragId, CancellationToken cancellationToken = default);
    Task<string> SubmitUmmeldungAsync(Guid auftragId, CancellationToken cancellationToken = default);
    Task<IKfzStatusDto> GetStatusAsync(string transactionId, CancellationToken cancellationToken = default);
}

public class IKfzStatusDto
{
    public string TransactionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Beschreibung { get; set; }
    public DateTime LastUpdated { get; set; }
}

namespace FahrzeugZulassung.Infrastructure.ExternalServices;

public interface IKfzClient
{
    Task<KfzZulassungResponse?> ZulassungBeantragen(KfzZulassungRequest request, CancellationToken cancellationToken = default);
    Task<KfzAbmeldungResponse?> AbmeldungBeantragen(KfzAbmeldungRequest request, CancellationToken cancellationToken = default);
    Task<KfzUmschreibungResponse?> UmschreibungBeantragen(KfzUmschreibungRequest request, CancellationToken cancellationToken = default);
    Task<KfzStatusResponse?> StatusAbfragen(string referenz, CancellationToken cancellationToken = default);
}

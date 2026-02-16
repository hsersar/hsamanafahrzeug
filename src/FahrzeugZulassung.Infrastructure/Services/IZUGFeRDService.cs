using FahrzeugZulassung.Domain.Entities;

namespace FahrzeugZulassung.Infrastructure.Services;

public interface IZUGFeRDService
{
    Task<ZUGFeRDResult> ErstelleERechnungAsync(Rechnung rechnung);
    Task<byte[]> ErstelleERechnungAlsPdfAsync(Rechnung rechnung);
    string GeneriereZUGFeRDXml(Rechnung rechnung);
    bool ValidiereZUGFeRDXml(string xml);
}

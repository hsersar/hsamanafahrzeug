namespace FahrzeugZulassung.Application.Interfaces;

public interface IEVBService
{
    Task<EVBValidierungResult> ValidiereEVBNummerAsync(string evbNummer);
    Task<EVBAbfrageResult> PruefeBeimGDVAsync(string evbNummer);  // Vorbereitet für API
    Task<EVBNummer> SpeichereEVBAsync(Guid auftragId, EVBEingabeDto dto);
    bool IstFormatGueltig(string evbNummer);
}

public class EVBValidierungResult
{
    public bool IstGueltig { get; set; }
    public string? Fehlermeldung { get; set; }
    public string? Versicherung { get; set; }
    public DateTime? GueltigBis { get; set; }
}

public class EVBAbfrageResult
{
    public bool Erfolgreich { get; set; }
    public string? Fehlermeldung { get; set; }
    public EVBValidierungResult? Validierung { get; set; }
}

public class EVBNummer
{
    public Guid Id { get; set; }
    public string Nummer { get; set; } = string.Empty;
    public string? Versicherungsgesellschaft { get; set; }
}

public class EVBEingabeDto
{
    public string Nummer { get; set; } = string.Empty;
    public string? Versicherungsgesellschaft { get; set; }
    public int Verwendungszweck { get; set; }
}

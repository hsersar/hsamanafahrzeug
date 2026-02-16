namespace FahrzeugZulassung.Application.DTOs.Provision;

public class ProvisionsModellDto
{
    public Guid Id { get; set; }
    public Guid? StandortId { get; set; }
    public string? StandortName { get; set; }
    public bool IstStandard { get; set; }
    public decimal MonatlicheGrundgebuehr { get; set; }
    public decimal ProvisionsProzentsatz { get; set; }
    public decimal? MinProvisionProRechnung { get; set; }
    public decimal? MaxProvisionProRechnung { get; set; }
    public DateTime GueltigAb { get; set; }
    public DateTime? GueltigBis { get; set; }
    public bool IstAktiv { get; set; }
}

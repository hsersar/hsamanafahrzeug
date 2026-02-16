namespace FahrzeugZulassung.Application.DTOs.Provision;

public class ProvisionsModellUpdateDto
{
    public decimal MonatlicheGrundgebuehr { get; set; }
    public decimal ProvisionsProzentsatz { get; set; }
    public decimal? MinProvisionProRechnung { get; set; }
    public decimal? MaxProvisionProRechnung { get; set; }
    public string? Aenderungsgrund { get; set; }
}

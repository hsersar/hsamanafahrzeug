namespace FahrzeugZulassung.Application.DTOs.Dashboard;

public class DashboardKpiDto
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Trend { get; set; }
}

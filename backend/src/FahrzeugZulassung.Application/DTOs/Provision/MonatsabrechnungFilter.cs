using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Application.DTOs.Provision;

public class MonatsabrechnungFilter
{
    public Guid? StandortId { get; set; }
    public int? Jahr { get; set; }
    public int? Monat { get; set; }
    public MonatsabrechnungStatus? Status { get; set; }
}

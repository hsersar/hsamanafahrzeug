namespace FahrzeugZulassung.Domain.Enums;

public enum AuftragStatus
{
    Eingereicht = 0,
    InBearbeitung = 1,
    WartetAufZahlung = 2,
    AnIKFZGesendet = 3,
    Genehmigt = 4,
    Abgeschlossen = 5,
    Abgelehnt = 6,
    Storniert = 7
}

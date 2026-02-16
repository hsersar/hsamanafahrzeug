namespace FahrzeugZulassung.Domain.Enums;

public enum AuftragStatus
{
    Entwurf = 1,
    Eingereicht = 2,
    InBearbeitung = 3,
    WartAufDokumente = 4,
    WartAufZahlung = 5,
    AnIKfzGesendet = 6,
    Genehmigt = 7,
    Abgelehnt = 8,
    Abgeschlossen = 9,
    Gesperrt = 10
}

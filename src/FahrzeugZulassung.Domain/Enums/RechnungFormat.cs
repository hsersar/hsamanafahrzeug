namespace FahrzeugZulassung.Domain.Enums;

public enum RechnungFormat
{
    Standard = 0,
    ZUGFeRD_Minimum = 1,
    ZUGFeRD_BasicWL = 2,
    ZUGFeRD_Basic = 3,
    ZUGFeRD_Comfort = 4,   // EN16931 konform - empfohlen
    ZUGFeRD_Extended = 5
}

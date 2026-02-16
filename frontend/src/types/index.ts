export enum RechnungFormat {
    Standard = 0,
    ZUGFeRD_Minimum = 1,
    ZUGFeRD_BasicWL = 2,
    ZUGFeRD_Basic = 3,
    ZUGFeRD_Comfort = 4,
    ZUGFeRD_Extended = 5
}

export interface RechnungsPositionDto {
    beschreibung: string;
    artikelnummer?: string;
    menge: number;
    einzelpreis: number;
    steuersatz: number;
}

export interface RechnungErstellenDto {
    kundeId: string;
    standortId: string;
    format: RechnungFormat;
    positionen: RechnungsPositionDto[];
}

export interface RechnungDto {
    id: string;
    rechnungsNummer: string;
    erstelltAm: string;
    faelligkeitsdatum: string;
    nettobetrag: number;
    steuerbetrag: number;
    bruttobetrag: number;
    format: RechnungFormat;
    hatZUGFeRDXml: boolean;
    hatPdf: boolean;
    kundeId: string;
    kundeName: string;
}

export interface KundeDto {
    id: string;
    vorname: string;
    nachname: string;
    email: string;
    strasse: string;
    plz: string;
    ort: string;
    firmenname?: string;
    ustID?: string;
}

export interface StandortDto {
    id: string;
    name: string;
    firmenname: string;
    strasse: string;
    plz: string;
    ort: string;
    ustID?: string;
    iban?: string;
    bic?: string;
}

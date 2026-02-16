export enum BenutzerRolle {
  Mitarbeiter = 1,
  StandortAdmin = 2,
  SuperAdmin = 3
}

export enum MonatsabrechnungStatus {
  Entwurf = 0,
  Erstellt = 1,
  Versandt = 2,
  Bezahlt = 3,
  Ueberfaellig = 4,
  Storniert = 5,
  Reklamiert = 6
}

export enum MonatsabrechnungPositionTyp {
  Grundgebuehr = 1,
  Provision = 2,
  Rabatt = 3,
  Sonstiges = 99
}

export interface ProvisionsModellDto {
  id: string;
  standortId?: string;
  standortName?: string;
  istStandard: boolean;
  monatlicheGrundgebuehr: number;
  provisionsProzentsatz: number;
  minProvisionProRechnung?: number;
  maxProvisionProRechnung?: number;
  gueltigAb: string;
  gueltigBis?: string;
  istAktiv: boolean;
}

export interface ProvisionsModellUpdateDto {
  monatlicheGrundgebuehr: number;
  provisionsProzentsatz: number;
  minProvisionProRechnung?: number;
  maxProvisionProRechnung?: number;
  aenderungsgrund: string;
}

export interface ProvisionsBerechnung {
  standortId: string;
  standortName: string;
  jahr: number;
  monat: number;
  anzahlRechnungen: number;
  anzahlAuftraege: number;
  gesamtUmsatz: number;
  grundgebuehr: number;
  provisionsProzentsatz: number;
  provisionsBetrag: number;
  nettobetrag: number;
  steuerbetrag: number;
  bruttobetrag: number;
  rechnungsdetails: ProvisionsRechnungDetail[];
}

export interface ProvisionsRechnungDetail {
  rechnungId: string;
  rechnungsNummer: string;
  kundeName: string;
  rechnungsBetrag: number;
  provisionsBetrag: number;
  rechnungsDatum: string;
}

export interface MonatsabrechnungDto {
  id: string;
  standortId: string;
  standortName: string;
  abrechnungsNummer: string;
  jahr: number;
  monat: number;
  zeitraum: string;
  anzahlRechnungen: number;
  anzahlAuftraege: number;
  gesamtUmsatz: number;
  grundgebuehr: number;
  provisionsProzentsatz: number;
  provisionsBetrag: number;
  nettobetrag: number;
  steuerbetrag: number;
  bruttobetrag: number;
  status: MonatsabrechnungStatus;
  faelligkeitsdatum?: string;
  bezahltAm?: string;
  positionen: MonatsabrechnungPositionDto[];
}

export interface MonatsabrechnungPositionDto {
  id: string;
  position: number;
  typ: MonatsabrechnungPositionTyp;
  beschreibung: string;
  menge: number;
  einheit: string;
  einzelpreis: number;
  nettobetrag: number;
  steuersatz: number;
  steuerbetrag: number;
  bruttobetrag: number;
}

export interface PlattformUmsatzDto {
  jahr: number;
  monat?: number;
  anzahlStandorte: number;
  anzahlAktiveStandorte: number;
  gesamtUmsatzAllerStandorte: number;
  gesamtGrundgebuehren: number;
  gesamtProvisionen: number;
  gesamtPlattformEinnahmen: number;
  offeneAbrechnungen: number;
  bezahlteAbrechnungen: number;
  ueberfaelligeAbrechnungen: number;
  standortDetails: StandortUmsatzDetail[];
}

export interface StandortUmsatzDetail {
  standortId: string;
  standortName: string;
  umsatz: number;
  provision: number;
  grundgebuehr: number;
  gesamtAbrechnung: number;
  abrechnungStatus: MonatsabrechnungStatus;
  anzahlAuftraege: number;
}

export interface MonatsabrechnungFilter {
  standortId?: string;
  jahr?: number;
  monat?: number;
  status?: MonatsabrechnungStatus;
}

export interface AbrechnungErstellenRequest {
  standortId: string;
  jahr: number;
  monat: number;
}

export interface ZahlungsBestaetigung {
  zahlungsReferenz: string;
}

export interface StornierungsRequest {
  grund: string;
}

export interface TrackingStatusResponse {
  trackingCode: string;
  auftragTyp: string;
  aktuellerStatus: string;
  statusBeschreibung: string;
  fortschrittProzent: number;
  erstelltAm: string;
  letzteAktualisierung?: string;
  kennzeichen?: string;
  zahlungErforderlich: boolean;
  zahlungErfolgt: boolean;
  historie: StatusHistorieEintrag[];
}

export interface StatusHistorieEintrag {
  status: string;
  beschreibung: string;
  zeitpunkt: string;
  istAktuell: boolean;
}

export interface PaymentInitRequest {
  rechnungId: string;
  methode: number;
}

export interface PaymentInitResult {
  zahlungId: string;
  methode: number;
  redirectUrl?: string;
  clientSecret?: string;
  ueberweisung?: UeberweisungsDetails;
  erfolg: boolean;
  fehlerNachricht?: string;
}

export interface UeberweisungsDetails {
  empfaenger: string;
  iban: string;
  bic: string;
  bank: string;
  verwendungszweck: string;
  betrag: number;
}

export interface ZahlungsMethode {
  id: number;
  name: string;
  icon: string;
}

// TypeScript Types für QES-Signatur

export enum SignaturTyp {
  Zulassungsantrag = 1,
  Abmeldungsantrag = 2,
  Ummeldungsantrag = 3,
  Vollmacht = 4,
  SEPA_Mandat = 5,
  Datenschutzerklaerung = 6,
  Sonstiges = 99
}

export enum SignaturStatus {
  Angefordert = 0,
  SessionErstellt = 1,
  WartAufAuth = 2,
  AuthErfolgreich = 3,
  InSignierung = 4,
  Signiert = 5,
  Fehlgeschlagen = 6,
  Abgelaufen = 7,
  Abgelehnt = 8,
  Storniert = 9
}

export enum SignaturLevel {
  SES = 1,
  AES = 2,
  QES = 3
}

export enum SignaturAuthMethode {
  SMS_TAN = 1,
  PushNotification = 2,
  App_TAN = 3,
  VideoIdent = 4,
  eID = 5
}

export interface SignaturAnforderungRequest {
  auftragId: string;
  typ: SignaturTyp;
  level?: SignaturLevel;
  signiererEmail: string;
  signiererName: string;
  signiererTelefon?: string;
  bevorzugteAuthMethode?: SignaturAuthMethode;
  redirectUrlNachSignatur?: string;
  dokumentBytes?: Uint8Array;
  dokumentName?: string;
}

export interface SignaturAnforderungResult {
  erfolg: boolean;
  signaturId?: string;
  redirectUrl?: string;
  sessionId?: string;
  fehlerNachricht?: string;
  sessionGueltigSekunden?: number;
}

export interface SignaturStatusResult {
  signaturId: string;
  status: SignaturStatus;
  statusBeschreibung: string;
  signiererName?: string;
  signiertAm?: string;
  zertifikatInfo?: string;
  dokumentVerfuegbar: boolean;
  fehlerNachricht?: string;
}

export interface SignaturValidierungResult {
  istGueltig: boolean;
  zertifikatGueltig: boolean;
  zeitstempelGueltig: boolean;
  dokumentUnveraendert: boolean;
  signiererName?: string;
  zertifikatAussteller?: string;
  signiertAm?: string;
  warnungen: string[];
  fehler: string[];
}

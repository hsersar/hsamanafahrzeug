export interface Vehicle {
  id: number;
  vin: string;
  licensePlate: string;
  brand: string;
  model: string;
  year: number;
  color: string;
  firstRegistrationDate: string;
  createdAt: string;
}

export interface RegistrationRequest {
  id: number;
  vin: string;
  requestedLicensePlate: string;
  brand: string;
  model: string;
  year: number;
  color: string;
  firstRegistrationDate: string;
  status: string;
  createdAt: string;
  updatedAt: string;
  rejectionReason?: string;
}

export interface CreateRegistrationRequestDto {
  vin: string;
  requestedLicensePlate: string;
  brand: string;
  model: string;
  year: number;
  color: string;
  firstRegistrationDate: string;
}

export interface VehicleCatalogItem {
  brand: string;
  models: string[];
}

export interface UserProfile {
  userId: string;
  displayName: string;
  email: string;
  role: string;
  createdAtUtc: string;
  lastLoginUtc: string;
}

export interface PersonalProfile {
  id: number;
  userId: string;
  anrede: string;
  titel?: string;
  vorname: string;
  nachname: string;
  geburtsdatum?: string;
  strasse: string;
  hausnummer: string;
  plz: string;
  ort: string;
  telefon: string;
  email: string;
  profilbildUrl?: string;
  createdAt: string;
  updatedAt: string;
}

export interface SavePersonalProfile {
  anrede: string;
  titel?: string;
  vorname: string;
  nachname: string;
  geburtsdatum?: string;
  strasse: string;
  hausnummer: string;
  plz: string;
  ort: string;
  telefon: string;
  email: string;
}

export interface CompanyProfile {
  id: number;
  userId: string;
  firmenname: string;
  rechtsform: string;
  handelsregisternummer: string;
  ustIdNr: string;
  strasse: string;
  hausnummer: string;
  plz: string;
  ort: string;
  telefon: string;
  email: string;
  website: string;
  ansprechpartnerAnrede: string;
  ansprechpartnerVorname: string;
  ansprechpartnerNachname: string;
  ansprechpartnerTelefon: string;
  ansprechpartnerEmail: string;
  ansprechpartnerPosition?: string;
  logoUrl?: string;
  createdAt: string;
  updatedAt: string;
}

export interface SaveCompanyProfile {
  firmenname: string;
  rechtsform: string;
  handelsregisternummer: string;
  ustIdNr: string;
  strasse: string;
  hausnummer: string;
  plz: string;
  ort: string;
  telefon: string;
  email: string;
  website: string;
  ansprechpartnerAnrede: string;
  ansprechpartnerVorname: string;
  ansprechpartnerNachname: string;
  ansprechpartnerTelefon: string;
  ansprechpartnerEmail: string;
  ansprechpartnerPosition?: string;
}

export interface DashboardOverview {
  openRequests: number;
  totalRequests: number;
  registeredVehicles: number;
  pendingReviews: number;
  latestRequestStatus?: string | null;
  latestRequestCreatedAt?: string | null;
}

export interface HelpArticle {
  id: number;
  category: string;
  title: string;
  summary: string;
  content: string;
  updatedAt: string;
}

export interface SearchRequestsFilter {
  query?: string;
  status?: string;
  fromDate?: string;
  toDate?: string;
}

export interface PaymentMethod {
  id: number;
  typ: string;
  bezeichnung: string;
  kontoinhaber?: string;
  iban?: string;
  bic?: string;
  bankname?: string;
  kartenNummer?: string;
  kartenInhaber?: string;
  gueltigBis?: string;
  paypalEmail?: string;
  istStandard: boolean;
  sepaMandatErteilt: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface SavePaymentMethod {
  typ: string;
  bezeichnung: string;
  kontoinhaber?: string;
  iban?: string;
  bic?: string;
  bankname?: string;
  kartenNummer?: string;
  kartenInhaber?: string;
  gueltigBis?: string;
  paypalEmail?: string;
  istStandard: boolean;
  sepaMandatErteilt: boolean;
}

export interface Invoice {
  id: number;
  rechnungsnummer: string;
  registrationRequestId?: number;
  beschreibung: string;
  nettobetrag: number;
  mwstSatz: number;
  mwstBetrag: number;
  bruttobetrag: number;
  status: string;
  rechnungsdatum: string;
  faelligkeitsdatum: string;
  bezahltAm?: string;
  zahlungsmethode?: string;
  createdAt: string;
}

export interface Announcement {
  id: number;
  titel: string;
  nachricht: string;
  typ: string;
}

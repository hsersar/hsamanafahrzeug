import React, { useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import ApiService from '../services/api';
import { CreateRegistrationRequestDto, VehicleCatalogItem } from '../types';

/* ------------------------------------------------------------------ */
/*  Constants                                                         */
/* ------------------------------------------------------------------ */
const STEPS = [
  { key: 'auftragsart', label: 'Auftragsart', icon: '📋' },
  { key: 'fahrzeug', label: 'Fahrzeugdaten', icon: '🚗' },
  { key: 'halter', label: 'Halterdaten', icon: '👤' },
  { key: 'dokumente', label: 'Dokumente', icon: '📁' },
  { key: 'zusammenfassung', label: 'Zusammenfassung', icon: '✅' },
];

const ORDER_TYPES = [
  {
    value: 'Neuzulassung',
    label: 'Neuzulassung',
    desc: 'Erstmalige Zulassung eines Fahrzeugs',
    icon: '🆕',
  },
  {
    value: 'Umschreibung',
    label: 'Umschreibung',
    desc: 'Halterwechsel / Kennzeichenänderung',
    icon: '🔄',
  },
  {
    value: 'Wiederzulassung',
    label: 'Wiederzulassung',
    desc: 'Erneute Zulassung eines abgemeldeten Fahrzeugs',
    icon: '♻️',
  },
  {
    value: 'Abmeldung',
    label: 'Abmeldung',
    desc: 'Außerbetriebsetzung eines Fahrzeugs',
    icon: '🚫',
  },
];

const COLOR_MAP: Record<string, string> = {
  Schwarz: '#1a1a1a',
  Weiß: '#ffffff',
  Grau: '#808080',
  Silber: '#c0c0c0',
  Blau: '#0047ab',
  Rot: '#d00000',
  Grün: '#2e7d32',
  Gelb: '#fdd835',
  Orange: '#fb8c00',
  Braun: '#5d4037',
  Beige: '#f5f5dc',
};

const REQUIRED_DOCS: Record<string, { key: string; label: string; hint: string }[]> = {
  Neuzulassung: [
    { key: 'zb2', label: 'Zulassungsbescheinigung Teil II (Fahrzeugbrief)', hint: 'Original oder beglaubigte Kopie' },
    { key: 'coc', label: 'COC-Bescheinigung (EG-Übereinstimmungsbescheinigung)', hint: 'Vom Hersteller ausgestelltes Dokument' },
    { key: 'evb', label: 'eVB-Nummer (Versicherungsbestätigung)', hint: '7-stellige alphanumerische Nummer' },
    { key: 'sepa', label: 'SEPA-Lastschriftmandat (Kfz-Steuer)', hint: 'Für den automatischen Einzug' },
    { key: 'ausweis', label: 'Personalausweis / Reisepass', hint: 'Gültig und aktuell' },
  ],
  Umschreibung: [
    { key: 'zb1', label: 'Zulassungsbescheinigung Teil I (Fahrzeugschein)', hint: 'Des bisherigen Halters' },
    { key: 'zb2', label: 'Zulassungsbescheinigung Teil II (Fahrzeugbrief)', hint: 'Original erforderlich' },
    { key: 'evb', label: 'eVB-Nummer (Versicherungsbestätigung)', hint: '7-stellige alphanumerische Nummer' },
    { key: 'sepa', label: 'SEPA-Lastschriftmandat (Kfz-Steuer)', hint: 'Für den automatischen Einzug' },
    { key: 'hu', label: 'Nachweis gültige HU (TÜV)', hint: 'Bei Umschreibung ohne Hauptuntersuchung' },
    { key: 'ausweis', label: 'Personalausweis / Reisepass', hint: 'Gültig und aktuell' },
  ],
  Wiederzulassung: [
    { key: 'zb1', label: 'Zulassungsbescheinigung Teil I (Fahrzeugschein)', hint: 'Mit Abmeldungsvermerk' },
    { key: 'zb2', label: 'Zulassungsbescheinigung Teil II (Fahrzeugbrief)', hint: 'Original erforderlich' },
    { key: 'evb', label: 'eVB-Nummer (Versicherungsbestätigung)', hint: '7-stellige alphanumerische Nummer' },
    { key: 'sepa', label: 'SEPA-Lastschriftmandat (Kfz-Steuer)', hint: 'Für den automatischen Einzug' },
    { key: 'hu', label: 'Nachweis gültige HU (TÜV)', hint: 'Pflicht bei Wiederzulassung' },
    { key: 'ausweis', label: 'Personalausweis / Reisepass', hint: 'Gültig und aktuell' },
  ],
  Abmeldung: [
    { key: 'zb1', label: 'Zulassungsbescheinigung Teil I (Fahrzeugschein)', hint: 'Original erforderlich' },
    { key: 'kennzeichen', label: 'Kennzeichenschilder', hint: 'Beide Kennzeichen mitbringen / Sicherheitscodes' },
    { key: 'ausweis', label: 'Personalausweis / Reisepass', hint: 'Gültig und aktuell' },
  ],
};

interface HalterData {
  halterTyp: 'privat' | 'firma';
  anrede: string;
  vorname: string;
  nachname: string;
  strasse: string;
  hausnummer: string;
  plz: string;
  ort: string;
  telefon: string;
  email: string;
  firmenname: string;
  handelsregisternummer: string;
}

const createInitialFormData = (): CreateRegistrationRequestDto => ({
  vin: '',
  requestedLicensePlate: '',
  brand: '',
  model: '',
  year: new Date().getFullYear(),
  color: '',
  firstRegistrationDate: new Date().toISOString().split('T')[0],
});

const createInitialHalterData = (): HalterData => ({
  halterTyp: 'privat',
  anrede: '',
  vorname: '',
  nachname: '',
  strasse: '',
  hausnummer: '',
  plz: '',
  ort: '',
  telefon: '',
  email: '',
  firmenname: '',
  handelsregisternummer: '',
});

/* ------------------------------------------------------------------ */
/*  Component                                                         */
/* ------------------------------------------------------------------ */
const NewRegistrationPage: React.FC = () => {
  const navigate = useNavigate();

  /* ----- wizard state ----- */
  const [currentStep, setCurrentStep] = useState(0);
  const [auftragsart, setAuftragsart] = useState('');

  /* ----- vehicle data ----- */
  const [formData, setFormData] = useState<CreateRegistrationRequestDto>(createInitialFormData());
  const [catalog, setCatalog] = useState<VehicleCatalogItem[]>([]);
  const [catalogLoading, setCatalogLoading] = useState(true);
  const [catalogError, setCatalogError] = useState(false);

  /* ----- halter data ----- */
  const [halterData, setHalterData] = useState<HalterData>(createInitialHalterData());

  /* ----- documents ----- */
  const [uploadedDocs, setUploadedDocs] = useState<Record<string, File | null>>({});

  /* ----- submit state ----- */
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  /* ----- step validation cache ----- */
  const [visitedSteps, setVisitedSteps] = useState<Set<number>>(new Set([0]));

  /* ---- catalog load ---- */
  useEffect(() => {
    const loadCatalog = async () => {
      try {
        setCatalogLoading(true);
        const data = await ApiService.getVehicleCatalog();
        setCatalog(data);
        setCatalogError(false);
      } catch {
        setCatalog([]);
        setCatalogError(true);
      } finally {
        setCatalogLoading(false);
      }
    };
    loadCatalog();
  }, []);

  /* ---- derived ---- */
  const selectedBrand = useMemo(
    () => catalog.find((i) => i.brand.toLowerCase() === formData.brand.toLowerCase()),
    [catalog, formData.brand],
  );

  const brandSuggestions = useMemo(() => catalog.map((i) => i.brand), [catalog]);

  const modelSuggestions = useMemo(() => {
    if (selectedBrand) return selectedBrand.models;
    return Array.from(new Set(catalog.flatMap((i) => i.models)));
  }, [selectedBrand, catalog]);

  const requiredDocs = useMemo(
    () => REQUIRED_DOCS[auftragsart] ?? [],
    [auftragsart],
  );

  /* ---- step validation ---- */
  const isStepValid = (step: number): boolean => {
    switch (step) {
      case 0:
        return auftragsart !== '';
      case 1:
        return (
          formData.vin.length === 17 &&
          formData.requestedLicensePlate.trim() !== '' &&
          formData.brand.trim() !== '' &&
          formData.model.trim() !== '' &&
          formData.year >= 1900 &&
          formData.year <= 2100 &&
          formData.color.trim() !== '' &&
          formData.firstRegistrationDate !== ''
        );
      case 2: {
        const h = halterData;
        const baseValid =
          h.anrede !== '' &&
          h.vorname.trim() !== '' &&
          h.nachname.trim() !== '' &&
          h.strasse.trim() !== '' &&
          h.hausnummer.trim() !== '' &&
          h.plz.trim() !== '' &&
          h.ort.trim() !== '' &&
          h.email.trim() !== '';
        if (h.halterTyp === 'firma') {
          return baseValid && h.firmenname.trim() !== '';
        }
        return baseValid;
      }
      case 3:
        return requiredDocs.every((doc) => uploadedDocs[doc.key]);
      case 4:
        return true;
      default:
        return false;
    }
  };

  const canProceed = isStepValid(currentStep);

  /* ---- handlers ---- */
  const handleVehicleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name } = e.target;
    let value = e.target.value;

    if (name === 'vin') value = value.toUpperCase().replace(/[^A-Z0-9]/g, '').slice(0, 17);
    if (name === 'requestedLicensePlate') value = value.toUpperCase().replace(/[^A-Z0-9\- ]/g, '').slice(0, 15);

    setFormData((prev) => {
      const next = { ...prev, [name]: name === 'year' ? parseInt(value) || 0 : value };
      if (name === 'brand') {
        const match = catalog.find((i) => i.brand.toLowerCase() === value.toLowerCase());
        if (match && !match.models.includes(next.model)) next.model = '';
      }
      return next;
    });
  };

  const handleHalterChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setHalterData((prev) => ({ ...prev, [name]: value }));
  };

  const handleFileChange = (docKey: string, file: File | null) => {
    setUploadedDocs((prev) => ({ ...prev, [docKey]: file }));
  };

  const goToStep = (step: number) => {
    if (visitedSteps.has(step) || (step === currentStep + 1 && canProceed)) {
      setCurrentStep(step);
      setVisitedSteps((prev) => new Set(prev).add(step));
    }
  };

  const handleNext = () => {
    if (currentStep < STEPS.length - 1 && canProceed) {
      const next = currentStep + 1;
      setCurrentStep(next);
      setVisitedSteps((prev) => new Set(prev).add(next));
    }
  };

  const handleBack = () => {
    if (currentStep > 0) setCurrentStep(currentStep - 1);
  };

  const handleSubmit = async () => {
    setLoading(true);
    setError(null);
    try {
      await ApiService.createRegistrationRequest(formData);
      navigate('/my-requests');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Antrag konnte nicht gesendet werden');
    } finally {
      setLoading(false);
    }
  };

  /* ---- license plate preview ---- */
  const plateText = formData.requestedLicensePlate || '– – – –';

  /* ================================================================ */
  /*  RENDER                                                          */
  /* ================================================================ */
  return (
    <div className="page new-registration-page wizard-page">
      {/* ---- Wizard Header ---- */}
      <div className="wizard-header">
        <h1>Neuer Zulassungsauftrag</h1>
        <p>Füllen Sie alle Schritte aus, um Ihren Antrag einzureichen.</p>
      </div>

      {/* ---- Step Indicator ---- */}
      <div className="wizard-steps">
        {STEPS.map((step, idx) => {
          const visited = visitedSteps.has(idx);
          const completed = idx < currentStep && isStepValid(idx);
          const active = idx === currentStep;
          return (
            <button
              key={step.key}
              type="button"
              className={`wizard-step-btn ${active ? 'active' : ''} ${completed ? 'completed' : ''} ${visited && !active ? 'visited' : ''}`}
              onClick={() => goToStep(idx)}
              disabled={!visited && !(idx === currentStep + 1 && canProceed)}
            >
              <span className="wizard-step-number">
                {completed ? '✓' : idx + 1}
              </span>
              <span className="wizard-step-label">{step.label}</span>
            </button>
          );
        })}
        <div
          className="wizard-progress-bar"
          style={{ width: `${(currentStep / (STEPS.length - 1)) * 100}%` }}
        />
      </div>

      {error && <div className="wizard-error">{error}</div>}

      {/* ---- Step Content ---- */}
      <div className="wizard-content card">
        {/* ========== STEP 0: Auftragsart ========== */}
        {currentStep === 0 && (
          <div className="wizard-step-content">
            <div className="wizard-step-title">
              <span className="wizard-step-title-icon">📋</span>
              <div>
                <h2>Auftragsart wählen</h2>
                <p>Welchen Zulassungsvorgang möchten Sie beauftragen?</p>
              </div>
            </div>
            <div className="order-type-grid">
              {ORDER_TYPES.map((type) => (
                <button
                  key={type.value}
                  type="button"
                  className={`order-type-card ${auftragsart === type.value ? 'selected' : ''}`}
                  onClick={() => setAuftragsart(type.value)}
                >
                  <span className="order-type-icon">{type.icon}</span>
                  <span className="order-type-label">{type.label}</span>
                  <span className="order-type-desc">{type.desc}</span>
                  {auftragsart === type.value && <span className="order-type-check">✓</span>}
                </button>
              ))}
            </div>
          </div>
        )}

        {/* ========== STEP 1: Fahrzeugdaten ========== */}
        {currentStep === 1 && (
          <div className="wizard-step-content">
            <div className="wizard-step-title">
              <span className="wizard-step-title-icon">🚗</span>
              <div>
                <h2>Fahrzeugdaten</h2>
                <p>Geben Sie die Fahrzeuginformationen ein.</p>
              </div>
            </div>

            {/* License plate preview */}
            <div className="wizard-plate-preview">
              <div className="license-plate-visual">
                <div className="plate-country">
                  <span className="plate-stars">★★★</span>
                  D
                </div>
                <div className="plate-text">{plateText}</div>
              </div>
            </div>

            <div className="wizard-form-grid">
              <div className="form-group">
                <label htmlFor="vin">FIN (Fahrzeug-Identifizierungsnummer) *</label>
                <input
                  type="text"
                  id="vin"
                  name="vin"
                  value={formData.vin}
                  onChange={handleVehicleChange}
                  maxLength={17}
                  placeholder="z. B. WBA3A5C50CF000000"
                  required
                />
                <span className="form-hint">
                  17 Zeichen — {formData.vin.length}/17{' '}
                  {formData.vin.length === 17 && <span className="hint-ok">✓ vollständig</span>}
                </span>
              </div>

              <div className="form-group">
                <label htmlFor="requestedLicensePlate">Wunschkennzeichen *</label>
                <input
                  type="text"
                  id="requestedLicensePlate"
                  name="requestedLicensePlate"
                  value={formData.requestedLicensePlate}
                  onChange={handleVehicleChange}
                  maxLength={15}
                  placeholder="z. B. W-AB 1234"
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="brand">Marke *</label>
                <input
                  type="text"
                  id="brand"
                  name="brand"
                  value={formData.brand}
                  onChange={handleVehicleChange}
                  list="brand-suggestions"
                  placeholder="Marke eingeben oder auswählen"
                  required
                />
                <datalist id="brand-suggestions">
                  {brandSuggestions.map((b) => (
                    <option key={b} value={b} />
                  ))}
                </datalist>
                <span className="form-hint">
                  {catalogLoading
                    ? 'Vorschläge werden geladen…'
                    : catalogError
                    ? 'Vorschläge derzeit nicht verfügbar.'
                    : 'z. B. Volkswagen, BMW, Audi'}
                </span>
              </div>

              <div className="form-group">
                <label htmlFor="model">Modell *</label>
                <input
                  type="text"
                  id="model"
                  name="model"
                  value={formData.model}
                  onChange={handleVehicleChange}
                  list="model-suggestions"
                  placeholder="Modell eingeben oder auswählen"
                  required
                />
                <datalist id="model-suggestions">
                  {modelSuggestions.map((m) => (
                    <option key={m} value={m} />
                  ))}
                </datalist>
                <span className="form-hint">
                  {selectedBrand
                    ? `Vorschläge für ${selectedBrand.brand}`
                    : 'Nach Markenwahl gefiltert'}
                </span>
              </div>

              <div className="form-group">
                <label htmlFor="year">Baujahr *</label>
                <input
                  type="number"
                  id="year"
                  name="year"
                  value={formData.year}
                  onChange={handleVehicleChange}
                  min={1900}
                  max={2100}
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="firstRegistrationDate">Erstzulassung *</label>
                <input
                  type="date"
                  id="firstRegistrationDate"
                  name="firstRegistrationDate"
                  value={formData.firstRegistrationDate}
                  onChange={handleVehicleChange}
                  required
                />
              </div>
            </div>

            <div className="form-group wizard-color-section">
              <label>Farbe *</label>
              <div className="color-grid">
                {Object.keys(COLOR_MAP)
                  .sort()
                  .map((c) => (
                    <button
                      key={c}
                      type="button"
                      className={`color-chip ${formData.color === c ? 'active' : ''}`}
                      onClick={() => setFormData({ ...formData, color: c })}
                    >
                      <div className="color-chip-swatch" style={{ backgroundColor: COLOR_MAP[c] }} />
                      <span className="color-chip-label">{c}</span>
                    </button>
                  ))}
              </div>
            </div>
          </div>
        )}

        {/* ========== STEP 2: Halterdaten ========== */}
        {currentStep === 2 && (
          <div className="wizard-step-content">
            <div className="wizard-step-title">
              <span className="wizard-step-title-icon">👤</span>
              <div>
                <h2>Halterdaten</h2>
                <p>Angaben zum Fahrzeughalter / Auftraggeber.</p>
              </div>
            </div>

            <div className="halter-type-toggle">
              <button
                type="button"
                className={`halter-type-btn ${halterData.halterTyp === 'privat' ? 'active' : ''}`}
                onClick={() => setHalterData((p) => ({ ...p, halterTyp: 'privat' }))}
              >
                👤 Privatperson
              </button>
              <button
                type="button"
                className={`halter-type-btn ${halterData.halterTyp === 'firma' ? 'active' : ''}`}
                onClick={() => setHalterData((p) => ({ ...p, halterTyp: 'firma' }))}
              >
                🏢 Unternehmen
              </button>
            </div>

            {halterData.halterTyp === 'firma' && (
              <div className="wizard-form-grid" style={{ marginBottom: 16 }}>
                <div className="form-group">
                  <label htmlFor="firmenname">Firmenname *</label>
                  <input
                    type="text"
                    id="firmenname"
                    name="firmenname"
                    value={halterData.firmenname}
                    onChange={handleHalterChange}
                    placeholder="z. B. Muster GmbH"
                    required
                  />
                </div>
                <div className="form-group">
                  <label htmlFor="handelsregisternummer">Handelsregisternummer</label>
                  <input
                    type="text"
                    id="handelsregisternummer"
                    name="handelsregisternummer"
                    value={halterData.handelsregisternummer}
                    onChange={handleHalterChange}
                    placeholder="z. B. HRB 12345"
                  />
                </div>
              </div>
            )}

            <div className="wizard-form-grid">
              <div className="form-group">
                <label htmlFor="anrede">Anrede *</label>
                <select
                  id="anrede"
                  name="anrede"
                  value={halterData.anrede}
                  onChange={handleHalterChange}
                  required
                >
                  <option value="">— Bitte wählen —</option>
                  <option value="Herr">Herr</option>
                  <option value="Frau">Frau</option>
                  <option value="Divers">Divers</option>
                </select>
              </div>

              <div className="form-group">
                <label htmlFor="vorname">Vorname *</label>
                <input
                  type="text"
                  id="vorname"
                  name="vorname"
                  value={halterData.vorname}
                  onChange={handleHalterChange}
                  placeholder="Vorname"
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="nachname">Nachname *</label>
                <input
                  type="text"
                  id="nachname"
                  name="nachname"
                  value={halterData.nachname}
                  onChange={handleHalterChange}
                  placeholder="Nachname"
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="email">E-Mail *</label>
                <input
                  type="email"
                  id="email"
                  name="email"
                  value={halterData.email}
                  onChange={handleHalterChange}
                  placeholder="E-Mail-Adresse"
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="telefon">Telefon</label>
                <input
                  type="tel"
                  id="telefon"
                  name="telefon"
                  value={halterData.telefon}
                  onChange={handleHalterChange}
                  placeholder="z. B. 0202 12345678"
                />
              </div>

              <div className="form-group" />

              <div className="form-group">
                <label htmlFor="strasse">Straße *</label>
                <input
                  type="text"
                  id="strasse"
                  name="strasse"
                  value={halterData.strasse}
                  onChange={handleHalterChange}
                  placeholder="Straße"
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="hausnummer">Hausnummer *</label>
                <input
                  type="text"
                  id="hausnummer"
                  name="hausnummer"
                  value={halterData.hausnummer}
                  onChange={handleHalterChange}
                  placeholder="Nr."
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="plz">PLZ *</label>
                <input
                  type="text"
                  id="plz"
                  name="plz"
                  value={halterData.plz}
                  onChange={handleHalterChange}
                  maxLength={5}
                  placeholder="z. B. 42103"
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="ort">Ort *</label>
                <input
                  type="text"
                  id="ort"
                  name="ort"
                  value={halterData.ort}
                  onChange={handleHalterChange}
                  placeholder="z. B. Wuppertal"
                  required
                />
              </div>
            </div>
          </div>
        )}

        {/* ========== STEP 3: Dokumente ========== */}
        {currentStep === 3 && (
          <div className="wizard-step-content">
            <div className="wizard-step-title">
              <span className="wizard-step-title-icon">📁</span>
              <div>
                <h2>Dokumente hochladen</h2>
                <p>
                  Laden Sie die benötigten Unterlagen für Ihre <strong>{auftragsart}</strong> hoch.
                </p>
              </div>
            </div>

            {requiredDocs.length === 0 ? (
              <div className="wizard-empty">Bitte wählen Sie zuerst eine Auftragsart.</div>
            ) : (
              <div className="doc-upload-list">
                {requiredDocs.map((doc) => {
                  const file = uploadedDocs[doc.key] ?? null;
                  return (
                    <div key={doc.key} className={`doc-upload-item ${file ? 'has-file' : ''}`}>
                      <div className="doc-upload-info">
                        <span className="doc-upload-icon">{file ? '✅' : '📄'}</span>
                        <div>
                          <strong>{doc.label}</strong>
                          <span className="doc-upload-hint">{doc.hint}</span>
                        </div>
                      </div>
                      <div className="doc-upload-actions">
                        {file ? (
                          <>
                            <span className="doc-file-name">{file.name}</span>
                            <button
                              type="button"
                              className="doc-remove-btn"
                              onClick={() => handleFileChange(doc.key, null)}
                            >
                              ✕
                            </button>
                          </>
                        ) : (
                          <label className="doc-upload-btn">
                            Datei auswählen
                            <input
                              type="file"
                              accept=".pdf,.jpg,.jpeg,.png"
                              style={{ display: 'none' }}
                              onChange={(e) =>
                                handleFileChange(doc.key, e.target.files?.[0] ?? null)
                              }
                            />
                          </label>
                        )}
                      </div>
                    </div>
                  );
                })}
              </div>
            )}

            <div className="doc-hint-box">
              <strong>📌 Hinweis:</strong> Erlaubte Formate: PDF, JPG, PNG. Maximale Dateigröße: 10 MB pro Datei.
            </div>
          </div>
        )}

        {/* ========== STEP 4: Zusammenfassung ========== */}
        {currentStep === 4 && (
          <div className="wizard-step-content">
            <div className="wizard-step-title">
              <span className="wizard-step-title-icon">✅</span>
              <div>
                <h2>Zusammenfassung</h2>
                <p>Prüfen Sie Ihre Angaben vor dem Absenden.</p>
              </div>
            </div>

            <div className="summary-sections">
              {/* Auftragsart */}
              <div className="summary-section">
                <div className="summary-section-header">
                  <h3>📋 Auftragsart</h3>
                  <button type="button" className="summary-edit-btn" onClick={() => goToStep(0)}>
                    Bearbeiten
                  </button>
                </div>
                <div className="summary-value-highlight">{auftragsart}</div>
              </div>

              {/* Fahrzeugdaten */}
              <div className="summary-section">
                <div className="summary-section-header">
                  <h3>🚗 Fahrzeugdaten</h3>
                  <button type="button" className="summary-edit-btn" onClick={() => goToStep(1)}>
                    Bearbeiten
                  </button>
                </div>
                <div className="wizard-plate-preview" style={{ margin: '12px 0' }}>
                  <div className="license-plate-visual">
                    <div className="plate-country">
                      <span className="plate-stars">★★★</span>D
                    </div>
                    <div className="plate-text">{plateText}</div>
                  </div>
                </div>
                <div className="summary-grid">
                  <div className="summary-row">
                    <span>FIN</span>
                    <strong>{formData.vin}</strong>
                  </div>
                  <div className="summary-row">
                    <span>Marke / Modell</span>
                    <strong>
                      {formData.brand} {formData.model}
                    </strong>
                  </div>
                  <div className="summary-row">
                    <span>Baujahr</span>
                    <strong>{formData.year}</strong>
                  </div>
                  <div className="summary-row">
                    <span>Farbe</span>
                    <strong>
                      {formData.color && (
                        <span
                          className="summary-color-dot"
                          style={{ backgroundColor: COLOR_MAP[formData.color] }}
                        />
                      )}
                      {formData.color}
                    </strong>
                  </div>
                  <div className="summary-row">
                    <span>Erstzulassung</span>
                    <strong>
                      {formData.firstRegistrationDate
                        ? new Date(formData.firstRegistrationDate).toLocaleDateString('de-DE')
                        : '—'}
                    </strong>
                  </div>
                </div>
              </div>

              {/* Halterdaten */}
              <div className="summary-section">
                <div className="summary-section-header">
                  <h3>👤 Halterdaten</h3>
                  <button type="button" className="summary-edit-btn" onClick={() => goToStep(2)}>
                    Bearbeiten
                  </button>
                </div>
                <div className="summary-grid">
                  {halterData.halterTyp === 'firma' && (
                    <div className="summary-row">
                      <span>Firma</span>
                      <strong>{halterData.firmenname}</strong>
                    </div>
                  )}
                  <div className="summary-row">
                    <span>Name</span>
                    <strong>
                      {halterData.anrede} {halterData.vorname} {halterData.nachname}
                    </strong>
                  </div>
                  <div className="summary-row">
                    <span>Adresse</span>
                    <strong>
                      {halterData.strasse} {halterData.hausnummer}, {halterData.plz} {halterData.ort}
                    </strong>
                  </div>
                  <div className="summary-row">
                    <span>E-Mail</span>
                    <strong>{halterData.email}</strong>
                  </div>
                  {halterData.telefon && (
                    <div className="summary-row">
                      <span>Telefon</span>
                      <strong>{halterData.telefon}</strong>
                    </div>
                  )}
                </div>
              </div>

              {/* Dokumente */}
              <div className="summary-section">
                <div className="summary-section-header">
                  <h3>📁 Dokumente</h3>
                  <button type="button" className="summary-edit-btn" onClick={() => goToStep(3)}>
                    Bearbeiten
                  </button>
                </div>
                <div className="summary-doc-list">
                  {requiredDocs.map((doc) => {
                    const file = uploadedDocs[doc.key];
                    return (
                      <div key={doc.key} className="summary-doc-row">
                        <span>{file ? '✅' : '❌'}</span>
                        <span>{doc.label}</span>
                        <span className="summary-doc-file">{file ? file.name : 'Fehlt'}</span>
                      </div>
                    );
                  })}
                </div>
              </div>
            </div>

            <div className="wizard-terms">
              <label className="wizard-checkbox-label">
                <input type="checkbox" id="agb-accept" />
                <span>
                  Ich bestätige die Richtigkeit meiner Angaben und akzeptiere die{' '}
                  <a href="/agb" target="_blank" rel="noopener noreferrer">
                    AGB
                  </a>{' '}
                  sowie die{' '}
                  <a href="/datenschutz" target="_blank" rel="noopener noreferrer">
                    Datenschutzerklärung
                  </a>
                  .
                </span>
              </label>
            </div>
          </div>
        )}
      </div>

      {/* ---- Footer / Navigation ---- */}
      <div className="wizard-footer">
        <button
          type="button"
          className="secondary-button"
          onClick={currentStep === 0 ? () => navigate('/') : handleBack}
        >
          {currentStep === 0 ? 'Abbrechen' : '← Zurück'}
        </button>

        <div className="wizard-footer-info">
          Schritt {currentStep + 1} von {STEPS.length}
        </div>

        {currentStep < STEPS.length - 1 ? (
          <button
            type="button"
            className="primary-button"
            disabled={!canProceed}
            onClick={handleNext}
          >
            Weiter →
          </button>
        ) : (
          <button
            type="button"
            className="primary-button"
            disabled={loading}
            onClick={handleSubmit}
          >
            {loading ? 'Wird gesendet…' : 'Antrag absenden'}
          </button>
        )}
      </div>
    </div>
  );
};

export default NewRegistrationPage;

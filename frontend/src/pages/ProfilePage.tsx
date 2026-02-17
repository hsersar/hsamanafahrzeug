import React, { useCallback, useEffect, useRef, useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import ApiService from '../services/api';
import {
  PersonalProfile,
  SavePersonalProfile,
  PaymentMethod,
  SavePaymentMethod,
} from '../types';

/* ─── Tab keys ──────────────────────────────────────────────────────── */
type TabKey = 'stammdaten' | 'zahlungsarten';

const TABS: { key: TabKey; label: string; icon: string }[] = [
  { key: 'stammdaten', label: 'Stammdaten', icon: '👤' },
  { key: 'zahlungsarten', label: 'Zahlungsarten', icon: '💳' },
];

/* ─── Payment helpers (lifted from old PaymentMethodsPage) ──────── */
type PaymentTyp = 'SEPA' | 'Kreditkarte' | 'PayPal' | 'Giropay';

const PAYMENT_TYPES: { value: PaymentTyp; label: string; icon: string }[] = [
  { value: 'SEPA', label: 'SEPA-Lastschrift', icon: '🏦' },
  { value: 'Kreditkarte', label: 'Kreditkarte', icon: '💳' },
  { value: 'PayPal', label: 'PayPal', icon: '🅿️' },
  { value: 'Giropay', label: 'Giropay', icon: '🔄' },
];

const emptyPaymentForm: SavePaymentMethod = {
  typ: 'SEPA',
  bezeichnung: '',
  kontoinhaber: '',
  iban: '',
  bic: '',
  bankname: '',
  kartenNummer: '',
  kartenInhaber: '',
  gueltigBis: '',
  paypalEmail: '',
  istStandard: false,
  sepaMandatErteilt: false,
};

const getTypeInfo = (typ: string) =>
  PAYMENT_TYPES.find((t) => t.value === typ) || { value: typ, label: typ, icon: '💰' };

const formatIBAN = (iban?: string) => {
  if (!iban) return '–';
  if (iban.length > 8) {
    return `${iban.substring(0, 4)} •••• •••• ${iban.substring(iban.length - 4)}`;
  }
  return iban;
};

/* ═══════════════════════════════════════════════════════════════════ */
/*  ProfilePage – unified tabbed page                                 */
/* ═══════════════════════════════════════════════════════════════════ */
const ProfilePage: React.FC = () => {
  const [searchParams, setSearchParams] = useSearchParams();
  const initialTab = (searchParams.get('tab') as TabKey) || 'stammdaten';
  const [activeTab, setActiveTab] = useState<TabKey>(initialTab);

  const switchTab = (key: TabKey) => {
    setActiveTab(key);
    setSearchParams({ tab: key });
  };

  /* ── Stammdaten state ──────────────────────────────────────────── */
  const [profile, setProfile] = useState<PersonalProfile | null>(null);
  const [pdLoading, setPdLoading] = useState(true);
  const [pdSaving, setPdSaving] = useState(false);
  const [pdError, setPdError] = useState<string | null>(null);
  const [pdSuccess, setPdSuccess] = useState<string | null>(null);
  const [photoPreview, setPhotoPreview] = useState<string | null>(null);
  const [uploadingPhoto, setUploadingPhoto] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const [pdForm, setPdForm] = useState<SavePersonalProfile>({
    anrede: 'Herr',
    titel: '',
    vorname: '',
    nachname: '',
    geburtsdatum: undefined,
    strasse: '',
    hausnummer: '',
    plz: '',
    ort: '',
    telefon: '',
    email: '',
  });

  useEffect(() => {
    const load = async () => {
      try {
        setPdLoading(true);
        const data = await ApiService.getPersonalProfile();
        setProfile(data);
        setPdForm({
          anrede: data.anrede || 'Herr',
          titel: data.titel || '',
          vorname: data.vorname || '',
          nachname: data.nachname || '',
          geburtsdatum: data.geburtsdatum ? data.geburtsdatum.substring(0, 10) : undefined,
          strasse: data.strasse || '',
          hausnummer: data.hausnummer || '',
          plz: data.plz || '',
          ort: data.ort || '',
          telefon: data.telefon || '',
          email: data.email || '',
        });
        if (data.profilbildUrl) setPhotoPreview(data.profilbildUrl);
        setPdError(null);
      } catch {
        setPdError('Daten konnten nicht geladen werden.');
      } finally {
        setPdLoading(false);
      }
    };
    load();
  }, []);

  const handlePdChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    setPdForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handlePdSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setPdSuccess(null);
    setPdError(null);
    try {
      setPdSaving(true);
      const saved = await ApiService.savePersonalProfile(pdForm);
      setProfile(saved);
      setPdSuccess('Änderungen wurden erfolgreich gespeichert.');
    } catch {
      setPdError('Speichern fehlgeschlagen. Bitte versuchen Sie es erneut.');
    } finally {
      setPdSaving(false);
    }
  };

  const handlePhotoClick = () => fileInputRef.current?.click();

  const handlePhotoChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    const allowed = ['image/jpeg', 'image/png', 'image/webp'];
    if (!allowed.includes(file.type)) {
      setPdError('Nur JPG, PNG und WebP sind erlaubt.');
      return;
    }
    if (file.size > 2 * 1024 * 1024) {
      setPdError('Datei ist zu groß (max. 2 MB).');
      return;
    }
    setPhotoPreview(URL.createObjectURL(file));
    try {
      setUploadingPhoto(true);
      setPdError(null);
      const saved = await ApiService.uploadProfilePhoto(file);
      setProfile(saved);
      if (saved.profilbildUrl) setPhotoPreview(saved.profilbildUrl);
      setPdSuccess('Profilbild wurde hochgeladen.');
    } catch {
      setPdError('Profilbild konnte nicht hochgeladen werden.');
    } finally {
      setUploadingPhoto(false);
    }
  };

  /* ── Zahlungsarten state ───────────────────────────────────────── */
  const [pmMethods, setPmMethods] = useState<PaymentMethod[]>([]);
  const [pmLoading, setPmLoading] = useState(true);
  const [pmSaving, setPmSaving] = useState(false);
  const [pmError, setPmError] = useState<string | null>(null);
  const [pmSuccess, setPmSuccess] = useState<string | null>(null);
  const [pmShowForm, setPmShowForm] = useState(false);
  const [pmEditId, setPmEditId] = useState<number | null>(null);
  const [pmForm, setPmForm] = useState<SavePaymentMethod>({ ...emptyPaymentForm });
  const [pmDeleteConfirm, setPmDeleteConfirm] = useState<number | null>(null);

  const loadMethods = useCallback(async () => {
    try {
      setPmLoading(true);
      const data = await ApiService.getPaymentMethods();
      setPmMethods(data);
      setPmError(null);
    } catch {
      setPmError('Zahlungsmethoden konnten nicht geladen werden.');
    } finally {
      setPmLoading(false);
    }
  }, []);

  useEffect(() => {
    loadMethods();
  }, [loadMethods]);

  const pmClearMsg = () => { setPmSuccess(null); setPmError(null); };

  const pmOpenNew = () => {
    pmClearMsg();
    setPmEditId(null);
    setPmForm({ ...emptyPaymentForm });
    setPmShowForm(true);
  };

  const pmOpenEdit = (m: PaymentMethod) => {
    pmClearMsg();
    setPmEditId(m.id);
    setPmForm({
      typ: m.typ,
      bezeichnung: m.bezeichnung,
      kontoinhaber: m.kontoinhaber || '',
      iban: m.iban || '',
      bic: m.bic || '',
      bankname: m.bankname || '',
      kartenNummer: m.kartenNummer || '',
      kartenInhaber: m.kartenInhaber || '',
      gueltigBis: m.gueltigBis || '',
      paypalEmail: m.paypalEmail || '',
      istStandard: m.istStandard,
      sepaMandatErteilt: m.sepaMandatErteilt,
    });
    setPmShowForm(true);
  };

  const pmCancelForm = () => {
    setPmShowForm(false);
    setPmEditId(null);
    setPmForm({ ...emptyPaymentForm });
  };

  const pmHandleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, type, value } = e.target;
    const checked = (e.target as HTMLInputElement).checked;
    setPmForm((prev) => ({ ...prev, [name]: type === 'checkbox' ? checked : value }));
  };

  const pmHandleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    pmClearMsg();
    if (!pmForm.bezeichnung.trim()) { setPmError('Bitte geben Sie eine Bezeichnung ein.'); return; }
    if (pmForm.typ === 'SEPA') {
      if (!pmForm.iban?.trim()) { setPmError('Bitte geben Sie die IBAN ein.'); return; }
      if (!pmForm.kontoinhaber?.trim()) { setPmError('Bitte geben Sie den Kontoinhaber ein.'); return; }
    }
    if (pmForm.typ === 'Kreditkarte') {
      if (!pmForm.kartenNummer?.trim()) { setPmError('Bitte geben Sie die letzten 4 Ziffern ein.'); return; }
      if (!pmForm.kartenInhaber?.trim()) { setPmError('Bitte geben Sie den Karteninhaber ein.'); return; }
    }
    if (pmForm.typ === 'PayPal' && !pmForm.paypalEmail?.trim()) {
      setPmError('Bitte geben Sie die PayPal-E-Mail ein.');
      return;
    }
    try {
      setPmSaving(true);
      if (pmEditId) {
        await ApiService.updatePaymentMethod(pmEditId, pmForm);
        setPmSuccess('Zahlungsmethode wurde erfolgreich aktualisiert.');
      } else {
        await ApiService.createPaymentMethod(pmForm);
        setPmSuccess('Zahlungsmethode wurde erfolgreich hinzugefügt.');
      }
      pmCancelForm();
      await loadMethods();
    } catch {
      setPmError('Speichern fehlgeschlagen.');
    } finally {
      setPmSaving(false);
    }
  };

  const pmHandleDelete = async (id: number) => {
    pmClearMsg();
    try {
      await ApiService.deletePaymentMethod(id);
      setPmDeleteConfirm(null);
      setPmSuccess('Zahlungsmethode wurde entfernt.');
      await loadMethods();
    } catch {
      setPmError('Löschen fehlgeschlagen.');
    }
  };

  const pmSetDefault = async (id: number) => {
    pmClearMsg();
    try {
      await ApiService.setDefaultPaymentMethod(id);
      setPmSuccess('Standard-Zahlungsmethode wurde geändert.');
      await loadMethods();
    } catch {
      setPmError('Änderung fehlgeschlagen.');
    }
  };

  /* ── Initials for bio panel ────────────────────────────────────── */
  const initials = (pdForm.vorname?.[0] || '') + (pdForm.nachname?.[0] || '') || 'HS';

  /* ═════════════════════════════════════════════════════════════════ */
  /*  Render                                                          */
  /* ═════════════════════════════════════════════════════════════════ */
  return (
    <div className="page profile-page">
      {/* ── Bio panel (always visible) ───────────────────────────── */}
      <div className="profile-bio-panel">
        <div
          className="profile-bio-avatar"
          onClick={handlePhotoClick}
          role="button"
          tabIndex={0}
          aria-label="Profilbild ändern"
        >
          {photoPreview ? (
            <img src={photoPreview} alt="Profilbild" className="profile-bio-avatar-img" />
          ) : (
            <span className="profile-bio-avatar-initials">{initials}</span>
          )}
          <div className="profile-bio-avatar-overlay">
            <span>📷</span>
          </div>
        </div>
        <input
          ref={fileInputRef}
          type="file"
          accept="image/jpeg,image/png,image/webp"
          onChange={handlePhotoChange}
          style={{ display: 'none' }}
        />
        <div className="profile-bio-info">
          <h1 className="profile-bio-name">
            {pdForm.vorname || ''} {pdForm.nachname || ''}
          </h1>
          <span className="profile-bio-hint">
            {uploadingPhoto
              ? 'Wird hochgeladen…'
              : 'Klicken Sie auf das Bild, um ein Profilbild hochzuladen'}
          </span>
        </div>
      </div>

      {/* ── Tab bar ──────────────────────────────────────────────── */}
      <div className="profile-tabs" role="tablist">
        {TABS.map((t) => (
          <button
            key={t.key}
            role="tab"
            aria-selected={activeTab === t.key}
            className={`profile-tab-btn ${activeTab === t.key ? 'active' : ''}`}
            onClick={() => switchTab(t.key)}
          >
            <span className="profile-tab-icon">{t.icon}</span>
            {t.label}
          </button>
        ))}
      </div>

      {/* ── Tab content ──────────────────────────────────────────── */}
      <div className="profile-tab-content">
        {/* ──────── Stammdaten ──────── */}
        {activeTab === 'stammdaten' && (
          <div className="profile-tab-panel" key="stammdaten">
            {pdLoading ? (
              <div className="loading-spinner">Stammdaten werden geladen…</div>
            ) : (
              <>
                {pdError && (
                  <div className="alert alert-error" role="alert">
                    <span className="alert-icon">!</span> {pdError}
                  </div>
                )}
                {pdSuccess && (
                  <div className="alert alert-success" role="alert">
                    <span className="alert-icon">✓</span> {pdSuccess}
                  </div>
                )}

                <form onSubmit={handlePdSubmit} className="registration-form">
                  <div className="form-section">
                    <h2>Anrede &amp; Name</h2>
                    <div className="details-fields-grid">
                      <div className="form-group">
                        <label htmlFor="pd-anrede">Anrede</label>
                        <select id="pd-anrede" name="anrede" value={pdForm.anrede} onChange={handlePdChange}>
                          <option value="Herr">Herr</option>
                          <option value="Frau">Frau</option>
                          <option value="Divers">Divers</option>
                        </select>
                      </div>
                      <div className="form-group">
                        <label htmlFor="pd-titel">Titel (optional)</label>
                        <input id="pd-titel" name="titel" value={pdForm.titel || ''} onChange={handlePdChange} placeholder="z. B. Dr., Prof." />
                      </div>
                      <div className="form-group">
                        <label htmlFor="pd-vorname">Vorname</label>
                        <input id="pd-vorname" name="vorname" value={pdForm.vorname} onChange={handlePdChange} required />
                      </div>
                      <div className="form-group">
                        <label htmlFor="pd-nachname">Nachname</label>
                        <input id="pd-nachname" name="nachname" value={pdForm.nachname} onChange={handlePdChange} required />
                      </div>
                      <div className="form-group">
                        <label htmlFor="pd-geburtsdatum">Geburtsdatum</label>
                        <input id="pd-geburtsdatum" name="geburtsdatum" type="date" value={pdForm.geburtsdatum || ''} onChange={handlePdChange} required />
                      </div>
                    </div>
                  </div>

                  <div className="form-section">
                    <h2>Adresse</h2>
                    <div className="details-fields-grid">
                      <div className="form-group">
                        <label htmlFor="pd-strasse">Straße</label>
                        <input id="pd-strasse" name="strasse" value={pdForm.strasse} onChange={handlePdChange} required />
                      </div>
                      <div className="form-group">
                        <label htmlFor="pd-hausnummer">Hausnummer</label>
                        <input id="pd-hausnummer" name="hausnummer" value={pdForm.hausnummer} onChange={handlePdChange} required />
                      </div>
                      <div className="form-group">
                        <label htmlFor="pd-plz">Postleitzahl</label>
                        <input id="pd-plz" name="plz" value={pdForm.plz} onChange={handlePdChange} maxLength={5} pattern="[0-9]{5}" required />
                      </div>
                      <div className="form-group">
                        <label htmlFor="pd-ort">Ort</label>
                        <input id="pd-ort" name="ort" value={pdForm.ort} onChange={handlePdChange} required />
                      </div>
                    </div>
                  </div>

                  <div className="form-section">
                    <h2>Kontaktdaten</h2>
                    <div className="details-fields-grid">
                      <div className="form-group">
                        <label htmlFor="pd-telefon">Telefon</label>
                        <input id="pd-telefon" name="telefon" type="tel" value={pdForm.telefon} onChange={handlePdChange} placeholder="+49 …" />
                      </div>
                      <div className="form-group">
                        <label htmlFor="pd-email">E-Mail-Adresse</label>
                        <input id="pd-email" name="email" type="email" value={pdForm.email} onChange={handlePdChange} required />
                      </div>
                    </div>
                  </div>

                  {profile && profile.id > 0 && (
                    <div className="form-section">
                      <h2>Kontoinformationen</h2>
                      <div className="info-row">
                        <span>Benutzerkennung</span>
                        <strong>{profile.userId}</strong>
                      </div>
                      <div className="info-row">
                        <span>Erstellt am</span>
                        <strong>{new Date(profile.createdAt).toLocaleDateString('de-DE')}</strong>
                      </div>
                      <div className="info-row">
                        <span>Letzte Änderung</span>
                        <strong>{new Date(profile.updatedAt).toLocaleString('de-DE')}</strong>
                      </div>
                    </div>
                  )}

                  <div className="form-footer">
                    <button type="submit" className="primary-button" disabled={pdSaving}>
                      {pdSaving ? 'Wird gespeichert…' : 'Änderungen speichern'}
                    </button>
                  </div>
                </form>
              </>
            )}
          </div>
        )}

        {/* ──────── Zahlungsarten ──────── */}
        {activeTab === 'zahlungsarten' && (
          <div className="profile-tab-panel" key="zahlungsarten">
            {pmLoading ? (
              <div className="loading-spinner">Zahlungsmethoden werden geladen…</div>
            ) : (
              <>
                <div className="payment-methods-header">
                  <div>
                    <h2>Zahlungsmethoden</h2>
                    <p className="payment-methods-subtitle">
                      Verwalten Sie Ihre Bankdaten und Zahlungsmethoden für Gebühren und Abgaben.
                    </p>
                  </div>
                  {!pmShowForm && (
                    <button className="btn btn-primary" onClick={pmOpenNew}>
                      + Zahlungsmethode hinzufügen
                    </button>
                  )}
                </div>

                {pmSuccess && (
                  <div className="alert alert-success" role="alert">
                    <span className="alert-icon">✓</span> {pmSuccess}
                  </div>
                )}
                {pmError && (
                  <div className="alert alert-error" role="alert">
                    <span className="alert-icon">!</span> {pmError}
                  </div>
                )}

                {/* Add / Edit form */}
                {pmShowForm && (
                  <div className="payment-form-card">
                    <h2>{pmEditId ? 'Zahlungsmethode bearbeiten' : 'Neue Zahlungsmethode'}</h2>
                    <form onSubmit={pmHandleSubmit}>
                      <div className="payment-form-grid">
                        <div className="form-group full-width">
                          <label>Zahlungsart *</label>
                          <div className="payment-type-selector">
                            {PAYMENT_TYPES.map((pt) => (
                              <button
                                key={pt.value}
                                type="button"
                                className={`payment-type-btn ${pmForm.typ === pt.value ? 'active' : ''}`}
                                onClick={() => setPmForm((prev) => ({ ...prev, typ: pt.value }))}
                              >
                                <span className="payment-type-icon">{pt.icon}</span>
                                <span>{pt.label}</span>
                              </button>
                            ))}
                          </div>
                        </div>
                        <div className="form-group">
                          <label htmlFor="pm-bezeichnung">Bezeichnung *</label>
                          <input id="pm-bezeichnung" name="bezeichnung" value={pmForm.bezeichnung} onChange={pmHandleChange} placeholder="z.B. Mein Hauptkonto" required />
                        </div>

                        {pmForm.typ === 'SEPA' && (
                          <>
                            <div className="form-group">
                              <label htmlFor="pm-kontoinhaber">Kontoinhaber *</label>
                              <input id="pm-kontoinhaber" name="kontoinhaber" value={pmForm.kontoinhaber || ''} onChange={pmHandleChange} required />
                            </div>
                            <div className="form-group">
                              <label htmlFor="pm-iban">IBAN *</label>
                              <input id="pm-iban" name="iban" value={pmForm.iban || ''} onChange={pmHandleChange} placeholder="DE89 3704 0044 0532 0130 00" required />
                            </div>
                            <div className="form-group">
                              <label htmlFor="pm-bic">BIC</label>
                              <input id="pm-bic" name="bic" value={pmForm.bic || ''} onChange={pmHandleChange} placeholder="COBADEFFXXX" />
                            </div>
                            <div className="form-group">
                              <label htmlFor="pm-bankname">Bankname</label>
                              <input id="pm-bankname" name="bankname" value={pmForm.bankname || ''} onChange={pmHandleChange} placeholder="Commerzbank" />
                            </div>
                            <div className="form-group full-width">
                              <label className="checkbox-label">
                                <input type="checkbox" name="sepaMandatErteilt" checked={pmForm.sepaMandatErteilt} onChange={pmHandleChange} />
                                <span>Ich erteile hiermit ein SEPA-Lastschriftmandat für wiederkehrende Zahlungen.</span>
                              </label>
                            </div>
                          </>
                        )}
                        {pmForm.typ === 'Kreditkarte' && (
                          <>
                            <div className="form-group">
                              <label htmlFor="pm-kartenInhaber">Karteninhaber *</label>
                              <input id="pm-kartenInhaber" name="kartenInhaber" value={pmForm.kartenInhaber || ''} onChange={pmHandleChange} required />
                            </div>
                            <div className="form-group">
                              <label htmlFor="pm-kartenNummer">Letzte 4 Ziffern *</label>
                              <input id="pm-kartenNummer" name="kartenNummer" value={pmForm.kartenNummer || ''} onChange={pmHandleChange} placeholder="1234" maxLength={4} pattern="\d{4}" required />
                            </div>
                            <div className="form-group">
                              <label htmlFor="pm-gueltigBis">Gültig bis</label>
                              <input id="pm-gueltigBis" name="gueltigBis" value={pmForm.gueltigBis || ''} onChange={pmHandleChange} placeholder="MM/YY" maxLength={5} />
                            </div>
                          </>
                        )}
                        {pmForm.typ === 'PayPal' && (
                          <div className="form-group">
                            <label htmlFor="pm-paypalEmail">PayPal E-Mail *</label>
                            <input id="pm-paypalEmail" name="paypalEmail" type="email" value={pmForm.paypalEmail || ''} onChange={pmHandleChange} required />
                          </div>
                        )}
                        {pmForm.typ === 'Giropay' && (
                          <div className="form-group full-width">
                            <p className="hint-text">Giropay wird über Ihr Online-Banking abgewickelt. Keine weiteren Daten erforderlich.</p>
                          </div>
                        )}
                        <div className="form-group full-width">
                          <label className="checkbox-label">
                            <input type="checkbox" name="istStandard" checked={pmForm.istStandard} onChange={pmHandleChange} />
                            <span>Als Standard-Zahlungsmethode festlegen</span>
                          </label>
                        </div>
                      </div>
                      <div className="payment-form-actions">
                        <button type="button" className="btn btn-secondary" onClick={pmCancelForm}>Abbrechen</button>
                        <button type="submit" className="btn btn-primary" disabled={pmSaving}>
                          {pmSaving ? 'Wird gespeichert…' : pmEditId ? 'Änderungen speichern' : 'Zahlungsmethode hinzufügen'}
                        </button>
                      </div>
                    </form>
                  </div>
                )}

                {/* Method list */}
                {pmMethods.length === 0 && !pmShowForm ? (
                  <div className="payment-empty-state">
                    <span className="payment-empty-icon">💳</span>
                    <h3>Keine Zahlungsmethoden hinterlegt</h3>
                    <p>Fügen Sie eine Zahlungsmethode hinzu, um Gebühren bequem bezahlen zu können.</p>
                    <button className="btn btn-primary" onClick={pmOpenNew}>+ Erste Zahlungsmethode hinzufügen</button>
                  </div>
                ) : (
                  <div className="payment-methods-grid">
                    {pmMethods.map((m) => {
                      const info = getTypeInfo(m.typ);
                      return (
                        <div key={m.id} className={`payment-card ${m.istStandard ? 'is-default' : ''}`}>
                          {m.istStandard && <span className="payment-default-badge">Standard</span>}
                          <div className="payment-card-header">
                            <span className="payment-card-icon">{info.icon}</span>
                            <div className="payment-card-title">
                              <strong>{m.bezeichnung || info.label}</strong>
                              <span className="payment-card-type">{info.label}</span>
                            </div>
                          </div>
                          <div className="payment-card-details">
                            {m.typ === 'SEPA' && (
                              <>
                                <div className="detail-row"><span className="detail-label">Kontoinhaber</span><span>{m.kontoinhaber || '–'}</span></div>
                                <div className="detail-row"><span className="detail-label">IBAN</span><span className="mono">{formatIBAN(m.iban)}</span></div>
                                {m.bankname && <div className="detail-row"><span className="detail-label">Bank</span><span>{m.bankname}</span></div>}
                                {m.sepaMandatErteilt && <div className="detail-row"><span className="sepa-badge">✓ SEPA-Mandat erteilt</span></div>}
                              </>
                            )}
                            {m.typ === 'Kreditkarte' && (
                              <>
                                <div className="detail-row"><span className="detail-label">Karteninhaber</span><span>{m.kartenInhaber || '–'}</span></div>
                                <div className="detail-row"><span className="detail-label">Kartennummer</span><span className="mono">•••• •••• •••• {m.kartenNummer}</span></div>
                                {m.gueltigBis && <div className="detail-row"><span className="detail-label">Gültig bis</span><span>{m.gueltigBis}</span></div>}
                              </>
                            )}
                            {m.typ === 'PayPal' && (
                              <div className="detail-row"><span className="detail-label">E-Mail</span><span>{m.paypalEmail || '–'}</span></div>
                            )}
                            {m.typ === 'Giropay' && (
                              <div className="detail-row"><span className="detail-label">Abwicklung</span><span>Online-Banking</span></div>
                            )}
                          </div>
                          <div className="payment-card-actions">
                            {!m.istStandard && (
                              <button className="btn-link" onClick={() => pmSetDefault(m.id)} title="Als Standard festlegen">⭐ Standard</button>
                            )}
                            <button className="btn-link" onClick={() => pmOpenEdit(m)} title="Bearbeiten">✏️ Bearbeiten</button>
                            {pmDeleteConfirm === m.id ? (
                              <span className="delete-confirm">
                                Wirklich löschen?{' '}
                                <button className="btn-link btn-link-danger" onClick={() => pmHandleDelete(m.id)}>Ja</button>{' '}
                                <button className="btn-link" onClick={() => setPmDeleteConfirm(null)}>Nein</button>
                              </span>
                            ) : (
                              <button className="btn-link btn-link-danger" onClick={() => setPmDeleteConfirm(m.id)} title="Löschen">🗑️ Löschen</button>
                            )}
                          </div>
                        </div>
                      );
                    })}
                  </div>
                )}
              </>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default ProfilePage;

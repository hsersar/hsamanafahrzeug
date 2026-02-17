import React, { useEffect, useState, useCallback } from 'react';
import ApiService from '../services/api';
import { PaymentMethod, SavePaymentMethod } from '../types';

type PaymentTyp = 'SEPA' | 'Kreditkarte' | 'PayPal' | 'Giropay';

const PAYMENT_TYPES: { value: PaymentTyp; label: string; icon: string }[] = [
  { value: 'SEPA', label: 'SEPA-Lastschrift', icon: '🏦' },
  { value: 'Kreditkarte', label: 'Kreditkarte', icon: '💳' },
  { value: 'PayPal', label: 'PayPal', icon: '🅿️' },
  { value: 'Giropay', label: 'Giropay', icon: '🔄' },
];

const emptyForm: SavePaymentMethod = {
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

const PaymentMethodsPage: React.FC = () => {
  const [methods, setMethods] = useState<PaymentMethod[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [successMsg, setSuccessMsg] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [formData, setFormData] = useState<SavePaymentMethod>({ ...emptyForm });
  const [deleteConfirmId, setDeleteConfirmId] = useState<number | null>(null);

  const loadMethods = useCallback(async () => {
    try {
      setLoading(true);
      const data = await ApiService.getPaymentMethods();
      setMethods(data);
      setError(null);
    } catch {
      setError('Zahlungsmethoden konnten nicht geladen werden.');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadMethods();
  }, [loadMethods]);

  const clearMessages = () => {
    setSuccessMsg(null);
    setError(null);
  };

  const openNewForm = () => {
    clearMessages();
    setEditId(null);
    setFormData({ ...emptyForm });
    setShowForm(true);
  };

  const openEditForm = (m: PaymentMethod) => {
    clearMessages();
    setEditId(m.id);
    setFormData({
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
    setShowForm(true);
  };

  const cancelForm = () => {
    setShowForm(false);
    setEditId(null);
    setFormData({ ...emptyForm });
  };

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, type, value } = e.target;
    const checked = (e.target as HTMLInputElement).checked;
    setFormData((prev) => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    clearMessages();

    // Basic validation
    if (!formData.bezeichnung.trim()) {
      setError('Bitte geben Sie eine Bezeichnung ein.');
      return;
    }
    if (formData.typ === 'SEPA') {
      if (!formData.iban?.trim()) {
        setError('Bitte geben Sie die IBAN ein.');
        return;
      }
      if (!formData.kontoinhaber?.trim()) {
        setError('Bitte geben Sie den Kontoinhaber ein.');
        return;
      }
    }
    if (formData.typ === 'Kreditkarte') {
      if (!formData.kartenNummer?.trim()) {
        setError('Bitte geben Sie die letzten 4 Ziffern der Kartennummer ein.');
        return;
      }
      if (!formData.kartenInhaber?.trim()) {
        setError('Bitte geben Sie den Karteninhaber ein.');
        return;
      }
    }
    if (formData.typ === 'PayPal' && !formData.paypalEmail?.trim()) {
      setError('Bitte geben Sie die PayPal-E-Mail-Adresse ein.');
      return;
    }

    try {
      setSaving(true);
      if (editId) {
        await ApiService.updatePaymentMethod(editId, formData);
        setSuccessMsg('Zahlungsmethode wurde erfolgreich aktualisiert.');
      } else {
        await ApiService.createPaymentMethod(formData);
        setSuccessMsg('Zahlungsmethode wurde erfolgreich hinzugefügt.');
      }
      cancelForm();
      await loadMethods();
    } catch {
      setError('Speichern fehlgeschlagen. Bitte versuchen Sie es erneut.');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (id: number) => {
    clearMessages();
    try {
      await ApiService.deletePaymentMethod(id);
      setDeleteConfirmId(null);
      setSuccessMsg('Zahlungsmethode wurde entfernt.');
      await loadMethods();
    } catch {
      setError('Löschen fehlgeschlagen.');
    }
  };

  const handleSetDefault = async (id: number) => {
    clearMessages();
    try {
      await ApiService.setDefaultPaymentMethod(id);
      setSuccessMsg('Standard-Zahlungsmethode wurde geändert.');
      await loadMethods();
    } catch {
      setError('Änderung fehlgeschlagen.');
    }
  };

  const getTypeInfo = (typ: string) =>
    PAYMENT_TYPES.find((t) => t.value === typ) || {
      value: typ,
      label: typ,
      icon: '💰',
    };

  const formatIBAN = (iban?: string) => {
    if (!iban) return '–';
    // Show first 4 and last 4, mask the rest
    if (iban.length > 8) {
      return `${iban.substring(0, 4)} •••• •••• ${iban.substring(iban.length - 4)}`;
    }
    return iban;
  };

  if (loading) {
    return (
      <div className="page-content payment-methods-page">
        <div className="loading-spinner">Zahlungsmethoden werden geladen…</div>
      </div>
    );
  }

  return (
    <div className="page-content payment-methods-page">
      <div className="payment-methods-header">
        <div>
          <h1>Zahlungsmethoden</h1>
          <p className="payment-methods-subtitle">
            Verwalten Sie Ihre Bankdaten und Zahlungsmethoden für Gebühren und Abgaben.
          </p>
        </div>
        {!showForm && (
          <button className="btn btn-primary" onClick={openNewForm}>
            + Zahlungsmethode hinzufügen
          </button>
        )}
      </div>

      {successMsg && (
        <div className="alert alert-success" role="alert">
          <span className="alert-icon">✓</span> {successMsg}
        </div>
      )}
      {error && (
        <div className="alert alert-error" role="alert">
          <span className="alert-icon">!</span> {error}
        </div>
      )}

      {/* ── Add / Edit form ────────────────────────────────────────── */}
      {showForm && (
        <div className="payment-form-card">
          <h2>{editId ? 'Zahlungsmethode bearbeiten' : 'Neue Zahlungsmethode'}</h2>
          <form onSubmit={handleSubmit}>
            <div className="payment-form-grid">
              {/* Typ */}
              <div className="form-group full-width">
                <label htmlFor="typ">Zahlungsart *</label>
                <div className="payment-type-selector">
                  {PAYMENT_TYPES.map((pt) => (
                    <button
                      key={pt.value}
                      type="button"
                      className={`payment-type-btn ${formData.typ === pt.value ? 'active' : ''}`}
                      onClick={() =>
                        setFormData((prev) => ({ ...prev, typ: pt.value }))
                      }
                    >
                      <span className="payment-type-icon">{pt.icon}</span>
                      <span>{pt.label}</span>
                    </button>
                  ))}
                </div>
              </div>

              {/* Bezeichnung */}
              <div className="form-group">
                <label htmlFor="bezeichnung">Bezeichnung *</label>
                <input
                  id="bezeichnung"
                  name="bezeichnung"
                  value={formData.bezeichnung}
                  onChange={handleChange}
                  placeholder="z.B. Mein Hauptkonto"
                  required
                />
              </div>

              {/* SEPA fields */}
              {formData.typ === 'SEPA' && (
                <>
                  <div className="form-group">
                    <label htmlFor="kontoinhaber">Kontoinhaber *</label>
                    <input
                      id="kontoinhaber"
                      name="kontoinhaber"
                      value={formData.kontoinhaber || ''}
                      onChange={handleChange}
                      placeholder="Max Mustermann"
                      required
                    />
                  </div>
                  <div className="form-group">
                    <label htmlFor="iban">IBAN *</label>
                    <input
                      id="iban"
                      name="iban"
                      value={formData.iban || ''}
                      onChange={handleChange}
                      placeholder="DE89 3704 0044 0532 0130 00"
                      required
                    />
                  </div>
                  <div className="form-group">
                    <label htmlFor="bic">BIC</label>
                    <input
                      id="bic"
                      name="bic"
                      value={formData.bic || ''}
                      onChange={handleChange}
                      placeholder="COBADEFFXXX"
                    />
                  </div>
                  <div className="form-group">
                    <label htmlFor="bankname">Bankname</label>
                    <input
                      id="bankname"
                      name="bankname"
                      value={formData.bankname || ''}
                      onChange={handleChange}
                      placeholder="Commerzbank"
                    />
                  </div>
                  <div className="form-group full-width">
                    <label className="checkbox-label">
                      <input
                        type="checkbox"
                        name="sepaMandatErteilt"
                        checked={formData.sepaMandatErteilt}
                        onChange={handleChange}
                      />
                      <span>
                        Ich erteile hiermit ein SEPA-Lastschriftmandat für wiederkehrende Zahlungen.
                      </span>
                    </label>
                  </div>
                </>
              )}

              {/* Kreditkarte fields */}
              {formData.typ === 'Kreditkarte' && (
                <>
                  <div className="form-group">
                    <label htmlFor="kartenInhaber">Karteninhaber *</label>
                    <input
                      id="kartenInhaber"
                      name="kartenInhaber"
                      value={formData.kartenInhaber || ''}
                      onChange={handleChange}
                      placeholder="Max Mustermann"
                      required
                    />
                  </div>
                  <div className="form-group">
                    <label htmlFor="kartenNummer">Letzte 4 Ziffern *</label>
                    <input
                      id="kartenNummer"
                      name="kartenNummer"
                      value={formData.kartenNummer || ''}
                      onChange={handleChange}
                      placeholder="1234"
                      maxLength={4}
                      pattern="\d{4}"
                      required
                    />
                  </div>
                  <div className="form-group">
                    <label htmlFor="gueltigBis">Gültig bis</label>
                    <input
                      id="gueltigBis"
                      name="gueltigBis"
                      value={formData.gueltigBis || ''}
                      onChange={handleChange}
                      placeholder="MM/YY"
                      maxLength={5}
                    />
                  </div>
                </>
              )}

              {/* PayPal fields */}
              {formData.typ === 'PayPal' && (
                <div className="form-group">
                  <label htmlFor="paypalEmail">PayPal E-Mail *</label>
                  <input
                    id="paypalEmail"
                    name="paypalEmail"
                    type="email"
                    value={formData.paypalEmail || ''}
                    onChange={handleChange}
                    placeholder="user@example.com"
                    required
                  />
                </div>
              )}

              {/* Giropay – no extra fields */}
              {formData.typ === 'Giropay' && (
                <div className="form-group full-width">
                  <p className="hint-text">
                    Giropay wird über Ihr Online-Banking abgewickelt. Keine weiteren Daten erforderlich.
                  </p>
                </div>
              )}

              {/* Standard checkbox */}
              <div className="form-group full-width">
                <label className="checkbox-label">
                  <input
                    type="checkbox"
                    name="istStandard"
                    checked={formData.istStandard}
                    onChange={handleChange}
                  />
                  <span>Als Standard-Zahlungsmethode festlegen</span>
                </label>
              </div>
            </div>

            <div className="payment-form-actions">
              <button type="button" className="btn btn-secondary" onClick={cancelForm}>
                Abbrechen
              </button>
              <button type="submit" className="btn btn-primary" disabled={saving}>
                {saving
                  ? 'Wird gespeichert…'
                  : editId
                  ? 'Änderungen speichern'
                  : 'Zahlungsmethode hinzufügen'}
              </button>
            </div>
          </form>
        </div>
      )}

      {/* ── Method list ────────────────────────────────────────────── */}
      {methods.length === 0 && !showForm ? (
        <div className="payment-empty-state">
          <span className="payment-empty-icon">💳</span>
          <h3>Keine Zahlungsmethoden hinterlegt</h3>
          <p>
            Fügen Sie eine Zahlungsmethode hinzu, um Gebühren bequem bezahlen zu können.
          </p>
          <button className="btn btn-primary" onClick={openNewForm}>
            + Erste Zahlungsmethode hinzufügen
          </button>
        </div>
      ) : (
        <div className="payment-methods-grid">
          {methods.map((m) => {
            const info = getTypeInfo(m.typ);
            return (
              <div
                key={m.id}
                className={`payment-card ${m.istStandard ? 'is-default' : ''}`}
              >
                {m.istStandard && (
                  <span className="payment-default-badge">Standard</span>
                )}

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
                      <div className="detail-row">
                        <span className="detail-label">Kontoinhaber</span>
                        <span>{m.kontoinhaber || '–'}</span>
                      </div>
                      <div className="detail-row">
                        <span className="detail-label">IBAN</span>
                        <span className="mono">{formatIBAN(m.iban)}</span>
                      </div>
                      {m.bankname && (
                        <div className="detail-row">
                          <span className="detail-label">Bank</span>
                          <span>{m.bankname}</span>
                        </div>
                      )}
                      {m.sepaMandatErteilt && (
                        <div className="detail-row">
                          <span className="sepa-badge">✓ SEPA-Mandat erteilt</span>
                        </div>
                      )}
                    </>
                  )}
                  {m.typ === 'Kreditkarte' && (
                    <>
                      <div className="detail-row">
                        <span className="detail-label">Karteninhaber</span>
                        <span>{m.kartenInhaber || '–'}</span>
                      </div>
                      <div className="detail-row">
                        <span className="detail-label">Kartennummer</span>
                        <span className="mono">•••• •••• •••• {m.kartenNummer}</span>
                      </div>
                      {m.gueltigBis && (
                        <div className="detail-row">
                          <span className="detail-label">Gültig bis</span>
                          <span>{m.gueltigBis}</span>
                        </div>
                      )}
                    </>
                  )}
                  {m.typ === 'PayPal' && (
                    <div className="detail-row">
                      <span className="detail-label">E-Mail</span>
                      <span>{m.paypalEmail || '–'}</span>
                    </div>
                  )}
                  {m.typ === 'Giropay' && (
                    <div className="detail-row">
                      <span className="detail-label">Abwicklung</span>
                      <span>Online-Banking</span>
                    </div>
                  )}
                </div>

                <div className="payment-card-actions">
                  {!m.istStandard && (
                    <button
                      className="btn-link"
                      onClick={() => handleSetDefault(m.id)}
                      title="Als Standard festlegen"
                    >
                      ⭐ Standard
                    </button>
                  )}
                  <button
                    className="btn-link"
                    onClick={() => openEditForm(m)}
                    title="Bearbeiten"
                  >
                    ✏️ Bearbeiten
                  </button>
                  {deleteConfirmId === m.id ? (
                    <span className="delete-confirm">
                      Wirklich löschen?{' '}
                      <button
                        className="btn-link btn-link-danger"
                        onClick={() => handleDelete(m.id)}
                      >
                        Ja
                      </button>{' '}
                      <button
                        className="btn-link"
                        onClick={() => setDeleteConfirmId(null)}
                      >
                        Nein
                      </button>
                    </span>
                  ) : (
                    <button
                      className="btn-link btn-link-danger"
                      onClick={() => setDeleteConfirmId(m.id)}
                      title="Löschen"
                    >
                      🗑️ Löschen
                    </button>
                  )}
                </div>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
};

export default PaymentMethodsPage;

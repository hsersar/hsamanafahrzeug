import React, { useEffect, useRef, useState } from 'react';
import ApiService from '../services/api';
import { CompanyProfile, SaveCompanyProfile } from '../types';

const CompanyProfilePage: React.FC = () => {
  const [profile, setProfile] = useState<CompanyProfile | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [successMsg, setSuccessMsg] = useState<string | null>(null);
  const [logoPreview, setLogoPreview] = useState<string | null>(null);
  const [uploadingLogo, setUploadingLogo] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const [formData, setFormData] = useState<SaveCompanyProfile>({
    firmenname: '',
    rechtsform: '',
    handelsregisternummer: '',
    ustIdNr: '',
    strasse: '',
    hausnummer: '',
    plz: '',
    ort: '',
    telefon: '',
    email: '',
    website: '',
    ansprechpartnerAnrede: 'Herr',
    ansprechpartnerVorname: '',
    ansprechpartnerNachname: '',
    ansprechpartnerTelefon: '',
    ansprechpartnerEmail: '',
    ansprechpartnerPosition: '',
  });

  useEffect(() => {
    const load = async () => {
      try {
        setLoading(true);
        const data = await ApiService.getCompanyProfile();
        setProfile(data);
        setFormData({
          firmenname: data.firmenname || '',
          rechtsform: data.rechtsform || '',
          handelsregisternummer: data.handelsregisternummer || '',
          ustIdNr: data.ustIdNr || '',
          strasse: data.strasse || '',
          hausnummer: data.hausnummer || '',
          plz: data.plz || '',
          ort: data.ort || '',
          telefon: data.telefon || '',
          email: data.email || '',
          website: data.website || '',
          ansprechpartnerAnrede: data.ansprechpartnerAnrede || 'Herr',
          ansprechpartnerVorname: data.ansprechpartnerVorname || '',
          ansprechpartnerNachname: data.ansprechpartnerNachname || '',
          ansprechpartnerTelefon: data.ansprechpartnerTelefon || '',
          ansprechpartnerEmail: data.ansprechpartnerEmail || '',
          ansprechpartnerPosition: data.ansprechpartnerPosition || '',
        });
        if (data.logoUrl) {
          setLogoPreview(data.logoUrl);
        }
        setError(null);
      } catch {
        setError('Unternehmensdaten konnten nicht geladen werden.');
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    setFormData((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSuccessMsg(null);
    setError(null);
    try {
      setSaving(true);
      const saved = await ApiService.saveCompanyProfile(formData);
      setProfile(saved);
      setSuccessMsg('Unternehmensdaten wurden erfolgreich gespeichert.');
    } catch {
      setError('Speichern fehlgeschlagen. Bitte versuchen Sie es erneut.');
    } finally {
      setSaving(false);
    }
  };

  const handleLogoClick = () => {
    fileInputRef.current?.click();
  };

  const handleLogoChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    const allowedTypes = ['image/jpeg', 'image/png', 'image/webp'];
    if (!allowedTypes.includes(file.type)) {
      setError('Nur JPG, PNG und WebP sind erlaubt.');
      return;
    }
    if (file.size > 2 * 1024 * 1024) {
      setError('Datei ist zu groß (max. 2 MB).');
      return;
    }

    const previewUrl = URL.createObjectURL(file);
    setLogoPreview(previewUrl);

    try {
      setUploadingLogo(true);
      setError(null);
      const saved = await ApiService.uploadCompanyLogo(file);
      setProfile(saved);
      if (saved.logoUrl) {
        setLogoPreview(saved.logoUrl);
      }
      setSuccessMsg('Firmenlogo wurde hochgeladen.');
    } catch {
      setError('Logo konnte nicht hochgeladen werden.');
    } finally {
      setUploadingLogo(false);
    }
  };

  if (loading) {
    return <div className="loading">Unternehmensdaten werden geladen…</div>;
  }

  return (
    <div className="page company-profile-page">
      <div className="page-header">
        <div>
          <h1>Unternehmensprofil</h1>
          <p>
            Verwalten Sie die Stammdaten Ihres Unternehmens sowie die
            Ansprechpartner-Informationen.
          </p>
        </div>
      </div>

      {error && <div className="alert alert-error">{error}</div>}
      {successMsg && <div className="alert alert-success">{successMsg}</div>}

      {/* Company Logo Section */}
      <div className="card profile-photo-card">
        <div className="profile-photo-section">
          <div
            className="profile-photo-circle company-logo-circle"
            onClick={handleLogoClick}
            role="button"
            tabIndex={0}
            aria-label="Firmenlogo ändern"
          >
            {logoPreview ? (
              <img
                src={logoPreview}
                alt="Firmenlogo"
                className="profile-photo-img"
              />
            ) : (
              <span className="profile-photo-initials">🏢</span>
            )}
            <div className="profile-photo-overlay">
              <span>📷</span>
            </div>
          </div>
          <div className="profile-photo-text">
            <strong>{formData.firmenname || 'Unternehmen'}</strong>
            <span className="profile-photo-hint">
              {uploadingLogo
                ? 'Wird hochgeladen…'
                : 'Klicken Sie auf das Bild, um ein Firmenlogo hochzuladen (JPG, PNG, WebP – max. 2 MB)'}
            </span>
          </div>
        </div>
        <input
          ref={fileInputRef}
          type="file"
          accept="image/jpeg,image/png,image/webp"
          onChange={handleLogoChange}
          style={{ display: 'none' }}
        />
      </div>

      <form onSubmit={handleSubmit} className="registration-form">
        <div className="form-section">
          <h2>Firmendaten</h2>
          <div className="details-fields-grid">
            <div className="form-group">
              <label htmlFor="firmenname">Firmenname</label>
              <input
                id="firmenname"
                name="firmenname"
                value={formData.firmenname}
                onChange={handleChange}
                required
              />
            </div>
            <div className="form-group">
              <label htmlFor="rechtsform">Rechtsform</label>
              <select
                id="rechtsform"
                name="rechtsform"
                value={formData.rechtsform}
                onChange={handleChange}
              >
                <option value="">Bitte wählen</option>
                <option value="GmbH">GmbH</option>
                <option value="AG">AG</option>
                <option value="e.K.">e.K.</option>
                <option value="OHG">OHG</option>
                <option value="KG">KG</option>
                <option value="GmbH & Co. KG">GmbH &amp; Co. KG</option>
                <option value="UG">UG (haftungsbeschränkt)</option>
                <option value="Sonstige">Sonstige</option>
              </select>
            </div>
            <div className="form-group">
              <label htmlFor="handelsregisternummer">Handelsregisternummer</label>
              <input
                id="handelsregisternummer"
                name="handelsregisternummer"
                value={formData.handelsregisternummer}
                onChange={handleChange}
                placeholder="z. B. HRB 12345"
              />
            </div>
            <div className="form-group">
              <label htmlFor="ustIdNr">USt-IdNr.</label>
              <input
                id="ustIdNr"
                name="ustIdNr"
                value={formData.ustIdNr}
                onChange={handleChange}
                placeholder="z. B. DE123456789"
              />
            </div>
          </div>
        </div>

        <div className="form-section">
          <h2>Firmenadresse</h2>
          <div className="details-fields-grid">
            <div className="form-group">
              <label htmlFor="strasse">Straße</label>
              <input
                id="strasse"
                name="strasse"
                value={formData.strasse}
                onChange={handleChange}
                required
              />
            </div>
            <div className="form-group">
              <label htmlFor="hausnummer">Hausnummer</label>
              <input
                id="hausnummer"
                name="hausnummer"
                value={formData.hausnummer}
                onChange={handleChange}
                required
              />
            </div>
            <div className="form-group">
              <label htmlFor="plz">Postleitzahl</label>
              <input
                id="plz"
                name="plz"
                value={formData.plz}
                onChange={handleChange}
                maxLength={5}
                pattern="[0-9]{5}"
                required
              />
            </div>
            <div className="form-group">
              <label htmlFor="ort">Ort</label>
              <input
                id="ort"
                name="ort"
                value={formData.ort}
                onChange={handleChange}
                required
              />
            </div>
          </div>
        </div>

        <div className="form-section">
          <h2>Firmenkontakt</h2>
          <div className="details-fields-grid">
            <div className="form-group">
              <label htmlFor="telefon">Telefon</label>
              <input
                id="telefon"
                name="telefon"
                type="tel"
                value={formData.telefon}
                onChange={handleChange}
                placeholder="+49 …"
              />
            </div>
            <div className="form-group">
              <label htmlFor="email">E-Mail</label>
              <input
                id="email"
                name="email"
                type="email"
                value={formData.email}
                onChange={handleChange}
              />
            </div>
            <div className="form-group">
              <label htmlFor="website">Website</label>
              <input
                id="website"
                name="website"
                type="url"
                value={formData.website}
                onChange={handleChange}
                placeholder="https://…"
              />
            </div>
          </div>
        </div>

        <div className="form-section">
          <h2>Ansprechpartner</h2>
          <div className="details-fields-grid">
            <div className="form-group">
              <label htmlFor="ansprechpartnerAnrede">Anrede</label>
              <select
                id="ansprechpartnerAnrede"
                name="ansprechpartnerAnrede"
                value={formData.ansprechpartnerAnrede}
                onChange={handleChange}
              >
                <option value="Herr">Herr</option>
                <option value="Frau">Frau</option>
                <option value="Divers">Divers</option>
              </select>
            </div>
            <div className="form-group">
              <label htmlFor="ansprechpartnerVorname">Vorname</label>
              <input
                id="ansprechpartnerVorname"
                name="ansprechpartnerVorname"
                value={formData.ansprechpartnerVorname}
                onChange={handleChange}
                required
              />
            </div>
            <div className="form-group">
              <label htmlFor="ansprechpartnerNachname">Nachname</label>
              <input
                id="ansprechpartnerNachname"
                name="ansprechpartnerNachname"
                value={formData.ansprechpartnerNachname}
                onChange={handleChange}
                required
              />
            </div>
            <div className="form-group">
              <label htmlFor="ansprechpartnerPosition">Position</label>
              <input
                id="ansprechpartnerPosition"
                name="ansprechpartnerPosition"
                value={formData.ansprechpartnerPosition || ''}
                onChange={handleChange}
                placeholder="z. B. Geschäftsführer"
              />
            </div>
            <div className="form-group">
              <label htmlFor="ansprechpartnerTelefon">Telefon</label>
              <input
                id="ansprechpartnerTelefon"
                name="ansprechpartnerTelefon"
                type="tel"
                value={formData.ansprechpartnerTelefon}
                onChange={handleChange}
                placeholder="+49 …"
              />
            </div>
            <div className="form-group">
              <label htmlFor="ansprechpartnerEmail">E-Mail</label>
              <input
                id="ansprechpartnerEmail"
                name="ansprechpartnerEmail"
                type="email"
                value={formData.ansprechpartnerEmail}
                onChange={handleChange}
              />
            </div>
          </div>
        </div>

        {profile && profile.id > 0 && (
          <div className="form-section">
            <h2>Profilinformationen</h2>
            <div className="info-row">
              <span>Benutzerkennung</span>
              <strong>{profile.userId}</strong>
            </div>
            <div className="info-row">
              <span>Erstellt am</span>
              <strong>
                {new Date(profile.createdAt).toLocaleDateString('de-DE')}
              </strong>
            </div>
            <div className="info-row">
              <span>Letzte Änderung</span>
              <strong>
                {new Date(profile.updatedAt).toLocaleString('de-DE')}
              </strong>
            </div>
          </div>
        )}

        <div className="form-footer">
          <button
            type="submit"
            className="primary-button"
            disabled={saving}
          >
            {saving ? 'Wird gespeichert…' : 'Änderungen speichern'}
          </button>
        </div>
      </form>
    </div>
  );
};

export default CompanyProfilePage;

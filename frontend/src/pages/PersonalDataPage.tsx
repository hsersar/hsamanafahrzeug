import React, { useEffect, useRef, useState } from 'react';
import ApiService from '../services/api';
import { PersonalProfile, SavePersonalProfile } from '../types';

const PersonalDataPage: React.FC = () => {
  const [profile, setProfile] = useState<PersonalProfile | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [successMsg, setSuccessMsg] = useState<string | null>(null);
  const [photoPreview, setPhotoPreview] = useState<string | null>(null);
  const [uploadingPhoto, setUploadingPhoto] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const [formData, setFormData] = useState<SavePersonalProfile>({
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
        setLoading(true);
        const data = await ApiService.getPersonalProfile();
        setProfile(data);
        setFormData({
          anrede: data.anrede || 'Herr',
          titel: data.titel || '',
          vorname: data.vorname || '',
          nachname: data.nachname || '',
          geburtsdatum: data.geburtsdatum
            ? data.geburtsdatum.substring(0, 10)
            : undefined,
          strasse: data.strasse || '',
          hausnummer: data.hausnummer || '',
          plz: data.plz || '',
          ort: data.ort || '',
          telefon: data.telefon || '',
          email: data.email || '',
        });
        if (data.profilbildUrl) {
          setPhotoPreview(data.profilbildUrl);
        }
        setError(null);
      } catch {
        setError('Daten konnten nicht geladen werden.');
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
      const saved = await ApiService.savePersonalProfile(formData);
      setProfile(saved);
      setSuccessMsg('Änderungen wurden erfolgreich gespeichert.');
    } catch {
      setError('Speichern fehlgeschlagen. Bitte versuchen Sie es erneut.');
    } finally {
      setSaving(false);
    }
  };

  const handlePhotoClick = () => {
    fileInputRef.current?.click();
  };

  const handlePhotoChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
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
    setPhotoPreview(previewUrl);

    try {
      setUploadingPhoto(true);
      setError(null);
      const saved = await ApiService.uploadProfilePhoto(file);
      setProfile(saved);
      if (saved.profilbildUrl) {
        setPhotoPreview(saved.profilbildUrl);
      }
      setSuccessMsg('Profilbild wurde hochgeladen.');
    } catch {
      setError('Profilbild konnte nicht hochgeladen werden.');
    } finally {
      setUploadingPhoto(false);
    }
  };

  if (loading) {
    return <div className="loading">Persönliche Daten werden geladen…</div>;
  }

  const initials =
    (formData.vorname?.[0] || '') + (formData.nachname?.[0] || '') || 'HS';

  return (
    <div className="page personal-data-page">
      <div className="page-header">
        <div>
          <h1>Persönliche Daten</h1>
          <p>
            Verwalten Sie Ihre Stammdaten für die Fahrzeugzulassung. Änderungen
            werden nach Prüfung übernommen.
          </p>
        </div>
      </div>

      {error && <div className="alert alert-error">{error}</div>}
      {successMsg && <div className="alert alert-success">{successMsg}</div>}

      {/* Profile Photo Section */}
      <div className="card profile-photo-card">
        <div className="profile-photo-section">
          <div
            className="profile-photo-circle"
            onClick={handlePhotoClick}
            role="button"
            tabIndex={0}
            aria-label="Profilbild ändern"
          >
            {photoPreview ? (
              <img
                src={photoPreview}
                alt="Profilbild"
                className="profile-photo-img"
              />
            ) : (
              <span className="profile-photo-initials">{initials}</span>
            )}
            <div className="profile-photo-overlay">
              <span>📷</span>
            </div>
          </div>
          <div className="profile-photo-text">
            <strong>
              {formData.vorname} {formData.nachname}
            </strong>
            <span className="profile-photo-hint">
              {uploadingPhoto
                ? 'Wird hochgeladen…'
                : 'Klicken Sie auf das Bild, um ein Profilbild hochzuladen (JPG, PNG, WebP – max. 2 MB)'}
            </span>
          </div>
        </div>
        <input
          ref={fileInputRef}
          type="file"
          accept="image/jpeg,image/png,image/webp"
          onChange={handlePhotoChange}
          style={{ display: 'none' }}
        />
      </div>

      <form onSubmit={handleSubmit} className="registration-form">
        <div className="form-section">
          <h2>Anrede &amp; Name</h2>
          <div className="details-fields-grid">
            <div className="form-group">
              <label htmlFor="anrede">Anrede</label>
              <select
                id="anrede"
                name="anrede"
                value={formData.anrede}
                onChange={handleChange}
              >
                <option value="Herr">Herr</option>
                <option value="Frau">Frau</option>
                <option value="Divers">Divers</option>
              </select>
            </div>
            <div className="form-group">
              <label htmlFor="titel">Titel (optional)</label>
              <input
                id="titel"
                name="titel"
                value={formData.titel || ''}
                onChange={handleChange}
                placeholder="z. B. Dr., Prof."
              />
            </div>
            <div className="form-group">
              <label htmlFor="vorname">Vorname</label>
              <input
                id="vorname"
                name="vorname"
                value={formData.vorname}
                onChange={handleChange}
                required
              />
            </div>
            <div className="form-group">
              <label htmlFor="nachname">Nachname</label>
              <input
                id="nachname"
                name="nachname"
                value={formData.nachname}
                onChange={handleChange}
                required
              />
            </div>
            <div className="form-group">
              <label htmlFor="geburtsdatum">Geburtsdatum</label>
              <input
                id="geburtsdatum"
                name="geburtsdatum"
                type="date"
                value={formData.geburtsdatum || ''}
                onChange={handleChange}
                required
              />
            </div>
          </div>
        </div>

        <div className="form-section">
          <h2>Adresse</h2>
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
          <h2>Kontaktdaten</h2>
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
              <label htmlFor="email">E-Mail-Adresse</label>
              <input
                id="email"
                name="email"
                type="email"
                value={formData.email}
                onChange={handleChange}
                required
              />
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

export default PersonalDataPage;

import React, { useState } from 'react';
import { SignaturAuthMethode } from '../../types/signatur';

export interface SignaturMethodenAuswahlProps {
  onMethodeGewaehlt: (methode: SignaturAuthMethode) => void;
  onZurueck?: () => void;
}

interface MethodenInfo {
  id: SignaturAuthMethode;
  icon: string;
  titel: string;
  beschreibung: string;
  verfuegbar: boolean;
}

const METHODEN: MethodenInfo[] = [
  {
    id: SignaturAuthMethode.SMS_TAN,
    icon: '📱',
    titel: 'SMS-TAN',
    beschreibung: 'Sie erhalten einen Bestätigungscode per SMS',
    verfuegbar: true
  },
  {
    id: SignaturAuthMethode.App_TAN,
    icon: '📲',
    titel: 'App-Bestätigung',
    beschreibung: 'Bestätigen Sie in der App Ihres Vertrauensdienstes',
    verfuegbar: true
  },
  {
    id: SignaturAuthMethode.eID,
    icon: '🪪',
    titel: 'eID (Personalausweis)',
    beschreibung: 'Nutzen Sie Ihren Personalausweis mit NFC-Funktion',
    verfuegbar: true
  },
  {
    id: SignaturAuthMethode.VideoIdent,
    icon: '📹',
    titel: 'Video-Ident',
    beschreibung: 'Identifizierung per Video-Call',
    verfuegbar: false
  }
];

/**
 * Auswahl der Authentifizierungsmethode für die QES
 */
const SignaturMethodenAuswahl: React.FC<SignaturMethodenAuswahlProps> = ({
  onMethodeGewaehlt,
  onZurueck
}) => {
  const [gewaehlteMethode, setGewaehlteMethode] = useState<SignaturAuthMethode | null>(null);

  const handleWeiter = () => {
    if (gewaehlteMethode !== null) {
      onMethodeGewaehlt(gewaehlteMethode);
    }
  };

  return (
    <div className="signatur-methoden-auswahl">
      <h2>Authentifizierungsmethode wählen</h2>
      <p className="info-text">
        Wählen Sie, wie Sie sich beim Vertrauensdiensteanbieter authentifizieren möchten:
      </p>

      <div className="methoden-grid">
        {METHODEN.map((methode) => (
          <div
            key={methode.id}
            className={`methoden-card ${
              gewaehlteMethode === methode.id ? 'selected' : ''
            } ${!methode.verfuegbar ? 'disabled' : ''}`}
            onClick={() => methode.verfuegbar && setGewaehlteMethode(methode.id)}
          >
            <div className="methoden-icon">{methode.icon}</div>
            <h3>{methode.titel}</h3>
            <p>{methode.beschreibung}</p>
            {!methode.verfuegbar && (
              <span className="badge badge-secondary">Demnächst verfügbar</span>
            )}
            {gewaehlteMethode === methode.id && (
              <div className="selected-indicator">✓</div>
            )}
          </div>
        ))}
      </div>

      <div className="info-box">
        <h3>ℹ️ Was ist eine qualifizierte elektronische Signatur?</h3>
        <p>
          Eine QES ist nach der eIDAS-Verordnung rechtlich gleichwertig zu einer handschriftlichen Unterschrift.
          Sie wird von einem zertifizierten Vertrauensdiensteanbieter (TSP) ausgestellt und garantiert:
        </p>
        <ul>
          <li>Ihre eindeutige Identifizierung als Unterzeichner</li>
          <li>Die Integrität des signierten Dokuments</li>
          <li>Rechtsgültigkeit vor Gericht</li>
        </ul>
        <p>
          <a href="#" target="_blank" rel="noopener noreferrer">
            Noch kein QES-Konto? Jetzt registrieren bei D-Trust →
          </a>
        </p>
      </div>

      <div className="actions">
        {onZurueck && (
          <button onClick={onZurueck} className="secondary">
            Zurück
          </button>
        )}
        <button
          onClick={handleWeiter}
          disabled={gewaehlteMethode === null}
          className="primary"
        >
          Weiter
        </button>
      </div>
    </div>
  );
};

export default SignaturMethodenAuswahl;

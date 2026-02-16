import React, { useEffect, useState } from 'react';
import { SignaturApi } from '../../services/signaturApi';
import { SignaturStatus } from '../../types/signatur';

export interface SignaturWartenProps {
  signaturId: string;
  onErfolg: (signaturId: string) => void;
  onFehler: (fehlerNachricht: string) => void;
}

/**
 * Wartebildschirm während der Kunde sich beim Provider authentifiziert
 */
const SignaturWarten: React.FC<SignaturWartenProps> = ({
  signaturId,
  onErfolg,
  onFehler
}) => {
  const [statusText, setStatusText] = useState('Initialisiere Signatur...');
  const [verbleibendeSekunden, setVerbleibendeSekunden] = useState(900); // 15 Minuten
  const [polling, setPolling] = useState(true);

  useEffect(() => {
    // Countdown Timer
    const timer = setInterval(() => {
      setVerbleibendeSekunden((prev) => {
        if (prev <= 0) {
          clearInterval(timer);
          return 0;
        }
        return prev - 1;
      });
    }, 1000);

    return () => clearInterval(timer);
  }, []);

  useEffect(() => {
    if (!polling) return;

    // Status-Polling
    SignaturApi.pollStatus(
      signaturId,
      (status) => {
        setStatusText(status.statusBeschreibung);

        if (status.status === SignaturStatus.Signiert) {
          setPolling(false);
          onErfolg(signaturId);
        } else if (
          status.status === SignaturStatus.Fehlgeschlagen ||
          status.status === SignaturStatus.Abgelaufen ||
          status.status === SignaturStatus.Abgelehnt
        ) {
          setPolling(false);
          onFehler(status.fehlerNachricht || 'Signatur konnte nicht abgeschlossen werden');
        }
      },
      3000, // Poll alle 3 Sekunden
      100   // Max 100 Versuche (= 5 Minuten)
    ).catch((error) => {
      setPolling(false);
      onFehler(error.message || 'Timeout beim Warten auf Signatur');
    });
  }, [signaturId, polling, onErfolg, onFehler]);

  const formatTime = (seconds: number): string => {
    const minutes = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${minutes}:${secs.toString().padStart(2, '0')}`;
  };

  return (
    <div className="signatur-warten">
      <div className="spinner-container">
        <div className="spinner"></div>
      </div>

      <h2>Bitte bestätigen Sie die Signatur</h2>
      
      <div className="status-info">
        <p className="status-text">{statusText}</p>
        <p className="timer-text">
          Verbleibende Zeit: <strong>{formatTime(verbleibendeSekunden)}</strong>
        </p>
      </div>

      <div className="instructions">
        <h3>Nächste Schritte:</h3>
        <ol>
          <li>Prüfen Sie Ihr Mobiltelefon oder E-Mail</li>
          <li>Folgen Sie den Anweisungen des Vertrauensdiensteanbieter</li>
          <li>Bestätigen Sie die Signatur mit dem erhaltenen Code</li>
        </ol>
      </div>

      <div className="info-box">
        <p>
          ℹ️ Das Fenster zur Authentifizierung wurde in einem neuen Tab geöffnet. 
          Bitte wechseln Sie zu diesem Tab und folgen Sie den Anweisungen.
        </p>
      </div>

      <div className="actions">
        <button
          className="secondary"
          onClick={() => onFehler('Vom Benutzer abgebrochen')}
        >
          Abbrechen
        </button>
      </div>
    </div>
  );
};

export default SignaturWarten;

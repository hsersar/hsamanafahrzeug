import React from 'react';

export interface SignaturFehlerProps {
  fehlerNachricht: string;
  onErneuerVersuch?: () => void;
  onAbbruch?: () => void;
}

/**
 * Fehleranzeige bei gescheiterter Signatur
 */
const SignaturFehler: React.FC<SignaturFehlerProps> = ({
  fehlerNachricht,
  onErneuerVersuch,
  onAbbruch
}) => {
  const getFehlerTyp = (nachricht: string): 'timeout' | 'abgelehnt' | 'technisch' => {
    const lower = nachricht.toLowerCase();
    if (lower.includes('abgelaufen') || lower.includes('timeout')) {
      return 'timeout';
    }
    if (lower.includes('abgelehnt') || lower.includes('abgebrochen')) {
      return 'abgelehnt';
    }
    return 'technisch';
  };

  const fehlerTyp = getFehlerTyp(fehlerNachricht);

  const getFehlerHilfe = () => {
    switch (fehlerTyp) {
      case 'timeout':
        return {
          icon: '⏱️',
          titel: 'Session abgelaufen',
          hilfe: [
            'Die Signatur-Session ist abgelaufen.',
            'Bitte starten Sie den Vorgang erneut.',
            'Sie haben 15 Minuten Zeit für die Authentifizierung.'
          ]
        };
      case 'abgelehnt':
        return {
          icon: '🚫',
          titel: 'Signatur abgelehnt',
          hilfe: [
            'Die Signatur wurde abgelehnt oder abgebrochen.',
            'Wenn Sie einen falschen Code eingegeben haben, können Sie es erneut versuchen.',
            'Bei weiteren Problemen wenden Sie sich an unseren Support.'
          ]
        };
      default:
        return {
          icon: '⚠️',
          titel: 'Technischer Fehler',
          hilfe: [
            'Es ist ein technischer Fehler aufgetreten.',
            'Bitte versuchen Sie es später erneut.',
            'Falls das Problem weiterhin besteht, kontaktieren Sie unseren Support.'
          ]
        };
    }
  };

  const { icon, titel, hilfe } = getFehlerHilfe();

  return (
    <div className="signatur-fehler">
      <div className="error-icon">
        <div className="icon-circle error">
          {icon}
        </div>
      </div>

      <h2>{titel}</h2>
      
      <div className="fehler-nachricht">
        <p><strong>Fehlermeldung:</strong></p>
        <p className="error-text">{fehlerNachricht}</p>
      </div>

      <div className="hilfe-box">
        <h3>Was können Sie tun?</h3>
        <ul>
          {hilfe.map((text, index) => (
            <li key={index}>{text}</li>
          ))}
        </ul>
      </div>

      <div className="haeufige-fehler">
        <h3>Häufige Probleme</h3>
        <details>
          <summary>Session abgelaufen</summary>
          <p>
            Die Signatur-Session ist nur 15 Minuten gültig. Wenn Sie die Authentifizierung
            nicht rechtzeitig abschließen, müssen Sie den Vorgang neu starten.
          </p>
          {onErneuerVersuch && (
            <button onClick={onErneuerVersuch} className="secondary small">
              Erneut starten
            </button>
          )}
        </details>

        <details>
          <summary>SMS-TAN nicht erhalten</summary>
          <p>
            Überprüfen Sie Ihre Telefonnummer und stellen Sie sicher, dass Sie eine
            gültige deutsche Mobilnummer angegeben haben. Die SMS kann einige Minuten dauern.
          </p>
        </details>

        <details>
          <summary>Provider nicht erreichbar</summary>
          <p>
            Der Vertrauensdiensteanbieter ist möglicherweise vorübergehend nicht verfügbar.
            Bitte versuchen Sie es in einigen Minuten erneut.
          </p>
        </details>
      </div>

      <div className="support-kontakt">
        <h3>📞 Support kontaktieren</h3>
        <p>
          Bei weiteren Fragen oder anhaltenden Problemen erreichen Sie unseren Support:
        </p>
        <ul>
          <li>E-Mail: support@example.de</li>
          <li>Telefon: 0800 123 456</li>
          <li>Montag - Freitag: 8:00 - 18:00 Uhr</li>
        </ul>
      </div>

      <div className="actions">
        {onAbbruch && (
          <button onClick={onAbbruch} className="secondary">
            Abbrechen
          </button>
        )}
        {onErneuerVersuch && (
          <button onClick={onErneuerVersuch} className="primary">
            Erneut versuchen
          </button>
        )}
      </div>
    </div>
  );
};

export default SignaturFehler;

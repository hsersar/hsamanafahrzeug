import React from 'react';
import { SignaturStatus } from '../../types/signatur';

export interface SignaturBadgeProps {
  status: SignaturStatus;
  compact?: boolean;
}

/**
 * Kleines Badge/Icon das den Signaturstatus anzeigt
 */
const SignaturBadge: React.FC<SignaturBadgeProps> = ({ status, compact = false }) => {
  const getStatusInfo = () => {
    switch (status) {
      case SignaturStatus.Signiert:
        return {
          icon: '✅',
          text: 'Signiert (QES)',
          className: 'badge-success'
        };
      case SignaturStatus.Angefordert:
      case SignaturStatus.SessionErstellt:
      case SignaturStatus.WartAufAuth:
      case SignaturStatus.AuthErfolgreich:
      case SignaturStatus.InSignierung:
        return {
          icon: '🔄',
          text: 'Signatur ausstehend',
          className: 'badge-warning'
        };
      case SignaturStatus.Fehlgeschlagen:
      case SignaturStatus.Abgelaufen:
      case SignaturStatus.Abgelehnt:
        return {
          icon: '❌',
          text: 'Fehlgeschlagen',
          className: 'badge-error'
        };
      case SignaturStatus.Storniert:
        return {
          icon: '⛔',
          text: 'Storniert',
          className: 'badge-secondary'
        };
      default:
        return {
          icon: '❓',
          text: 'Unbekannt',
          className: 'badge-secondary'
        };
    }
  };

  const { icon, text, className } = getStatusInfo();

  if (compact) {
    return (
      <span className={`signatur-badge compact ${className}`} title={text}>
        {icon}
      </span>
    );
  }

  return (
    <span className={`signatur-badge ${className}`}>
      <span className="badge-icon">{icon}</span>
      <span className="badge-text">{text}</span>
    </span>
  );
};

export default SignaturBadge;

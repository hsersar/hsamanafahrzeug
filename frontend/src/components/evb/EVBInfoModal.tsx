import React from 'react';

interface EVBInfoModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export function EVBInfoModal({ isOpen, onClose }: EVBInfoModalProps) {
  if (!isOpen) return null;

  return (
    <div style={{
      position: 'fixed',
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      backgroundColor: 'rgba(0, 0, 0, 0.5)',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      zIndex: 1000,
      padding: '1rem'
    }}>
      <div style={{
        backgroundColor: 'white',
        borderRadius: '0.5rem',
        padding: '2rem',
        maxWidth: '32rem',
        width: '100%',
        maxHeight: '90vh',
        overflowY: 'auto'
      }}>
        <h2 style={{ marginTop: 0 }}>Was ist eine eVB-Nummer?</h2>
        
        <p>
          Die eVB-Nummer (elektronische Versicherungsbestätigung) ist ein 7-stelliger 
          Code, den Sie von Ihrer KFZ-Versicherung erhalten. Diese Nummer wird bei 
          der Fahrzeugzulassung benötigt.
        </p>

        <h3>Wo finde ich meine eVB-Nummer?</h3>
        <ol>
          <li>Kontaktieren Sie Ihre KFZ-Versicherung</li>
          <li>Die eVB-Nummer wird meist per E-Mail oder SMS verschickt</li>
          <li>Alternativ finden Sie die Nummer in Ihrem Versicherungsportal</li>
        </ol>

        <h3>Format der eVB-Nummer</h3>
        <ul>
          <li>7 Zeichen lang</li>
          <li>Nur Großbuchstaben (A-Z) und Ziffern (0-9)</li>
          <li>Keine Umlaute (ä, ö, ü)</li>
          <li>Buchstaben I, O, Q werden nicht verwendet</li>
          <li>Beispiel: <code>ABC1234</code> oder <code>XYZ9876</code></li>
        </ul>

        <h3>Gängige KFZ-Versicherer</h3>
        <div style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(2, 1fr)',
          gap: '0.5rem',
          fontSize: '0.875rem'
        }}>
          <div>• Allianz</div>
          <div>• HUK-COBURG</div>
          <div>• ADAC</div>
          <div>• AXA</div>
          <div>• Generali</div>
          <div>• HDI</div>
          <div>• R+V Versicherung</div>
          <div>• DEVK</div>
          <div>• VHV</div>
          <div>• ERGO</div>
        </div>

        <div style={{ marginTop: '2rem', textAlign: 'right' }}>
          <button
            onClick={onClose}
            style={{
              backgroundColor: '#2563eb',
              color: 'white',
              border: 'none',
              padding: '0.5rem 1rem',
              borderRadius: '0.375rem',
              cursor: 'pointer',
              fontWeight: 600
            }}
          >
            Verstanden
          </button>
        </div>
      </div>
    </div>
  );
}

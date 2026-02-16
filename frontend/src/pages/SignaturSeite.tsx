import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import SignaturFlow from '../components/signatur/SignaturFlow';
import { SignaturTyp } from '../types/signatur';

/**
 * Eigenständige Seite für den Signatur-Prozess
 * Erreichbar über Link in E-Mail, QR-Code oder aus dem Kundenportal
 */
const SignaturSeite: React.FC = () => {
  const { auftragId } = useParams<{ auftragId: string }>();
  const navigate = useNavigate();

  // TODO: Lade Auftragsdaten und Kundendaten
  // Für Demo verwenden wir Mock-Daten
  const mockAuftragId = auftragId || '00000000-0000-0000-0000-000000000000';
  const mockSigniererName = 'Max Mustermann';
  const mockSigniererEmail = 'max.mustermann@example.de';
  const mockSigniererTelefon = '+49170123456789';

  const handleAbschluss = (signaturId: string) => {
    console.log('Signatur abgeschlossen:', signaturId);
    // Navigiere zurück zum Auftrag oder zur Übersicht
    navigate(`/auftrag/${auftragId}`);
  };

  const handleAbbruch = () => {
    console.log('Signatur abgebrochen');
    // Navigiere zurück
    navigate(-1);
  };

  return (
    <div className="signatur-seite">
      <div className="container">
        <div className="page-header">
          <h1>Qualifizierte Elektronische Signatur</h1>
          <p className="subtitle">
            Signieren Sie Ihre Dokumente rechtsgültig und sicher
          </p>
        </div>

        <div className="signatur-content">
          <SignaturFlow
            auftragId={mockAuftragId}
            typ={SignaturTyp.Zulassungsantrag}
            signiererName={mockSigniererName}
            signiererEmail={mockSigniererEmail}
            signiererTelefon={mockSigniererTelefon}
            onAbschluss={handleAbschluss}
            onAbbruch={handleAbbruch}
          />
        </div>

        <div className="page-footer">
          <div className="security-badges">
            <div className="badge">
              <span className="badge-icon">🔒</span>
              <span className="badge-text">SSL verschlüsselt</span>
            </div>
            <div className="badge">
              <span className="badge-icon">✓</span>
              <span className="badge-text">eIDAS-konform</span>
            </div>
            <div className="badge">
              <span className="badge-icon">🏛️</span>
              <span className="badge-text">Rechtsgültig</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default SignaturSeite;

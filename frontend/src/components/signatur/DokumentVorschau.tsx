import React, { useState } from 'react';

export interface DokumentVorschauProps {
  dokumentName?: string;
  dokumentUrl?: string;
  onBestaetigt: () => void;
  onAbbruch?: () => void;
}

/**
 * PDF-Vorschau Komponente
 * Zeigt das zu signierende Dokument an
 */
const DokumentVorschau: React.FC<DokumentVorschauProps> = ({
  dokumentName = 'Dokument.pdf',
  dokumentUrl,
  onBestaetigt,
  onAbbruch
}) => {
  const [dokumentGelesen, setDokumentGelesen] = useState(false);

  return (
    <div className="dokument-vorschau">
      <h2>Dokument-Vorschau</h2>
      <p className="info-text">
        Bitte lesen Sie das folgende Dokument sorgfältig durch, bevor Sie es signieren.
      </p>

      <div className="dokument-viewer">
        {dokumentUrl ? (
          <iframe
            src={dokumentUrl}
            title={dokumentName}
            width="100%"
            height="600px"
            style={{ border: '1px solid #ccc' }}
          />
        ) : (
          <div className="dokument-placeholder">
            <div className="pdf-icon">📄</div>
            <p><strong>{dokumentName}</strong></p>
            <p className="text-muted">Dokument-Vorschau wird geladen...</p>
          </div>
        )}
      </div>

      <div className="dokument-bestaetigung">
        <label className="checkbox-container">
          <input
            type="checkbox"
            checked={dokumentGelesen}
            onChange={(e) => setDokumentGelesen(e.target.checked)}
          />
          <span>
            Ich habe das Dokument gelesen und verstanden. Ich möchte es jetzt elektronisch signieren.
          </span>
        </label>
      </div>

      <div className="info-box">
        <h3>ℹ️ Was ist eine qualifizierte elektronische Signatur (QES)?</h3>
        <p>
          Die QES ist nach eIDAS-Verordnung rechtlich gleichwertig zu einer handschriftlichen Unterschrift.
          Sie authentifiziert Sie eindeutig als Unterzeichner und gewährleistet die Integrität des Dokuments.
        </p>
      </div>

      <div className="actions">
        {onAbbruch && (
          <button onClick={onAbbruch} className="secondary">
            Abbrechen
          </button>
        )}
        <button
          onClick={onBestaetigt}
          disabled={!dokumentGelesen}
          className="primary"
        >
          Weiter zur Signatur
        </button>
      </div>
    </div>
  );
};

export default DokumentVorschau;

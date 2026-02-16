import React, { useEffect, useState } from 'react';
import { SignaturApi } from '../../services/signaturApi';
import { SignaturStatusResult } from '../../types/signatur';

export interface SignaturErfolgProps {
  signaturId: string;
  onWeiter?: () => void;
}

/**
 * Erfolgsbestätigung nach erfolgreicher Signatur
 */
const SignaturErfolg: React.FC<SignaturErfolgProps> = ({
  signaturId,
  onWeiter
}) => {
  const [signaturInfo, setSignaturInfo] = useState<SignaturStatusResult | null>(null);
  const [downloading, setDownloading] = useState(false);

  useEffect(() => {
    // Lade Signatur-Details
    SignaturApi.getStatus(signaturId)
      .then(setSignaturInfo)
      .catch(console.error);
  }, [signaturId]);

  const handleDownload = async () => {
    try {
      setDownloading(true);
      const blob = await SignaturApi.downloadSigniertesDokument(signaturId);
      
      // Erstelle Download-Link
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `signiertes_dokument_${signaturId}.pdf`;
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);
    } catch (error) {
      console.error('Download fehlgeschlagen:', error);
      alert('Download fehlgeschlagen. Bitte versuchen Sie es später erneut.');
    } finally {
      setDownloading(false);
    }
  };

  return (
    <div className="signatur-erfolg">
      <div className="success-animation">
        <div className="checkmark-circle">
          <div className="checkmark">✓</div>
        </div>
      </div>

      <h2>Dokument erfolgreich signiert!</h2>
      
      <p className="success-message">
        Ihre qualifizierte elektronische Signatur wurde erfolgreich erstellt.
      </p>

      {signaturInfo && (
        <div className="signatur-details">
          <h3>Signatur-Details</h3>
          <dl className="details-list">
            <dt>Signiert von:</dt>
            <dd>{signaturInfo.signiererName || 'Unbekannt'}</dd>
            
            <dt>Signiert am:</dt>
            <dd>
              {signaturInfo.signiertAm 
                ? new Date(signaturInfo.signiertAm).toLocaleString('de-DE', {
                    day: '2-digit',
                    month: '2-digit',
                    year: 'numeric',
                    hour: '2-digit',
                    minute: '2-digit'
                  })
                : 'Unbekannt'
              }
            </dd>
            
            {signaturInfo.zertifikatInfo && (
              <>
                <dt>Zertifikat:</dt>
                <dd>{signaturInfo.zertifikatInfo}</dd>
              </>
            )}
            
            <dt>Signatur-Level:</dt>
            <dd>
              <span className="badge badge-success">
                Qualifiziert (QES)
              </span>
            </dd>
          </dl>
        </div>
      )}

      <div className="info-box">
        <h4>✅ Rechtsgültigkeit</h4>
        <p>
          Ihre qualifizierte elektronische Signatur ist nach der eIDAS-Verordnung 
          rechtlich gleichwertig zu einer handschriftlichen Unterschrift.
        </p>
      </div>

      <div className="actions">
        <button
          onClick={handleDownload}
          disabled={downloading}
          className="primary"
        >
          {downloading ? 'Wird heruntergeladen...' : '📥 Signiertes Dokument herunterladen'}
        </button>
        {onWeiter && (
          <button onClick={onWeiter} className="primary">
            Weiter zum Antrag →
          </button>
        )}
      </div>

      <div className="signature-seal">
        <div className="seal-icon">🔐</div>
        <p className="seal-text">Elektronisch signiert und versiegelt</p>
      </div>
    </div>
  );
};

export default SignaturErfolg;

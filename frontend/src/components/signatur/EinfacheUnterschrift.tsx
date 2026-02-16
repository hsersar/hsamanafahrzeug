import React, { useRef, useEffect, useState } from 'react';
import SignatureCanvas from 'react-signature-canvas';

export interface EinfacheUnterschriftProps {
  onSave: (signaturDataUrl: string) => void;
  onCancel?: () => void;
  width?: number;
  height?: number;
}

/**
 * Handschriftliche Unterschrift auf Touchscreen
 * Fallback für Fälle wo keine QES benötigt wird
 */
const EinfacheUnterschrift: React.FC<EinfacheUnterschriftProps> = ({
  onSave,
  onCancel,
  width = 600,
  height = 300
}) => {
  const signaturePadRef = useRef<SignatureCanvas>(null);
  const [isEmpty, setIsEmpty] = useState(true);

  useEffect(() => {
    // Prüfe ob Canvas leer ist bei jedem Stroke
    const canvas = signaturePadRef.current;
    if (canvas) {
      const originalOnEnd = canvas.toData;
      // @ts-ignore
      canvas.toData = () => {
        setIsEmpty(canvas.isEmpty());
        return originalOnEnd.call(canvas);
      };
    }
  }, []);

  const handleClear = () => {
    signaturePadRef.current?.clear();
    setIsEmpty(true);
  };

  const handleSave = () => {
    if (signaturePadRef.current && !isEmpty) {
      const dataUrl = signaturePadRef.current.toDataURL('image/png');
      onSave(dataUrl);
    }
  };

  return (
    <div className="einfache-unterschrift">
      <h2>Unterschrift</h2>
      <p className="info-text">
        Bitte unterschreiben Sie mit Ihrem Finger oder einem Stift im untenstehenden Feld.
      </p>

      <div className="signature-container">
        <SignatureCanvas
          ref={signaturePadRef}
          canvasProps={{
            width,
            height,
            className: 'signature-canvas'
          }}
          backgroundColor="white"
          penColor="black"
          minWidth={1}
          maxWidth={3}
          onEnd={() => setIsEmpty(signaturePadRef.current?.isEmpty() ?? true)}
        />
        <div className="signature-line"></div>
      </div>

      <div className="info-box">
        <p>
          ℹ️ <strong>Hinweis:</strong> Dies ist eine einfache elektronische Unterschrift (SES).
          Sie ist <strong>nicht</strong> rechtlich gleichwertig zu einer handschriftlichen Unterschrift.
          Für rechtsgültige Dokumente verwenden Sie bitte die qualifizierte elektronische Signatur (QES).
        </p>
      </div>

      <div className="actions">
        {onCancel && (
          <button onClick={onCancel} className="secondary">
            Abbrechen
          </button>
        )}
        <button onClick={handleClear} className="secondary" disabled={isEmpty}>
          Löschen
        </button>
        <button onClick={handleSave} className="primary" disabled={isEmpty}>
          Unterschrift bestätigen
        </button>
      </div>
    </div>
  );
};

export default EinfacheUnterschrift;

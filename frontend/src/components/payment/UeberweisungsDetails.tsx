import { QRCodeSVG } from 'qrcode.react';
import type { UeberweisungsDetails as UeberweisungsDetailsType } from '../../types';
import './UeberweisungsDetails.css';

interface Props {
  details: UeberweisungsDetailsType;
}

export default function UeberweisungsDetails({ details }: Props) {
  const copyToClipboard = (text: string) => {
    navigator.clipboard.writeText(text);
  };

  // Generate EPC QR Code (GiroCode) data
  const generateEPCData = () => {
    return [
      'BCD',
      '002',
      '1',
      'SCT',
      details.bic,
      details.empfaenger,
      details.iban.replace(/\s/g, ''),
      `EUR${details.betrag.toFixed(2)}`,
      '',
      '',
      details.verwendungszweck,
      '',
    ].join('\n');
  };

  const formatIBAN = (iban: string) => {
    return iban.replace(/(.{4})/g, '$1 ').trim();
  };

  return (
    <div className="ueberweisung-details">
      <h3>🏛️ Überweisung</h3>
      <p className="hint">
        Bitte überweisen Sie den Betrag mit den folgenden Daten:
      </p>

      <div className="details-card">
        <div className="detail-row">
          <span className="label">Empfänger:</span>
          <span className="value">{details.empfaenger}</span>
        </div>
        <div className="detail-row">
          <span className="label">IBAN:</span>
          <span className="value">
            {formatIBAN(details.iban)}
            <button
              className="copy-btn"
              onClick={() => copyToClipboard(details.iban)}
              title="IBAN kopieren"
            >
              📋
            </button>
          </span>
        </div>
        <div className="detail-row">
          <span className="label">BIC:</span>
          <span className="value">{details.bic}</span>
        </div>
        <div className="detail-row">
          <span className="label">Bank:</span>
          <span className="value">{details.bank}</span>
        </div>
        <div className="detail-row">
          <span className="label">Verwendungszweck:</span>
          <span className="value">
            {details.verwendungszweck}
            <button
              className="copy-btn"
              onClick={() => copyToClipboard(details.verwendungszweck)}
              title="Verwendungszweck kopieren"
            >
              📋
            </button>
          </span>
        </div>
        <div className="detail-row highlight">
          <span className="label">Betrag:</span>
          <span className="value amount">{details.betrag.toFixed(2)} EUR</span>
        </div>
      </div>

      <div className="qr-section">
        <h4>GiroCode (für Banking-Apps)</h4>
        <div className="qr-code">
          <QRCodeSVG value={generateEPCData()} size={200} level="M" />
        </div>
        <p className="qr-hint">
          Scannen Sie den QR-Code mit Ihrer Banking-App für eine einfache Überweisung
        </p>
      </div>

      <div className="info-box">
        <p>
          ⏰ Nach Zahlungseingang wird Ihr Antrag automatisch weiterbearbeitet.
          Dies kann 1-2 Werktage dauern.
        </p>
      </div>
    </div>
  );
}

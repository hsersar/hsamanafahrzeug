import { QRCodeSVG } from 'qrcode.react';
import './TrackingQRCode.css';

interface Props {
  trackingCode: string;
}

export default function TrackingQRCode({ trackingCode }: Props) {
  const trackingUrl = window.location.href;

  const handleShare = async () => {
    if (navigator.share) {
      try {
        await navigator.share({
          title: 'KFZ-Zulassung Tracking',
          text: `Tracking-Code: ${trackingCode}`,
          url: trackingUrl,
        });
      } catch (err) {
        console.error('Error sharing:', err);
      }
    }
  };

  const handlePrint = () => {
    window.print();
  };

  const handleDownload = () => {
    const canvas = document.querySelector('.qr-code canvas') as HTMLCanvasElement;
    if (canvas) {
      const url = canvas.toDataURL('image/png');
      const link = document.createElement('a');
      link.download = `tracking-${trackingCode}.png`;
      link.href = url;
      link.click();
    }
  };

  return (
    <div className="tracking-qrcode">
      <h3>📱 QR-Code teilen</h3>
      <div className="qr-code">
        <QRCodeSVG value={trackingUrl} size={200} level="H" />
      </div>
      <p className="qr-hint">
        Scannen Sie diesen QR-Code, um den Status jederzeit zu überprüfen
      </p>
      <div className="qr-actions">
        {typeof navigator.share === 'function' && (
          <button onClick={handleShare} className="btn btn-primary">
            📤 Teilen
          </button>
        )}
        <button onClick={handlePrint} className="btn btn-secondary">
          🖨️ Drucken
        </button>
        <button onClick={handleDownload} className="btn btn-secondary">
          💾 Speichern
        </button>
      </div>
    </div>
  );
}

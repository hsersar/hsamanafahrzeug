import { useEffect, useState } from 'react';
import { useLocation, useParams } from 'react-router-dom';
import { HubConnectionBuilder, HubConnection } from '@microsoft/signalr';
import { trackingApi } from '../services/api';
import type { TrackingStatusResponse } from '../types';
import TrackingTimeline from '../components/tracking/TrackingTimeline';
import TrackingQRCode from '../components/tracking/TrackingQRCode';
import './TrackingPage.css';

export default function TrackingPage() {
  const { trackingCode } = useParams<{ trackingCode: string }>();
  const location = useLocation();
  const [status, setStatus] = useState<TrackingStatusResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [connection, setConnection] = useState<HubConnection | null>(null);

  useEffect(() => {
    const params = new URLSearchParams(location.search);
    const token = params.get('token');

    if (!trackingCode || !token) {
      setError('Ungültiger Tracking-Link');
      setLoading(false);
      return;
    }

    loadStatus(trackingCode, token);
    setupSignalR(trackingCode, token);

    return () => {
      if (connection) {
        connection.stop();
      }
    };
  }, [trackingCode, location.search]);

  const loadStatus = async (code: string, token: string) => {
    try {
      setLoading(true);
      const data = await trackingApi.getStatus(code, token);
      setStatus(data);
      setError(null);
    } catch (err) {
      setError('Fehler beim Laden des Status. Bitte überprüfen Sie Ihren Tracking-Link.');
    } finally {
      setLoading(false);
    }
  };

  const setupSignalR = async (code: string, token: string) => {
    const hubUrl = import.meta.env.VITE_API_URL?.replace('/api', '') || 'http://localhost:5000';
    const newConnection = new HubConnectionBuilder()
      .withUrl(`${hubUrl}/hubs/tracking`)
      .withAutomaticReconnect()
      .build();

    try {
      await newConnection.start();
      await newConnection.invoke('SubscribeToTracking', code, token);

      newConnection.on('StatusUpdate', (newStatus: TrackingStatusResponse) => {
        setStatus(newStatus);
      });

      setConnection(newConnection);
    } catch (err) {
      console.error('SignalR connection error:', err);
    }
  };

  if (loading) {
    return (
      <div className="tracking-page">
        <div className="loading">
          <div className="spinner"></div>
          <p>Laden...</p>
        </div>
      </div>
    );
  }

  if (error || !status) {
    return (
      <div className="tracking-page">
        <div className="error">
          <h2>❌ Fehler</h2>
          <p>{error || 'Status nicht gefunden'}</p>
        </div>
      </div>
    );
  }

  return (
    <div className="tracking-page">
      <header className="tracking-header">
        <h1>📦 KFZ-Zulassung Tracking</h1>
        <div className="tracking-code">
          <span className="label">Tracking-Code:</span>
          <span className="code">{status.trackingCode}</span>
        </div>
      </header>

      <div className="status-card">
        <div className="status-icon">
          {status.fortschrittProzent === 100 ? '✅' : '🔄'}
        </div>
        <h2>{status.statusBeschreibung}</h2>
        <div className="progress-bar">
          <div 
            className="progress-fill" 
            style={{ width: `${status.fortschrittProzent}%` }}
          ></div>
        </div>
        <p className="progress-text">{status.fortschrittProzent}% abgeschlossen</p>
      </div>

      <div className="info-grid">
        <div className="info-item">
          <span className="info-label">Typ:</span>
          <span className="info-value">{status.auftragTyp}</span>
        </div>
        {status.kennzeichen && (
          <div className="info-item">
            <span className="info-label">Kennzeichen:</span>
            <span className="info-value">{status.kennzeichen}</span>
          </div>
        )}
        <div className="info-item">
          <span className="info-label">Erstellt:</span>
          <span className="info-value">
            {new Date(status.erstelltAm).toLocaleDateString('de-DE')}
          </span>
        </div>
      </div>

      {status.zahlungErforderlich && !status.zahlungErfolgt && (
        <div className="payment-banner">
          <p>⚠️ Zahlung erforderlich</p>
          <button className="payment-button">Jetzt bezahlen</button>
        </div>
      )}

      <TrackingTimeline historie={status.historie} />

      <TrackingQRCode trackingCode={status.trackingCode} />
    </div>
  );
}

import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import ApiService from '../services/api';
import { DashboardOverview } from '../types';

const HomePage: React.FC = () => {
  const [overview, setOverview] = useState<DashboardOverview | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const loadOverview = async () => {
      try {
        setLoading(true);
        const data = await ApiService.getDashboardOverview();
        setOverview(data);
        setError(null);
      } catch (err) {
        setError('Uebersicht konnte nicht geladen werden.');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    loadOverview();
  }, []);

  const statusLabel = (status?: string | null) => {
    const map: Record<string, string> = {
      Pending: 'Eingegangen',
      UnderReview: 'In Prüfung',
      Approved: 'Genehmigt',
      Rejected: 'Abgelehnt',
    };

    if (!status) return 'Kein Vorgang';
    return map[status] ?? status;
  };

  return (
    <div className="page home-page">
      <div className="page-header">
        <div>
          <h1>Übersicht</h1>
          <p>Verwalten Sie Ihre Zulassungen und Fahrzeugdaten in einem Vorgang.</p>
        </div>
        <span className="badge">
          {loading ? 'Lade...' : statusLabel(overview?.latestRequestStatus)}
        </span>
      </div>
      {error && <div className="error">{error}</div>}
      <div className="page-grid">
        <div className="card hero-card">
          <div className="card-header">
            <div>
              <div className="card-title">Antrag auf Fahrzeugzulassung</div>
              <div className="card-subtitle">
                Geführte Erfassung mit Plausibilitätsprüfung.
              </div>
            </div>
            <span className="badge">Schritt 1</span>
          </div>
          <p>
            Legen Sie einen neuen Antrag an und verfolgen Sie die Bearbeitung
            durchgehend.
          </p>
          <div className="card-actions">
            <Link to="/new-registration" className="primary-button">
              Antrag erstellen
            </Link>
            <Link to="/my-requests" className="secondary-button">
              Bearbeitungsstand
            </Link>
          </div>
        </div>

        <div className="card">
          <div className="card-title">Aktueller Vorgang</div>
          <div className="card-subtitle">Übersicht der letzten 30 Tage</div>
          <div className="info-row">
            <span>Offene Anträge</span>
            <strong>{overview?.openRequests ?? 0}</strong>
          </div>
          <div className="info-row">
            <span>Registrierte Fahrzeuge</span>
            <strong>{overview?.registeredVehicles ?? 0}</strong>
          </div>
          <div className="info-row">
            <span>Rückfragen</span>
            <strong>{overview?.pendingReviews ?? 0}</strong>
          </div>
        </div>
      </div>

      <div className="page-grid">
        <div className="card">
          <div className="card-title">Sicherheitsniveau</div>
          <p>OAuth2/OIDC, ISO 27001, keine personenbezogenen Daten in Logs.</p>
        </div>
        <div className="card">
          <div className="card-title">Datenschutz</div>
          <p>DSGVO-konform mit verschlüsselter Kommunikation.</p>
        </div>
        <div className="card">
          <div className="card-title">Unterstützung</div>
          <p>Digitale Hilfe und Schritt-für-Schritt-Anleitung.</p>
        </div>
      </div>
    </div>
  );
};

export default HomePage;

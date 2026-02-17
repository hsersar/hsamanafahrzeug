import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import ApiService from '../services/api';
import { RegistrationRequest } from '../types';

interface TimelineEntry {
  status: string;
  label: string;
  icon: string;
  date?: string;
  beschreibung: string;
  completed: boolean;
  active: boolean;
}

const STATUS_ORDER = ['Pending', 'UnderReview', 'Approved', 'Rejected'];
const STATUS_LABELS: Record<string, string> = {
  Pending: 'Eingegangen',
  UnderReview: 'In Prüfung',
  Approved: 'Genehmigt',
  Rejected: 'Abgelehnt',
};
const STATUS_ICONS: Record<string, string> = {
  Pending: '📩',
  UnderReview: '🔍',
  Approved: '✅',
  Rejected: '❌',
};

const OrderDetailPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [request, setRequest] = useState<RegistrationRequest | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const load = async () => {
      if (!id) return;
      try {
        setLoading(true);
        const data = await ApiService.getRegistrationRequest(Number(id));
        setRequest(data);
        setError(null);
      } catch {
        setError('Antrag konnte nicht geladen werden.');
      } finally {
        setLoading(false);
      }
    };
    load();
  }, [id]);

  const formatDate = (dateStr: string) =>
    new Date(dateStr).toLocaleDateString('de-DE', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });

  const getTimeline = (req: RegistrationRequest): TimelineEntry[] => {
    const currentIdx = STATUS_ORDER.indexOf(req.status);
    const isRejected = req.status === 'Rejected';

    const steps: TimelineEntry[] = [
      {
        status: 'Pending',
        label: 'Antrag eingegangen',
        icon: '📩',
        date: req.createdAt,
        beschreibung: 'Ihr Antrag wurde erfolgreich übermittelt und wird bearbeitet.',
        completed: currentIdx >= 0,
        active: req.status === 'Pending',
      },
      {
        status: 'UnderReview',
        label: 'In Prüfung',
        icon: '🔍',
        date: currentIdx >= 1 ? req.updatedAt : undefined,
        beschreibung: 'Ihr Antrag wird von einem Sachbearbeiter geprüft.',
        completed: currentIdx >= 1,
        active: req.status === 'UnderReview',
      },
    ];

    if (isRejected) {
      steps.push({
        status: 'Rejected',
        label: 'Abgelehnt',
        icon: '❌',
        date: req.updatedAt,
        beschreibung: req.rejectionReason || 'Ihr Antrag wurde leider abgelehnt.',
        completed: true,
        active: true,
      });
    } else {
      steps.push({
        status: 'Approved',
        label: 'Genehmigt',
        icon: '✅',
        date: currentIdx >= 2 ? req.updatedAt : undefined,
        beschreibung: 'Ihr Antrag wurde genehmigt. Das Kennzeichen wird zugeteilt.',
        completed: currentIdx >= 2,
        active: req.status === 'Approved',
      });
    }

    return steps;
  };

  if (loading) {
    return (
      <div className="page order-detail-page">
        <div className="loading-spinner">Antrag wird geladen…</div>
      </div>
    );
  }

  if (error || !request) {
    return (
      <div className="page order-detail-page">
        <div className="alert alert-error" role="alert">
          <span className="alert-icon">!</span> {error || 'Antrag nicht gefunden.'}
        </div>
        <button className="btn btn-secondary" onClick={() => navigate('/my-requests')}>
          ← Zurück zu Meine Anträge
        </button>
      </div>
    );
  }

  const timeline = getTimeline(request);
  const statusInfo = STATUS_LABELS[request.status] || request.status;
  const statusIcon = STATUS_ICONS[request.status] || '📋';

  return (
    <div className="page order-detail-page">
      <div className="order-detail-nav">
        <button className="btn-link" onClick={() => navigate('/my-requests')}>
          ← Zurück zu Meine Anträge
        </button>
      </div>

      <div className="order-detail-header">
        <div className="order-detail-title">
          <h1>Zulassungsantrag #{request.id}</h1>
          <span className={`order-status-badge order-status--${request.status.toLowerCase()}`}>
            {statusIcon} {statusInfo}
          </span>
        </div>
        <div className="order-detail-meta">
          <span>Erstellt am {formatDate(request.createdAt)}</span>
          <span>Zuletzt aktualisiert: {formatDate(request.updatedAt)}</span>
        </div>
      </div>

      <div className="order-detail-grid">
        {/* Left: Vehicle & License info */}
        <div className="order-detail-section">
          <div className="order-detail-card">
            <h2>Kennzeichen</h2>
            <div className="license-plate-display">
              <div className="license-plate-visual">
                <span className="license-plate-eu">D</span>
                <span className="license-plate-text">{request.requestedLicensePlate}</span>
              </div>
            </div>
          </div>

          <div className="order-detail-card">
            <h2>Fahrzeugdaten</h2>
            <div className="order-info-grid">
              <div className="order-info-row">
                <span className="order-info-label">Marke</span>
                <span className="order-info-value">{request.brand}</span>
              </div>
              <div className="order-info-row">
                <span className="order-info-label">Modell</span>
                <span className="order-info-value">{request.model}</span>
              </div>
              <div className="order-info-row">
                <span className="order-info-label">Baujahr</span>
                <span className="order-info-value">{request.year}</span>
              </div>
              <div className="order-info-row">
                <span className="order-info-label">Farbe</span>
                <span className="order-info-value">{request.color}</span>
              </div>
              <div className="order-info-row">
                <span className="order-info-label">FIN</span>
                <span className="order-info-value mono">{request.vin}</span>
              </div>
              <div className="order-info-row">
                <span className="order-info-label">Erstzulassung</span>
                <span className="order-info-value">
                  {new Date(request.firstRegistrationDate).toLocaleDateString('de-DE')}
                </span>
              </div>
            </div>
          </div>
        </div>

        {/* Right: Timeline */}
        <div className="order-detail-section">
          <div className="order-detail-card">
            <h2>Bearbeitungsverlauf</h2>
            <div className="order-timeline">
              {timeline.map((entry, idx) => (
                <div
                  key={entry.status}
                  className={`timeline-item ${
                    entry.completed ? 'completed' : ''
                  } ${entry.active ? 'active' : ''}`}
                >
                  <div className="timeline-connector">
                    <span className="timeline-dot">{entry.icon}</span>
                    {idx < timeline.length - 1 && (
                      <span
                        className={`timeline-line ${
                          entry.completed ? 'completed' : ''
                        }`}
                      />
                    )}
                  </div>
                  <div className="timeline-content">
                    <div className="timeline-header">
                      <strong>{entry.label}</strong>
                      {entry.date && (
                        <span className="timeline-date">{formatDate(entry.date)}</span>
                      )}
                    </div>
                    <p className="timeline-desc">{entry.beschreibung}</p>
                  </div>
                </div>
              ))}
            </div>
          </div>

          {request.rejectionReason && (
            <div className="order-detail-card order-detail-card--warning">
              <h2>Ablehnungsgrund</h2>
              <p>{request.rejectionReason}</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default OrderDetailPage;

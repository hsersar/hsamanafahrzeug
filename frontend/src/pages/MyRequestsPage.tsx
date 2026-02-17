import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import ApiService from '../services/api';
import { RegistrationRequest } from '../types';

const MyRequestsPage: React.FC = () => {
  const [requests, setRequests] = useState<RegistrationRequest[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadRequests();
  }, []);

  const loadRequests = async () => {
    try {
      setLoading(true);
      const data = await ApiService.getMyRequests();
      setRequests(data);
      setError(null);
    } catch (err) {
      setError('Anträge konnten nicht geladen werden');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="loading">Vorgang wird geladen...</div>;
  }

  if (error) {
    return <div className="error">{error}</div>;
  }

  const statusLabel = (status: string) => {
    const map: Record<string, string> = {
      Pending: 'Eingegangen',
      UnderReview: 'In Prüfung',
      Approved: 'Genehmigt',
      Rejected: 'Abgelehnt',
    };

    return map[status] ?? status;
  };

  return (
    <div className="page my-requests-page">
      <div className="page-header">
        <div>
          <h1>Meine Anträge</h1>
          <p>Verfolgen Sie den aktuellen Bearbeitungsstand.</p>
        </div>
      </div>
      {requests.length === 0 ? (
        <div className="card">Derzeit liegen keine Anträge vor.</div>
      ) : (
        <div className="requests-list">
          {requests.map((request) => (
            <Link to={`/my-requests/${request.id}`} key={request.id} className="request-card request-card-link">
              <div className="card-header">
                <div className="card-title">
                  {request.brand} {request.model} ({request.year})
                </div>
                <span className="status-pill-small">
                  {statusLabel(request.status)}
                </span>
              </div>
              <div className="request-details">
                <p>
                  <strong>FIN:</strong> {request.vin}
                </p>
                <p>
                  <strong>Wunschkennzeichen:</strong> {request.requestedLicensePlate}
                </p>
                <p>
                  <strong>Farbe:</strong> {request.color}
                </p>
                <p>
                  <strong>Eingereicht:</strong>{' '}
                  {new Date(request.createdAt).toLocaleDateString()}
                </p>
                {request.rejectionReason && (
                  <p className="rejection-reason">
                    <strong>Ablehnungsgrund:</strong> {request.rejectionReason}
                  </p>
                )}
              </div>
            </Link>
          ))}
        </div>
      )}
    </div>
  );
};

export default MyRequestsPage;

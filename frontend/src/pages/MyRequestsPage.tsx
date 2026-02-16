import React, { useState, useEffect } from 'react';
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
      setError('Failed to load registration requests');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  if (error) {
    return <div className="error">{error}</div>;
  }

  return (
    <div className="my-requests-page">
      <h1>My Registration Requests</h1>
      {requests.length === 0 ? (
        <p>No registration requests found.</p>
      ) : (
        <div className="requests-list">
          {requests.map((request) => (
            <div key={request.id} className="request-card">
              <h3>
                {request.brand} {request.model} ({request.year})
              </h3>
              <div className="request-details">
                <p>
                  <strong>VIN:</strong> {request.vin}
                </p>
                <p>
                  <strong>Requested License Plate:</strong> {request.requestedLicensePlate}
                </p>
                <p>
                  <strong>Color:</strong> {request.color}
                </p>
                <p>
                  <strong>Status:</strong>{' '}
                  <span className={`status-${request.status.toLowerCase()}`}>
                    {request.status}
                  </span>
                </p>
                <p>
                  <strong>Submitted:</strong>{' '}
                  {new Date(request.createdAt).toLocaleDateString()}
                </p>
                {request.rejectionReason && (
                  <p className="rejection-reason">
                    <strong>Rejection Reason:</strong> {request.rejectionReason}
                  </p>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default MyRequestsPage;

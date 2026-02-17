import React, { useState } from 'react';
import ApiService from '../services/api';
import { RegistrationRequest, SearchRequestsFilter } from '../types';

const SearchPage: React.FC = () => {
  const [filter, setFilter] = useState<SearchRequestsFilter>({
    query: '',
    status: '',
    fromDate: '',
    toDate: '',
  });
  const [results, setResults] = useState<RegistrationRequest[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [hasSearched, setHasSearched] = useState(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFilter((prev) => ({ ...prev, [name]: value }));
  };

  const handleSearch = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const data = await ApiService.searchRequests({
        query: filter.query?.trim() || undefined,
        status: filter.status || undefined,
        fromDate: filter.fromDate || undefined,
        toDate: filter.toDate || undefined,
      });
      setResults(data);
      setHasSearched(true);
    } catch (err) {
      setError('Die Suche konnte nicht ausgeführt werden.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

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
    <div className="page search-page">
      <div className="page-header">
        <div>
          <h1>Suche</h1>
          <p>Durchsuchen Sie Ihre Anträge nach Merkmalen und Status.</p>
        </div>
      </div>

      <form className="card search-filters" onSubmit={handleSearch}>
        <div className="search-grid">
          <div className="form-group">
            <label htmlFor="query">Suchbegriff</label>
            <input
              id="query"
              name="query"
              type="text"
              value={filter.query}
              onChange={handleChange}
              placeholder="FIN, Kennzeichen, Marke oder Modell"
            />
          </div>
          <div className="form-group">
            <label htmlFor="status">Status</label>
            <select id="status" name="status" value={filter.status} onChange={handleChange}>
              <option value="">Alle</option>
              <option value="Pending">Eingegangen</option>
              <option value="UnderReview">In Prüfung</option>
              <option value="Approved">Genehmigt</option>
              <option value="Rejected">Abgelehnt</option>
            </select>
          </div>
          <div className="form-group">
            <label htmlFor="fromDate">Von</label>
            <input
              id="fromDate"
              name="fromDate"
              type="date"
              value={filter.fromDate}
              onChange={handleChange}
            />
          </div>
          <div className="form-group">
            <label htmlFor="toDate">Bis</label>
            <input
              id="toDate"
              name="toDate"
              type="date"
              value={filter.toDate}
              onChange={handleChange}
            />
          </div>
        </div>
        <div className="form-footer">
          <button type="submit" className="primary-button" disabled={loading}>
            {loading ? 'Suche läuft...' : 'Suche starten'}
          </button>
        </div>
      </form>

      {error && <div className="error">{error}</div>}

      {hasSearched && results.length === 0 ? (
        <div className="card">Keine Treffer gefunden.</div>
      ) : (
        <div className="requests-list">
          {results.map((request) => (
            <div key={request.id} className="request-card">
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
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default SearchPage;

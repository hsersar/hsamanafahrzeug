import React from 'react';

const MitarbeiterPage: React.FC = () => {
  return (
    <div className="page admin-page">
      <div className="page-header">
        <div>
          <h1>Mitarbeiter-Inbox</h1>
          <p>Offene Anträge zur Prüfung (Mock-Daten).</p>
        </div>
      </div>
      <div className="requests-list">
        {['IKFZ-1032', 'IKFZ-1033', 'IKFZ-1034'].map((ref) => (
          <div key={ref} className="request-card">
            <div className="card-header">
              <div className="card-title">Antrag {ref}</div>
              <span className="status-pill-small">In Prüfung</span>
            </div>
            <div className="request-details">
              <p><strong>Antragsteller:</strong> Max M.</p>
              <p><strong>Fahrzeug:</strong> VW Golf, 2021</p>
              <p><strong>Eingang:</strong> 16.02.2026</p>
            </div>
            <div className="card-actions">
              <button className="secondary-button" type="button">Rückfrage</button>
              <button className="primary-button" type="button">Entscheiden</button>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default MitarbeiterPage;

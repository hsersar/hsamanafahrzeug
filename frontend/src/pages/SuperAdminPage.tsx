import React from 'react';

const SuperAdminPage: React.FC = () => {
  return (
    <div className="page admin-page">
      <div className="page-header">
        <div>
          <h1>Super-Admin Übersicht</h1>
          <p>Systemweite Kennzahlen und Steuerung (Mock-Daten).</p>
        </div>
      </div>
      <div className="page-grid admin-grid">
        <div className="card">
          <div className="card-title">Standorte</div>
          <div className="info-row">
            <span>Aktive Standorte</span>
            <strong>12</strong>
          </div>
          <div className="info-row">
            <span>Offene Audits</span>
            <strong>2</strong>
          </div>
        </div>
        <div className="card">
          <div className="card-title">Nutzer</div>
          <div className="info-row">
            <span>Gesamt</span>
            <strong>1.248</strong>
          </div>
          <div className="info-row">
            <span>Mitarbeiter</span>
            <strong>84</strong>
          </div>
        </div>
        <div className="card">
          <div className="card-title">Anträge</div>
          <div className="info-row">
            <span>Offen</span>
            <strong>31</strong>
          </div>
          <div className="info-row">
            <span>Genehmigt (30 Tage)</span>
            <strong>412</strong>
          </div>
        </div>
      </div>
      <div className="card">
        <div className="card-title">Letzte Aktivitäten</div>
        <div className="admin-list">
          <div>Standort Berlin – Mitarbeiterin zugewiesen</div>
          <div>Systemwartung – 17.02.2026 08:30</div>
          <div>Rollenreview – 12 Nutzer aktualisiert</div>
        </div>
      </div>
    </div>
  );
};

export default SuperAdminPage;

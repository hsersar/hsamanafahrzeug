import React from 'react';

const StandortAdminPage: React.FC = () => {
  return (
    <div className="page admin-page">
      <div className="page-header">
        <div>
          <h1>Standort-Admin</h1>
          <p>Übersicht für Ihren Standort (Mock-Daten).</p>
        </div>
      </div>
      <div className="page-grid admin-grid">
        <div className="card">
          <div className="card-title">Standortstatus</div>
          <div className="info-row">
            <span>Offene Anträge</span>
            <strong>9</strong>
          </div>
          <div className="info-row">
            <span>In Prüfung</span>
            <strong>4</strong>
          </div>
          <div className="info-row">
            <span>Durchschnittliche Bearbeitung</span>
            <strong>2,4 Tage</strong>
          </div>
        </div>
        <div className="card">
          <div className="card-title">Mitarbeitende</div>
          <div className="admin-list">
            <div>Anna K. – Sachbearbeitung</div>
            <div>David M. – Prüfung</div>
            <div>Yasmin R. – Kontrolle</div>
          </div>
        </div>
      </div>
      <div className="card">
        <div className="card-title">Auffälligkeiten</div>
        <div className="admin-list">
          <div>3 Anträge länger als 5 Tage offen</div>
          <div>2 Rückfragen ohne Antwort</div>
        </div>
      </div>
    </div>
  );
};

export default StandortAdminPage;

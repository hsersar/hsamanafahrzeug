import React from 'react';

const DatenschutzPage: React.FC = () => {
  return (
    <div className="page legal-page">
      <div className="page-header">
        <div>
          <h1>Datenschutz</h1>
          <p>Informationen zur Verarbeitung personenbezogener Daten.</p>
        </div>
      </div>
      <div className="card">
        <section className="legal-section">
          <h2>Verantwortliche Stelle</h2>
          <p>iKfz Beispiel GmbH, Musterstrasse 12, 10115 Berlin</p>
        </section>
        <section className="legal-section">
          <h2>Zwecke der Verarbeitung</h2>
          <ul>
            <li>Bearbeitung von Zulassungsanträgen</li>
            <li>Statuskommunikation und Rückfragen</li>
            <li>Erfüllung gesetzlicher Pflichten</li>
          </ul>
        </section>
        <section className="legal-section">
          <h2>Ihre Rechte</h2>
          <p>
            Sie haben das Recht auf Auskunft, Berichtigung, Löschung und
            Einschränkung der Verarbeitung Ihrer Daten.
          </p>
        </section>
      </div>
    </div>
  );
};

export default DatenschutzPage;

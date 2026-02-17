import React from 'react';

const AgbPage: React.FC = () => {
  return (
    <div className="page legal-page">
      <div className="page-header">
        <div>
          <h1>Allgemeine Geschäftsbedingungen (AGB)</h1>
          <p>Rahmenbedingungen für die Nutzung der iKfz-Online-Services.</p>
        </div>
      </div>
      <div className="card">
        <section className="legal-section">
          <h2>Geltungsbereich</h2>
          <p>
            Diese AGB gelten für die Nutzung der digitalen iKfz-Dienste. Mit der
            Nutzung erkennen Sie die Bedingungen an.
          </p>
        </section>
        <section className="legal-section">
          <h2>Leistungsbeschreibung</h2>
          <p>
            Der Dienst ermöglicht die digitale Antragstellung und Statusverfolgung
            von Fahrzeugzulassungen.
          </p>
        </section>
        <section className="legal-section">
          <h2>Pflichten der Nutzerinnen und Nutzer</h2>
          <ul>
            <li>Angaben müssen vollständig und korrekt sein.</li>
            <li>Zugangsdaten sind vertraulich zu behandeln.</li>
            <li>Missbrauch ist untersagt.</li>
          </ul>
        </section>
      </div>
    </div>
  );
};

export default AgbPage;

import React from 'react';

const ImpressumPage: React.FC = () => {
  return (
    <div className="page legal-page">
      <div className="page-header">
        <div>
          <h1>Impressum</h1>
          <p>Angaben gemäß § 5 TMG und Kontaktinformationen.</p>
        </div>
      </div>
      <div className="card">
        <section className="legal-section">
          <h2>Betreiber</h2>
          <p>iKfz Beispiel GmbH</p>
          <p>Musterstraße 12</p>
          <p>10115 Berlin</p>
        </section>
        <section className="legal-section">
          <h2>Kontakt</h2>
          <p>Telefon: +49 30 1234 5678</p>
          <p>E-Mail: kontakt@ikfz-beispiel.de</p>
        </section>
        <section className="legal-section">
          <h2>Vertretungsberechtigt</h2>
          <p>Geschäftsführung: Max Mustermann</p>
          <p>Handelsregister: HRB 123456 B, Amtsgericht Berlin-Charlottenburg</p>
        </section>
      </div>
    </div>
  );
};

export default ImpressumPage;

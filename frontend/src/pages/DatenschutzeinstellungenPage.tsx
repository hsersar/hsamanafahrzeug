import React from 'react';

const DatenschutzeinstellungenPage: React.FC = () => {
  return (
    <div className="page legal-page">
      <div className="page-header">
        <div>
          <h1>Datenschutzeinstellungen</h1>
          <p>Verwalten Sie Ihre Einwilligungen und Einstellungen.</p>
        </div>
      </div>
      <div className="card">
        <section className="legal-section">
          <h2>Notwendige Cookies</h2>
          <p>Diese Cookies sind für den Betrieb der Anwendung erforderlich.</p>
        </section>
        <section className="legal-section">
          <h2>Optionale Einstellungen</h2>
          <ul>
            <li>Analyse zur Verbesserung der Nutzererfahrung</li>
            <li>Personalisierung von Inhalten</li>
          </ul>
        </section>
        <section className="legal-section">
          <h2>Kontakt</h2>
          <p>Bei Fragen wenden Sie sich an datenschutz@ikfz-beispiel.de.</p>
        </section>
      </div>
    </div>
  );
};

export default DatenschutzeinstellungenPage;

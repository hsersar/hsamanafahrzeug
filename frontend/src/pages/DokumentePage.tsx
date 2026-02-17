import React from 'react';

interface Dokument {
  id: number;
  name: string;
  kategorie: string;
  datum: string;
  groesse: string;
}

const demoDokumente: Dokument[] = [
  {
    id: 1,
    name: 'Anleitung – Online-Zulassung Schritt für Schritt',
    kategorie: 'Anleitung',
    datum: '2026-01-15',
    groesse: '420 KB',
  },
  {
    id: 2,
    name: 'Vollmacht-Vorlage (blanko)',
    kategorie: 'Vorlage',
    datum: '2025-12-01',
    groesse: '85 KB',
  },
  {
    id: 3,
    name: 'SEPA-Lastschriftmandat für Kfz-Steuer',
    kategorie: 'Formular',
    datum: '2026-02-01',
    groesse: '112 KB',
  },
  {
    id: 4,
    name: 'Gebührenübersicht Zulassungsstelle 2026',
    kategorie: 'Information',
    datum: '2026-01-02',
    groesse: '240 KB',
  },
  {
    id: 5,
    name: 'Datenschutzerklärung iKfz-Portal',
    kategorie: 'Rechtliches',
    datum: '2025-11-20',
    groesse: '98 KB',
  },
  {
    id: 6,
    name: 'Checkliste – Unterlagen für die Erstzulassung',
    kategorie: 'Anleitung',
    datum: '2026-01-10',
    groesse: '65 KB',
  },
];

const kategorieIcon: Record<string, string> = {
  Anleitung: '📘',
  Vorlage: '📝',
  Formular: '📋',
  Information: 'ℹ️',
  Rechtliches: '⚖️',
};

const DokumentePage: React.FC = () => {
  return (
    <div className="page dokumente-page">
      <div className="page-header">
        <div>
          <h1>Dokumente</h1>
          <p>
            Vorlagen, Anleitungen und Formulare rund um die
            Fahrzeugzulassung.
          </p>
        </div>
      </div>

      <div className="page-grid">
        {demoDokumente.map((doc) => (
          <div key={doc.id} className="card">
            <div className="card-header">
              <div className="card-title">
                <span style={{ marginRight: 8 }}>
                  {kategorieIcon[doc.kategorie] || '📄'}
                </span>
                {doc.name}
              </div>
            </div>
            <div className="request-details">
              <p>
                <strong>Kategorie:</strong> {doc.kategorie}
              </p>
              <p>
                <strong>Datum:</strong>{' '}
                {new Date(doc.datum).toLocaleDateString('de-DE')}
              </p>
              <p>
                <strong>Größe:</strong> {doc.groesse}
              </p>
            </div>
            <div className="card-actions">
              <button className="secondary-button" type="button">
                Herunterladen
              </button>
            </div>
          </div>
        ))}
      </div>

      <div className="card">
        <div className="card-title">Hinweise</div>
        <div className="admin-list">
          <div>
            Alle Dokumente stehen im PDF-Format zum Download bereit.
          </div>
          <div>
            Im Produktivbetrieb werden die Dateien aus dem
            Dokumentenmanagementsystem geladen.
          </div>
          <div>
            Fehlende Unterlagen können Sie über die Hilfe-Seite anfordern.
          </div>
        </div>
      </div>
    </div>
  );
};

export default DokumentePage;

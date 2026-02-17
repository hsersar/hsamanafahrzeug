import React from 'react';

interface Bescheid {
  id: number;
  aktenzeichen: string;
  typ: string;
  datum: string;
  status: 'Zugestellt' | 'Ausstehend' | 'Abgelehnt';
  fahrzeug: string;
}

const demoBescheide: Bescheid[] = [
  {
    id: 1,
    aktenzeichen: 'IKFZ-2026-0412',
    typ: 'Zulassungsbescheid',
    datum: '2026-02-10',
    status: 'Zugestellt',
    fahrzeug: 'VW Golf – W-AB 1234',
  },
  {
    id: 2,
    aktenzeichen: 'IKFZ-2026-0398',
    typ: 'Abmeldebescheinigung',
    datum: '2026-01-22',
    status: 'Zugestellt',
    fahrzeug: 'BMW 3er – W-CD 5678',
  },
  {
    id: 3,
    aktenzeichen: 'IKFZ-2026-0455',
    typ: 'Zulassungsbescheid',
    datum: '2026-02-15',
    status: 'Ausstehend',
    fahrzeug: 'Audi A4 – (beantragt)',
  },
  {
    id: 4,
    aktenzeichen: 'IKFZ-2025-1187',
    typ: 'Ablehnungsbescheid',
    datum: '2025-11-03',
    status: 'Abgelehnt',
    fahrzeug: 'Opel Astra – W-EF 9012',
  },
];

const BescheidePage: React.FC = () => {
  const statusBadge = (status: Bescheid['status']) => {
    if (status === 'Zugestellt') return 'badge badge-success';
    if (status === 'Ausstehend') return 'badge';
    return 'badge badge-neutral';
  };

  return (
    <div className="page bescheide-page">
      <div className="page-header">
        <div>
          <h1>Bescheide</h1>
          <p>Entscheidungen und Dokumente zu Ihren Zulassungsvorgängen.</p>
        </div>
      </div>

      {demoBescheide.length === 0 ? (
        <div className="card">
          <p>Keine Bescheide vorhanden.</p>
        </div>
      ) : (
        <div className="requests-list">
          {demoBescheide.map((b) => (
            <div key={b.id} className="request-card">
              <div className="card-header">
                <div className="card-title">{b.aktenzeichen}</div>
                <span className={statusBadge(b.status)}>{b.status}</span>
              </div>
              <div className="request-details">
                <p>
                  <strong>Typ:</strong> {b.typ}
                </p>
                <p>
                  <strong>Fahrzeug:</strong> {b.fahrzeug}
                </p>
                <p>
                  <strong>Datum:</strong>{' '}
                  {new Date(b.datum).toLocaleDateString('de-DE')}
                </p>
              </div>
              <div className="card-actions">
                {b.status === 'Zugestellt' && (
                  <button className="secondary-button" type="button">
                    PDF herunterladen
                  </button>
                )}
                <button className="secondary-button" type="button">
                  Details anzeigen
                </button>
              </div>
            </div>
          ))}
        </div>
      )}

      <div className="card">
        <div className="card-title">Hinweise</div>
        <div className="admin-list">
          <div>
            Zugestellte Bescheide können als PDF heruntergeladen werden.
          </div>
          <div>
            Ausstehende Bescheide werden nach Bearbeitung automatisch
            bereitgestellt.
          </div>
          <div>
            Bei Fragen zu einem Bescheid nutzen Sie bitte die Hilfe-Seite oder
            das Kontaktformular.
          </div>
        </div>
      </div>
    </div>
  );
};

export default BescheidePage;

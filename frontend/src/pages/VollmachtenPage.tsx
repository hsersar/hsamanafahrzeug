import React, { useState } from 'react';

interface Vollmacht {
  id: number;
  bevollmaechtigter: string;
  zweck: string;
  gueltigBis: string;
  status: 'Aktiv' | 'Abgelaufen' | 'Widerrufen';
}

const demoVollmachten: Vollmacht[] = [
  {
    id: 1,
    bevollmaechtigter: 'Autohaus Schmidt GmbH',
    zweck: 'Zulassung und Abmeldung',
    gueltigBis: '2026-12-31',
    status: 'Aktiv',
  },
  {
    id: 2,
    bevollmaechtigter: 'Maria Mustermann',
    zweck: 'Abholung von Dokumenten',
    gueltigBis: '2026-06-30',
    status: 'Aktiv',
  },
  {
    id: 3,
    bevollmaechtigter: 'Kfz-Dienst Wuppertal',
    zweck: 'Wiederzulassung',
    gueltigBis: '2025-09-15',
    status: 'Abgelaufen',
  },
];

const VollmachtenPage: React.FC = () => {
  const [vollmachten, setVollmachten] = useState<Vollmacht[]>(demoVollmachten);
  const [showForm, setShowForm] = useState(false);
  const [formData, setFormData] = useState({
    bevollmaechtigter: '',
    zweck: 'Zulassung und Abmeldung',
    gueltigBis: '',
  });

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    setFormData((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handleAdd = (e: React.FormEvent) => {
    e.preventDefault();
    const newVollmacht: Vollmacht = {
      id: Date.now(),
      bevollmaechtigter: formData.bevollmaechtigter,
      zweck: formData.zweck,
      gueltigBis: formData.gueltigBis,
      status: 'Aktiv',
    };
    setVollmachten((prev) => [newVollmacht, ...prev]);
    setFormData({ bevollmaechtigter: '', zweck: 'Zulassung und Abmeldung', gueltigBis: '' });
    setShowForm(false);
  };

  const handleRevoke = (id: number) => {
    setVollmachten((prev) =>
      prev.map((v) => (v.id === id ? { ...v, status: 'Widerrufen' as const } : v))
    );
  };

  const statusBadgeClass = (status: Vollmacht['status']) => {
    if (status === 'Aktiv') return 'badge badge-success';
    if (status === 'Abgelaufen') return 'badge badge-neutral';
    return 'badge';
  };

  return (
    <div className="page vollmachten-page">
      <div className="page-header">
        <div>
          <h1>Vollmachten</h1>
          <p>
            Verwalten Sie Ihre erteilten Vollmachten für Fahrzeugzulassungen und
            verwandte Vorgänge.
          </p>
        </div>
        <button
          className="primary-button"
          onClick={() => setShowForm((prev) => !prev)}
        >
          {showForm ? 'Abbrechen' : 'Neue Vollmacht'}
        </button>
      </div>

      {showForm && (
        <div className="card">
          <div className="card-title">Neue Vollmacht erteilen</div>
          <form onSubmit={handleAdd} className="registration-form" style={{ marginTop: 12 }}>
            <div className="form-section">
              <div className="details-fields-grid">
                <div className="form-group">
                  <label htmlFor="bevollmaechtigter">Bevollmächtigte/r</label>
                  <input
                    id="bevollmaechtigter"
                    name="bevollmaechtigter"
                    value={formData.bevollmaechtigter}
                    onChange={handleChange}
                    placeholder="Name oder Firma"
                    required
                  />
                </div>
                <div className="form-group">
                  <label htmlFor="zweck">Zweck</label>
                  <select
                    id="zweck"
                    name="zweck"
                    value={formData.zweck}
                    onChange={handleChange}
                  >
                    <option>Zulassung und Abmeldung</option>
                    <option>Abholung von Dokumenten</option>
                    <option>Wiederzulassung</option>
                    <option>Umschreibung</option>
                    <option>Sonstiges</option>
                  </select>
                </div>
                <div className="form-group">
                  <label htmlFor="gueltigBis">Gültig bis</label>
                  <input
                    id="gueltigBis"
                    name="gueltigBis"
                    type="date"
                    value={formData.gueltigBis}
                    onChange={handleChange}
                    required
                  />
                </div>
              </div>
            </div>
            <div className="form-footer">
              <button type="submit" className="primary-button">
                Vollmacht erteilen
              </button>
            </div>
          </form>
        </div>
      )}

      {vollmachten.length === 0 ? (
        <div className="card">
          <p>Keine Vollmachten vorhanden.</p>
        </div>
      ) : (
        <div className="requests-list">
          {vollmachten.map((v) => (
            <div key={v.id} className="request-card">
              <div className="card-header">
                <div className="card-title">{v.bevollmaechtigter}</div>
                <span className={statusBadgeClass(v.status)}>{v.status}</span>
              </div>
              <div className="request-details">
                <p>
                  <strong>Zweck:</strong> {v.zweck}
                </p>
                <p>
                  <strong>Gültig bis:</strong>{' '}
                  {new Date(v.gueltigBis).toLocaleDateString('de-DE')}
                </p>
              </div>
              {v.status === 'Aktiv' && (
                <div className="card-actions">
                  <button
                    className="secondary-button"
                    type="button"
                    onClick={() => handleRevoke(v.id)}
                  >
                    Widerrufen
                  </button>
                </div>
              )}
            </div>
          ))}
        </div>
      )}

      <div className="card">
        <div className="card-title">Hinweise</div>
        <div className="admin-list">
          <div>
            Vollmachten berechtigen Dritte, in Ihrem Namen Zulassungsvorgänge
            durchzuführen.
          </div>
          <div>
            Nach Widerruf kann die bevollmächtigte Person keine weiteren Aktionen
            mehr ausführen.
          </div>
          <div>
            Im Produktivbetrieb wird die Vollmacht digital signiert und dem
            Zulassungssystem übermittelt.
          </div>
        </div>
      </div>
    </div>
  );
};

export default VollmachtenPage;

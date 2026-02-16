import React, { useState, useEffect } from 'react';
import { provisionsApi } from '../../services/provisionsApi';
import { monatsabrechnungApi } from '../../services/monatsabrechnungApi';
import type { ProvisionsModellDto, MonatsabrechnungDto, PlattformUmsatzDto } from '../../types/provision';

export const SuperAdminDashboard: React.FC = () => {
  const [standardModell, setStandardModell] = useState<ProvisionsModellDto | null>(null);
  const [abrechnungen, setAbrechnungen] = useState<MonatsabrechnungDto[]>([]);
  const [plattformUmsatz, setPlattformUmsatz] = useState<PlattformUmsatzDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      setError(null);

      // Load Standard-Provisionsmodell
      const modell = await provisionsApi.getStandard();
      setStandardModell(modell);

      // Load recent billings
      const allAbrechnungen = await monatsabrechnungApi.getAll();
      setAbrechnungen(allAbrechnungen);

      // Load platform revenue
      const currentYear = new Date().getFullYear();
      const umsatz = await monatsabrechnungApi.getPlattformUmsatz(currentYear);
      setPlattformUmsatz(umsatz);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  const createAllBillings = async () => {
    try {
      setError(null);
      const now = new Date();
      const jahr = now.getFullYear();
      const monat = now.getMonth() + 1;
      
      await monatsabrechnungApi.createAll(jahr, monat);
      await loadData(); // Reload data
      alert('Alle Monatsabrechnungen wurden erfolgreich erstellt!');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Fehler beim Erstellen der Abrechnungen');
    }
  };

  if (loading) {
    return (
      <div style={{ padding: '20px' }}>
        <h1>SuperAdmin Dashboard</h1>
        <p>Lädt Daten...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div style={{ padding: '20px' }}>
        <h1>SuperAdmin Dashboard</h1>
        <div style={{ color: 'red', padding: '10px', border: '1px solid red', borderRadius: '4px' }}>
          <strong>Fehler:</strong> {error}
        </div>
        <button onClick={loadData} style={{ marginTop: '10px' }}>Neu laden</button>
      </div>
    );
  }

  return (
    <div style={{ padding: '20px', fontFamily: 'Arial, sans-serif' }}>
      <h1>SuperAdmin Dashboard - Provisions- und Abrechnungsverwaltung</h1>

      {/* Standard-Provisionsmodell */}
      <section style={{ marginTop: '30px', padding: '15px', border: '1px solid #ddd', borderRadius: '8px' }}>
        <h2>📊 Standard-Provisionsmodell</h2>
        {standardModell && (
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(2, 1fr)', gap: '10px' }}>
            <div>
              <strong>Grundgebühr:</strong> {standardModell.monatlicheGrundgebuehr.toFixed(2)} €
            </div>
            <div>
              <strong>Provisionssatz:</strong> {standardModell.provisionsProzentsatz.toFixed(1)} %
            </div>
            <div>
              <strong>Gültig ab:</strong> {new Date(standardModell.gueltigAb).toLocaleDateString('de-DE')}
            </div>
            <div>
              <strong>Status:</strong> {standardModell.istAktiv ? '✅ Aktiv' : '❌ Inaktiv'}
            </div>
          </div>
        )}
      </section>

      {/* Plattform-Umsatz */}
      {plattformUmsatz && (
        <section style={{ marginTop: '30px', padding: '15px', border: '1px solid #ddd', borderRadius: '8px' }}>
          <h2>💰 Plattform-Umsatz {plattformUmsatz.jahr}</h2>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: '15px', marginTop: '15px' }}>
            <div style={{ padding: '15px', backgroundColor: '#e3f2fd', borderRadius: '8px' }}>
              <div style={{ fontSize: '14px', color: '#666' }}>Standorte</div>
              <div style={{ fontSize: '24px', fontWeight: 'bold' }}>
                {plattformUmsatz.anzahlAktiveStandorte} / {plattformUmsatz.anzahlStandorte}
              </div>
            </div>
            <div style={{ padding: '15px', backgroundColor: '#e8f5e9', borderRadius: '8px' }}>
              <div style={{ fontSize: '14px', color: '#666' }}>Gesamt-Umsatz</div>
              <div style={{ fontSize: '24px', fontWeight: 'bold' }}>
                {plattformUmsatz.gesamtUmsatzAllerStandorte.toFixed(2)} €
              </div>
            </div>
            <div style={{ padding: '15px', backgroundColor: '#fff3e0', borderRadius: '8px' }}>
              <div style={{ fontSize: '14px', color: '#666' }}>Plattform-Einnahmen</div>
              <div style={{ fontSize: '24px', fontWeight: 'bold' }}>
                {plattformUmsatz.gesamtPlattformEinnahmen.toFixed(2)} €
              </div>
            </div>
            <div style={{ padding: '15px', backgroundColor: '#fce4ec', borderRadius: '8px' }}>
              <div style={{ fontSize: '14px', color: '#666' }}>Grundgebühren</div>
              <div style={{ fontSize: '24px', fontWeight: 'bold' }}>
                {plattformUmsatz.gesamtGrundgebuehren.toFixed(2)} €
              </div>
            </div>
            <div style={{ padding: '15px', backgroundColor: '#f3e5f5', borderRadius: '8px' }}>
              <div style={{ fontSize: '14px', color: '#666' }}>Provisionen</div>
              <div style={{ fontSize: '24px', fontWeight: 'bold' }}>
                {plattformUmsatz.gesamtProvisionen.toFixed(2)} €
              </div>
            </div>
            <div style={{ padding: '15px', backgroundColor: '#e0f2f1', borderRadius: '8px' }}>
              <div style={{ fontSize: '14px', color: '#666' }}>Bezahlter Gesamtbetrag</div>
              <div style={{ fontSize: '24px', fontWeight: 'bold' }}>
                {plattformUmsatz.bezahlteAbrechnungen.toFixed(2)} €
              </div>
            </div>
          </div>
        </section>
      )}

      {/* Monatsabrechnungen */}
      <section style={{ marginTop: '30px', padding: '15px', border: '1px solid #ddd', borderRadius: '8px' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2>📋 Monatsabrechnungen ({abrechnungen.length})</h2>
          <button 
            onClick={createAllBillings}
            style={{
              padding: '10px 20px',
              backgroundColor: '#1976d2',
              color: 'white',
              border: 'none',
              borderRadius: '4px',
              cursor: 'pointer'
            }}
          >
            Alle Abrechnungen erstellen
          </button>
        </div>
        
        {abrechnungen.length === 0 ? (
          <p style={{ marginTop: '15px', color: '#666' }}>Keine Abrechnungen vorhanden</p>
        ) : (
          <table style={{ width: '100%', marginTop: '15px', borderCollapse: 'collapse' }}>
            <thead>
              <tr style={{ backgroundColor: '#f5f5f5' }}>
                <th style={{ padding: '10px', textAlign: 'left', borderBottom: '2px solid #ddd' }}>Nr.</th>
                <th style={{ padding: '10px', textAlign: 'left', borderBottom: '2px solid #ddd' }}>Standort</th>
                <th style={{ padding: '10px', textAlign: 'left', borderBottom: '2px solid #ddd' }}>Zeitraum</th>
                <th style={{ padding: '10px', textAlign: 'right', borderBottom: '2px solid #ddd' }}>Netto</th>
                <th style={{ padding: '10px', textAlign: 'right', borderBottom: '2px solid #ddd' }}>Brutto</th>
                <th style={{ padding: '10px', textAlign: 'center', borderBottom: '2px solid #ddd' }}>Status</th>
              </tr>
            </thead>
            <tbody>
              {abrechnungen.slice(0, 10).map((abr) => (
                <tr key={abr.id} style={{ borderBottom: '1px solid #eee' }}>
                  <td style={{ padding: '10px' }}>{abr.abrechnungsNummer}</td>
                  <td style={{ padding: '10px' }}>{abr.standortName}</td>
                  <td style={{ padding: '10px' }}>{abr.zeitraum}</td>
                  <td style={{ padding: '10px', textAlign: 'right' }}>{abr.nettobetrag.toFixed(2)} €</td>
                  <td style={{ padding: '10px', textAlign: 'right' }}>{abr.bruttobetrag.toFixed(2)} €</td>
                  <td style={{ padding: '10px', textAlign: 'center' }}>
                    <span style={{
                      padding: '4px 8px',
                      borderRadius: '4px',
                      backgroundColor: abr.status === 3 ? '#4caf50' : '#ff9800',
                      color: 'white',
                      fontSize: '12px'
                    }}>
                      {['Entwurf', 'Erstellt', 'Versandt', 'Bezahlt', 'Überfällig', 'Storniert', 'Reklamiert'][abr.status]}
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>

      <div style={{ marginTop: '30px', padding: '15px', backgroundColor: '#f5f5f5', borderRadius: '8px' }}>
        <h3>ℹ️ System-Information</h3>
        <p><strong>Backend API:</strong> http://localhost:5000/api</p>
        <p><strong>Dokumentation:</strong> Siehe backend/README.md</p>
        <p><strong>Version:</strong> 1.0.0-demo</p>
      </div>
    </div>
  );
};

import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { RechnungDto } from '../types';
import { rechnungService } from '../services/api';

const RechnungsListe: React.FC = () => {
    const [rechnungen, setRechnungen] = useState<RechnungDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        loadRechnungen();
    }, []);

    const loadRechnungen = async () => {
        try {
            const data = await rechnungService.getRechnungen();
            setRechnungen(data);
        } catch (err) {
            setError('Fehler beim Laden der Rechnungen');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const handleDownloadPdf = async (id: string, rechnungsNummer: string) => {
        try {
            const blob = await rechnungService.downloadPdf(id);
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `${rechnungsNummer}.pdf`;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);
        } catch (err) {
            alert('Fehler beim Download der PDF');
            console.error(err);
        }
    };

    const handleDownloadXml = async (id: string, rechnungsNummer: string) => {
        try {
            const blob = await rechnungService.downloadXml(id);
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `${rechnungsNummer}.xml`;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);
        } catch (err) {
            alert('Fehler beim Download des XML');
            console.error(err);
        }
    };

    if (loading) return <div style={{ textAlign: 'center', padding: '40px' }}>Laden...</div>;
    if (error) return <div style={{ color: 'red', padding: '20px' }}>{error}</div>;

    return (
        <div style={{ maxWidth: '1200px', margin: '0 auto', padding: '20px' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
                <h1>Rechnungen</h1>
                <Link 
                    to="/rechnungen/neu"
                    style={{ 
                        padding: '12px 24px', 
                        backgroundColor: '#28a745', 
                        color: 'white', 
                        textDecoration: 'none',
                        borderRadius: '4px'
                    }}
                >
                    + Neue Rechnung
                </Link>
            </div>

            {rechnungen.length === 0 ? (
                <div style={{ textAlign: 'center', padding: '40px', color: '#666' }}>
                    Keine Rechnungen vorhanden. Erstellen Sie eine neue Rechnung.
                </div>
            ) : (
                <table style={{ width: '100%', borderCollapse: 'collapse', backgroundColor: 'white' }}>
                    <thead>
                        <tr style={{ borderBottom: '2px solid #333', backgroundColor: '#f8f9fa' }}>
                            <th style={{ textAlign: 'left', padding: '12px' }}>Nr.</th>
                            <th style={{ textAlign: 'left', padding: '12px' }}>Kunde</th>
                            <th style={{ textAlign: 'left', padding: '12px' }}>Erstellt</th>
                            <th style={{ textAlign: 'left', padding: '12px' }}>Fällig</th>
                            <th style={{ textAlign: 'right', padding: '12px' }}>Brutto</th>
                            <th style={{ textAlign: 'center', padding: '12px' }}>ZUGFeRD</th>
                            <th style={{ textAlign: 'center', padding: '12px' }}>Aktionen</th>
                        </tr>
                    </thead>
                    <tbody>
                        {rechnungen.map(rechnung => (
                            <tr key={rechnung.id} style={{ borderBottom: '1px solid #dee2e6' }}>
                                <td style={{ padding: '12px' }}>
                                    <strong>{rechnung.rechnungsNummer}</strong>
                                </td>
                                <td style={{ padding: '12px' }}>
                                    {rechnung.kundeName}
                                </td>
                                <td style={{ padding: '12px' }}>
                                    {new Date(rechnung.erstelltAm).toLocaleDateString('de-DE')}
                                </td>
                                <td style={{ padding: '12px' }}>
                                    {new Date(rechnung.faelligkeitsdatum).toLocaleDateString('de-DE')}
                                </td>
                                <td style={{ textAlign: 'right', padding: '12px' }}>
                                    <strong>{rechnung.bruttobetrag.toFixed(2)} €</strong>
                                </td>
                                <td style={{ textAlign: 'center', padding: '12px' }}>
                                    {rechnung.hatZUGFeRDXml && (
                                        <span 
                                            style={{ 
                                                backgroundColor: '#28a745', 
                                                color: 'white', 
                                                padding: '4px 8px', 
                                                borderRadius: '4px',
                                                fontSize: '12px'
                                            }}
                                        >
                                            ✓ E-Rechnung
                                        </span>
                                    )}
                                </td>
                                <td style={{ textAlign: 'center', padding: '12px' }}>
                                    <div style={{ display: 'flex', gap: '8px', justifyContent: 'center' }}>
                                        <button 
                                            onClick={() => handleDownloadPdf(rechnung.id, rechnung.rechnungsNummer)}
                                            style={{ 
                                                padding: '6px 12px', 
                                                backgroundColor: '#007bff', 
                                                color: 'white', 
                                                border: 'none', 
                                                borderRadius: '4px',
                                                cursor: 'pointer'
                                            }}
                                        >
                                            📄 PDF
                                        </button>
                                        {rechnung.hatZUGFeRDXml && (
                                            <button 
                                                onClick={() => handleDownloadXml(rechnung.id, rechnung.rechnungsNummer)}
                                                style={{ 
                                                    padding: '6px 12px', 
                                                    backgroundColor: '#17a2b8', 
                                                    color: 'white', 
                                                    border: 'none', 
                                                    borderRadius: '4px',
                                                    cursor: 'pointer'
                                                }}
                                            >
                                                📋 XML
                                            </button>
                                        )}
                                    </div>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
};

export default RechnungsListe;

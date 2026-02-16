import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { RechnungsPositionDto, RechnungFormat, KundeDto, StandortDto } from '../../types';
import RechnungsPositionen from './RechnungsPositionen';
import { rechnungService, kundenService, standortService } from '../../services/api';

const RechnungErstellen: React.FC = () => {
    const navigate = useNavigate();
    const [kunden, setKunden] = useState<KundeDto[]>([]);
    const [standorte, setStandorte] = useState<StandortDto[]>([]);
    const [kundeId, setKundeId] = useState('');
    const [standortId, setStandortId] = useState('');
    const [format, setFormat] = useState<RechnungFormat>(RechnungFormat.ZUGFeRD_Comfort);
    const [positionen, setPositionen] = useState<RechnungsPositionDto[]>([
        { beschreibung: 'Fahrzeuganmeldung', menge: 1, einzelpreis: 100, steuersatz: 19 }
    ]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        loadData();
    }, []);

    const loadData = async () => {
        try {
            const [kundenData, standorteData] = await Promise.all([
                kundenService.getKunden(),
                standortService.getStandorte()
            ]);
            setKunden(kundenData);
            setStandorte(standorteData);
            
            if (kundenData.length > 0) setKundeId(kundenData[0].id);
            if (standorteData.length > 0) setStandortId(standorteData[0].id);
        } catch (err) {
            setError('Fehler beim Laden der Daten');
            console.error(err);
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        
        if (!kundeId || !standortId || positionen.length === 0) {
            setError('Bitte alle Felder ausfüllen und mindestens eine Position hinzufügen');
            return;
        }

        setLoading(true);
        setError(null);

        try {
            const rechnung = await rechnungService.createRechnung({
                kundeId,
                standortId,
                format,
                positionen
            });

            alert(`Rechnung ${rechnung.rechnungsNummer} erfolgreich erstellt!`);
            navigate('/rechnungen');
        } catch (err: any) {
            setError(err.response?.data?.message || 'Fehler beim Erstellen der Rechnung');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{ maxWidth: '1200px', margin: '0 auto', padding: '20px' }}>
            <h1>Rechnung erstellen</h1>

            {error && (
                <div style={{ 
                    padding: '12px', 
                    backgroundColor: '#f8d7da', 
                    color: '#721c24', 
                    borderRadius: '4px',
                    marginBottom: '16px'
                }}>
                    {error}
                </div>
            )}

            <form onSubmit={handleSubmit}>
                <div style={{ marginBottom: '24px' }}>
                    <h3>Rechnungsdaten</h3>
                    
                    <div style={{ marginBottom: '16px' }}>
                        <label style={{ display: 'block', marginBottom: '4px', fontWeight: 'bold' }}>
                            Kunde:
                        </label>
                        <select 
                            value={kundeId}
                            onChange={(e) => setKundeId(e.target.value)}
                            required
                            style={{ width: '100%', padding: '8px', fontSize: '16px' }}
                        >
                            <option value="">-- Kunde wählen --</option>
                            {kunden.map(kunde => (
                                <option key={kunde.id} value={kunde.id}>
                                    {kunde.vorname} {kunde.nachname} ({kunde.email})
                                </option>
                            ))}
                        </select>
                    </div>

                    <div style={{ marginBottom: '16px' }}>
                        <label style={{ display: 'block', marginBottom: '4px', fontWeight: 'bold' }}>
                            Standort:
                        </label>
                        <select 
                            value={standortId}
                            onChange={(e) => setStandortId(e.target.value)}
                            required
                            style={{ width: '100%', padding: '8px', fontSize: '16px' }}
                        >
                            <option value="">-- Standort wählen --</option>
                            {standorte.map(standort => (
                                <option key={standort.id} value={standort.id}>
                                    {standort.name} - {standort.firmenname}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div style={{ marginBottom: '16px' }}>
                        <label style={{ display: 'block', marginBottom: '4px', fontWeight: 'bold' }}>
                            ZUGFeRD Profil:
                        </label>
                        <select 
                            value={format}
                            onChange={(e) => setFormat(parseInt(e.target.value) as RechnungFormat)}
                            style={{ width: '100%', padding: '8px', fontSize: '16px' }}
                        >
                            <option value={RechnungFormat.ZUGFeRD_Minimum}>Minimum</option>
                            <option value={RechnungFormat.ZUGFeRD_BasicWL}>Basic WL</option>
                            <option value={RechnungFormat.ZUGFeRD_Basic}>Basic</option>
                            <option value={RechnungFormat.ZUGFeRD_Comfort}>Comfort (EN16931) - Empfohlen</option>
                            <option value={RechnungFormat.ZUGFeRD_Extended}>Extended</option>
                        </select>
                    </div>
                </div>

                <RechnungsPositionen 
                    positionen={positionen}
                    onChange={setPositionen}
                />

                <div style={{ marginTop: '24px', display: 'flex', gap: '12px' }}>
                    <button 
                        type="submit"
                        disabled={loading}
                        style={{ 
                            padding: '12px 24px', 
                            backgroundColor: '#28a745', 
                            color: 'white', 
                            border: 'none', 
                            borderRadius: '4px',
                            fontSize: '16px',
                            cursor: loading ? 'not-allowed' : 'pointer'
                        }}
                    >
                        {loading ? 'Erstelle...' : 'Rechnung erstellen'}
                    </button>

                    <button 
                        type="button"
                        onClick={() => navigate('/rechnungen')}
                        style={{ 
                            padding: '12px 24px', 
                            backgroundColor: '#6c757d', 
                            color: 'white', 
                            border: 'none', 
                            borderRadius: '4px',
                            fontSize: '16px',
                            cursor: 'pointer'
                        }}
                    >
                        Abbrechen
                    </button>
                </div>
            </form>
        </div>
    );
};

export default RechnungErstellen;

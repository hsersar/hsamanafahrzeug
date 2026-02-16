import React from 'react';
import { RechnungsPositionDto } from '../../types';

interface Props {
    positionen: RechnungsPositionDto[];
    onChange: (positionen: RechnungsPositionDto[]) => void;
}

const RechnungsPositionen: React.FC<Props> = ({ positionen, onChange }) => {
    const handleAdd = () => {
        onChange([
            ...positionen,
            { beschreibung: '', menge: 1, einzelpreis: 0, steuersatz: 19 }
        ]);
    };

    const handleRemove = (index: number) => {
        onChange(positionen.filter((_, i) => i !== index));
    };

    const handleChange = (index: number, field: keyof RechnungsPositionDto, value: any) => {
        const updated = [...positionen];
        updated[index] = { ...updated[index], [field]: value };
        onChange(updated);
    };

    const berechneNetto = (pos: RechnungsPositionDto) => pos.menge * pos.einzelpreis;
    const berechneSteuer = (pos: RechnungsPositionDto) => berechneNetto(pos) * (pos.steuersatz / 100);
    const berechneBrutto = (pos: RechnungsPositionDto) => berechneNetto(pos) + berechneSteuer(pos);

    const gesamtNetto = positionen.reduce((sum, pos) => sum + berechneNetto(pos), 0);
    const gesamtSteuer = positionen.reduce((sum, pos) => sum + berechneSteuer(pos), 0);
    const gesamtBrutto = gesamtNetto + gesamtSteuer;

    return (
        <div className="rechnungs-positionen">
            <h3>Rechnungspositionen</h3>
            
            <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                <thead>
                    <tr style={{ borderBottom: '2px solid #333' }}>
                        <th style={{ textAlign: 'left', padding: '8px' }}>Beschreibung</th>
                        <th style={{ textAlign: 'right', padding: '8px' }}>Menge</th>
                        <th style={{ textAlign: 'right', padding: '8px' }}>Einzelpreis (€)</th>
                        <th style={{ textAlign: 'right', padding: '8px' }}>MwSt (%)</th>
                        <th style={{ textAlign: 'right', padding: '8px' }}>Netto (€)</th>
                        <th style={{ textAlign: 'right', padding: '8px' }}>Steuer (€)</th>
                        <th style={{ textAlign: 'right', padding: '8px' }}>Brutto (€)</th>
                        <th style={{ padding: '8px' }}></th>
                    </tr>
                </thead>
                <tbody>
                    {positionen.map((pos, index) => (
                        <tr key={index} style={{ borderBottom: '1px solid #ddd' }}>
                            <td style={{ padding: '8px' }}>
                                <input
                                    type="text"
                                    value={pos.beschreibung}
                                    onChange={(e) => handleChange(index, 'beschreibung', e.target.value)}
                                    placeholder="Beschreibung"
                                    style={{ width: '100%', padding: '4px' }}
                                />
                            </td>
                            <td style={{ padding: '8px' }}>
                                <input
                                    type="number"
                                    value={pos.menge}
                                    onChange={(e) => handleChange(index, 'menge', parseFloat(e.target.value))}
                                    min="0"
                                    step="0.01"
                                    style={{ width: '80px', textAlign: 'right', padding: '4px' }}
                                />
                            </td>
                            <td style={{ padding: '8px' }}>
                                <input
                                    type="number"
                                    value={pos.einzelpreis}
                                    onChange={(e) => handleChange(index, 'einzelpreis', parseFloat(e.target.value))}
                                    min="0"
                                    step="0.01"
                                    style={{ width: '100px', textAlign: 'right', padding: '4px' }}
                                />
                            </td>
                            <td style={{ padding: '8px' }}>
                                <select
                                    value={pos.steuersatz}
                                    onChange={(e) => handleChange(index, 'steuersatz', parseFloat(e.target.value))}
                                    style={{ width: '80px', padding: '4px' }}
                                >
                                    <option value="0">0%</option>
                                    <option value="7">7%</option>
                                    <option value="19">19%</option>
                                </select>
                            </td>
                            <td style={{ textAlign: 'right', padding: '8px' }}>
                                {berechneNetto(pos).toFixed(2)}
                            </td>
                            <td style={{ textAlign: 'right', padding: '8px' }}>
                                {berechneSteuer(pos).toFixed(2)}
                            </td>
                            <td style={{ textAlign: 'right', padding: '8px' }}>
                                {berechneBrutto(pos).toFixed(2)}
                            </td>
                            <td style={{ padding: '8px' }}>
                                <button onClick={() => handleRemove(index)} style={{ color: 'red' }}>
                                    ✕
                                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
                <tfoot>
                    <tr style={{ borderTop: '2px solid #333', fontWeight: 'bold' }}>
                        <td colSpan={4} style={{ textAlign: 'right', padding: '8px' }}>Summe:</td>
                        <td style={{ textAlign: 'right', padding: '8px' }}>{gesamtNetto.toFixed(2)} €</td>
                        <td style={{ textAlign: 'right', padding: '8px' }}>{gesamtSteuer.toFixed(2)} €</td>
                        <td style={{ textAlign: 'right', padding: '8px' }}>{gesamtBrutto.toFixed(2)} €</td>
                        <td></td>
                    </tr>
                </tfoot>
            </table>

            <button 
                onClick={handleAdd}
                style={{ 
                    marginTop: '16px', 
                    padding: '8px 16px', 
                    backgroundColor: '#007bff', 
                    color: 'white', 
                    border: 'none', 
                    borderRadius: '4px',
                    cursor: 'pointer'
                }}
            >
                + Position hinzufügen
            </button>
        </div>
    );
};

export default RechnungsPositionen;

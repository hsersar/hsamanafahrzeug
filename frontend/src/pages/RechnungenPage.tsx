import React, { useEffect, useState } from 'react';
import ApiService from '../services/api';
import { Invoice } from '../types';

const STATUS_MAP: Record<string, { label: string; className: string }> = {
  Offen: { label: 'Offen', className: 'invoice-status--offen' },
  Bezahlt: { label: 'Bezahlt', className: 'invoice-status--bezahlt' },
  Ueberfaellig: { label: 'Überfällig', className: 'invoice-status--ueberfaellig' },
  Storniert: { label: 'Storniert', className: 'invoice-status--storniert' },
};

const RechnungenPage: React.FC = () => {
  const [invoices, setInvoices] = useState<Invoice[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filter, setFilter] = useState<string>('alle');

  useEffect(() => {
    const load = async () => {
      try {
        setLoading(true);
        const data = await ApiService.getMyInvoices();
        setInvoices(data);
        setError(null);
      } catch {
        setError('Rechnungen konnten nicht geladen werden.');
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  const formatCurrency = (amount: number) =>
    new Intl.NumberFormat('de-DE', { style: 'currency', currency: 'EUR' }).format(amount);

  const formatDate = (dateStr: string) =>
    new Date(dateStr).toLocaleDateString('de-DE', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
    });

  const filtered = filter === 'alle'
    ? invoices
    : invoices.filter((i) => i.status === filter);

  const totals = {
    offen: invoices.filter((i) => i.status === 'Offen').reduce((s, i) => s + i.bruttobetrag, 0),
    bezahlt: invoices.filter((i) => i.status === 'Bezahlt').reduce((s, i) => s + i.bruttobetrag, 0),
    ueberfaellig: invoices.filter((i) => i.status === 'Ueberfaellig').reduce((s, i) => s + i.bruttobetrag, 0),
  };

  if (loading) {
    return (
      <div className="page rechnungen-page">
        <div className="loading-spinner">Rechnungen werden geladen…</div>
      </div>
    );
  }

  return (
    <div className="page rechnungen-page">
      <div className="page-header">
        <h1>Rechnungen</h1>
        <p>Übersicht Ihrer Rechnungen und Zahlungen.</p>
      </div>

      {error && (
        <div className="alert alert-error" role="alert">
          <span className="alert-icon">!</span> {error}
        </div>
      )}

      {/* Summary cards */}
      <div className="invoice-summary-grid">
        <div className="invoice-summary-card invoice-summary--offen">
          <span className="invoice-summary-icon">📋</span>
          <div className="invoice-summary-info">
            <span className="invoice-summary-label">Offene Rechnungen</span>
            <span className="invoice-summary-value">{formatCurrency(totals.offen)}</span>
            <span className="invoice-summary-count">
              {invoices.filter((i) => i.status === 'Offen').length} Rechnung(en)
            </span>
          </div>
        </div>
        <div className="invoice-summary-card invoice-summary--bezahlt">
          <span className="invoice-summary-icon">✅</span>
          <div className="invoice-summary-info">
            <span className="invoice-summary-label">Bezahlt</span>
            <span className="invoice-summary-value">{formatCurrency(totals.bezahlt)}</span>
            <span className="invoice-summary-count">
              {invoices.filter((i) => i.status === 'Bezahlt').length} Rechnung(en)
            </span>
          </div>
        </div>
        <div className="invoice-summary-card invoice-summary--ueberfaellig">
          <span className="invoice-summary-icon">⚠️</span>
          <div className="invoice-summary-info">
            <span className="invoice-summary-label">Überfällig</span>
            <span className="invoice-summary-value">{formatCurrency(totals.ueberfaellig)}</span>
            <span className="invoice-summary-count">
              {invoices.filter((i) => i.status === 'Ueberfaellig').length} Rechnung(en)
            </span>
          </div>
        </div>
      </div>

      {/* Filter tabs */}
      <div className="invoice-filter-bar">
        {[
          { key: 'alle', label: 'Alle' },
          { key: 'Offen', label: 'Offen' },
          { key: 'Bezahlt', label: 'Bezahlt' },
          { key: 'Ueberfaellig', label: 'Überfällig' },
          { key: 'Storniert', label: 'Storniert' },
        ].map((f) => (
          <button
            key={f.key}
            className={`invoice-filter-tab ${filter === f.key ? 'active' : ''}`}
            onClick={() => setFilter(f.key)}
          >
            {f.label}
            {f.key !== 'alle' && (
              <span className="invoice-filter-count">
                {invoices.filter(
                  (i) => i.status === f.key
                ).length}
              </span>
            )}
          </button>
        ))}
      </div>

      {/* Invoice table */}
      {filtered.length === 0 ? (
        <div className="invoice-empty-state">
          <span className="invoice-empty-icon">📄</span>
          <h3>Keine Rechnungen vorhanden</h3>
          <p>
            {filter === 'alle'
              ? 'Es liegen derzeit keine Rechnungen vor.'
              : `Keine Rechnungen mit Status „${STATUS_MAP[filter]?.label || filter}" gefunden.`}
          </p>
        </div>
      ) : (
        <div className="invoice-table-wrapper">
          <table className="invoice-table">
            <thead>
              <tr>
                <th>Rechnungsnr.</th>
                <th>Beschreibung</th>
                <th>Datum</th>
                <th>Fällig am</th>
                <th>Betrag</th>
                <th>Status</th>
                <th>Aktionen</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map((inv) => {
                const st = STATUS_MAP[inv.status] || {
                  label: inv.status,
                  className: '',
                };
                return (
                  <tr key={inv.id}>
                    <td className="mono">{inv.rechnungsnummer}</td>
                    <td>{inv.beschreibung}</td>
                    <td>{formatDate(inv.rechnungsdatum)}</td>
                    <td>{formatDate(inv.faelligkeitsdatum)}</td>
                    <td className="invoice-amount">{formatCurrency(inv.bruttobetrag)}</td>
                    <td>
                      <span className={`invoice-status-badge ${st.className}`}>
                        {st.label}
                      </span>
                    </td>
                    <td>
                      <button className="btn-link" title="Herunterladen">
                        📥 PDF
                      </button>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

export default RechnungenPage;

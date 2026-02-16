import type {
  MonatsabrechnungDto,
  AbrechnungErstellenRequest,
  MonatsabrechnungFilter,
  PlattformUmsatzDto,
  ZahlungsBestaetigung,
  StornierungsRequest
} from '../types/provision';

const API_BASE_URL = 'http://localhost:5000/api';

export const monatsabrechnungApi = {
  // Abrechnungen erstellen
  async create(data: AbrechnungErstellenRequest): Promise<MonatsabrechnungDto> {
    const response = await fetch(`${API_BASE_URL}/monatsabrechnungen/erstellen`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data)
    });
    if (!response.ok) throw new Error('Failed to create monthly billing');
    return response.json();
  },

  async createAll(jahr: number, monat: number): Promise<MonatsabrechnungDto[]> {
    const response = await fetch(
      `${API_BASE_URL}/monatsabrechnungen/alle-erstellen?jahr=${jahr}&monat=${monat}`,
      { method: 'POST' }
    );
    if (!response.ok) throw new Error('Failed to create all monthly billings');
    return response.json();
  },

  // Abrechnungen abrufen
  async getAll(filter?: MonatsabrechnungFilter): Promise<MonatsabrechnungDto[]> {
    const params = new URLSearchParams();
    if (filter?.standortId) params.append('standortId', filter.standortId);
    if (filter?.jahr) params.append('jahr', filter.jahr.toString());
    if (filter?.monat) params.append('monat', filter.monat.toString());
    if (filter?.status !== undefined) params.append('status', filter.status.toString());

    const response = await fetch(`${API_BASE_URL}/monatsabrechnungen?${params}`);
    if (!response.ok) throw new Error('Failed to fetch monthly billings');
    return response.json();
  },

  async getById(id: string): Promise<MonatsabrechnungDto> {
    const response = await fetch(`${API_BASE_URL}/monatsabrechnungen/${id}`);
    if (!response.ok) throw new Error('Failed to fetch monthly billing');
    return response.json();
  },

  async getForStandort(standortId: string, jahr?: number): Promise<MonatsabrechnungDto[]> {
    const params = jahr ? `?jahr=${jahr}` : '';
    const response = await fetch(`${API_BASE_URL}/monatsabrechnungen/standort/${standortId}${params}`);
    if (!response.ok) throw new Error('Failed to fetch billings for location');
    return response.json();
  },

  // Aktionen
  async send(id: string): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/monatsabrechnungen/${id}/versenden`, {
      method: 'POST'
    });
    if (!response.ok) throw new Error('Failed to send billing');
  },

  async markAsPaid(id: string, data: ZahlungsBestaetigung): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/monatsabrechnungen/${id}/bezahlt`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data)
    });
    if (!response.ok) throw new Error('Failed to mark billing as paid');
  },

  async cancel(id: string, data: StornierungsRequest): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/monatsabrechnungen/${id}/stornieren`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data)
    });
    if (!response.ok) throw new Error('Failed to cancel billing');
  },

  // Export
  async downloadPdf(id: string): Promise<Blob> {
    const response = await fetch(`${API_BASE_URL}/monatsabrechnungen/${id}/pdf`);
    if (!response.ok) throw new Error('Failed to download PDF');
    return response.blob();
  },

  async exportCsv(filter?: MonatsabrechnungFilter): Promise<Blob> {
    const params = new URLSearchParams();
    if (filter?.standortId) params.append('standortId', filter.standortId);
    if (filter?.jahr) params.append('jahr', filter.jahr.toString());
    if (filter?.monat) params.append('monat', filter.monat.toString());
    if (filter?.status !== undefined) params.append('status', filter.status.toString());

    const response = await fetch(`${API_BASE_URL}/monatsabrechnungen/export/csv?${params}`);
    if (!response.ok) throw new Error('Failed to export CSV');
    return response.blob();
  },

  // Dashboard
  async getPlattformUmsatz(jahr: number, monat?: number): Promise<PlattformUmsatzDto> {
    const params = monat ? `&monat=${monat}` : '';
    const response = await fetch(`${API_BASE_URL}/monatsabrechnungen/plattform-umsatz?jahr=${jahr}${params}`);
    if (!response.ok) throw new Error('Failed to fetch platform revenue');
    return response.json();
  }
};

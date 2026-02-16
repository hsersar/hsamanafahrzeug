import type {
  ProvisionsModellDto,
  ProvisionsModellUpdateDto,
  ProvisionsBerechnung
} from '../types/provision';

const API_BASE_URL = 'http://localhost:5000/api';

export const provisionsApi = {
  // Standard-Provisionsmodell
  async getStandard(): Promise<ProvisionsModellDto> {
    const response = await fetch(`${API_BASE_URL}/provisionen/standard`);
    if (!response.ok) throw new Error('Failed to fetch standard commission model');
    return response.json();
  },

  async setStandard(data: ProvisionsModellUpdateDto): Promise<ProvisionsModellDto> {
    const response = await fetch(`${API_BASE_URL}/provisionen/standard`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data)
    });
    if (!response.ok) throw new Error('Failed to update standard commission model');
    return response.json();
  },

  // Standort-spezifisches Provisionsmodell
  async getForStandort(standortId: string): Promise<ProvisionsModellDto> {
    const response = await fetch(`${API_BASE_URL}/provisionen/standort/${standortId}`);
    if (!response.ok) throw new Error('Failed to fetch commission model for location');
    return response.json();
  },

  async setForStandort(standortId: string, data: ProvisionsModellUpdateDto): Promise<ProvisionsModellDto> {
    const response = await fetch(`${API_BASE_URL}/provisionen/standort/${standortId}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data)
    });
    if (!response.ok) throw new Error('Failed to update commission model for location');
    return response.json();
  },

  async resetToStandard(standortId: string): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/provisionen/standort/${standortId}`, {
      method: 'DELETE'
    });
    if (!response.ok) throw new Error('Failed to reset to standard commission model');
  },

  async getHistory(standortId: string): Promise<ProvisionsModellDto[]> {
    const response = await fetch(`${API_BASE_URL}/provisionen/standort/${standortId}/historie`);
    if (!response.ok) throw new Error('Failed to fetch commission model history');
    return response.json();
  },

  async getPreview(standortId: string, jahr: number, monat: number): Promise<ProvisionsBerechnung> {
    const response = await fetch(
      `${API_BASE_URL}/provisionen/standort/${standortId}/vorschau?jahr=${jahr}&monat=${monat}`
    );
    if (!response.ok) throw new Error('Failed to fetch commission preview');
    return response.json();
  }
};

// API Service für QES-Signatur

import {
  SignaturAnforderungRequest,
  SignaturAnforderungResult,
  SignaturStatusResult,
  SignaturValidierungResult
} from '../types/signatur';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

export class SignaturApi {
  /**
   * Fordert eine neue QES-Signatur an
   */
  static async signaturAnfordern(request: SignaturAnforderungRequest): Promise<SignaturAnforderungResult> {
    const response = await fetch(`${API_BASE_URL}/signatur/anfordern`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        // TODO: Authorization header mit Token
      },
      body: JSON.stringify(request)
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Fehler bei Signatur-Anforderung');
    }

    return response.json();
  }

  /**
   * Prüft den Status einer Signatur
   */
  static async getStatus(signaturId: string): Promise<SignaturStatusResult> {
    const response = await fetch(`${API_BASE_URL}/signatur/${signaturId}/status`, {
      headers: {
        // TODO: Authorization header mit Token
      }
    });

    if (!response.ok) {
      throw new Error('Fehler beim Abrufen des Status');
    }

    return response.json();
  }

  /**
   * Lädt das signierte Dokument herunter
   */
  static async downloadSigniertesDokument(signaturId: string): Promise<Blob> {
    const response = await fetch(`${API_BASE_URL}/signatur/${signaturId}/dokument`, {
      headers: {
        // TODO: Authorization header mit Token
      }
    });

    if (!response.ok) {
      throw new Error('Fehler beim Download des Dokuments');
    }

    return response.blob();
  }

  /**
   * Validiert eine Signatur
   */
  static async validieren(signaturId: string): Promise<SignaturValidierungResult> {
    const response = await fetch(`${API_BASE_URL}/signatur/${signaturId}/validieren`, {
      headers: {
        // TODO: Authorization header mit Token
      }
    });

    if (!response.ok) {
      throw new Error('Fehler bei der Validierung');
    }

    return response.json();
  }

  /**
   * Storniert eine Signatur (Mitarbeiter only)
   */
  static async stornieren(signaturId: string): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/signatur/${signaturId}/stornieren`, {
      method: 'POST',
      headers: {
        // TODO: Authorization header mit Token
      }
    });

    if (!response.ok) {
      throw new Error('Fehler beim Stornieren');
    }
  }

  /**
   * Polling: Wartet auf Statusänderung
   */
  static async pollStatus(
    signaturId: string,
    onStatusChange: (status: SignaturStatusResult) => void,
    interval: number = 3000,
    maxAttempts: number = 100
  ): Promise<void> {
    let attempts = 0;

    const poll = async () => {
      if (attempts >= maxAttempts) {
        throw new Error('Timeout beim Warten auf Signatur-Status');
      }

      attempts++;
      const status = await this.getStatus(signaturId);
      onStatusChange(status);

      // Stoppe Polling wenn Endzustand erreicht
      if (
        status.status === 5 || // Signiert
        status.status === 6 || // Fehlgeschlagen
        status.status === 7 || // Abgelaufen
        status.status === 8 || // Abgelehnt
        status.status === 9    // Storniert
      ) {
        return;
      }

      // Nächstes Polling
      await new Promise(resolve => setTimeout(resolve, interval));
      await poll();
    };

    await poll();
  }
}

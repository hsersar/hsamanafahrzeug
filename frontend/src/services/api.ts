const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

export const trackingApi = {
  getStatus: async (trackingCode: string, token: string) => {
    const response = await fetch(
      `${API_BASE_URL}/tracking/${trackingCode}?token=${token}`
    );
    if (!response.ok) {
      throw new Error('Failed to fetch tracking status');
    }
    return response.json();
  },

  getQRCode: (trackingCode: string) => {
    return `${API_BASE_URL}/tracking/${trackingCode}/qrcode`;
  },
};

export const paymentApi = {
  getAvailableMethods: async () => {
    const response = await fetch(`${API_BASE_URL}/zahlungen/methoden`);
    if (!response.ok) {
      throw new Error('Failed to fetch payment methods');
    }
    return response.json();
  },

  initiatePayment: async (request: { rechnungId: string; methode: number }) => {
    const response = await fetch(`${API_BASE_URL}/zahlungen/initiieren`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    });
    if (!response.ok) {
      throw new Error('Failed to initiate payment');
    }
    return response.json();
  },

  confirmPayment: async (zahlungId: string) => {
    const response = await fetch(
      `${API_BASE_URL}/zahlungen/${zahlungId}/bestaetigen`,
      {
        method: 'POST',
      }
    );
    if (!response.ok) {
      throw new Error('Failed to confirm payment');
    }
    return response.json();
  },
};

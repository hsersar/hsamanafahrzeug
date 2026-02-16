import axios from 'axios';
import { RechnungDto, RechnungErstellenDto, KundeDto, StandortDto } from '../types';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

export const rechnungService = {
    async createRechnung(dto: RechnungErstellenDto): Promise<RechnungDto> {
        const response = await axios.post<RechnungDto>(`${API_BASE_URL}/rechnungen`, dto);
        return response.data;
    },

    async getRechnungen(): Promise<RechnungDto[]> {
        const response = await axios.get<RechnungDto[]>(`${API_BASE_URL}/rechnungen`);
        return response.data;
    },

    async getRechnung(id: string): Promise<RechnungDto> {
        const response = await axios.get<RechnungDto>(`${API_BASE_URL}/rechnungen/${id}`);
        return response.data;
    },

    async downloadPdf(id: string): Promise<Blob> {
        const response = await axios.get(`${API_BASE_URL}/rechnungen/${id}/pdf`, {
            responseType: 'blob'
        });
        return response.data;
    },

    async downloadXml(id: string): Promise<Blob> {
        const response = await axios.get(`${API_BASE_URL}/rechnungen/${id}/xml`, {
            responseType: 'blob'
        });
        return response.data;
    }
};

export const kundenService = {
    async getKunden(): Promise<KundeDto[]> {
        const response = await axios.get<KundeDto[]>(`${API_BASE_URL}/kunden`);
        return response.data;
    }
};

export const standortService = {
    async getStandorte(): Promise<StandortDto[]> {
        const response = await axios.get<StandortDto[]>(`${API_BASE_URL}/standorte`);
        return response.data;
    }
};

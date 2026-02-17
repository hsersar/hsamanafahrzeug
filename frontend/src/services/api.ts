import axios, { AxiosInstance } from 'axios';
import {
  Vehicle,
  RegistrationRequest,
  CreateRegistrationRequestDto,
  VehicleCatalogItem,
  UserProfile,
  DashboardOverview,
  HelpArticle,
  SearchRequestsFilter,
  PersonalProfile,
  SavePersonalProfile,
  CompanyProfile,
  SaveCompanyProfile,
  PaymentMethod,
  SavePaymentMethod,
  Invoice,
  Announcement,
} from '../types';

class ApiService {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: process.env.REACT_APP_API_URL || 'http://localhost:5141/api',
      headers: {
        'Content-Type': 'application/json',
      },
      withCredentials: true,
    });

    // Add request interceptor for authentication
    this.client.interceptors.request.use(
      (config) => {
        // Get token from localStorage (in production, use HttpOnly cookies)
        const token = localStorage.getItem('access_token');
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        // Send role header for server-side enforcement (mock mode)
        const role = localStorage.getItem('user_role');
        if (role) {
          config.headers['X-Mock-Role'] = role;
        }
        return config;
      },
      (error) => {
        return Promise.reject(error);
      }
    );

    // Add response interceptor for error handling
    this.client.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          // Redirect to login or refresh token
          localStorage.removeItem('access_token');
          window.location.href = '/login';
        }
        return Promise.reject(error);
      }
    );
  }

  // Vehicle endpoints
  async getMyVehicles(): Promise<Vehicle[]> {
    const response = await this.client.get<Vehicle[]>('/vehicles/my-vehicles');
    return response.data;
  }

  async getVehicle(id: number): Promise<Vehicle> {
    const response = await this.client.get<Vehicle>(`/vehicles/${id}`);
    return response.data;
  }

  async getVehicleCatalog(): Promise<VehicleCatalogItem[]> {
    const response = await this.client.get<VehicleCatalogItem[]>('/vehicles/catalog');
    return response.data;
  }

  // Registration request endpoints
  async createRegistrationRequest(
    data: CreateRegistrationRequestDto
  ): Promise<RegistrationRequest> {
    const response = await this.client.post<RegistrationRequest>('/registration', data);
    return response.data;
  }

  async getRegistrationRequest(id: number): Promise<RegistrationRequest> {
    const response = await this.client.get<RegistrationRequest>(`/registration/${id}`);
    return response.data;
  }

  async getMyRequests(): Promise<RegistrationRequest[]> {
    const response = await this.client.get<RegistrationRequest[]>('/registration/my-requests');
    return response.data;
  }

  // Profile
  async getProfile(): Promise<UserProfile> {
    const response = await this.client.get<UserProfile>('/users/me');
    return response.data;
  }

  // Dashboard
  async getDashboardOverview(): Promise<DashboardOverview> {
    const response = await this.client.get<DashboardOverview>('/dashboard/overview');
    return response.data;
  }

  // Help
  async getHelpArticles(): Promise<HelpArticle[]> {
    const response = await this.client.get<HelpArticle[]>('/help');
    return response.data;
  }

  // Search
  async searchRequests(filter: SearchRequestsFilter): Promise<RegistrationRequest[]> {
    const response = await this.client.post<RegistrationRequest[]>('/search/requests', filter);
    return response.data;
  }

  // Auth
  async logout(): Promise<void> {
    await this.client.post('/auth/logout');
  }

  // ── Personal Profile ──
  async getPersonalProfile(): Promise<PersonalProfile> {
    const response = await this.client.get<PersonalProfile>('/profile/personal');
    return response.data;
  }

  async savePersonalProfile(data: SavePersonalProfile): Promise<PersonalProfile> {
    const response = await this.client.put<PersonalProfile>('/profile/personal', data);
    return response.data;
  }

  async uploadProfilePhoto(file: File): Promise<PersonalProfile> {
    const formData = new FormData();
    formData.append('file', file);
    const response = await this.client.post<PersonalProfile>('/profile/personal/photo', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return response.data;
  }

  // ── Company Profile ──
  async getCompanyProfile(): Promise<CompanyProfile> {
    const response = await this.client.get<CompanyProfile>('/profile/company');
    return response.data;
  }

  async saveCompanyProfile(data: SaveCompanyProfile): Promise<CompanyProfile> {
    const response = await this.client.put<CompanyProfile>('/profile/company', data);
    return response.data;
  }

  async uploadCompanyLogo(file: File): Promise<CompanyProfile> {
    const formData = new FormData();
    formData.append('file', file);
    const response = await this.client.post<CompanyProfile>('/profile/company/logo', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return response.data;
  }
  // ── Payment Methods ──
  async getPaymentMethods(): Promise<PaymentMethod[]> {
    const response = await this.client.get<PaymentMethod[]>('/profile/payments');
    return response.data;
  }

  async createPaymentMethod(data: SavePaymentMethod): Promise<PaymentMethod> {
    const response = await this.client.post<PaymentMethod>('/profile/payments', data);
    return response.data;
  }

  async updatePaymentMethod(id: number, data: SavePaymentMethod): Promise<PaymentMethod> {
    const response = await this.client.put<PaymentMethod>(`/profile/payments/${id}`, data);
    return response.data;
  }

  async deletePaymentMethod(id: number): Promise<void> {
    await this.client.delete(`/profile/payments/${id}`);
  }

  async setDefaultPaymentMethod(id: number): Promise<void> {
    await this.client.post(`/profile/payments/${id}/default`);
  }

  // ── Invoices ──
  async getMyInvoices(): Promise<Invoice[]> {
    const response = await this.client.get<Invoice[]>('/invoices');
    return response.data;
  }

  async getInvoice(id: number): Promise<Invoice> {
    const response = await this.client.get<Invoice>(`/invoices/${id}`);
    return response.data;
  }

  // ── Announcements ──
  async getAnnouncements(): Promise<Announcement[]> {
    const response = await this.client.get<Announcement[]>('/announcements');
    return response.data;
  }
}

const apiService = new ApiService();
export default apiService;

import axios, { AxiosInstance } from 'axios';
import { Vehicle, RegistrationRequest, CreateRegistrationRequestDto } from '../types';

class ApiService {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: process.env.REACT_APP_API_URL || 'https://localhost:7000/api',
      headers: {
        'Content-Type': 'application/json',
      },
      withCredentials: true,
    });

    // Add request interceptor for authentication
    this.client.interceptors.request.use(
      (config) => {
        // Get token from localStorage (in production, use more secure storage)
        const token = localStorage.getItem('access_token');
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
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
}

const apiService = new ApiService();
export default apiService;

export interface Vehicle {
  id: number;
  vin: string;
  licensePlate: string;
  brand: string;
  model: string;
  year: number;
  color: string;
  firstRegistrationDate: string;
  createdAt: string;
}

export interface RegistrationRequest {
  id: number;
  vin: string;
  requestedLicensePlate: string;
  brand: string;
  model: string;
  year: number;
  color: string;
  firstRegistrationDate: string;
  status: string;
  createdAt: string;
  updatedAt: string;
  rejectionReason?: string;
}

export interface CreateRegistrationRequestDto {
  vin: string;
  requestedLicensePlate: string;
  brand: string;
  model: string;
  year: number;
  color: string;
  firstRegistrationDate: string;
}

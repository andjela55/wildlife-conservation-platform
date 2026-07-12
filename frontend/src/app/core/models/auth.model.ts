import { UserRole } from './user-role.enum';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
}

export interface AuthenticatedUser {
  id: number;
  fullName: string;
  email: string;
  role: UserRole;
  assignedLocationName: string | null;
  assignedLatitude: number | null;
  assignedLongitude: number | null;
  assignedMapZoom: number | null;
}

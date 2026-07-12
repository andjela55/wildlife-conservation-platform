import { UserRole } from './user-role.enum';

export interface User {
  id: number;
  fullName: string;
  email: string;
  role: UserRole;
  isActive: boolean;
  assignedLocationName: string | null;
  assignedLatitude: number | null;
  assignedLongitude: number | null;
  assignedMapZoom: number | null;
}

export interface CreateUserRequest {
  fullName: string;
  email: string;
  password: string;
  role: UserRole;
  isActive: boolean;
  assignedLocationName: string | null;
  assignedLatitude: number | null;
  assignedLongitude: number | null;
  assignedMapZoom: number | null;
}

export interface UpdateUserAssignedAreaRequest {
  assignedLocationName: string | null;
  assignedLatitude: number | null;
  assignedLongitude: number | null;
  assignedMapZoom: number | null;
}

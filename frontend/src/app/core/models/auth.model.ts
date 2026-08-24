import { PermissionCode } from './permission-code.enum';
import { Role } from './role.model';

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
  roles: ReadonlyArray<Role>;
  permissions: ReadonlyArray<PermissionCode>;
  assignedLocationName: string | null;
  assignedLatitude: number | null;
  assignedLongitude: number | null;
  assignedMapZoom: number | null;
}

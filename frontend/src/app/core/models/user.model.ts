import { Role } from './role.model';

export interface User {
  id: number;
  fullName: string;
  email: string;
  roles: ReadonlyArray<Role>;
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
  roleIds: ReadonlyArray<number>;
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

export interface UpdateUserRequest {
  fullName: string;
  email: string;
  password: string | null;
  roleIds: ReadonlyArray<number>;
  isActive: boolean;
  assignedLocationName: string | null;
  assignedLatitude: number | null;
  assignedLongitude: number | null;
  assignedMapZoom: number | null;
}

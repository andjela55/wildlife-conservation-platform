import { ApiEnum } from './api-enum.type';

export const UserRoles = {
  Ranger: 'Ranger',
  Researcher: 'Researcher',
  Admin: 'Admin',
  Master: 'Master'
} as const;

export type UserRoleKey = typeof UserRoles[keyof typeof UserRoles];
export type UserRole = ApiEnum<UserRoleKey>;

export const userRoleOptions: Array<UserRoleKey> = Object.values(UserRoles);

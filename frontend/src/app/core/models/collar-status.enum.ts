import { ApiEnum } from './api-enum.type';

export type CollarStatusKey = 'Available' | 'Assigned' | 'Inactive' | 'Lost' | 'Damaged';
export type CollarStatus = ApiEnum<CollarStatusKey>;

export const collarStatusOptions: Array<CollarStatusKey> = ['Available', 'Assigned', 'Inactive', 'Lost', 'Damaged'];

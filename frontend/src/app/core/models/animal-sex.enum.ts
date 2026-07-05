import { ApiEnum } from './api-enum.type';

export type AnimalSexKey = 'Unknown' | 'Male' | 'Female';
export type AnimalSex = ApiEnum<AnimalSexKey>;

export const animalSexOptions: Array<AnimalSexKey> = ['Unknown', 'Male', 'Female'];

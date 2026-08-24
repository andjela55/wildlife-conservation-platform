import { AnimalSex } from './animal-sex.enum';

export interface Animal {
  id: number;
  name: string;
  subspeciesId: number;
  sex: AnimalSex;
  dateOfBirth: string | null;
  notes: string | null;
  isActive: boolean;
}

export interface UpsertAnimalRequest {
  name: string;
  subspeciesId: number;
  sex: AnimalSex;
  dateOfBirth: string | null;
  notes: string | null;
  isActive: boolean;
}

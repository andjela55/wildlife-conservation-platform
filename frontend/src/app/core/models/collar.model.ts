import { CollarStatus } from './collar-status.enum';

export interface Collar {
  id: number;
  serialNumber: string;
  model: string | null;
  manufacturer: string | null;
  status: CollarStatus;
  notes: string | null;
}

export interface CreateCollarRequest {
  serialNumber: string;
  model: string | null;
  manufacturer: string | null;
  status: CollarStatus;
  notes: string | null;
}

export type UpdateCollarRequest = CreateCollarRequest;

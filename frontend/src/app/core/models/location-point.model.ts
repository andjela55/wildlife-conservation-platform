import { SignalType } from './signal-type.enum';

export interface LocationPoint {
  id: number;
  animalId: number;
  animalName?: string;
  collarId: number;
  collarSerialNumber?: string;
  latitude: number;
  longitude: number;
  altitude: number | null;
  recordedAt: string;
  signalType: SignalType;
  notes: string | null;
}

export interface LocationPointReceived extends LocationPoint {
  animalName: string;
  collarSerialNumber: string;
}

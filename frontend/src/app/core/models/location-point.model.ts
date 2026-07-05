import { SignalType } from './signal-type.enum';

export interface LocationPoint {
  id: number;
  animalId: number;
  collarId: number;
  latitude: number;
  longitude: number;
  altitude: number | null;
  recordedAt: string;
  signalType: SignalType;
  notes: string | null;
}

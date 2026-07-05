export type ApiEnum<T extends string> = T | number;

export type AnimalSexKey = 'Unknown' | 'Male' | 'Female';
export type AnimalSex = ApiEnum<AnimalSexKey>;
export type CollarStatusKey = 'Available' | 'Assigned' | 'Inactive' | 'Lost' | 'Damaged';
export type CollarStatus = ApiEnum<CollarStatusKey>;
export type SignalTypeKey = 'Cellular' | 'Satellite' | 'LoRaWAN' | 'Manual' | 'Simulator';
export type SignalType = ApiEnum<SignalTypeKey>;
export type ReportTypeKey =
  | 'Sighting'
  | 'Injury'
  | 'CollarIssue'
  | 'PoachingSigns'
  | 'HabitatIssue'
  | 'Emergency'
  | 'Other';
export type ReportType = ApiEnum<ReportTypeKey>;
export type SeverityKey = 'Low' | 'Medium' | 'High' | 'Critical';
export type Severity = ApiEnum<SeverityKey>;
export type AlertTypeKey =
  | 'NoMovement'
  | 'LeftSafeZone'
  | 'CollarBatteryLow'
  | 'CollarSignalLost'
  | 'Manual'
  | 'Other';
export type AlertType = ApiEnum<AlertTypeKey>;

export interface Species {
  id: number;
  name: string;
  description: string;
}

export interface CreateSpeciesRequest {
  name: string;
  description: string;
}

export interface Subspecies {
  id: number;
  speciesId: number;
  name: string;
  description: string;
}

export interface CreateSubspeciesRequest {
  speciesId: number;
  name: string;
  description: string;
}

export interface Animal {
  id: number;
  name: string;
  subspeciesId: number;
  sex: AnimalSex;
  dateOfBirth: string | null;
  notes: string | null;
  isActive: boolean;
}

export interface CreateAnimalRequest {
  name: string;
  subspeciesId: number;
  sex: AnimalSex;
  dateOfBirth: string | null;
  notes: string | null;
  isActive: boolean;
}

export type UpdateAnimalRequest = CreateAnimalRequest;

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

export interface CollarAssignment {
  id: number;
  animalId: number;
  collarId: number;
  assignedAt: string;
  unassignedAt: string | null;
  reason: string | null;
  notes: string | null;
}

export interface CreateCollarAssignmentRequest {
  animalId: number;
  collarId: number;
  assignedAt: string;
  reason: string | null;
  notes: string | null;
}

export interface UnassignCollarRequest {
  unassignedAt: string | null;
  reason: string | null;
  notes: string | null;
}

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

export interface RangerReport {
  id: number;
  animalId: number | null;
  userId: number;
  reportType: ReportType;
  severity: Severity;
  latitude: number;
  longitude: number;
  description: string;
  createdAt: string;
}

export interface CreateRangerReportRequest {
  animalId: number | null;
  userId: number;
  reportType: ReportType;
  severity: Severity;
  latitude: number;
  longitude: number;
  description: string;
  createdAt: string;
}

export interface Alert {
  id: number;
  animalId: number;
  collarId: number | null;
  createdByUserId: number | null;
  alertType: AlertType;
  severity: Severity;
  description: string;
  isResolved: boolean;
  createdAt: string;
  resolvedAt: string | null;
}

export interface CreateAlertRequest {
  animalId: number;
  collarId: number | null;
  createdByUserId: number | null;
  alertType: AlertType;
  severity: Severity;
  description: string;
  createdAt: string;
}

export interface ResolveAlertRequest {
  resolvedAt: string | null;
}

export const animalSexOptions: AnimalSexKey[] = ['Unknown', 'Male', 'Female'];
export const collarStatusOptions: CollarStatusKey[] = ['Available', 'Assigned', 'Inactive', 'Lost', 'Damaged'];
export const reportTypeOptions: ReportTypeKey[] = [
  'Sighting',
  'Injury',
  'CollarIssue',
  'PoachingSigns',
  'HabitatIssue',
  'Emergency',
  'Other'
];
export const severityOptions: SeverityKey[] = ['Low', 'Medium', 'High', 'Critical'];
export const alertTypeOptions: AlertTypeKey[] = [
  'NoMovement',
  'LeftSafeZone',
  'CollarBatteryLow',
  'CollarSignalLost',
  'Manual',
  'Other'
];

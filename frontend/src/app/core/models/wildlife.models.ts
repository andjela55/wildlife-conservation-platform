export type AnimalSex = 'Unknown' | 'Male' | 'Female';
export type CollarStatus = 'Available' | 'Assigned' | 'Inactive' | 'Lost' | 'Damaged';
export type SignalType = 'Cellular' | 'Satellite' | 'LoRaWAN' | 'Manual' | 'Simulator';
export type ReportType =
  | 'Sighting'
  | 'Injury'
  | 'CollarIssue'
  | 'PoachingSigns'
  | 'HabitatIssue'
  | 'Emergency'
  | 'Other';
export type Severity = 'Low' | 'Medium' | 'High' | 'Critical';
export type AlertType =
  | 'NoMovement'
  | 'LeftSafeZone'
  | 'CollarBatteryLow'
  | 'CollarSignalLost'
  | 'Manual'
  | 'Other';

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

export interface CreateLocationPointRequest {
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

export const animalSexOptions: AnimalSex[] = ['Unknown', 'Male', 'Female'];
export const collarStatusOptions: CollarStatus[] = ['Available', 'Assigned', 'Inactive', 'Lost', 'Damaged'];
export const signalTypeOptions: SignalType[] = ['Cellular', 'Satellite', 'LoRaWAN', 'Manual', 'Simulator'];
export const reportTypeOptions: ReportType[] = [
  'Sighting',
  'Injury',
  'CollarIssue',
  'PoachingSigns',
  'HabitatIssue',
  'Emergency',
  'Other'
];
export const severityOptions: Severity[] = ['Low', 'Medium', 'High', 'Critical'];
export const alertTypeOptions: AlertType[] = [
  'NoMovement',
  'LeftSafeZone',
  'CollarBatteryLow',
  'CollarSignalLost',
  'Manual',
  'Other'
];

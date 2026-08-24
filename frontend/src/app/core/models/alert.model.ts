import { AlertType } from './alert-type.enum';
import { Severity } from './severity.enum';

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
  alertType: AlertType;
  severity: Severity;
  description: string;
}

export interface ResolveAlertRequest {
  resolvedAt: string | null;
}

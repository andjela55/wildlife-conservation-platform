import { ReportType } from './report-type.enum';
import { Severity } from './severity.enum';

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

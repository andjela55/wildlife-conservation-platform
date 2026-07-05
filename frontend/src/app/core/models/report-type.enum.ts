import { ApiEnum } from './api-enum.type';

export type ReportTypeKey =
  | 'Sighting'
  | 'Injury'
  | 'CollarIssue'
  | 'PoachingSigns'
  | 'HabitatIssue'
  | 'Emergency'
  | 'Other';

export type ReportType = ApiEnum<ReportTypeKey>;

export const reportTypeOptions: Array<ReportTypeKey> = [
  'Sighting',
  'Injury',
  'CollarIssue',
  'PoachingSigns',
  'HabitatIssue',
  'Emergency',
  'Other'
];

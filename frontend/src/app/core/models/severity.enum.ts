import { ApiEnum } from './api-enum.type';

export type SeverityKey = 'Low' | 'Medium' | 'High' | 'Critical';
export type Severity = ApiEnum<SeverityKey>;

export const severityOptions: Array<SeverityKey> = ['Low', 'Medium', 'High', 'Critical'];

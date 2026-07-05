import { ApiEnum } from './api-enum.type';

export type AlertTypeKey =
  | 'NoMovement'
  | 'LeftSafeZone'
  | 'CollarBatteryLow'
  | 'CollarSignalLost'
  | 'Manual'
  | 'Other';

export type AlertType = ApiEnum<AlertTypeKey>;

export const alertTypeOptions: Array<AlertTypeKey> = [
  'NoMovement',
  'LeftSafeZone',
  'CollarBatteryLow',
  'CollarSignalLost',
  'Manual',
  'Other'
];

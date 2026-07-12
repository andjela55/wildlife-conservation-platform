export type EnumName =
  | 'AnimalSex'
  | 'CollarStatus'
  | 'SignalType'
  | 'ReportType'
  | 'Severity'
  | 'AlertType'
  | 'UserRole';

const enumValues: Record<EnumName, Array<string>> = {
  AnimalSex: ['Unknown', 'Male', 'Female'],
  CollarStatus: ['Available', 'Assigned', 'Inactive', 'Lost', 'Damaged'],
  SignalType: ['Cellular', 'Satellite', 'LoRaWAN', 'Manual', 'Simulator'],
  ReportType: ['Sighting', 'Injury', 'CollarIssue', 'PoachingSigns', 'HabitatIssue', 'Emergency', 'Other'],
  Severity: ['Low', 'Medium', 'High', 'Critical'],
  AlertType: ['NoMovement', 'LeftSafeZone', 'CollarBatteryLow', 'CollarSignalLost', 'Manual', 'Other'],
  UserRole: ['Ranger', 'Researcher', 'Admin', 'Master']
};

const enumLabels: Record<string, string> = {
  Unknown: 'Unknown',
  Male: 'Male',
  Female: 'Female',
  Available: 'Available',
  Assigned: 'Assigned',
  Inactive: 'Inactive',
  Lost: 'Lost',
  Damaged: 'Damaged',
  Cellular: 'Cellular',
  Satellite: 'Satellite',
  LoRaWAN: 'LoRaWAN',
  Manual: 'Manual',
  Simulator: 'Simulator',
  Sighting: 'Sighting',
  Injury: 'Injury',
  CollarIssue: 'Collar issue',
  PoachingSigns: 'Poaching signs',
  HabitatIssue: 'Habitat issue',
  Emergency: 'Emergency',
  Other: 'Other',
  Low: 'Low',
  Medium: 'Medium',
  High: 'High',
  Critical: 'Critical',
  NoMovement: 'No movement',
  LeftSafeZone: 'Left safe zone',
  CollarBatteryLow: 'Collar battery low',
  CollarSignalLost: 'Collar signal lost',
  Ranger: 'Ranger',
  Researcher: 'Researcher',
  Admin: 'Admin',
  Master: 'Master'
};

export function enumKey(value: unknown, enumName?: EnumName): string {
  if (value === null || value === undefined || value === '') {
    return '';
  }

  if (enumName && typeof value === 'number') {
    return enumValues[enumName][value] ?? String(value);
  }

  const text = String(value);
  if (enumName && /^\d+$/.test(text)) {
    return enumValues[enumName][Number(text)] ?? text;
  }

  return text;
}

export function enumLabel(value: unknown, enumName?: EnumName): string {
  const key = enumKey(value, enumName);

  return enumLabels[key] ?? key.replace(/([a-z])([A-Z])/g, '$1 $2');
}

export function enumEquals(value: unknown, enumName: EnumName, expected: string): boolean {
  return enumKey(value, enumName) === expected;
}

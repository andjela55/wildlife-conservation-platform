export const PermissionCodes = {
  AnimalsRead: 'AnimalsRead',
  AnimalsWrite: 'AnimalsWrite',
  AlertsRead: 'AlertsRead',
  AlertsWrite: 'AlertsWrite',
  CollarsRead: 'CollarsRead',
  CollarsWrite: 'CollarsWrite',
  CollarAssignmentsRead: 'CollarAssignmentsRead',
  CollarAssignmentsWrite: 'CollarAssignmentsWrite',
  LocationPointsRead: 'LocationPointsRead',
  LocationPointsWrite: 'LocationPointsWrite',
  RangerReportsRead: 'RangerReportsRead',
  RangerReportsWrite: 'RangerReportsWrite',
  SpeciesRead: 'SpeciesRead',
  SpeciesWrite: 'SpeciesWrite',
  SubspeciesRead: 'SubspeciesRead',
  SubspeciesWrite: 'SubspeciesWrite',
  UsersRead: 'UsersRead',
  UsersWrite: 'UsersWrite',
  RolesRead: 'RolesRead',
  RolesWrite: 'RolesWrite',
  Master: 'Master'
} as const;

export type PermissionCode = typeof PermissionCodes[keyof typeof PermissionCodes];

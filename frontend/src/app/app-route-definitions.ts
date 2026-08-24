import { PermissionCode, PermissionCodes } from './core/models';

export interface AppRouteDefinition {
  Path: string;
  Link: string;
  Label?: string;
  Icon?: string;
  Exact?: boolean;
  Permission?: PermissionCode;
}

export interface AppNavigationRoute {
  Label: string;
  Link: string;
  Icon: string;
  Exact?: boolean;
  Permission: PermissionCode;
}

export const AppRoutes = {
  Login: {
    Path: 'login',
    Link: '/login'
  },
  Dashboard: {
    Path: '',
    Link: '/',
    Label: 'Dashboard',
    Icon: 'assets/icons/home.svg',
    Exact: true,
    Permission: PermissionCodes.AnimalsRead
  },
  Species: {
    Path: 'species',
    Link: '/species',
    Label: 'Species',
    Icon: 'assets/icons/category.svg',
    Permission: PermissionCodes.SpeciesRead
  },
  Animals: {
    Path: 'animals',
    Link: '/animals',
    Label: 'Animals',
    Icon: 'assets/icons/paw.svg',
    Permission: PermissionCodes.AnimalsRead
  },
  Collars: {
    Path: 'collars',
    Link: '/collars',
    Label: 'Collars',
    Icon: 'assets/icons/collar.svg',
    Permission: PermissionCodes.CollarsRead
  },
  Reports: {
    Path: 'reports',
    Link: '/reports',
    Label: 'Reports',
    Icon: 'assets/icons/report.svg',
    Permission: PermissionCodes.RangerReportsRead
  },
  Alerts: {
    Path: 'alerts',
    Link: '/alerts',
    Label: 'Alerts',
    Icon: 'assets/icons/alert.svg',
    Permission: PermissionCodes.AlertsRead
  },
  Users: {
    Path: 'users',
    Link: '/users',
    Label: 'Users',
    Icon: 'assets/icons/category.svg',
    Permission: PermissionCodes.UsersWrite
  }
} satisfies Record<string, AppRouteDefinition>;

export function toNavigationRoute(route: AppRouteDefinition): AppNavigationRoute {
  if (!route.Label || !route.Icon || !route.Permission) {
    throw new Error(`Route '${route.Path}' is missing navigation metadata.`);
  }

  return {
    Label: route.Label,
    Link: route.Link,
    Icon: route.Icon,
    Exact: route.Exact,
    Permission: route.Permission
  };
}

import { UserRole, UserRoles } from './core/models';

export interface AppRouteDefinition {
  Path: string;
  Link: string;
  Label?: string;
  Icon?: string;
  Exact?: boolean;
  Roles?: Array<UserRole>;
}

export interface AppNavigationRoute {
  Label: string;
  Link: string;
  Icon: string;
  Exact?: boolean;
  Roles: Array<UserRole>;
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
    Roles: [UserRoles.Admin, UserRoles.Ranger, UserRoles.Researcher]
  },
  Species: {
    Path: 'species',
    Link: '/species',
    Label: 'Species',
    Icon: 'assets/icons/category.svg',
    Roles: [UserRoles.Admin, UserRoles.Researcher]
  },
  Animals: {
    Path: 'animals',
    Link: '/animals',
    Label: 'Animals',
    Icon: 'assets/icons/paw.svg',
    Roles: [UserRoles.Admin, UserRoles.Ranger, UserRoles.Researcher]
  },
  Collars: {
    Path: 'collars',
    Link: '/collars',
    Label: 'Collars',
    Icon: 'assets/icons/collar.svg',
    Roles: [UserRoles.Admin, UserRoles.Researcher]
  },
  Reports: {
    Path: 'reports',
    Link: '/reports',
    Label: 'Reports',
    Icon: 'assets/icons/report.svg',
    Roles: [UserRoles.Admin, UserRoles.Ranger, UserRoles.Researcher]
  },
  Alerts: {
    Path: 'alerts',
    Link: '/alerts',
    Label: 'Alerts',
    Icon: 'assets/icons/alert.svg',
    Roles: [UserRoles.Admin, UserRoles.Ranger, UserRoles.Researcher]
  },
  Users: {
    Path: 'users',
    Link: '/users',
    Label: 'Users',
    Icon: 'assets/icons/category.svg',
    Roles: [UserRoles.Master]
  }
} satisfies Record<string, AppRouteDefinition>;

export function toNavigationRoute(route: AppRouteDefinition): AppNavigationRoute {
  if (!route.Label || !route.Icon || !route.Roles) {
    throw new Error(`Route '${route.Path}' is missing navigation metadata.`);
  }

  return {
    Label: route.Label,
    Link: route.Link,
    Icon: route.Icon,
    Exact: route.Exact,
    Roles: route.Roles
  };
}

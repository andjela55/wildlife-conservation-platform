import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthenticatedUser, UserRole, UserRoles } from '../core/models';
import { AuthService } from '../core/services/auth.service';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.scss']
})
export class AuthComponent {
  isSidebarCollapsed = false;
  readonly currentUser$: Observable<AuthenticatedUser | null>;
  readonly navigationItems: Array<{
    label: string;
    route: string;
    icon: string;
    exact?: boolean;
    roles: Array<UserRole>;
  }> = [
    {
      label: 'Dashboard',
      route: '/',
      icon: 'assets/icons/home.svg',
      exact: true,
      roles: [UserRoles.Admin, UserRoles.Ranger, UserRoles.Researcher]
    },
    {
      label: 'Animals',
      route: '/animals',
      icon: 'assets/icons/paw.svg',
      roles: [UserRoles.Admin, UserRoles.Ranger, UserRoles.Researcher]
    },
    {
      label: 'Species',
      route: '/species',
      icon: 'assets/icons/category.svg',
      roles: [UserRoles.Admin, UserRoles.Researcher]
    },
    {
      label: 'Reports',
      route: '/reports',
      icon: 'assets/icons/report.svg',
      roles: [UserRoles.Admin, UserRoles.Ranger, UserRoles.Researcher]
    },
    {
      label: 'Alerts',
      route: '/alerts',
      icon: 'assets/icons/alert.svg',
      roles: [UserRoles.Admin, UserRoles.Ranger, UserRoles.Researcher]
    },
    {
      label: 'Collars',
      route: '/collars',
      icon: 'assets/icons/collar.svg',
      roles: [UserRoles.Admin, UserRoles.Researcher]
    },
    {
      label: 'Users',
      route: '/users',
      icon: 'assets/icons/category.svg',
      roles: [UserRoles.Master]
    }
  ];

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {
    this.currentUser$ = this.authService.currentUser$;
  }

  toggleSidebar(): void {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }

  canShowNavigationItem(item: { roles: Array<UserRole> }): boolean {
    return this.authService.hasAnyRole(item.roles);
  }

  logout(): void {
    this.authService.logout();
    void this.router.navigate(['/login']);
  }
}

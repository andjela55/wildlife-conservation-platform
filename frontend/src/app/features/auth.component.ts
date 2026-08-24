import { Component } from '@angular/core';
import { Route, Router } from '@angular/router';
import { map, Observable } from 'rxjs';
import { AppNavigationRoute } from '../app-route-definitions';
import { AuthenticatedUser } from '../core/models';
import { AuthService } from '../core/services/auth.service';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.scss']
})
export class AuthComponent {
  isSidebarCollapsed = false;
  readonly currentUserViewModel$: Observable<{ user: AuthenticatedUser; roleNames: string } | null>;
  readonly navigationItems$: Observable<Array<AppNavigationRoute>>;

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {
    const navigationItems = this.getNavigationItems();

    this.currentUserViewModel$ = this.authService.currentUser$.pipe(
      map((user) => user
        ? {
            user,
            roleNames: user.roles.map((role) => role.name).join(', ') || 'No role'
          }
        : null)
    );
    this.navigationItems$ = this.authService.currentUser$.pipe(
      map(() => navigationItems.filter((item) => this.authService.hasPermission(item.Permission)))
    );
  }

  toggleSidebar(): void {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }

  logout(): void {
    this.authService.logout();
    void this.router.navigate(['/login']);
  }

  private getNavigationItems(): Array<AppNavigationRoute> {
    const authenticatedRoute = this.router.config.find((route) => route.component === AuthComponent);

    return (authenticatedRoute?.children ?? [])
      .map((route) => this.getNavigationItem(route))
      .filter((item): item is AppNavigationRoute => !!item);
  }

  private getNavigationItem(route: Route): AppNavigationRoute | null {
    return route.data?.['navigation'] as AppNavigationRoute | null;
  }
}

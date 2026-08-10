import { Component } from '@angular/core';
import { Route, Router } from '@angular/router';
import { Observable } from 'rxjs';
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
  readonly currentUser$: Observable<AuthenticatedUser | null>;
  readonly navigationItems: Array<AppNavigationRoute>;

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {
    this.currentUser$ = this.authService.currentUser$;
    this.navigationItems = this.getNavigationItems();
  }

  toggleSidebar(): void {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }

  canShowNavigationItem(item: AppNavigationRoute): boolean {
    return this.authService.hasAnyRole(item.Roles);
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

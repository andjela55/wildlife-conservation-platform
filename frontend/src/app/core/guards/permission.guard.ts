import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { PermissionCode } from '../models';
import { AuthService } from '../services/auth.service';

@Injectable({ providedIn: 'root' })
export class PermissionGuard implements CanActivate {
  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree {
    const permission = route.data['permission'] as PermissionCode;

    if (!this.authService.currentUser) {
      return this.router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
    }

    return this.authService.hasPermission(permission)
      ? true
      : this.router.createUrlTree(['/']);
  }
}

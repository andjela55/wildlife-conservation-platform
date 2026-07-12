import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { catchError, map, Observable, of } from 'rxjs';
import { UserRole } from '../models';
import { AuthService } from '../services/auth.service';

@Injectable({ providedIn: 'root' })
export class RoleGuard implements CanActivate {
  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree | Observable<boolean | UrlTree> {
    const roles = (route.data['roles'] ?? []) as Array<UserRole>;

    return this.authService.loadCurrentUser().pipe(
      map(() => this.authService.hasAnyRole(roles) ? true : this.router.createUrlTree(['/'])),
      catchError(() => of(this.router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } })))
    );
  }
}

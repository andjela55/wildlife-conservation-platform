import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { catchError, map, Observable, of } from 'rxjs';
import { AppRoutes } from 'src/app/app-route-definitions';
import { AuthService } from '../services/auth.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  canActivate(_route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree | Observable<boolean | UrlTree> {
    if (!this.authService.token) {
      return this.createLoginUrl(state.url);
    }

    return this.authService.loadCurrentUser().pipe(
      map(() => true),
      catchError(() => of(this.createLoginUrl(state.url)))
    );
  }

  private createLoginUrl(returnUrl: string): UrlTree {
    return this.router.createUrlTree([`/${AppRoutes.Login.Link}`], { queryParams: { returnUrl } });
  }
}

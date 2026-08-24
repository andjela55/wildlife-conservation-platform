import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, Observable, of, switchMap, tap, throwError } from 'rxjs';
import { environment } from 'src/environments/environment';
import { AuthenticatedUser, LoginRequest, LoginResponse, PermissionCode, PermissionCodes } from '../models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly storageKey = 'wildtrack.token';
  private readonly legacyStorageKey = 'wildtrack.auth';
  private readonly tokenSubject = new BehaviorSubject<string | null>(this.getStoredToken());
  private readonly currentUserSubject = new BehaviorSubject<AuthenticatedUser | null>(null);

  readonly currentUser$ = this.currentUserSubject.asObservable();
  readonly isAuthenticated$ = this.tokenSubject.pipe(map((token) => !!token));

  constructor(private readonly http: HttpClient) {}

  get currentUser(): AuthenticatedUser | null {
    return this.currentUserSubject.value;
  }

  get token(): string | null {
    return this.tokenSubject.value;
  }

  login(request: LoginRequest): Observable<AuthenticatedUser> {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/api/auth/login`, request)
      .pipe(
        tap((response) => this.setToken(response.token)),
        switchMap(() => this.loadCurrentUser())
      );
  }

  logout(): void {
    localStorage.removeItem(this.storageKey);
    localStorage.removeItem(this.legacyStorageKey);
    this.tokenSubject.next(null);
    this.currentUserSubject.next(null);
  }

  loadCurrentUser(): Observable<AuthenticatedUser> {
    const currentUser = this.currentUserSubject.value;
    if (currentUser) {
      return of(currentUser);
    }

    if (!this.token) {
      return throwError(() => new Error('User is not authenticated.'));
    }

    return this.http.get<AuthenticatedUser>(`${environment.apiUrl}/api/auth/current-user`)
      .pipe(
        tap((user) => this.currentUserSubject.next(user)),
        catchError((error) => {
          this.logout();
          return throwError(() => error);
        })
      );
  }

  refreshCurrentUser(): Observable<AuthenticatedUser> {
    if (!this.token) {
      return throwError(() => new Error('User is not authenticated.'));
    }

    return this.http.get<AuthenticatedUser>(`${environment.apiUrl}/api/auth/current-user`)
      .pipe(
        tap((user) => this.currentUserSubject.next(user)),
        catchError((error) => {
          this.logout();
          return throwError(() => error);
        })
      );
  }

  hasPermission(permission: PermissionCode): boolean {
    return !!this.currentUser && (
      this.currentUser.permissions.includes(PermissionCodes.Master) ||
      this.currentUser.permissions.includes(permission)
    );
  }

  private setToken(token: string): void {
    localStorage.removeItem(this.legacyStorageKey);
    localStorage.setItem(this.storageKey, token);
    this.tokenSubject.next(token);
  }

  private getStoredToken(): string | null {
    localStorage.removeItem(this.legacyStorageKey);
    return localStorage.getItem(this.storageKey);
  }
}

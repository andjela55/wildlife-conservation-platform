import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Role } from '../models';

@Injectable({ providedIn: 'root' })
export class RoleApiService {
  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<ReadonlyArray<Role>> {
    return this.http.get<ReadonlyArray<Role>>(`${environment.apiUrl}/api/roles`);
  }
}

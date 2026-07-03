import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Alert, CreateAlertRequest, ResolveAlertRequest } from '../models/wildlife.models';
import { BaseApiService } from './base-api.service';

@Injectable({ providedIn: 'root' })
export class AlertApiService extends BaseApiService<Alert, CreateAlertRequest> {
  protected readonly resourcePath = 'alerts';

  constructor(http: HttpClient) {
    super(http);
  }

  resolve(id: number, request: ResolveAlertRequest): Observable<Alert> {
    return this.http.put<Alert>(`${this.baseUrl}/${id}/resolve`, request);
  }
}

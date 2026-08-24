import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Alert, CreateAlertRequest, PagedResult, PaginationQuery, ResolveAlertRequest } from '../models';
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

  getByAnimalPaged(animalId: number, pagination: PaginationQuery = {}): Observable<PagedResult<Alert>> {
    return this.http.get<PagedResult<Alert>>(`${this.baseUrl}/by-animal/${animalId}`, {
      params: this.createPaginationParams(pagination)
    });
  }
}

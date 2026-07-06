import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Alert, Animal, CreateAnimalRequest, LocationPoint, PagedResult, PaginationQuery, RangerReport, UpdateAnimalRequest } from '../models';
import { BaseApiService } from './base-api.service';

@Injectable({ providedIn: 'root' })
export class AnimalApiService extends BaseApiService<Animal, CreateAnimalRequest, UpdateAnimalRequest> {
  protected readonly resourcePath = 'animals';

  constructor(http: HttpClient) {
    super(http);
  }

  getLocations(id: number): Observable<Array<LocationPoint>> {
    return this.getAllPagedItems<LocationPoint>(`${this.baseUrl}/${id}/locations`);
  }

  getLocationsPaged(id: number, pagination: PaginationQuery = {}): Observable<PagedResult<LocationPoint>> {
    return this.http.get<PagedResult<LocationPoint>>(`${this.baseUrl}/${id}/locations`, {
      params: this.createPaginationParams(pagination)
    });
  }

  getReports(id: number): Observable<Array<RangerReport>> {
    return this.getAllPagedItems<RangerReport>(`${this.baseUrl}/${id}/reports`);
  }

  getReportsPaged(id: number, pagination: PaginationQuery = {}): Observable<PagedResult<RangerReport>> {
    return this.http.get<PagedResult<RangerReport>>(`${this.baseUrl}/${id}/reports`, {
      params: this.createPaginationParams(pagination)
    });
  }

  getAlerts(id: number): Observable<Array<Alert>> {
    return this.getAllPagedItems<Alert>(`${this.baseUrl}/${id}/alerts`);
  }

  getAlertsPaged(id: number, pagination: PaginationQuery = {}): Observable<PagedResult<Alert>> {
    return this.http.get<PagedResult<Alert>>(`${this.baseUrl}/${id}/alerts`, {
      params: this.createPaginationParams(pagination)
    });
  }
}

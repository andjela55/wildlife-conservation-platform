import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Alert, Animal, CreateAnimalRequest, LocationPoint, RangerReport, UpdateAnimalRequest } from '../models';
import { BaseApiService } from './base-api.service';

@Injectable({ providedIn: 'root' })
export class AnimalApiService extends BaseApiService<Animal, CreateAnimalRequest, UpdateAnimalRequest> {
  protected readonly resourcePath = 'animals';

  constructor(http: HttpClient) {
    super(http);
  }

  getLocations(id: number): Observable<Array<LocationPoint>> {
    return this.http.get<Array<LocationPoint>>(`${this.baseUrl}/${id}/locations`);
  }

  getReports(id: number): Observable<Array<RangerReport>> {
    return this.http.get<Array<RangerReport>>(`${this.baseUrl}/${id}/reports`);
  }

  getAlerts(id: number): Observable<Array<Alert>> {
    return this.http.get<Array<Alert>>(`${this.baseUrl}/${id}/alerts`);
  }
}

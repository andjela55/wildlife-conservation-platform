import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Alert, Animal, CreateAnimalRequest, LocationPoint, RangerReport, UpdateAnimalRequest } from '../models/wildlife.models';
import { BaseApiService } from './base-api.service';

@Injectable({ providedIn: 'root' })
export class AnimalApiService extends BaseApiService<Animal, CreateAnimalRequest, UpdateAnimalRequest> {
  protected readonly resourcePath = 'animals';

  constructor(http: HttpClient) {
    super(http);
  }

  getLocations(id: number): Observable<LocationPoint[]> {
    return this.http.get<LocationPoint[]>(`${this.baseUrl}/${id}/locations`);
  }

  getReports(id: number): Observable<RangerReport[]> {
    return this.http.get<RangerReport[]>(`${this.baseUrl}/${id}/reports`);
  }

  getAlerts(id: number): Observable<Alert[]> {
    return this.http.get<Alert[]>(`${this.baseUrl}/${id}/alerts`);
  }
}

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateLocationPointRequest, LocationPoint } from '../models/wildlife.models';
import { BaseApiService } from './base-api.service';

@Injectable({ providedIn: 'root' })
export class LocationPointApiService extends BaseApiService<LocationPoint, CreateLocationPointRequest> {
  protected readonly resourcePath = 'location-points';

  constructor(http: HttpClient) {
    super(http);
  }

  getLatest(): Observable<LocationPoint[]> {
    return this.http.get<LocationPoint[]>(`${this.baseUrl}/latest`);
  }

  getByAnimal(animalId: number): Observable<LocationPoint[]> {
    return this.http.get<LocationPoint[]>(`${this.baseUrl}/by-animal/${animalId}`);
  }
}

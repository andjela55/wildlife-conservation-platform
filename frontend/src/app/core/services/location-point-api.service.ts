import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { LocationPoint } from '../models/wildlife.models';

@Injectable({ providedIn: 'root' })
export class LocationPointApiService {
  private readonly baseUrl = `${environment.apiUrl}/api/location-points`;

  constructor(private readonly http: HttpClient) {}

  getLatest(): Observable<LocationPoint[]> {
    return this.http.get<LocationPoint[]>(`${this.baseUrl}/latest`);
  }

  getByAnimal(animalId: number): Observable<LocationPoint[]> {
    return this.http.get<LocationPoint[]>(`${this.baseUrl}/by-animal/${animalId}`);
  }
}

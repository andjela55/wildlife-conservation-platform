import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { LocationPoint, PagedResult, PaginationQuery } from '../models';
import { BaseApiService } from './base-api.service';

@Injectable({ providedIn: 'root' })
export class LocationPointApiService extends BaseApiService<LocationPoint, never> {
  protected readonly resourcePath = 'location-points';

  constructor(http: HttpClient) {
    super(http);
  }

  getLatest(): Observable<Array<LocationPoint>> {
    return this.getAllPagedItems<LocationPoint>(`${this.baseUrl}/latest`);
  }

  getByAnimal(animalId: number): Observable<Array<LocationPoint>> {
    return this.getAllPagedItems<LocationPoint>(`${this.baseUrl}/by-animal/${animalId}`);
  }

  getByAnimalPaged(animalId: number, pagination: PaginationQuery = {}): Observable<PagedResult<LocationPoint>> {
    return this.http.get<PagedResult<LocationPoint>>(`${this.baseUrl}/by-animal/${animalId}`, {
      params: this.createPaginationParams(pagination)
    });
  }
}

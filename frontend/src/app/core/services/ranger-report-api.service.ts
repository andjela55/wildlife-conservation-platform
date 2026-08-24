import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateRangerReportRequest, PagedResult, PaginationQuery, RangerReport } from '../models';
import { BaseApiService } from './base-api.service';

@Injectable({ providedIn: 'root' })
export class RangerReportApiService extends BaseApiService<RangerReport, CreateRangerReportRequest> {
  protected readonly resourcePath = 'ranger-reports';

  constructor(http: HttpClient) {
    super(http);
  }

  getByAnimalPaged(animalId: number, pagination: PaginationQuery = {}): Observable<PagedResult<RangerReport>> {
    return this.http.get<PagedResult<RangerReport>>(`${this.baseUrl}/by-animal/${animalId}`, {
      params: this.createPaginationParams(pagination)
    });
  }
}

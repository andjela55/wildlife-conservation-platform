import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateRangerReportRequest, RangerReport } from '../models';
import { BaseApiService } from './base-api.service';

@Injectable({ providedIn: 'root' })
export class RangerReportApiService extends BaseApiService<RangerReport, CreateRangerReportRequest> {
  protected readonly resourcePath = 'ranger-reports';

  constructor(http: HttpClient) {
    super(http);
  }
}

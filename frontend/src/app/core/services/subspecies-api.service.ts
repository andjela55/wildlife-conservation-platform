import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Subspecies, UpsertSubspeciesRequest } from '../models';
import { BaseApiService } from './base-api.service';

@Injectable({ providedIn: 'root' })
export class SubspeciesApiService extends BaseApiService<Subspecies, UpsertSubspeciesRequest> {
  protected readonly resourcePath = 'subspecies';

  constructor(http: HttpClient) {
    super(http);
  }
}

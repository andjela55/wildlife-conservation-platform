import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateSubspeciesRequest, Subspecies } from '../models/wildlife.models';
import { BaseApiService } from './base-api.service';

@Injectable({ providedIn: 'root' })
export class SubspeciesApiService extends BaseApiService<Subspecies, CreateSubspeciesRequest> {
  protected readonly resourcePath = 'subspecies';

  constructor(http: HttpClient) {
    super(http);
  }
}

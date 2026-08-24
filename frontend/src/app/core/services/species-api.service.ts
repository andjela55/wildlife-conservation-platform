import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Species, UpsertSpeciesRequest } from '../models';
import { BaseApiService } from './base-api.service';

@Injectable({ providedIn: 'root' })
export class SpeciesApiService extends BaseApiService<Species, UpsertSpeciesRequest> {
  protected readonly resourcePath = 'species';

  constructor(http: HttpClient) {
    super(http);
  }
}

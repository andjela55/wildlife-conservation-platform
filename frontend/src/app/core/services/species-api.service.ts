import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateSpeciesRequest, Species } from '../models';
import { BaseApiService } from './base-api.service';

@Injectable({ providedIn: 'root' })
export class SpeciesApiService extends BaseApiService<Species, CreateSpeciesRequest> {
  protected readonly resourcePath = 'species';

  constructor(http: HttpClient) {
    super(http);
  }
}

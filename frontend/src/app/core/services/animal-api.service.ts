import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Animal, UpsertAnimalRequest } from '../models';
import { BaseApiService } from './base-api.service';

@Injectable({ providedIn: 'root' })
export class AnimalApiService extends BaseApiService<Animal, UpsertAnimalRequest> {
  protected readonly resourcePath = 'animals';

  constructor(http: HttpClient) {
    super(http);
  }

}

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateUserRequest, UpdateUserAssignedAreaRequest, UpdateUserRequest, User } from '../models';
import { BaseApiService } from './base-api.service';

@Injectable({ providedIn: 'root' })
export class UserApiService extends BaseApiService<User, CreateUserRequest, UpdateUserRequest> {
  protected readonly resourcePath = 'users';

  constructor(http: HttpClient) {
    super(http);
  }

  updateAssignedArea(id: number, request: UpdateUserAssignedAreaRequest): Observable<User> {
    return this.http.put<User>(`${this.baseUrl}/${id}/assigned-area`, request);
  }
}

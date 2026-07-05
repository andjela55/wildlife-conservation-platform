import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Collar, CollarAssignment, CreateCollarAssignmentRequest, CreateCollarRequest, UnassignCollarRequest, UpdateCollarRequest } from '../models';
import { BaseApiService } from './base-api.service';

@Injectable({ providedIn: 'root' })
export class CollarApiService extends BaseApiService<Collar, CreateCollarRequest, UpdateCollarRequest> {
  protected readonly resourcePath = 'collars';
  private readonly assignmentsUrl = `${environment.apiUrl}/api/collar-assignments`;

  constructor(http: HttpClient) {
    super(http);
  }

  assign(request: CreateCollarAssignmentRequest): Observable<CollarAssignment> {
    return this.http.post<CollarAssignment>(this.assignmentsUrl, request);
  }

  getActiveAssignments(): Observable<Array<CollarAssignment>> {
    return this.http.get<Array<CollarAssignment>>(`${this.assignmentsUrl}/active`);
  }

  unassign(id: number, request: UnassignCollarRequest): Observable<CollarAssignment> {
    return this.http.put<CollarAssignment>(`${this.assignmentsUrl}/${id}/unassign`, request);
  }
}

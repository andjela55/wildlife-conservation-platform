import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable, of, switchMap } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Collar, CollarAssignment, CollarAssignmentQuery, CreateCollarAssignmentRequest, UnassignCollarRequest, UpsertCollarRequest } from '../models';
import { BaseApiService } from './base-api.service';

@Injectable({ providedIn: 'root' })
export class CollarApiService extends BaseApiService<Collar, UpsertCollarRequest> {
  protected readonly resourcePath = 'collars';
  private readonly assignmentsUrl = `${environment.apiUrl}/api/collar-assignments`;

  constructor(http: HttpClient) {
    super(http);
  }

  assign(request: CreateCollarAssignmentRequest): Observable<CollarAssignment> {
    return this.http.post<CollarAssignment>(this.assignmentsUrl, request);
  }

  getActiveAssignments(): Observable<Array<CollarAssignment>> {
    return this.getAllPagedItems<CollarAssignment>(`${this.assignmentsUrl}/active`);
  }

  getAssignments(query: CollarAssignmentQuery = {}): Observable<Array<CollarAssignment>> {
    return this.getAssignmentPage(query, 1);
  }

  private getAssignmentPage(query: CollarAssignmentQuery, pageNumber: number): Observable<Array<CollarAssignment>> {
    const params = Object.entries(query).reduce(
      (result, [key, value]) => value === undefined ? result : result.set(key, String(value)),
      this.createPaginationParams({ pageNumber, pageSize: 100 })
    );
    return this.http.get<{ items: Array<CollarAssignment>; pageNumber: number; totalPages: number }>(this.assignmentsUrl, { params }).pipe(
      switchMap((result) => result.pageNumber >= result.totalPages
        ? of(result.items)
        : this.getAssignmentPage(query, pageNumber + 1).pipe(map((items) => [...result.items, ...items])))
    );
  }

  unassign(id: number, request: UnassignCollarRequest): Observable<CollarAssignment> {
    return this.http.put<CollarAssignment>(`${this.assignmentsUrl}/${id}/unassign`, request);
  }
}

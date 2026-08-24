import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { PagedResult, PaginationQuery } from '../models';

@Injectable()
export abstract class BaseApiService<T, TCreate, TUpdate = TCreate> {

  protected abstract readonly resourcePath: string;

  protected get baseUrl(): string {
    return `${environment.apiUrl}/api/${this.resourcePath}`;
  }

  constructor(protected readonly http: HttpClient) {}

  getAll(): Observable<Array<T>> {
    return this.getAllPagedItems<T>(this.baseUrl);
  }

  getPaged(pagination: PaginationQuery = {}): Observable<PagedResult<T>> {
    return this.http.get<PagedResult<T>>(this.baseUrl, {
      params: this.createPaginationParams(pagination)
    });
  }

  create(request: TCreate): Observable<T> {
    return this.http.post<T>(this.baseUrl, request);
  }

  update(id: number, request: TUpdate): Observable<T> {
    return this.http.put<T>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  protected createPaginationParams(pagination: PaginationQuery = {}): HttpParams {
    return new HttpParams()
      .set('pageNumber', String(pagination.pageNumber ?? 1))
      .set('pageSize', String(pagination.pageSize ?? 100));
  }

  protected getAllPagedItems<TItem>(url: string): Observable<Array<TItem>> {
    return this.getPagedItems<TItem>(url, { pageNumber: 1, pageSize: 100 });
  }

  private getPagedItems<TItem>(url: string, pagination: Required<PaginationQuery>): Observable<Array<TItem>> {
    return this.http.get<PagedResult<TItem>>(url, {
      params: this.createPaginationParams(pagination)
    }).pipe(
      switchMap((result) => {
        if (result.pageNumber >= result.totalPages) {
          return of(result.items);
        }

        return this.getPagedItems<TItem>(url, {
          pageNumber: result.pageNumber + 1,
          pageSize: result.pageSize
        }).pipe(map((items) => [...result.items, ...items]));
      })
    );
  }
}

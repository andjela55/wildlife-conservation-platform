import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable()
export abstract class BaseApiService<T, TCreate, TUpdate = TCreate> {

  protected abstract readonly resourcePath: string;

  protected get baseUrl(): string {
    return `${environment.apiUrl}/api/${this.resourcePath}`;
  }

  constructor(protected readonly http: HttpClient) {}

  getAll(): Observable<T[]> {
    return this.http.get<T[]>(this.baseUrl);
  }

  create(request: TCreate): Observable<T> {
    return this.http.post<T>(this.baseUrl, request);
  }

  update(id: number, request: TUpdate): Observable<T> {
    return this.http.put<T>(`${this.baseUrl}/${id}`, request);
  }
}

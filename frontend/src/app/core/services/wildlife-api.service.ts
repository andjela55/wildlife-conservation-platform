import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

import {
  Alert,
  Animal,
  Collar,
  CollarAssignment,
  CreateAlertRequest,
  CreateAnimalRequest,
  CreateCollarAssignmentRequest,
  CreateCollarRequest,
  CreateLocationPointRequest,
  CreateRangerReportRequest,
  CreateSpeciesRequest,
  CreateSubspeciesRequest,
  LocationPoint,
  RangerReport,
  ResolveAlertRequest,
  Species,
  Subspecies,
  UnassignCollarRequest,
  UpdateAnimalRequest,
  UpdateCollarRequest
} from '../models/wildlife.models';

@Injectable({ providedIn: 'root' })
export class WildlifeApiService {

  private readonly baseUrl = `${environment.apiUrl}/api`;

  constructor(private readonly http: HttpClient) {}

  getSpecies(): Observable<Species[]> {
    return this.http.get<Species[]>(`${this.baseUrl}/species`);
  }

  createSpecies(request: CreateSpeciesRequest): Observable<Species> {
    return this.http.post<Species>(`${this.baseUrl}/species`, request);
  }

  getSubspecies(): Observable<Subspecies[]> {
    return this.http.get<Subspecies[]>(`${this.baseUrl}/subspecies`);
  }

  createSubspecies(request: CreateSubspeciesRequest): Observable<Subspecies> {
    return this.http.post<Subspecies>(`${this.baseUrl}/subspecies`, request);
  }

  getAnimals(): Observable<Animal[]> {
    return this.http.get<Animal[]>(`${this.baseUrl}/animals`);
  }

  createAnimal(request: CreateAnimalRequest): Observable<Animal> {
    return this.http.post<Animal>(`${this.baseUrl}/animals`, request);
  }

  updateAnimal(id: number, request: UpdateAnimalRequest): Observable<Animal> {
    return this.http.put<Animal>(`${this.baseUrl}/animals/${id}`, request);
  }

  getAnimalLocations(id: number): Observable<LocationPoint[]> {
    return this.http.get<LocationPoint[]>(`${this.baseUrl}/animals/${id}/locations`);
  }

  getAnimalReports(id: number): Observable<RangerReport[]> {
    return this.http.get<RangerReport[]>(`${this.baseUrl}/animals/${id}/reports`);
  }

  getAnimalAlerts(id: number): Observable<Alert[]> {
    return this.http.get<Alert[]>(`${this.baseUrl}/animals/${id}/alerts`);
  }

  getCollars(): Observable<Collar[]> {
    return this.http.get<Collar[]>(`${this.baseUrl}/collars`);
  }

  createCollar(request: CreateCollarRequest): Observable<Collar> {
    return this.http.post<Collar>(`${this.baseUrl}/collars`, request);
  }

  updateCollar(id: number, request: UpdateCollarRequest): Observable<Collar> {
    return this.http.put<Collar>(`${this.baseUrl}/collars/${id}`, request);
  }

  assignCollar(request: CreateCollarAssignmentRequest): Observable<CollarAssignment> {
    return this.http.post<CollarAssignment>(`${this.baseUrl}/collar-assignments`, request);
  }

  unassignCollar(id: number, request: UnassignCollarRequest): Observable<CollarAssignment> {
    return this.http.put<CollarAssignment>(`${this.baseUrl}/collar-assignments/${id}/unassign`, request);
  }

  createLocationPoint(request: CreateLocationPointRequest): Observable<LocationPoint> {
    return this.http.post<LocationPoint>(`${this.baseUrl}/location-points`, request);
  }

  getLatestLocationPoints(): Observable<LocationPoint[]> {
    return this.http.get<LocationPoint[]>(`${this.baseUrl}/location-points/latest`);
  }

  getLocationPointsByAnimal(animalId: number): Observable<LocationPoint[]> {
    return this.http.get<LocationPoint[]>(`${this.baseUrl}/location-points/by-animal/${animalId}`);
  }

  getRangerReports(): Observable<RangerReport[]> {
    return this.http.get<RangerReport[]>(`${this.baseUrl}/ranger-reports`);
  }

  createRangerReport(request: CreateRangerReportRequest): Observable<RangerReport> {
    return this.http.post<RangerReport>(`${this.baseUrl}/ranger-reports`, request);
  }

  getAlerts(): Observable<Alert[]> {
    return this.http.get<Alert[]>(`${this.baseUrl}/alerts`);
  }

  createAlert(request: CreateAlertRequest): Observable<Alert> {
    return this.http.post<Alert>(`${this.baseUrl}/alerts`, request);
  }

  resolveAlert(id: number, request: ResolveAlertRequest): Observable<Alert> {
    return this.http.put<Alert>(`${this.baseUrl}/alerts/${id}/resolve`, request);
  }
}
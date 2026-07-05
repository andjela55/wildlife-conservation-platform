import { Component, OnDestroy, OnInit } from '@angular/core';
import { catchError, finalize, forkJoin, map, Observable, of, Subject, takeUntil } from 'rxjs';
import { Alert, Animal, Collar, LocationPoint, RangerReport, Severity } from '../../core/models';
import { AlertApiService } from '../../core/services/alert-api.service';
import { AnimalApiService } from '../../core/services/animal-api.service';
import { CollarApiService } from '../../core/services/collar-api.service';
import { LocationPointApiService } from '../../core/services/location-point-api.service';
import { RangerReportApiService } from '../../core/services/ranger-report-api.service';
import { enumEquals, enumKey } from '../../core/utils/enum-utils';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit, OnDestroy {
  animals: Array<Animal> = [];
  collars: Array<Collar> = [];
  latestLocations: Array<LocationPoint> = [];
  reports: Array<RangerReport> = [];
  alerts: Array<Alert> = [];
  isLoading = false;
  errorMessage = '';
  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly animalApi: AnimalApiService,
    private readonly collarApi: CollarApiService,
    private readonly locationPointApi: LocationPointApiService,
    private readonly rangerReportApi: RangerReportApiService,
    private readonly alertApi: AlertApiService
  ) {}

  ngOnInit(): void {
    this.loadData().pipe(takeUntil(this.destroy$)).subscribe();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  get activeAnimalsCount(): number {
    return this.animals.filter((animal) => animal.isActive).length;
  }

  get assignedCollarsCount(): number {
    return this.collars.filter((collar) => enumEquals(collar.status, 'CollarStatus', 'Assigned')).length;
  }

  get unresolvedAlertsCount(): number {
    return this.alerts.filter((alert) => !alert.isResolved).length;
  }

  get criticalAlerts(): Array<Alert> {
    return this.alerts.filter((alert) => !alert.isResolved && enumEquals(alert.severity, 'Severity', 'Critical')).slice(0, 5);
  }

  get recentAlerts(): Array<Alert> {
    return this.alerts.filter((alert) => !alert.isResolved).slice(0, 3);
  }

  get recentReports(): Array<RangerReport> {
    return this.reports.slice(0, 3);
  }

  get topAnimals(): Array<Animal> {
    return this.animals.filter((animal) => animal.isActive).slice(0, 4);
  }

  get mapLocations(): Array<LocationPoint> {
    return this.latestLocations.slice(0, 6);
  }

  mapX(point: LocationPoint): number {
    return Math.min(88, Math.max(8, ((point.longitude + 180) / 360) * 100));
  }

  mapY(point: LocationPoint): number {
    return Math.min(84, Math.max(12, ((90 - point.latitude) / 180) * 100));
  }

  getAnimalName(animalId: number | null | undefined): string {
    if (!animalId) {
      return 'Area report';
    }

    return this.animals.find((animal) => animal.id === animalId)?.name ?? `Animal #${animalId}`;
  }

  getSeverityIcon(severity: Severity): string {
    return `assets/icons/alerts/alarm_${enumKey(severity, 'Severity').toLowerCase()}.svg`;
  }

  getSeverityClass(severity: Severity): string {
    return enumKey(severity, 'Severity').toLowerCase();
  }

  loadData(): Observable<void> {
    this.isLoading = true;
    this.errorMessage = '';

    return forkJoin({
      animals: this.animalApi.getAll(),
      collars: this.collarApi.getAll(),
      latestLocations: this.locationPointApi.getLatest(),
      reports: this.rangerReportApi.getAll(),
      alerts: this.alertApi.getAll()
    })
      .pipe(
        map((result) => this.mapLoadData(result)),
        catchError(() => {
          this.errorMessage = 'Unable to load dashboard data.';
          return of(void 0);
        }),
        finalize(() => (this.isLoading = false))
      );
  }

  private mapLoadData(result: {
    animals: Array<Animal>;
    collars: Array<Collar>;
    latestLocations: Array<LocationPoint>;
    reports: Array<RangerReport>;
    alerts: Array<Alert>;
  }): void {
    this.animals = result.animals;
    this.collars = result.collars;
    this.latestLocations = result.latestLocations;
    this.reports = result.reports;
    this.alerts = result.alerts;
  }
}

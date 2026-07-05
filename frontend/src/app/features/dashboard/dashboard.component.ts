import { Component, OnInit } from '@angular/core';
import { finalize, forkJoin } from 'rxjs';
import { Alert, Animal, Collar, LocationPoint, RangerReport, Severity } from '../../core/models/wildlife.models';
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
export class DashboardComponent implements OnInit {
  animals: Animal[] = [];
  collars: Collar[] = [];
  latestLocations: LocationPoint[] = [];
  reports: RangerReport[] = [];
  alerts: Alert[] = [];
  isLoading = false;
  errorMessage = '';

  constructor(
    private readonly animalApi: AnimalApiService,
    private readonly collarApi: CollarApiService,
    private readonly locationPointApi: LocationPointApiService,
    private readonly rangerReportApi: RangerReportApiService,
    private readonly alertApi: AlertApiService
  ) {}

  ngOnInit(): void {
    this.load();
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

  get criticalAlerts(): Alert[] {
    return this.alerts.filter((alert) => !alert.isResolved && enumEquals(alert.severity, 'Severity', 'Critical')).slice(0, 5);
  }

  get recentAlerts(): Alert[] {
    return this.alerts.filter((alert) => !alert.isResolved).slice(0, 3);
  }

  get recentReports(): RangerReport[] {
    return this.reports.slice(0, 3);
  }

  get topAnimals(): Animal[] {
    return this.animals.filter((animal) => animal.isActive).slice(0, 4);
  }

  get mapLocations(): LocationPoint[] {
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

  load(): void {
    this.isLoading = true;
    this.errorMessage = '';

    forkJoin({
      animals: this.animalApi.getAll(),
      collars: this.collarApi.getAll(),
      latestLocations: this.locationPointApi.getLatest(),
      reports: this.rangerReportApi.getAll(),
      alerts: this.alertApi.getAll()
    })
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (result) => {
          this.animals = result.animals;
          this.collars = result.collars;
          this.latestLocations = result.latestLocations;
          this.reports = result.reports;
          this.alerts = result.alerts;
        },
        error: () => {
          this.errorMessage = 'Unable to load dashboard data.';
        }
      });
  }
}

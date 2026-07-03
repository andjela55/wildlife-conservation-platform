import { Component, OnInit } from '@angular/core';
import { forkJoin, finalize } from 'rxjs';
import { Alert, Animal, Collar, LocationPoint, RangerReport } from '../../core/models/wildlife.models';
import { WildlifeApiService } from '../../core/services/wildlife-api.service';

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

  constructor(private readonly api: WildlifeApiService) {}

  ngOnInit(): void {
    this.load();
  }

  get activeAnimalsCount(): number {
    return this.animals.filter((animal) => animal.isActive).length;
  }

  get assignedCollarsCount(): number {
    return this.collars.filter((collar) => collar.status === 'Assigned').length;
  }

  get unresolvedAlertsCount(): number {
    return this.alerts.filter((alert) => !alert.isResolved).length;
  }

  get criticalAlerts(): Alert[] {
    return this.alerts.filter((alert) => !alert.isResolved && alert.severity === 'Critical').slice(0, 5);
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

  load(): void {
    this.isLoading = true;
    this.errorMessage = '';

    forkJoin({
      animals: this.api.getAnimals(),
      collars: this.api.getCollars(),
      latestLocations: this.api.getLatestLocationPoints(),
      reports: this.api.getRangerReports(),
      alerts: this.api.getAlerts()
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

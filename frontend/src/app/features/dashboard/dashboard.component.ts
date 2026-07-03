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

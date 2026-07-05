import {
  Component,
  ElementRef,
  NgZone,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import * as L from 'leaflet';
import {
  catchError,
  finalize,
  forkJoin,
  map,
  Observable,
  of,
  Subject,
  takeUntil,
} from 'rxjs';
import {
  Alert,
  Animal,
  Collar,
  LocationPoint,
  RangerReport,
  Severity,
} from '../../core/models';
import { AlertApiService } from '../../core/services/alert-api.service';
import { AnimalApiService } from '../../core/services/animal-api.service';
import { CollarApiService } from '../../core/services/collar-api.service';
import { LocationPointApiService } from '../../core/services/location-point-api.service';
import { RangerReportApiService } from '../../core/services/ranger-report-api.service';
import { enumEquals, enumKey } from '../../core/utils/enum-utils';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit, OnDestroy {
  @ViewChild('wildlifeMap') wildlifeMap!: ElementRef<HTMLDivElement>;

  animals: Array<Animal> = [];
  collars: Array<Collar> = [];
  latestLocations: Array<LocationPoint> = [];
  reports: Array<RangerReport> = [];
  alerts: Array<Alert> = [];
  isLoading = false;
  errorMessage = '';
  selectedLocation: LocationPoint | null = null;

  private map?: L.Map;
  private markers = new Map<number, L.Marker>();
  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly animalApi: AnimalApiService,
    private readonly collarApi: CollarApiService,
    private readonly locationPointApi: LocationPointApiService,
    private readonly rangerReportApi: RangerReportApiService,
    private readonly alertApi: AlertApiService,
    private ngZone: NgZone,
  ) {}

  ngOnInit(): void {
    this.loadData().pipe(takeUntil(this.destroy$)).subscribe();
  }
  ngAfterViewInit(): void {
    this.initMap();
    this.renderMarkers(this.mapLocations);
  }

  ngOnDestroy(): void {
    this.map?.remove();
    this.destroy$.next();
    this.destroy$.complete();
  }

  get activeAnimalsCount(): number {
    return this.animals.filter((animal) => animal.isActive).length;
  }

  get assignedCollarsCount(): number {
    return this.collars.filter((collar) =>
      enumEquals(collar.status, 'CollarStatus', 'Assigned'),
    ).length;
  }

  get unresolvedAlertsCount(): number {
    return this.alerts.filter((alert) => !alert.isResolved).length;
  }

  get criticalAlerts(): Array<Alert> {
    return this.alerts
      .filter(
        (alert) =>
          !alert.isResolved &&
          enumEquals(alert.severity, 'Severity', 'Critical'),
      )
      .slice(0, 5);
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

    return (
      this.animals.find((animal) => animal.id === animalId)?.name ??
      `Animal #${animalId}`
    );
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
      alerts: this.alertApi.getAll(),
    }).pipe(
      map((result) => this.mapLoadData(result)),
      catchError(() => {
        this.errorMessage = 'Unable to load dashboard data.';
        return of(void 0);
      }),
      finalize(() => (this.isLoading = false)),
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

    this.renderMarkers(this.mapLocations);
    this.fitToCurrentLocations();
  }
  private initMap(): void {
    if (this.map) {
      return;
    }

    this.map = L.map(this.wildlifeMap.nativeElement, {
      center: [0, 0],
      zoom: 2,
      zoomControl: true,
    });

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors',
      maxZoom: 19,
    }).addTo(this.map);

    this.map.on('click', () => {
      this.ngZone.run(() => {
        this.selectedLocation = null;
      });

      this.map?.closePopup();
    });

    setTimeout(() => {
      this.map?.invalidateSize();
    }, 100);

    setTimeout(() => {
      this.map?.invalidateSize();
    }, 500);
  }
renderMarkers(points: Array<LocationPoint>): void {
  if (!this.map) {
    return;
  }

  this.markers.forEach((marker) => marker.remove());
  this.markers.clear();

  points.forEach((point) => {
    if (!this.isValidLocation(point)) {
      return;
    }

    const marker = L.marker([point.latitude, point.longitude], {
      icon: this.createAnimalMarkerIcon(point),
      bubblingMouseEvents: false,
    });

    marker.bindTooltip(this.getAnimalName(point.animalId), {
      direction: 'top',
      offset: [0, -12],
    });

    marker.on('click', () => {
      this.ngZone.run(() => {
        this.selectedLocation = point;
      });

      this.map?.closePopup();
    });

    marker.addTo(this.map!);
    this.markers.set(point.animalId, marker);
  });
}
  fitToCurrentLocations(): void {
    if (!this.map) {
      return;
    }

    const validLocations = this.mapLocations.filter((point) =>
      this.isValidLocation(point),
    );

    if (!validLocations.length) {
      return;
    }

    if (validLocations.length === 1) {
      const point = validLocations[0];

      this.map.setView([point.latitude, point.longitude], 14);
      return;
    }

    const bounds = L.latLngBounds(
      validLocations.map(
        (point) => [point.latitude, point.longitude] as [number, number],
      ),
    );

    this.map.fitBounds(bounds, {
      padding: [40, 40],
      maxZoom: 14,
    });
  }
  private isValidLocation(point: LocationPoint): boolean {
    return (
      typeof point.latitude === 'number' &&
      typeof point.longitude === 'number' &&
      !Number.isNaN(point.latitude) &&
      !Number.isNaN(point.longitude) &&
      point.latitude >= -90 &&
      point.latitude <= 90 &&
      point.longitude >= -180 &&
      point.longitude <= 180
    );
  }
  private createAnimalMarkerIcon(point: LocationPoint): L.DivIcon {
    const statusClass = this.getSignalStatusClass(point.recordedAt);

    return L.divIcon({
      className: `animal-marker ${statusClass}`,
      html: `
        <span class="animal-marker-pulse"></span>
        <span class="animal-marker-dot"></span>
      `,
      iconSize: [28, 28],
      iconAnchor: [14, 14],
    });
  }
  private getSignalStatusClass(recordedAt: string): string {
    const minutesAgo =
      (Date.now() - new Date(recordedAt).getTime()) / 1000 / 60;

    if (minutesAgo <= 30) {
      return 'signal-fresh';
    }

    if (minutesAgo <= 120) {
      return 'signal-stale';
    }

    return 'signal-old';
  }
  private centerMapOnAssignedArea(): void {
    if (!this.map) return;

    const assignedArea = {
      latitude: 44.7866,
      longitude: 20.4489,
      radiusMeters: 2000,
    };

    this.map.setView([assignedArea.latitude, assignedArea.longitude], 14);

    L.circle([assignedArea.latitude, assignedArea.longitude], {
      radius: assignedArea.radiusMeters,
      color: '#23733d',
      fillColor: '#23733d',
      fillOpacity: 0.08,
      weight: 2,
    }).addTo(this.map);
  }
}

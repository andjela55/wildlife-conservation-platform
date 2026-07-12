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
  LocationPointReceived,
  RangerReport,
  Severity,
} from '../../core/models';
import { AlertApiService } from '../../core/services/alert-api.service';
import { AnimalApiService } from '../../core/services/animal-api.service';
import { AnimalTrackingSignalRService } from '../../core/services/animal-tracking-signal-r.service';
import { CollarApiService } from '../../core/services/collar-api.service';
import { LocationPointApiService } from '../../core/services/location-point-api.service';
import { RangerReportApiService } from '../../core/services/ranger-report-api.service';
import { AuthService } from '../../core/services/auth.service';
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
  private mapResizeFrame?: number;
  private hasAppliedInitialMapView = false;
  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly animalApi: AnimalApiService,
    private readonly collarApi: CollarApiService,
    private readonly locationPointApi: LocationPointApiService,
    private readonly rangerReportApi: RangerReportApiService,
    private readonly alertApi: AlertApiService,
    private readonly animalTrackingSignalR: AnimalTrackingSignalRService,
    private readonly authService: AuthService,
    private ngZone: NgZone,
  ) {}

  ngOnInit(): void {
    this.loadData().pipe(takeUntil(this.destroy$)).subscribe();
    this.animalTrackingSignalR.locationPointReceived$
      .pipe(takeUntil(this.destroy$))
      .subscribe((locationPoint) => {
        this.ngZone.run(() => this.handleLocationPointReceived(locationPoint));
      });

    this.animalTrackingSignalR.start()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        error: () => {
          this.errorMessage = 'Unable to connect to animal tracking updates.';
        }
      });
  }
  ngAfterViewInit(): void {
    this.initMap();
    this.renderMarkers(this.mapLocations);
  }

  ngOnDestroy(): void {
    if (this.mapResizeFrame !== undefined) {
      cancelAnimationFrame(this.mapResizeFrame);
    }

    this.map?.remove();
    this.animalTrackingSignalR.stop().subscribe();
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

  getLocationAnimalName(point: LocationPoint): string {
    return point.animalName || this.getAnimalName(point.animalId);
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
      currentUser: this.authService.refreshCurrentUser(),
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
    currentUser: unknown;
  }): void {
    this.animals = result.animals;
    this.collars = result.collars;
    this.latestLocations = result.latestLocations;
    this.reports = result.reports;
    this.alerts = result.alerts;

    this.renderMarkers(this.mapLocations);
    this.applyInitialMapView();
  }

  private handleLocationPointReceived(locationPoint: LocationPointReceived): void {
    const existingIndex = this.latestLocations.findIndex((point) => point.animalId === locationPoint.animalId);

    if (existingIndex >= 0) {
      this.latestLocations = this.latestLocations.map((point, index) =>
        index === existingIndex ? locationPoint : point);
    } else {
      this.latestLocations = [locationPoint, ...this.latestLocations];
    }

    this.upsertMarker(locationPoint);

    if (this.selectedLocation?.animalId === locationPoint.animalId) {
      this.selectedLocation = locationPoint;
    }
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

    this.mapResizeFrame = requestAnimationFrame(() => {
      this.map?.invalidateSize();
      this.applyInitialMapView();
      this.mapResizeFrame = undefined;
    });
  }
  renderMarkers(points: Array<LocationPoint>): void {
    if (!this.map) {
      return;
    }

    this.markers.forEach((marker) => marker.remove());
    this.markers.clear();

    points.forEach((point) => this.upsertMarker(point));
  }

  private upsertMarker(point: LocationPoint): void {
    if (!this.map || !this.isValidLocation(point)) {
      return;
    }

    const marker = this.markers.get(point.animalId);
    const latLng: L.LatLngExpression = [point.latitude, point.longitude];

    if (marker) {
      marker.setLatLng(latLng);
      marker.setIcon(this.createAnimalMarkerIcon(point));
      marker.bindTooltip(this.getLocationAnimalName(point), {
        direction: 'top',
        offset: [0, -12],
      });
      this.bindMarkerClick(marker, point);
      return;
    }

    const newMarker = L.marker(latLng, {
      icon: this.createAnimalMarkerIcon(point),
      bubblingMouseEvents: false,
    });

    newMarker.bindTooltip(this.getLocationAnimalName(point), {
      direction: 'top',
      offset: [0, -12],
    });

    this.bindMarkerClick(newMarker, point);

    newMarker.addTo(this.map);
    this.markers.set(point.animalId, newMarker);
  }

  private bindMarkerClick(marker: L.Marker, point: LocationPoint): void {
    marker.off('click');
    marker.on('click', () => {
      this.ngZone.run(() => {
        this.selectedLocation = point;
      });

      this.map?.closePopup();
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

  private applyInitialMapView(): void {
    if (!this.map || this.hasAppliedInitialMapView) {
      return;
    }

    if (this.centerMapOnAssignedArea()) {
      this.hasAppliedInitialMapView = true;
      return;
    }

    if (!this.mapLocations.length) {
      return;
    }

    this.fitToCurrentLocations();
    this.hasAppliedInitialMapView = true;
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
  private centerMapOnAssignedArea(): boolean {
    const currentUser = this.authService.currentUser;

    if (
      !this.map ||
      !currentUser ||
      currentUser.assignedLatitude === null ||
      currentUser.assignedLatitude === undefined ||
      currentUser.assignedLongitude === null ||
      currentUser.assignedLongitude === undefined
    ) {
      return false;
    }

    this.map.setView(
      [currentUser.assignedLatitude, currentUser.assignedLongitude],
      currentUser.assignedMapZoom ?? 11,
    );
    return true;
  }
}

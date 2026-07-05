import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { catchError, finalize, forkJoin, map, Observable, of, Subject, takeUntil } from 'rxjs';
import { Alert, alertTypeOptions, Animal, Collar, CreateAlertRequest, Severity, severityOptions } from '../../core/models';
import { AlertApiService } from '../../core/services/alert-api.service';
import { AnimalApiService } from '../../core/services/animal-api.service';
import { CollarApiService } from '../../core/services/collar-api.service';
import { CurrentUserService } from '../../core/services/current-user.service';
import { localDateTimeInputToIso, toLocalDateTimeInputValue } from '../../core/utils/date-utils';
import { enumKey } from '../../core/utils/enum-utils';

@Component({
  selector: 'app-alerts',
  templateUrl: './alerts.component.html',
  styleUrls: ['./alerts.component.scss']
})
export class AlertsComponent implements OnInit, OnDestroy {
  alerts: Array<Alert> = [];
  animals: Array<Animal> = [];
  collars: Array<Collar> = [];
  alertColumns: Array<string> = ['animal', 'collar', 'type', 'severity', 'status', 'actions'];
  alertTypeOptions = alertTypeOptions;
  severityOptions = severityOptions;
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  private readonly destroy$ = new Subject<void>();

  alertForm = this.fb.group({
    animalId: [null, [Validators.required, Validators.min(1)]],
    collarId: [null],
    alertType: ['Manual', Validators.required],
    severity: ['Medium', Validators.required],
    description: ['', [Validators.required, Validators.maxLength(2000)]],
    createdAt: [toLocalDateTimeInputValue(), Validators.required]
  });

  constructor(
    private readonly alertApi: AlertApiService,
    private readonly animalApi: AnimalApiService,
    private readonly collarApi: CollarApiService,
    private readonly currentUser: CurrentUserService,
    private readonly fb: UntypedFormBuilder
  ) {}

  ngOnInit(): void {
    this.loadData().pipe(takeUntil(this.destroy$)).subscribe();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  getAnimalName(animalId: number): string {
    return this.animals.find((animal) => animal.id === animalId)?.name ?? `Animal #${animalId}`;
  }

  getCollarSerial(collarId: number | null): string {
    if (!collarId) {
      return '-';
    }

    return this.collars.find((collar) => collar.id === collarId)?.serialNumber ?? `Collar #${collarId}`;
  }

  getSeverityClass(severity: Severity): string {
    return enumKey(severity, 'Severity').toLowerCase();
  }

  refresh(): void {
    this.loadData().pipe(takeUntil(this.destroy$)).subscribe();
  }

  loadData(): Observable<void> {
    this.isLoading = true;
    this.errorMessage = '';

    return forkJoin({
      alerts: this.alertApi.getAll(),
      animals: this.animalApi.getAll(),
      collars: this.collarApi.getAll()
    })
      .pipe(
        map((result) => this.mapLoadData(result)),
        catchError(() => {
          this.errorMessage = 'Unable to load alerts.';
          return of(void 0);
        }),
        finalize(() => (this.isLoading = false))
      );
  }

  createAlert(): void {
    if (this.alertForm.invalid) {
      this.alertForm.markAllAsTouched();
      return;
    }

    this.alertApi
      .create(this.mapCreateAlertRequest())
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.successMessage = 'Alert created.';
          this.alertForm.reset({
            animalId: null,
            collarId: null,
            alertType: 'Manual',
            severity: 'Medium',
            createdAt: toLocalDateTimeInputValue()
          });
          this.refresh();
        },
        error: () => {
          this.errorMessage = 'Unable to create alert.';
        }
      });
  }

  resolveAlert(alert: Alert): void {
    this.alertApi.resolve(alert.id, { resolvedAt: new Date().toISOString() }).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.successMessage = 'Alert resolved.';
        this.refresh();
      },
      error: () => {
        this.errorMessage = 'Unable to resolve alert.';
      }
    });
  }

  private mapLoadData(result: { alerts: Array<Alert>; animals: Array<Animal>; collars: Array<Collar> }): void {
    this.alerts = result.alerts;
    this.animals = result.animals;
    this.collars = result.collars;
  }

  private mapCreateAlertRequest(): CreateAlertRequest {
    const value = this.alertForm.getRawValue();
    return {
      animalId: value.animalId,
      collarId: value.collarId || null,
      createdByUserId: this.currentUser.userId,
      alertType: value.alertType,
      severity: value.severity,
      description: value.description,
      createdAt: localDateTimeInputToIso(value.createdAt)
    };
  }
}

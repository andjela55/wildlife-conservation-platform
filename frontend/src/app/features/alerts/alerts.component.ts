import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { PageEvent } from '@angular/material/paginator';
import { catchError, finalize, forkJoin, map, Observable, of, Subject, takeUntil } from 'rxjs';
import { Alert, alertTypeOptions, Animal, Collar, CollarAssignment, CreateAlertRequest, PagedResult, Severity, severityOptions } from '../../core/models';
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
  activeAssignments: Array<CollarAssignment> = [];
  alertColumns: Array<string> = ['animal', 'collar', 'type', 'severity', 'status', 'actions'];
  alertTypeOptions = alertTypeOptions;
  severityOptions = severityOptions;
  pageSizeOptions: Array<number> = [5, 10, 20];
  alertPageIndex = 0;
  alertPageSize = 10;
  alertsTotalCount = 0;
  isLoading = false;
  errorMessage = '';
  alertFormErrorMessage = '';
  successMessage = '';
  private readonly destroy$ = new Subject<void>();

  alertForm = this.fb.group({
    animalId: [null, [Validators.required, Validators.min(1)]],
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

  getSelectedAnimalCollarSerial(): string {
    const animalId = this.alertForm.getRawValue().animalId;

    if (!animalId) {
      return 'Select animal first';
    }

    const assignment = this.activeAssignments.find((x) => x.animalId === animalId);

    return assignment ? this.getCollarSerial(assignment.collarId) : 'No active collar assigned';
  }

  getSeverityClass(severity: Severity): string {
    return enumKey(severity, 'Severity').toLowerCase();
  }

  refresh(): void {
    this.loadData().pipe(takeUntil(this.destroy$)).subscribe();
  }

  onAlertPageChanged(event: PageEvent): void {
    this.alertPageIndex = event.pageIndex;
    this.alertPageSize = event.pageSize;
    this.refresh();
  }

  loadData(): Observable<void> {
    this.isLoading = true;
    this.errorMessage = '';

    return forkJoin({
      alerts: this.alertApi.getPaged({
        pageNumber: this.alertPageIndex + 1,
        pageSize: this.alertPageSize
      }),
      animals: this.animalApi.getAll(),
      collars: this.collarApi.getAll(),
      activeAssignments: this.collarApi.getActiveAssignments()
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
    this.alertFormErrorMessage = '';
    this.successMessage = '';

    if (this.alertForm.invalid) {
      this.alertForm.markAllAsTouched();
      this.alertFormErrorMessage = 'Please fix the highlighted fields.';
      return;
    }

    this.alertApi
      .create(this.mapCreateAlertRequest())
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.successMessage = 'Alert created.';
          this.alertFormErrorMessage = '';
          this.alertForm.reset({
            animalId: null,
            alertType: 'Manual',
            severity: 'Medium',
            createdAt: toLocalDateTimeInputValue()
          });
          this.refresh();
        },
        error: () => {
          this.alertFormErrorMessage = 'Unable to create alert.';
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

  private mapLoadData(result: {
    alerts: PagedResult<Alert>;
    animals: Array<Animal>;
    collars: Array<Collar>;
    activeAssignments: Array<CollarAssignment>;
  }): void {
    this.alerts = result.alerts.items;
    this.alertsTotalCount = result.alerts.totalCount;
    this.alertPageIndex = result.alerts.pageNumber - 1;
    this.alertPageSize = result.alerts.pageSize;
    this.animals = result.animals;
    this.collars = result.collars;
    this.activeAssignments = result.activeAssignments;
  }

  private mapCreateAlertRequest(): CreateAlertRequest {
    const value = this.alertForm.getRawValue();
    return {
      animalId: value.animalId,
      createdByUserId: this.currentUser.userId,
      alertType: value.alertType,
      severity: value.severity,
      description: value.description,
      createdAt: localDateTimeInputToIso(value.createdAt)
    };
  }
}

import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { PageEvent } from '@angular/material/paginator';
import { catchError, finalize, forkJoin, map, Observable, of, Subject, takeUntil } from 'rxjs';
import { Alert, alertTypeOptions, Animal, Collar, CollarAssignment, CreateAlertRequest, PagedResult, PermissionCodes, severityOptions } from '../../core/models';
import { AlertApiService } from '../../core/services/alert-api.service';
import { AnimalApiService } from '../../core/services/animal-api.service';
import { CollarApiService } from '../../core/services/collar-api.service';
import { enumKey } from '../../core/utils/enum-utils';
import { SearchableSelectOption } from '../../shared/searchable-select/searchable-select.component';
import { AuthService } from '../../core/services/auth.service';

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
  animalOptions: Array<SearchableSelectOption> = [];
  animalNames: Record<number, string> = {};
  collarSerials: Record<number, string> = {};
  severityClasses: Record<number, string> = {};
  selectedAnimalCollarSerial = 'Select animal first';
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
    description: ['', [Validators.required, Validators.maxLength(2000)]]
  });

  constructor(
    private readonly alertApi: AlertApiService,
    private readonly animalApi: AnimalApiService,
    private readonly collarApi: CollarApiService,
    private readonly authService: AuthService,
    private readonly fb: UntypedFormBuilder
  ) {}

  canManageAlerts = false;

  ngOnInit(): void {
    this.canManageAlerts = this.authService.hasPermission(PermissionCodes.AlertsWrite);
    if (!this.canManageAlerts) {
      this.alertColumns = ['animal', 'collar', 'type', 'severity', 'status'];
    }

    this.loadData().pipe(takeUntil(this.destroy$)).subscribe();
    this.alertForm.controls['animalId'].valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe((animalId) => this.updateSelectedAnimalCollarSerial(animalId));
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
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
            severity: 'Medium'
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
    this.animalOptions = this.animals.map((animal) => ({ value: animal.id, label: animal.name }));
    this.animalNames = Object.fromEntries(this.animals.map((animal) => [animal.id, animal.name]));
    this.collarSerials = Object.fromEntries(this.collars.map((collar) => [collar.id, collar.serialNumber]));
    this.severityClasses = Object.fromEntries(
      this.alerts.map((alert) => [alert.id, enumKey(alert.severity, 'Severity').toLowerCase()])
    );
    this.updateSelectedAnimalCollarSerial(this.alertForm.getRawValue().animalId);
  }

  private mapCreateAlertRequest(): CreateAlertRequest {
    const value = this.alertForm.getRawValue();
    return {
      animalId: value.animalId,
      alertType: value.alertType,
      severity: value.severity,
      description: value.description
    };
  }

  private updateSelectedAnimalCollarSerial(animalId: number | null): void {
    if (!animalId) {
      this.selectedAnimalCollarSerial = 'Select animal first';
      return;
    }

    const assignment = this.activeAssignments.find((item) => item.animalId === animalId);
    this.selectedAnimalCollarSerial = assignment
      ? this.collarSerials[assignment.collarId] ?? `Collar #${assignment.collarId}`
      : 'No active collar assigned';
  }
}

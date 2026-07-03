import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { finalize, forkJoin } from 'rxjs';
import { Alert, alertTypeOptions, Animal, Collar, severityOptions } from '../../core/models/wildlife.models';
import { WildlifeApiService } from '../../core/services/wildlife-api.service';
import { localDateTimeInputToIso, toLocalDateTimeInputValue } from '../../core/utils/date-utils';

@Component({
  selector: 'app-alerts',
  templateUrl: './alerts.component.html',
  styleUrls: ['./alerts.component.scss']
})
export class AlertsComponent implements OnInit {
  alerts: Alert[] = [];
  animals: Animal[] = [];
  collars: Collar[] = [];
  alertTypeOptions = alertTypeOptions;
  severityOptions = severityOptions;
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  alertForm = this.fb.group({
    animalId: [null, [Validators.required, Validators.min(1)]],
    collarId: [null],
    createdByUserId: [null],
    alertType: ['Manual', Validators.required],
    severity: ['Medium', Validators.required],
    description: ['', [Validators.required, Validators.maxLength(2000)]],
    createdAt: [toLocalDateTimeInputValue(), Validators.required]
  });

  constructor(
    private readonly api: WildlifeApiService,
    private readonly fb: UntypedFormBuilder
  ) {}

  ngOnInit(): void {
    this.load();
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

  load(): void {
    this.isLoading = true;
    this.errorMessage = '';

    forkJoin({
      alerts: this.api.getAlerts(),
      animals: this.api.getAnimals(),
      collars: this.api.getCollars()
    })
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (result) => {
          this.alerts = result.alerts;
          this.animals = result.animals;
          this.collars = result.collars;
        },
        error: () => {
          this.errorMessage = 'Unable to load alerts.';
        }
      });
  }

  createAlert(): void {
    if (this.alertForm.invalid) {
      this.alertForm.markAllAsTouched();
      return;
    }

    const value = this.alertForm.getRawValue();
    this.api
      .createAlert({
        animalId: value.animalId,
        collarId: value.collarId || null,
        createdByUserId: value.createdByUserId || null,
        alertType: value.alertType,
        severity: value.severity,
        description: value.description,
        createdAt: localDateTimeInputToIso(value.createdAt)
      })
      .subscribe({
        next: () => {
          this.successMessage = 'Alert created.';
          this.alertForm.reset({
            animalId: null,
            collarId: null,
            createdByUserId: null,
            alertType: 'Manual',
            severity: 'Medium',
            createdAt: toLocalDateTimeInputValue()
          });
          this.load();
        },
        error: () => {
          this.errorMessage = 'Unable to create alert.';
        }
      });
  }

  resolveAlert(alert: Alert): void {
    this.api.resolveAlert(alert.id, { resolvedAt: new Date().toISOString() }).subscribe({
      next: () => {
        this.successMessage = 'Alert resolved.';
        this.load();
      },
      error: () => {
        this.errorMessage = 'Unable to resolve alert.';
      }
    });
  }
}

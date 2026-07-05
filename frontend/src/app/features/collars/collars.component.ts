import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { catchError, finalize, forkJoin, map, Observable, of, Subject, takeUntil } from 'rxjs';
import {
  Animal,
  Collar,
  CollarAssignment,
  collarStatusOptions,
  CreateCollarAssignmentRequest,
  CreateCollarRequest,
  UnassignCollarRequest
} from '../../core/models';
import { AnimalApiService } from '../../core/services/animal-api.service';
import { CollarApiService } from '../../core/services/collar-api.service';
import { localDateTimeInputToIso, toLocalDateTimeInputValue } from '../../core/utils/date-utils';

@Component({
  selector: 'app-collars',
  templateUrl: './collars.component.html',
  styleUrls: ['./collars.component.scss']
})
export class CollarsComponent implements OnInit, OnDestroy {
  collars: Array<Collar> = [];
  animals: Array<Animal> = [];
  activeAssignments: Array<CollarAssignment> = [];
  collarColumns: Array<string> = ['serialNumber', 'model', 'manufacturer', 'status'];
  collarStatusOptions = collarStatusOptions;
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  private readonly destroy$ = new Subject<void>();

  collarForm = this.fb.group({
    serialNumber: ['', [Validators.required, Validators.maxLength(120)]],
    model: ['', Validators.maxLength(120)],
    manufacturer: ['', Validators.maxLength(120)],
    status: ['Available', Validators.required],
    notes: ['', Validators.maxLength(1000)]
  });

  assignmentForm = this.fb.group({
    animalId: [null, [Validators.required, Validators.min(1)]],
    collarId: [null, [Validators.required, Validators.min(1)]],
    assignedAt: [toLocalDateTimeInputValue(), Validators.required],
    reason: ['', Validators.maxLength(250)],
    notes: ['', Validators.maxLength(1000)]
  });

  unassignForm = this.fb.group({
    assignmentId: [null, [Validators.required, Validators.min(1)]],
    unassignedAt: [toLocalDateTimeInputValue()],
    reason: ['', Validators.maxLength(250)],
    notes: ['', Validators.maxLength(1000)]
  });

  constructor(
    private readonly collarApi: CollarApiService,
    private readonly animalApi: AnimalApiService,
    private readonly fb: UntypedFormBuilder
  ) {}

  ngOnInit(): void {
    this.loadData().pipe(takeUntil(this.destroy$)).subscribe();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  refresh(): void {
    this.loadData().pipe(takeUntil(this.destroy$)).subscribe();
  }

  loadData(): Observable<void> {
    this.isLoading = true;
    this.errorMessage = '';

    return forkJoin({
      collars: this.collarApi.getAll(),
      animals: this.animalApi.getAll(),
      activeAssignments: this.collarApi.getActiveAssignments()
    })
      .pipe(
        map((result) => this.mapLoadData(result)),
        catchError(() => {
          this.errorMessage = 'Unable to load collars.';
          return of(void 0);
        }),
        finalize(() => (this.isLoading = false))
      );
  }

  getAnimalName(animalId: number): string {
    return this.animals.find((animal) => animal.id === animalId)?.name ?? `Animal #${animalId}`;
  }

  getCollarSerial(collarId: number): string {
    return this.collars.find((collar) => collar.id === collarId)?.serialNumber ?? `Collar #${collarId}`;
  }

  getAssignmentLabel(assignment: CollarAssignment): string {
    const assignedAt = new Date(assignment.assignedAt).toLocaleString();
    return `${this.getAnimalName(assignment.animalId)} - ${this.getCollarSerial(assignment.collarId)} - ${assignedAt}`;
  }

  createCollar(): void {
    if (this.collarForm.invalid) {
      this.collarForm.markAllAsTouched();
      return;
    }

    this.collarApi
      .create(this.mapCreateCollarRequest())
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.successMessage = 'Collar created.';
          this.collarForm.reset({ status: 'Available' });
          this.refresh();
        },
        error: () => {
          this.errorMessage = 'Unable to create collar.';
        }
      });
  }

  assignCollar(): void {
    if (this.assignmentForm.invalid) {
      this.assignmentForm.markAllAsTouched();
      return;
    }

    this.collarApi
      .assign(this.mapAssignCollarRequest())
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.successMessage = 'Collar assigned.';
          this.assignmentForm.reset({ assignedAt: toLocalDateTimeInputValue() });
          this.refresh();
        },
        error: () => {
          this.errorMessage = 'Unable to assign collar.';
        }
      });
  }

  unassignCollar(): void {
    if (this.unassignForm.invalid) {
      this.unassignForm.markAllAsTouched();
      return;
    }

    const value = this.unassignForm.getRawValue();
    this.collarApi
      .unassign(value.assignmentId, this.mapUnassignCollarRequest())
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.successMessage = 'Collar unassigned.';
          this.unassignForm.reset({ unassignedAt: toLocalDateTimeInputValue() });
          this.refresh();
        },
        error: () => {
          this.errorMessage = 'Unable to unassign collar.';
        }
      });
  }

  private mapLoadData(result: {
    collars: Array<Collar>;
    animals: Array<Animal>;
    activeAssignments: Array<CollarAssignment>;
  }): void {
    this.collars = result.collars;
    this.animals = result.animals;
    this.activeAssignments = result.activeAssignments;
  }

  private mapCreateCollarRequest(): CreateCollarRequest {
    const value = this.collarForm.getRawValue();
    return {
      serialNumber: value.serialNumber,
      model: value.model || null,
      manufacturer: value.manufacturer || null,
      status: value.status,
      notes: value.notes || null
    };
  }

  private mapAssignCollarRequest(): CreateCollarAssignmentRequest {
    const value = this.assignmentForm.getRawValue();
    return {
      animalId: value.animalId,
      collarId: value.collarId,
      assignedAt: localDateTimeInputToIso(value.assignedAt),
      reason: value.reason || null,
      notes: value.notes || null
    };
  }

  private mapUnassignCollarRequest(): UnassignCollarRequest {
    const value = this.unassignForm.getRawValue();
    return {
      unassignedAt: localDateTimeInputToIso(value.unassignedAt),
      reason: value.reason || null,
      notes: value.notes || null
    };
  }
}

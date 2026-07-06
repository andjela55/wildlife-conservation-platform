import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { PageEvent } from '@angular/material/paginator';
import { catchError, finalize, forkJoin, map, Observable, of, Subject, takeUntil } from 'rxjs';
import {
  Animal,
  Collar,
  CollarAssignment,
  collarStatusOptions,
  CreateCollarAssignmentRequest,
  CreateCollarRequest,
  PagedResult,
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
  collarOptions: Array<Collar> = [];
  animals: Array<Animal> = [];
  activeAssignments: Array<CollarAssignment> = [];
  collarColumns: Array<string> = ['serialNumber', 'model', 'manufacturer', 'status'];
  collarStatusOptions = collarStatusOptions;
  pageSizeOptions: Array<number> = [5, 10, 20];
  collarPageIndex = 0;
  collarPageSize = 10;
  collarsTotalCount = 0;
  isLoading = false;
  errorMessage = '';
  collarFormErrorMessage = '';
  assignmentFormErrorMessage = '';
  unassignFormErrorMessage = '';
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

  onCollarPageChanged(event: PageEvent): void {
    this.collarPageIndex = event.pageIndex;
    this.collarPageSize = event.pageSize;
    this.refresh();
  }

  loadData(): Observable<void> {
    this.isLoading = true;
    this.errorMessage = '';

    return forkJoin({
      collars: this.collarApi.getPaged({
        pageNumber: this.collarPageIndex + 1,
        pageSize: this.collarPageSize
      }),
      collarOptions: this.collarApi.getAll(),
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
    return this.collarOptions.find((collar) => collar.id === collarId)?.serialNumber ?? `Collar #${collarId}`;
  }

  getAssignmentLabel(assignment: CollarAssignment): string {
    const assignedAt = new Date(assignment.assignedAt).toLocaleString();
    return `${this.getAnimalName(assignment.animalId)} - ${this.getCollarSerial(assignment.collarId)} - ${assignedAt}`;
  }

  createCollar(): void {
    this.collarFormErrorMessage = '';
    this.successMessage = '';

    if (this.collarForm.invalid) {
      this.collarForm.markAllAsTouched();
      this.collarFormErrorMessage = 'Please fix the highlighted fields.';
      return;
    }

    this.collarApi
      .create(this.mapCreateCollarRequest())
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.successMessage = 'Collar created.';
          this.collarFormErrorMessage = '';
          this.collarForm.reset({ status: 'Available' });
          this.refresh();
        },
        error: () => {
          this.collarFormErrorMessage = 'Unable to create collar.';
        }
      });
  }

  assignCollar(): void {
    this.assignmentFormErrorMessage = '';
    this.successMessage = '';

    if (this.assignmentForm.invalid) {
      this.assignmentForm.markAllAsTouched();
      this.assignmentFormErrorMessage = 'Please fix the highlighted fields.';
      return;
    }

    this.collarApi
      .assign(this.mapAssignCollarRequest())
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.successMessage = 'Collar assigned.';
          this.assignmentFormErrorMessage = '';
          this.assignmentForm.reset({ assignedAt: toLocalDateTimeInputValue() });
          this.refresh();
        },
        error: () => {
          this.assignmentFormErrorMessage = 'Unable to assign collar.';
        }
      });
  }

  unassignCollar(): void {
    this.unassignFormErrorMessage = '';
    this.successMessage = '';

    if (this.unassignForm.invalid) {
      this.unassignForm.markAllAsTouched();
      this.unassignFormErrorMessage = 'Please fix the highlighted fields.';
      return;
    }

    const value = this.unassignForm.getRawValue();
    this.collarApi
      .unassign(value.assignmentId, this.mapUnassignCollarRequest())
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.successMessage = 'Collar unassigned.';
          this.unassignFormErrorMessage = '';
          this.unassignForm.reset({ unassignedAt: toLocalDateTimeInputValue() });
          this.refresh();
        },
        error: () => {
          this.unassignFormErrorMessage = 'Unable to unassign collar.';
        }
      });
  }

  private mapLoadData(result: {
    collars: PagedResult<Collar>;
    collarOptions: Array<Collar>;
    animals: Array<Animal>;
    activeAssignments: Array<CollarAssignment>;
  }): void {
    this.collars = result.collars.items;
    this.collarOptions = result.collarOptions;
    this.collarsTotalCount = result.collars.totalCount;
    this.collarPageIndex = result.collars.pageNumber - 1;
    this.collarPageSize = result.collars.pageSize;
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

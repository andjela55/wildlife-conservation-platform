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
  PermissionCodes,
  UpsertCollarRequest
} from '../../core/models';
import { AnimalApiService } from '../../core/services/animal-api.service';
import { AuthService } from '../../core/services/auth.service';
import { CollarApiService } from '../../core/services/collar-api.service';
import { localDateBoundaryToIso, localDateTimeInputToIso, toLocalDateTimeInputValue } from '../../core/utils/date-utils';
import { SearchableSelectOption } from '../../shared/searchable-select/searchable-select.component';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../../shared/confirm-dialog/confirm-dialog.component';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-collars',
  templateUrl: './collars.component.html',
  styleUrls: ['./collars.component.scss']
})
export class CollarsComponent implements OnInit, OnDestroy {
  collars: Array<Collar> = [];
  collarOptions: Array<Collar> = [];
  animals: Array<Animal> = [];
  assignments: Array<CollarAssignment> = [];
  animalOptions: Array<SearchableSelectOption> = [];
  availableCollarOptions: Array<SearchableSelectOption> = [];
  animalNames: Record<number, string> = {};
  collarSerials: Record<number, string> = {};
  collarColumns: Array<string> = ['serialNumber', 'model', 'manufacturer', 'status', 'actions'];
  assignmentColumns: Array<string> = ['animal', 'collar', 'assignedAt', 'unassignedAt', 'actions'];
  collarStatusOptions = collarStatusOptions;
  pageSizeOptions: Array<number> = [5, 10, 20];
  collarPageIndex = 0;
  collarPageSize = 10;
  collarsTotalCount = 0;
  workflowTabIndex = 0;
  isLoading = false;
  errorMessage = '';
  collarFormErrorMessage = '';
  assignmentFormErrorMessage = '';
  assignmentFilterErrorMessage = '';
  unassignFormErrorMessage = '';
  successMessage = '';
  editingCollarId: number | null = null;
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

  assignmentFilterForm = this.fb.group({
    assignedFrom: [null],
    assignedTo: [null]
  });

  constructor(
    private readonly collarApi: CollarApiService,
    private readonly animalApi: AnimalApiService,
    private readonly authService: AuthService,
    private readonly dialog: MatDialog,
    private readonly fb: UntypedFormBuilder
  ) {}

  canManageCollars = false;
  canManageAssignments = false;

  ngOnInit(): void {
    this.canManageCollars = this.authService.hasPermission(PermissionCodes.CollarsWrite);
    this.canManageAssignments = this.authService.hasPermission(PermissionCodes.CollarAssignmentsWrite);
    if (!this.canManageCollars) {
      this.collarColumns = ['serialNumber', 'model', 'manufacturer', 'status'];
    }
    if (!this.canManageAssignments) {
      this.assignmentColumns = ['animal', 'collar', 'assignedAt', 'unassignedAt'];
    }

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
    this.applyCollarPage();
  }

  loadData(): Observable<void> {
    this.isLoading = true;
    this.errorMessage = '';

    return forkJoin({
      collarOptions: this.collarApi.getAll(),
      animals: this.animalApi.getAll(),
      assignments: this.collarApi.getAssignments(this.mapAssignmentFilter())
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

  createCollar(): void {
    this.collarFormErrorMessage = '';
    this.successMessage = '';

    if (this.collarForm.invalid) {
      this.collarForm.markAllAsTouched();
      this.collarFormErrorMessage = 'Please fix the highlighted fields.';
      return;
    }

    const request = this.mapUpsertCollarRequest();
    const operation = this.editingCollarId === null
      ? this.collarApi.create(request)
      : this.collarApi.update(this.editingCollarId, request);
    operation
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.successMessage = this.editingCollarId === null ? 'Collar created.' : 'Collar updated.';
          this.collarFormErrorMessage = '';
          this.collarForm.reset({ status: 'Available' });
          this.editingCollarId = null;
          this.refresh();
        },
        error: () => {
          this.collarFormErrorMessage = 'Unable to save collar.';
        }
      });
  }

  editCollar(collar: Collar): void {
    this.editingCollarId = collar.id;
    this.workflowTabIndex = 0;
    this.collarForm.reset({
      serialNumber: collar.serialNumber,
      model: collar.model ?? '',
      manufacturer: collar.manufacturer ?? '',
      status: collar.status,
      notes: collar.notes ?? ''
    });
  }

  deleteCollar(collar: Collar): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, { data: {
      title: 'Delete collar', message: `Delete collar "${collar.serialNumber}"?`
    }, panelClass: 'confirm-dialog-panel' });
    dialogRef.componentInstance.getConfirm().pipe(take(1), takeUntil(this.destroy$)).subscribe(() => this.collarApi.delete(collar.id).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => { this.successMessage = 'Collar deleted.'; this.refresh(); },
      error: () => this.errorMessage = 'Unable to delete collar.'
    }));
  }

  cancelCollarEdit(): void {
    this.editingCollarId = null;
    this.collarForm.reset({ status: 'Available' });
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

  unassignCollar(assignment: CollarAssignment): void {
    this.unassignFormErrorMessage = '';
    this.successMessage = '';

    this.collarApi
      .unassign(assignment.id, { unassignedAt: null, reason: null, notes: null })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.successMessage = 'Collar unassigned.';
          this.unassignFormErrorMessage = '';
          this.refresh();
        },
        error: () => {
          this.unassignFormErrorMessage = 'Unable to unassign collar.';
        }
      });
  }

  applyAssignmentFilter(): void {
    this.assignmentFilterErrorMessage = '';
    const value = this.assignmentFilterForm.getRawValue();
    if (value.assignedFrom && value.assignedTo && new Date(value.assignedFrom) > new Date(value.assignedTo)) {
      this.assignmentFilterErrorMessage = 'Assigned from cannot be later than Assigned to.';
      return;
    }
    this.refresh();
  }

  clearAssignmentFilter(): void {
    this.assignmentFilterErrorMessage = '';
    this.assignmentFilterForm.reset();
    this.refresh();
  }

  private mapLoadData(result: {
    collarOptions: Array<Collar>;
    animals: Array<Animal>;
    assignments: Array<CollarAssignment>;
  }): void {
    this.collarOptions = result.collarOptions;
    this.collarsTotalCount = result.collarOptions.length;
    this.applyCollarPage();
    this.animals = result.animals;
    this.assignments = result.assignments;
    this.animalOptions = this.animals.map((animal) => ({ value: animal.id, label: animal.name }));
    this.availableCollarOptions = this.collarOptions
      .filter((collar) => collar.status === 'Available')
      .map((collar) => ({ value: collar.id, label: collar.serialNumber }));
    this.animalNames = Object.fromEntries(this.animals.map((animal) => [animal.id, animal.name]));
    this.collarSerials = Object.fromEntries(this.collarOptions.map((collar) => [collar.id, collar.serialNumber]));
  }

  private applyCollarPage(): void {
    const start = this.collarPageIndex * this.collarPageSize;
    this.collars = this.collarOptions.slice(start, start + this.collarPageSize);
  }

  private mapUpsertCollarRequest(): UpsertCollarRequest {
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

  private mapAssignmentFilter(): { assignedFrom?: string; assignedTo?: string } {
    const value = this.assignmentFilterForm.getRawValue();
    return {
      assignedFrom: value.assignedFrom ? localDateBoundaryToIso(value.assignedFrom, false) : undefined,
      assignedTo: value.assignedTo ? localDateBoundaryToIso(value.assignedTo, true) : undefined
    };
  }
}

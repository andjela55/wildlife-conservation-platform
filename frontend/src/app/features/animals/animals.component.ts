import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { PageEvent } from '@angular/material/paginator';
import { catchError, finalize, forkJoin, map, Observable, of, Subject, takeUntil } from 'rxjs';
import {
    Alert,
    Animal,
    animalSexOptions,
    LocationPoint,
    PagedResult,
    RangerReport,
    Collar,
    CollarAssignment,
    Subspecies,
    PermissionCodes,
    UpsertAnimalRequest
} from '../../core/models';
import { AnimalApiService } from '../../core/services/animal-api.service';
import { AlertApiService } from '../../core/services/alert-api.service';
import { LocationPointApiService } from '../../core/services/location-point-api.service';
import { RangerReportApiService } from '../../core/services/ranger-report-api.service';
import { SubspeciesApiService } from '../../core/services/subspecies-api.service';
import { localDateInputToIso } from '../../core/utils/date-utils';
import { CollarApiService } from '../../core/services/collar-api.service';
import { SearchableSelectOption } from '../../shared/searchable-select/searchable-select.component';
import { AuthService } from '../../core/services/auth.service';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../../shared/confirm-dialog/confirm-dialog.component';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-animals',
  templateUrl: './animals.component.html',
  styleUrls: ['./animals.component.scss']
})
export class AnimalsComponent implements OnInit, OnDestroy {
  animals: Array<Animal> = [];
  subspecies: Array<Subspecies> = [];
  collars: Array<Collar> = [];
  selectedAnimal: Animal | null = null;
  selectedLocations: Array<LocationPoint> = [];
  selectedAssignments: Array<CollarAssignment> = [];
  subspeciesOptions: Array<SearchableSelectOption> = [];
  subspeciesNames: Record<number, string> = {};
  collarLabels: Record<number, string> = {};
  currentAssignment: CollarAssignment | undefined;
  animalColumns: Array<string> = ['name', 'subspecies', 'sex', 'status', 'actions'];
  locationColumns: Array<string> = ['coordinates', 'recordedAt', 'signalType'];
  assignmentColumns: Array<string> = ['collar', 'assignedAt', 'unassignedAt'];
  animalSexOptions = animalSexOptions;
  pageSizeOptions: Array<number> = [5, 10, 20];
  animalPageIndex = 0;
  animalPageSize = 10;
  animalsTotalCount = 0;
  locationPageIndex = 0;
  locationPageSize = 5;
  locationsTotalCount = 0;
  reportsTotalCount = 0;
  alertsTotalCount = 0;
  isLoading = false;
  errorMessage = '';
  animalFormErrorMessage = '';
  successMessage = '';
  editingAnimalId: number | null = null;
  private readonly destroy$ = new Subject<void>();

  animalForm = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(120)]],
    subspeciesId: [null, [Validators.required, Validators.min(1)]],
    sex: ['Unknown', Validators.required],
    dateOfBirth: [null],
    notes: ['', Validators.maxLength(1000)],
    isActive: [true]
  });

  constructor(
    private readonly animalApi: AnimalApiService,
    private readonly alertApi: AlertApiService,
    private readonly locationPointApi: LocationPointApiService,
    private readonly rangerReportApi: RangerReportApiService,
    private readonly subspeciesApi: SubspeciesApiService,
    private readonly collarApi: CollarApiService,
    private readonly authService: AuthService,
    private readonly dialog: MatDialog,
    private readonly fb: UntypedFormBuilder
  ) {}

  canManageAnimals = false;

  ngOnInit(): void {
    this.canManageAnimals = this.authService.hasPermission(PermissionCodes.AnimalsWrite);
    if (!this.canManageAnimals) {
      this.animalColumns = ['name', 'subspecies', 'sex', 'status'];
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

  onAnimalPageChanged(event: PageEvent): void {
    this.animalPageIndex = event.pageIndex;
    this.animalPageSize = event.pageSize;
    this.refresh();
  }

  onLocationPageChanged(event: PageEvent): void {
    if (!this.selectedAnimal) {
      return;
    }

    this.locationPageIndex = event.pageIndex;
    this.locationPageSize = event.pageSize;
    this.loadSelectedAnimalData(this.selectedAnimal.id);
  }

  loadData(): Observable<void> {
    this.isLoading = true;
    this.errorMessage = '';

    return forkJoin({
      animals: this.animalApi.getPaged({
        pageNumber: this.animalPageIndex + 1,
        pageSize: this.animalPageSize
      }),
      subspecies: this.subspeciesApi.getAll(),
      collars: this.collarApi.getAll()
    })
      .pipe(
        map((result) => this.mapLoadData(result)),
        catchError(() => {
          this.errorMessage = 'Unable to load animals.';
          return of(void 0);
        }),
        finalize(() => (this.isLoading = false))
      );
  }

  selectAnimal(animal: Animal): void {
    this.selectedAnimal = animal;
    this.locationPageIndex = 0;
    this.loadSelectedAnimalData(animal.id);
  }

  private loadSelectedAnimalData(animalId: number): void {
    forkJoin({
      locations: this.locationPointApi.getByAnimalPaged(animalId, {
        pageNumber: this.locationPageIndex + 1,
        pageSize: this.locationPageSize
      }),
      reports: this.rangerReportApi.getByAnimalPaged(animalId, { pageNumber: 1, pageSize: 1 }),
      alerts: this.alertApi.getByAnimalPaged(animalId, { pageNumber: 1, pageSize: 1 }),
      assignments: this.collarApi.getAssignments({ animalId })
    }).pipe(takeUntil(this.destroy$)).subscribe({
      next: (result) => {
        this.mapSelectedAnimalData(result);
      },
      error: () => {
        this.errorMessage = 'Unable to load animal details.';
      }
    });
  }

  createAnimal(): void {
    this.animalFormErrorMessage = '';
    this.successMessage = '';

    if (this.animalForm.invalid) {
      this.animalForm.markAllAsTouched();
      this.animalFormErrorMessage = 'Please fix the highlighted fields.';
      return;
    }

    const request = this.mapUpsertAnimalRequest();
    const operation = this.editingAnimalId === null
      ? this.animalApi.create(request)
      : this.animalApi.update(this.editingAnimalId, request);
    operation
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (animal) => {
          this.successMessage = this.editingAnimalId === null ? 'Animal created.' : 'Animal updated.';
          this.animalFormErrorMessage = '';
          this.animalForm.reset({ sex: 'Unknown', isActive: true });
          this.editingAnimalId = null;
          this.refresh();
          this.selectAnimal(animal);
        },
        error: () => {
          this.animalFormErrorMessage = 'Unable to save animal.';
        }
      });
  }

  editAnimal(animal: Animal): void {
    this.editingAnimalId = animal.id;
    this.animalForm.reset({
      name: animal.name,
      subspeciesId: animal.subspeciesId,
      sex: animal.sex,
      dateOfBirth: animal.dateOfBirth ? new Date(animal.dateOfBirth) : null,
      notes: animal.notes ?? '',
      isActive: animal.isActive
    });
  }

  deleteAnimal(animal: Animal): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, { data: {
      title: 'Delete animal', message: `Delete animal "${animal.name}"?`
    }, panelClass: 'confirm-dialog-panel' });
    dialogRef.componentInstance.getConfirm().pipe(take(1), takeUntil(this.destroy$)).subscribe(() => this.animalApi.delete(animal.id).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => { this.successMessage = 'Animal deleted.'; this.selectedAnimal = null; this.refresh(); },
      error: () => this.errorMessage = 'Unable to delete animal.'
    }));
  }

  cancelEdit(): void {
    this.editingAnimalId = null;
    this.animalForm.reset({ sex: 'Unknown', isActive: true });
  }

  private mapLoadData(result: { animals: PagedResult<Animal>; subspecies: Array<Subspecies>; collars: Array<Collar> }): void {
    this.animals = result.animals.items;
    this.animalsTotalCount = result.animals.totalCount;
    this.animalPageIndex = result.animals.pageNumber - 1;
    this.animalPageSize = result.animals.pageSize;
    this.subspecies = result.subspecies;
    this.collars = result.collars;
    this.subspeciesOptions = this.subspecies.map((item) => ({ value: item.id, label: item.name }));
    this.subspeciesNames = Object.fromEntries(this.subspecies.map((item) => [item.id, item.name]));
    this.collarLabels = Object.fromEntries(this.collars.map((collar) => [collar.id, collar.serialNumber]));
    if (!this.selectedAnimal && this.animals.length) {
      this.selectAnimal(this.animals[0]);
    }
  }

  private mapSelectedAnimalData(result: {
    locations: PagedResult<LocationPoint>;
    reports: PagedResult<RangerReport>;
    alerts: PagedResult<Alert>;
    assignments: Array<CollarAssignment>;
  }): void {
    this.selectedLocations = result.locations.items;
    this.locationsTotalCount = result.locations.totalCount;
    this.locationPageIndex = result.locations.pageNumber - 1;
    this.locationPageSize = result.locations.pageSize;
    this.reportsTotalCount = result.reports.totalCount;
    this.alertsTotalCount = result.alerts.totalCount;
    this.selectedAssignments = result.assignments;
    this.currentAssignment = this.selectedAssignments.find((assignment) => !assignment.unassignedAt);
  }

  private mapUpsertAnimalRequest(): UpsertAnimalRequest {
    const value = this.animalForm.getRawValue();
    return {
      name: value.name,
      subspeciesId: value.subspeciesId,
      sex: value.sex,
      dateOfBirth: localDateInputToIso(value.dateOfBirth),
      notes: value.notes || null,
      isActive: value.isActive
    };
  }
}

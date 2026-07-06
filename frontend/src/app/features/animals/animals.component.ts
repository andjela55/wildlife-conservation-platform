import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { PageEvent } from '@angular/material/paginator';
import { catchError, finalize, forkJoin, map, Observable, of, Subject, takeUntil } from 'rxjs';
import {
    Alert,
    Animal,
    animalSexOptions,
    CreateAnimalRequest,
    LocationPoint,
    PagedResult,
    RangerReport,
    Subspecies
} from '../../core/models';
import { AnimalApiService } from '../../core/services/animal-api.service';
import { SubspeciesApiService } from '../../core/services/subspecies-api.service';
import { localDateInputToIso } from '../../core/utils/date-utils';

@Component({
  selector: 'app-animals',
  templateUrl: './animals.component.html',
  styleUrls: ['./animals.component.scss']
})
export class AnimalsComponent implements OnInit, OnDestroy {
  animals: Array<Animal> = [];
  subspecies: Array<Subspecies> = [];
  selectedAnimal: Animal | null = null;
  selectedLocations: Array<LocationPoint> = [];
  animalColumns: Array<string> = ['name', 'subspecies', 'sex', 'status'];
  locationColumns: Array<string> = ['coordinates', 'recordedAt', 'signalType'];
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
    private readonly subspeciesApi: SubspeciesApiService,
    private readonly fb: UntypedFormBuilder
  ) {}

  ngOnInit(): void {
    this.loadData().pipe(takeUntil(this.destroy$)).subscribe();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  getSubspeciesName(subspeciesId: number): string {
    return this.subspecies.find((item) => item.id === subspeciesId)?.name ?? `Subspecies #${subspeciesId}`;
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
      subspecies: this.subspeciesApi.getAll()
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
      locations: this.animalApi.getLocationsPaged(animalId, {
        pageNumber: this.locationPageIndex + 1,
        pageSize: this.locationPageSize
      }),
      reports: this.animalApi.getReportsPaged(animalId, { pageNumber: 1, pageSize: 1 }),
      alerts: this.animalApi.getAlertsPaged(animalId, { pageNumber: 1, pageSize: 1 })
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

    this.animalApi
      .create(this.mapCreateAnimalRequest())
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (animal) => {
          this.successMessage = 'Animal created.';
          this.animalFormErrorMessage = '';
          this.animalForm.reset({ sex: 'Unknown', isActive: true });
          this.refresh();
          this.selectAnimal(animal);
        },
        error: () => {
          this.animalFormErrorMessage = 'Unable to create animal.';
        }
      });
  }

  private mapLoadData(result: { animals: PagedResult<Animal>; subspecies: Array<Subspecies> }): void {
    this.animals = result.animals.items;
    this.animalsTotalCount = result.animals.totalCount;
    this.animalPageIndex = result.animals.pageNumber - 1;
    this.animalPageSize = result.animals.pageSize;
    this.subspecies = result.subspecies;
    if (!this.selectedAnimal && this.animals.length) {
      this.selectAnimal(this.animals[0]);
    }
  }

  private mapSelectedAnimalData(result: {
    locations: PagedResult<LocationPoint>;
    reports: PagedResult<RangerReport>;
    alerts: PagedResult<Alert>;
  }): void {
    this.selectedLocations = result.locations.items;
    this.locationsTotalCount = result.locations.totalCount;
    this.locationPageIndex = result.locations.pageNumber - 1;
    this.locationPageSize = result.locations.pageSize;
    this.reportsTotalCount = result.reports.totalCount;
    this.alertsTotalCount = result.alerts.totalCount;
  }

  private mapCreateAnimalRequest(): CreateAnimalRequest {
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

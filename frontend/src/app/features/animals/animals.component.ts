import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { catchError, finalize, forkJoin, map, Observable, of, Subject, takeUntil } from 'rxjs';
import {
    Alert,
    Animal,
    animalSexOptions,
    CreateAnimalRequest,
    LocationPoint,
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
  selectedReports: Array<RangerReport> = [];
  selectedAlerts: Array<Alert> = [];
  animalColumns: Array<string> = ['name', 'subspecies', 'sex', 'status'];
  locationColumns: Array<string> = ['coordinates', 'recordedAt', 'signalType'];
  animalSexOptions = animalSexOptions;
  isLoading = false;
  errorMessage = '';
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

  loadData(): Observable<void> {
    this.isLoading = true;
    this.errorMessage = '';

    return forkJoin({
      animals: this.animalApi.getAll(),
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
    forkJoin({
      locations: this.animalApi.getLocations(animal.id),
      reports: this.animalApi.getReports(animal.id),
      alerts: this.animalApi.getAlerts(animal.id)
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
    if (this.animalForm.invalid) {
      this.animalForm.markAllAsTouched();
      return;
    }

    this.animalApi
      .create(this.mapCreateAnimalRequest())
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (animal) => {
          this.successMessage = 'Animal created.';
          this.animalForm.reset({ sex: 'Unknown', isActive: true });
          this.refresh();
          this.selectAnimal(animal);
        },
        error: () => {
          this.errorMessage = 'Unable to create animal.';
        }
      });
  }

  private mapLoadData(result: { animals: Array<Animal>; subspecies: Array<Subspecies> }): void {
    this.animals = result.animals;
    this.subspecies = result.subspecies;
    if (!this.selectedAnimal && this.animals.length) {
      this.selectAnimal(this.animals[0]);
    }
  }

  private mapSelectedAnimalData(result: {
    locations: Array<LocationPoint>;
    reports: Array<RangerReport>;
    alerts: Array<Alert>;
  }): void {
    this.selectedLocations = result.locations;
    this.selectedReports = result.reports;
    this.selectedAlerts = result.alerts;
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

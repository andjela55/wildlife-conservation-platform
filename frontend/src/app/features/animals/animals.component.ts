import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { finalize, forkJoin } from 'rxjs';
import {
  Alert,
  Animal,
  animalSexOptions,
  Collar,
  LocationPoint,
  RangerReport,
  signalTypeOptions,
  Subspecies
} from '../../core/models/wildlife.models';
import { WildlifeApiService } from '../../core/services/wildlife-api.service';
import { localDateInputToIso, localDateTimeInputToIso, toLocalDateTimeInputValue } from '../../core/utils/date-utils';

@Component({
  selector: 'app-animals',
  templateUrl: './animals.component.html',
  styleUrls: ['./animals.component.scss']
})
export class AnimalsComponent implements OnInit {
  animals: Animal[] = [];
  subspecies: Subspecies[] = [];
  collars: Collar[] = [];
  selectedAnimal: Animal | null = null;
  selectedLocations: LocationPoint[] = [];
  selectedReports: RangerReport[] = [];
  selectedAlerts: Alert[] = [];
  animalSexOptions = animalSexOptions;
  signalTypeOptions = signalTypeOptions;
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  animalForm = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(120)]],
    subspeciesId: [null, [Validators.required, Validators.min(1)]],
    sex: ['Unknown', Validators.required],
    dateOfBirth: [null],
    notes: ['', Validators.maxLength(1000)],
    isActive: [true]
  });

  locationForm = this.fb.group({
    animalId: [null, [Validators.required, Validators.min(1)]],
    collarId: [null, [Validators.required, Validators.min(1)]],
    latitude: [0, [Validators.required, Validators.min(-90), Validators.max(90)]],
    longitude: [0, [Validators.required, Validators.min(-180), Validators.max(180)]],
    altitude: [null],
    recordedAt: [toLocalDateTimeInputValue(), Validators.required],
    signalType: ['Manual', Validators.required],
    notes: ['', Validators.maxLength(1000)]
  });

  constructor(
    private readonly api: WildlifeApiService,
    private readonly fb: UntypedFormBuilder
  ) {}

  ngOnInit(): void {
    this.load();
  }

  getSubspeciesName(subspeciesId: number): string {
    return this.subspecies.find((item) => item.id === subspeciesId)?.name ?? `Subspecies #${subspeciesId}`;
  }

  load(): void {
    this.isLoading = true;
    this.errorMessage = '';

    forkJoin({
      animals: this.api.getAnimals(),
      subspecies: this.api.getSubspecies(),
      collars: this.api.getCollars()
    })
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (result) => {
          this.animals = result.animals;
          this.subspecies = result.subspecies;
          this.collars = result.collars;
          if (!this.selectedAnimal && this.animals.length) {
            this.selectAnimal(this.animals[0]);
          }
        },
        error: () => {
          this.errorMessage = 'Unable to load animals.';
        }
      });
  }

  selectAnimal(animal: Animal): void {
    this.selectedAnimal = animal;
    this.locationForm.patchValue({ animalId: animal.id });
    forkJoin({
      locations: this.api.getAnimalLocations(animal.id),
      reports: this.api.getAnimalReports(animal.id),
      alerts: this.api.getAnimalAlerts(animal.id)
    }).subscribe({
      next: (result) => {
        this.selectedLocations = result.locations;
        this.selectedReports = result.reports;
        this.selectedAlerts = result.alerts;
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

    const value = this.animalForm.getRawValue();
    this.api
      .createAnimal({
        name: value.name,
        subspeciesId: value.subspeciesId,
        sex: value.sex,
        dateOfBirth: localDateInputToIso(value.dateOfBirth),
        notes: value.notes || null,
        isActive: value.isActive
      })
      .subscribe({
        next: (animal) => {
          this.successMessage = 'Animal created.';
          this.animalForm.reset({ sex: 'Unknown', isActive: true });
          this.load();
          this.selectAnimal(animal);
        },
        error: () => {
          this.errorMessage = 'Unable to create animal.';
        }
      });
  }

  createLocationPoint(): void {
    if (this.locationForm.invalid) {
      this.locationForm.markAllAsTouched();
      return;
    }

    const value = this.locationForm.getRawValue();
    this.api
      .createLocationPoint({
        animalId: value.animalId,
        collarId: value.collarId,
        latitude: value.latitude,
        longitude: value.longitude,
        altitude: value.altitude || null,
        recordedAt: localDateTimeInputToIso(value.recordedAt),
        signalType: value.signalType,
        notes: value.notes || null
      })
      .subscribe({
        next: () => {
          this.successMessage = 'Location point created.';
          this.locationForm.reset({
            animalId: this.selectedAnimal?.id ?? null,
            collarId: null,
            latitude: 0,
            longitude: 0,
            altitude: null,
            recordedAt: toLocalDateTimeInputValue(),
            signalType: 'Manual'
          });
          if (this.selectedAnimal) {
            this.selectAnimal(this.selectedAnimal);
          }
        },
        error: () => {
          this.errorMessage = 'Unable to create location point.';
        }
      });
  }
}

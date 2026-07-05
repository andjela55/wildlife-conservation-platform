import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { finalize, forkJoin } from 'rxjs';
import {
    Alert,
    Animal,
    animalSexOptions,
    LocationPoint,
    RangerReport,
    Subspecies
} from '../../core/models/wildlife.models';
import { AnimalApiService } from '../../core/services/animal-api.service';
import { SubspeciesApiService } from '../../core/services/subspecies-api.service';
import { localDateInputToIso } from '../../core/utils/date-utils';

@Component({
  selector: 'app-animals',
  templateUrl: './animals.component.html',
  styleUrls: ['./animals.component.scss']
})
export class AnimalsComponent implements OnInit {
  animals: Animal[] = [];
  subspecies: Subspecies[] = [];
  selectedAnimal: Animal | null = null;
  selectedLocations: LocationPoint[] = [];
  selectedReports: RangerReport[] = [];
  selectedAlerts: Alert[] = [];
  animalSexOptions = animalSexOptions;
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

  constructor(
    private readonly animalApi: AnimalApiService,
    private readonly subspeciesApi: SubspeciesApiService,
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
      animals: this.animalApi.getAll(),
      subspecies: this.subspeciesApi.getAll()
    })
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (result) => {
          this.animals = result.animals;
          this.subspecies = result.subspecies;
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
    forkJoin({
      locations: this.animalApi.getLocations(animal.id),
      reports: this.animalApi.getReports(animal.id),
      alerts: this.animalApi.getAlerts(animal.id)
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
    this.animalApi
      .create({
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

}

import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { finalize, forkJoin } from 'rxjs';
import { Species, Subspecies } from '../../core/models/wildlife.models';
import { SpeciesApiService } from '../../core/services/species-api.service';
import { SubspeciesApiService } from '../../core/services/subspecies-api.service';

@Component({
  selector: 'app-species',
  templateUrl: './species.component.html',
  styleUrls: ['./species.component.scss']
})
export class SpeciesComponent implements OnInit {
  species: Species[] = [];
  subspecies: Subspecies[] = [];
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  speciesForm = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(120)]],
    description: ['', [Validators.required, Validators.maxLength(1000)]]
  });

  subspeciesForm = this.fb.group({
    speciesId: [null, [Validators.required, Validators.min(1)]],
    name: ['', [Validators.required, Validators.maxLength(120)]],
    description: ['', [Validators.required, Validators.maxLength(1000)]]
  });

  constructor(
    private readonly speciesApi: SpeciesApiService,
    private readonly subspeciesApi: SubspeciesApiService,
    private readonly fb: UntypedFormBuilder
  ) {}

  ngOnInit(): void {
    this.load();
  }

  getSpeciesName(speciesId: number): string {
    return this.species.find((item) => item.id === speciesId)?.name ?? `Species #${speciesId}`;
  }

  load(): void {
    this.isLoading = true;
    this.errorMessage = '';

    forkJoin({
      species: this.speciesApi.getAll(),
      subspecies: this.subspeciesApi.getAll()
    })
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (result) => {
          this.species = result.species;
          this.subspecies = result.subspecies;
        },
        error: () => {
          this.errorMessage = 'Unable to load species data.';
        }
      });
  }

  createSpecies(): void {
    if (this.speciesForm.invalid) {
      this.speciesForm.markAllAsTouched();
      return;
    }

    this.speciesApi.create(this.speciesForm.getRawValue()).subscribe({
      next: () => {
        this.successMessage = 'Species created.';
        this.speciesForm.reset();
        this.load();
      },
      error: () => {
        this.errorMessage = 'Unable to create species.';
      }
    });
  }

  createSubspecies(): void {
    if (this.subspeciesForm.invalid) {
      this.subspeciesForm.markAllAsTouched();
      return;
    }

    this.subspeciesApi.create(this.subspeciesForm.getRawValue()).subscribe({
      next: () => {
        this.successMessage = 'Subspecies created.';
        this.subspeciesForm.reset();
        this.load();
      },
      error: () => {
        this.errorMessage = 'Unable to create subspecies.';
      }
    });
  }
}

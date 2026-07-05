import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { catchError, finalize, forkJoin, map, Observable, of, Subject, takeUntil } from 'rxjs';
import { CreateSpeciesRequest, CreateSubspeciesRequest, Species, Subspecies } from '../../core/models';
import { SpeciesApiService } from '../../core/services/species-api.service';
import { SubspeciesApiService } from '../../core/services/subspecies-api.service';

@Component({
  selector: 'app-species',
  templateUrl: './species.component.html',
  styleUrls: ['./species.component.scss']
})
export class SpeciesComponent implements OnInit, OnDestroy {
  species: Array<Species> = [];
  subspecies: Array<Subspecies> = [];
  speciesColumns: Array<string> = ['name', 'description'];
  subspeciesColumns: Array<string> = ['species', 'name'];
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  private readonly destroy$ = new Subject<void>();

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
    this.loadData().pipe(takeUntil(this.destroy$)).subscribe();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  getSpeciesName(speciesId: number): string {
    return this.species.find((item) => item.id === speciesId)?.name ?? `Species #${speciesId}`;
  }

  refresh(): void {
    this.loadData().pipe(takeUntil(this.destroy$)).subscribe();
  }

  loadData(): Observable<void> {
    this.isLoading = true;
    this.errorMessage = '';

    return forkJoin({
      species: this.speciesApi.getAll(),
      subspecies: this.subspeciesApi.getAll()
    })
      .pipe(
        map((result) => this.mapLoadData(result)),
        catchError(() => {
          this.errorMessage = 'Unable to load species data.';
          return of(void 0);
        }),
        finalize(() => (this.isLoading = false))
      );
  }

  createSpecies(): void {
    if (this.speciesForm.invalid) {
      this.speciesForm.markAllAsTouched();
      return;
    }

    this.speciesApi.create(this.mapCreateSpeciesRequest()).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.successMessage = 'Species created.';
        this.speciesForm.reset();
        this.refresh();
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

    this.subspeciesApi.create(this.mapCreateSubspeciesRequest()).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.successMessage = 'Subspecies created.';
        this.subspeciesForm.reset();
        this.refresh();
      },
      error: () => {
        this.errorMessage = 'Unable to create subspecies.';
      }
    });
  }

  private mapLoadData(result: { species: Array<Species>; subspecies: Array<Subspecies> }): void {
    this.species = result.species;
    this.subspecies = result.subspecies;
  }

  private mapCreateSpeciesRequest(): CreateSpeciesRequest {
    return this.speciesForm.getRawValue();
  }

  private mapCreateSubspeciesRequest(): CreateSubspeciesRequest {
    return this.subspeciesForm.getRawValue();
  }
}

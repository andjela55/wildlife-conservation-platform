import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { PageEvent } from '@angular/material/paginator';
import { catchError, finalize, forkJoin, map, Observable, of, Subject, takeUntil } from 'rxjs';
import { CreateSpeciesRequest, CreateSubspeciesRequest, PagedResult, Species, Subspecies } from '../../core/models';
import { SpeciesApiService } from '../../core/services/species-api.service';
import { SubspeciesApiService } from '../../core/services/subspecies-api.service';

@Component({
  selector: 'app-species',
  templateUrl: './species.component.html',
  styleUrls: ['./species.component.scss']
})
export class SpeciesComponent implements OnInit, OnDestroy {
  species: Array<Species> = [];
  speciesOptions: Array<Species> = [];
  subspecies: Array<Subspecies> = [];
  speciesColumns: Array<string> = ['name', 'description'];
  subspeciesColumns: Array<string> = ['species', 'name'];
  pageSizeOptions: Array<number> = [5, 10, 20];
  speciesPageIndex = 0;
  speciesPageSize = 10;
  speciesTotalCount = 0;
  subspeciesPageIndex = 0;
  subspeciesPageSize = 10;
  subspeciesTotalCount = 0;
  isLoading = false;
  errorMessage = '';
  speciesFormErrorMessage = '';
  subspeciesFormErrorMessage = '';
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
    return this.speciesOptions.find((item) => item.id === speciesId)?.name ?? `Species #${speciesId}`;
  }

  refresh(): void {
    this.loadData().pipe(takeUntil(this.destroy$)).subscribe();
  }

  onSpeciesPageChanged(event: PageEvent): void {
    this.speciesPageIndex = event.pageIndex;
    this.speciesPageSize = event.pageSize;
    this.refresh();
  }

  onSubspeciesPageChanged(event: PageEvent): void {
    this.subspeciesPageIndex = event.pageIndex;
    this.subspeciesPageSize = event.pageSize;
    this.refresh();
  }

  loadData(): Observable<void> {
    this.isLoading = true;
    this.errorMessage = '';

    return forkJoin({
      species: this.speciesApi.getPaged({
        pageNumber: this.speciesPageIndex + 1,
        pageSize: this.speciesPageSize
      }),
      speciesOptions: this.speciesApi.getAll(),
      subspecies: this.subspeciesApi.getPaged({
        pageNumber: this.subspeciesPageIndex + 1,
        pageSize: this.subspeciesPageSize
      })
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
    this.speciesFormErrorMessage = '';
    this.successMessage = '';

    if (this.speciesForm.invalid) {
      this.speciesForm.markAllAsTouched();
      this.speciesFormErrorMessage = 'Please fix the highlighted fields.';
      return;
    }

    this.speciesApi.create(this.mapCreateSpeciesRequest()).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.successMessage = 'Species created.';
        this.speciesFormErrorMessage = '';
        this.speciesForm.reset();
        this.refresh();
      },
      error: () => {
        this.speciesFormErrorMessage = 'Unable to create species.';
      }
    });
  }

  createSubspecies(): void {
    this.subspeciesFormErrorMessage = '';
    this.successMessage = '';

    if (this.subspeciesForm.invalid) {
      this.subspeciesForm.markAllAsTouched();
      this.subspeciesFormErrorMessage = 'Please fix the highlighted fields.';
      return;
    }

    this.subspeciesApi.create(this.mapCreateSubspeciesRequest()).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.successMessage = 'Subspecies created.';
        this.subspeciesFormErrorMessage = '';
        this.subspeciesForm.reset();
        this.refresh();
      },
      error: () => {
        this.subspeciesFormErrorMessage = 'Unable to create subspecies.';
      }
    });
  }

  private mapLoadData(result: {
    species: PagedResult<Species>;
    speciesOptions: Array<Species>;
    subspecies: PagedResult<Subspecies>;
  }): void {
    this.species = result.species.items;
    this.speciesOptions = result.speciesOptions;
    this.speciesTotalCount = result.species.totalCount;
    this.speciesPageIndex = result.species.pageNumber - 1;
    this.speciesPageSize = result.species.pageSize;
    this.subspecies = result.subspecies.items;
    this.subspeciesTotalCount = result.subspecies.totalCount;
    this.subspeciesPageIndex = result.subspecies.pageNumber - 1;
    this.subspeciesPageSize = result.subspecies.pageSize;
  }

  private mapCreateSpeciesRequest(): CreateSpeciesRequest {
    return this.speciesForm.getRawValue();
  }

  private mapCreateSubspeciesRequest(): CreateSubspeciesRequest {
    return this.subspeciesForm.getRawValue();
  }
}

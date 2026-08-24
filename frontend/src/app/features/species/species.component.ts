import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { PageEvent } from '@angular/material/paginator';
import { catchError, finalize, forkJoin, map, Observable, of, Subject, takeUntil } from 'rxjs';
import { PagedResult, PermissionCodes, Species, Subspecies, UpsertSpeciesRequest, UpsertSubspeciesRequest } from '../../core/models';
import { AuthService } from '../../core/services/auth.service';
import { SpeciesApiService } from '../../core/services/species-api.service';
import { SubspeciesApiService } from '../../core/services/subspecies-api.service';
import { SearchableSelectOption } from '../../shared/searchable-select/searchable-select.component';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../../shared/confirm-dialog/confirm-dialog.component';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-species',
  templateUrl: './species.component.html',
  styleUrls: ['./species.component.scss']
})
export class SpeciesComponent implements OnInit, OnDestroy {
  species: Array<Species> = [];
  speciesOptions: Array<Species> = [];
  subspecies: Array<Subspecies> = [];
  searchableSpeciesOptions: Array<SearchableSelectOption> = [];
  speciesNames: Record<number, string> = {};
  speciesColumns: Array<string> = ['name', 'description', 'actions'];
  subspeciesColumns: Array<string> = ['species', 'name', 'actions'];
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
  editingSpeciesId: number | null = null;
  editingSubspeciesId: number | null = null;
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
    private readonly authService: AuthService,
    private readonly dialog: MatDialog,
    private readonly fb: UntypedFormBuilder
  ) {}

  canManageSpecies = false;
  canManageSubspecies = false;

  ngOnInit(): void {
    this.canManageSpecies = this.authService.hasPermission(PermissionCodes.SpeciesWrite);
    this.canManageSubspecies = this.authService.hasPermission(PermissionCodes.SubspeciesWrite);
    if (!this.canManageSpecies) {
      this.speciesColumns = ['name', 'description'];
    }
    if (!this.canManageSubspecies) {
      this.subspeciesColumns = ['species', 'name'];
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

    const request = this.mapUpsertSpeciesRequest();
    const operation = this.editingSpeciesId === null
      ? this.speciesApi.create(request)
      : this.speciesApi.update(this.editingSpeciesId, request);
    operation.pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.successMessage = this.editingSpeciesId === null ? 'Species created.' : 'Species updated.';
        this.speciesFormErrorMessage = '';
        this.speciesForm.reset();
        this.editingSpeciesId = null;
        this.refresh();
      },
      error: () => {
        this.speciesFormErrorMessage = 'Unable to save species.';
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

    const request = this.mapUpsertSubspeciesRequest();
    const operation = this.editingSubspeciesId === null
      ? this.subspeciesApi.create(request)
      : this.subspeciesApi.update(this.editingSubspeciesId, request);
    operation.pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.successMessage = this.editingSubspeciesId === null ? 'Subspecies created.' : 'Subspecies updated.';
        this.subspeciesFormErrorMessage = '';
        this.subspeciesForm.reset();
        this.editingSubspeciesId = null;
        this.refresh();
      },
      error: () => {
        this.subspeciesFormErrorMessage = 'Unable to save subspecies.';
      }
    });
  }

  editSpecies(item: Species): void {
    this.editingSpeciesId = item.id;
    this.speciesForm.reset({ name: item.name, description: item.description });
  }

  deleteSpecies(item: Species): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, { data: {
      title: 'Delete species', message: `Delete species "${item.name}"?`
    }, panelClass: 'confirm-dialog-panel' });
    dialogRef.componentInstance.getConfirm().pipe(take(1), takeUntil(this.destroy$)).subscribe(() => this.speciesApi.delete(item.id).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => { this.successMessage = 'Species deleted.'; this.refresh(); },
      error: () => this.errorMessage = 'Unable to delete species. It may still have subspecies.'
    }));
  }

  cancelSpeciesEdit(): void {
    this.editingSpeciesId = null;
    this.speciesForm.reset();
  }

  editSubspecies(item: Subspecies): void {
    this.editingSubspeciesId = item.id;
    this.subspeciesForm.reset({ speciesId: item.speciesId, name: item.name, description: item.description });
  }

  deleteSubspecies(item: Subspecies): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, { data: {
      title: 'Delete subspecies', message: `Delete subspecies "${item.name}"?`
    }, panelClass: 'confirm-dialog-panel' });
    dialogRef.componentInstance.getConfirm().pipe(take(1), takeUntil(this.destroy$)).subscribe(() => this.subspeciesApi.delete(item.id).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => { this.successMessage = 'Subspecies deleted.'; this.refresh(); },
      error: () => this.errorMessage = 'Unable to delete subspecies. It may still have animals.'
    }));
  }

  cancelSubspeciesEdit(): void {
    this.editingSubspeciesId = null;
    this.subspeciesForm.reset();
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
    this.searchableSpeciesOptions = this.speciesOptions.map((item) => ({ value: item.id, label: item.name }));
    this.speciesNames = Object.fromEntries(this.speciesOptions.map((item) => [item.id, item.name]));
  }

  private mapUpsertSpeciesRequest(): UpsertSpeciesRequest {
    return this.speciesForm.getRawValue();
  }

  private mapUpsertSubspeciesRequest(): UpsertSubspeciesRequest {
    return this.subspeciesForm.getRawValue();
  }
}

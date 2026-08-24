import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { PageEvent } from '@angular/material/paginator';
import { catchError, finalize, forkJoin, map, Observable, of, Subject, takeUntil } from 'rxjs';
import { Animal, CreateRangerReportRequest, PagedResult, PermissionCodes, RangerReport, reportTypeOptions, severityOptions } from '../../core/models';
import { AnimalApiService } from '../../core/services/animal-api.service';
import { RangerReportApiService } from '../../core/services/ranger-report-api.service';
import { enumKey } from '../../core/utils/enum-utils';
import { SearchableSelectOption } from '../../shared/searchable-select/searchable-select.component';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss']
})
export class ReportsComponent implements OnInit, OnDestroy {
  reports: Array<RangerReport> = [];
  animals: Array<Animal> = [];
  animalOptions: Array<SearchableSelectOption> = [];
  animalNames: Record<number, string> = {};
  severityClasses: Record<number, string> = {};
  reportColumns: Array<string> = ['subject', 'type', 'severity', 'latitude', 'longitude', 'createdAt', 'description'];
  reportTypeOptions = reportTypeOptions;
  severityOptions = severityOptions;
  pageSizeOptions: Array<number> = [5, 10, 20];
  reportPageIndex = 0;
  reportPageSize = 10;
  reportsTotalCount = 0;
  isLoading = false;
  errorMessage = '';
  reportFormErrorMessage = '';
  successMessage = '';
  private readonly destroy$ = new Subject<void>();

  reportForm = this.fb.group({
    animalId: [null],
    reportType: ['Sighting', Validators.required],
    severity: ['Low', Validators.required],
    latitude: [null, [Validators.required, Validators.min(-90), Validators.max(90)]],
    longitude: [null, [Validators.required, Validators.min(-180), Validators.max(180)]],
    description: ['', [Validators.required, Validators.maxLength(2000)]]
  });

  constructor(
    private readonly rangerReportApi: RangerReportApiService,
    private readonly animalApi: AnimalApiService,
    private readonly authService: AuthService,
    private readonly fb: UntypedFormBuilder
  ) {}

  canCreateReports = false;

  ngOnInit(): void {
    this.canCreateReports = this.authService.hasPermission(PermissionCodes.RangerReportsWrite);
    this.loadData().pipe(takeUntil(this.destroy$)).subscribe();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  refresh(): void {
    this.loadData().pipe(takeUntil(this.destroy$)).subscribe();
  }

  onReportPageChanged(event: PageEvent): void {
    this.reportPageIndex = event.pageIndex;
    this.reportPageSize = event.pageSize;
    this.refresh();
  }

  loadData(): Observable<void> {
    this.isLoading = true;
    this.errorMessage = '';

    return forkJoin({
      reports: this.rangerReportApi.getPaged({
        pageNumber: this.reportPageIndex + 1,
        pageSize: this.reportPageSize
      }),
      animals: this.animalApi.getAll()
    })
      .pipe(
        map((result) => this.mapLoadData(result)),
        catchError(() => {
          this.errorMessage = 'Unable to load ranger reports.';
          return of(void 0);
        }),
        finalize(() => (this.isLoading = false))
      );
  }

  createReport(): void {
    this.reportFormErrorMessage = '';
    this.successMessage = '';

    if (this.reportForm.invalid) {
      this.reportForm.markAllAsTouched();
      this.reportFormErrorMessage = 'Please fix the highlighted fields.';
      return;
    }

    this.rangerReportApi
      .create(this.mapCreateReportRequest())
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.successMessage = 'Ranger report created.';
          this.reportFormErrorMessage = '';
          this.reportForm.reset({
            animalId: null,
            reportType: 'Sighting',
            severity: 'Low',
            latitude: null,
            longitude: null
          });
          this.refresh();
        },
        error: () => {
          this.reportFormErrorMessage = 'Unable to create ranger report.';
        }
      });
  }

  private mapLoadData(result: { reports: PagedResult<RangerReport>; animals: Array<Animal> }): void {
    this.reports = result.reports.items;
    this.reportsTotalCount = result.reports.totalCount;
    this.reportPageIndex = result.reports.pageNumber - 1;
    this.reportPageSize = result.reports.pageSize;
    this.animals = result.animals;
    this.animalOptions = [
      { value: null, label: 'Area report (no animal)' },
      ...this.animals.map((animal) => ({ value: animal.id, label: animal.name }))
    ];
    this.animalNames = Object.fromEntries(this.animals.map((animal) => [animal.id, animal.name]));
    this.severityClasses = Object.fromEntries(
      this.reports.map((report) => [report.id, enumKey(report.severity, 'Severity').toLowerCase()])
    );
  }

  private mapCreateReportRequest(): CreateRangerReportRequest {
    const value = this.reportForm.getRawValue();
    return {
      animalId: value.animalId || null,
      reportType: value.reportType,
      severity: value.severity,
      latitude: value.latitude,
      longitude: value.longitude,
      description: value.description
    };
  }
}

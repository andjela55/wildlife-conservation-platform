import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { finalize, forkJoin } from 'rxjs';
import { Animal, RangerReport, reportTypeOptions, severityOptions } from '../../core/models/wildlife.models';
import { WildlifeApiService } from '../../core/services/wildlife-api.service';
import { localDateTimeInputToIso, toLocalDateTimeInputValue } from '../../core/utils/date-utils';

@Component({
  selector: 'app-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss']
})
export class ReportsComponent implements OnInit {
  reports: RangerReport[] = [];
  animals: Animal[] = [];
  reportTypeOptions = reportTypeOptions;
  severityOptions = severityOptions;
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  reportForm = this.fb.group({
    animalId: [null],
    userId: [1, [Validators.required, Validators.min(1)]],
    reportType: ['Sighting', Validators.required],
    severity: ['Low', Validators.required],
    latitude: [0, [Validators.required, Validators.min(-90), Validators.max(90)]],
    longitude: [0, [Validators.required, Validators.min(-180), Validators.max(180)]],
    description: ['', [Validators.required, Validators.maxLength(2000)]],
    createdAt: [toLocalDateTimeInputValue(), Validators.required]
  });

  constructor(
    private readonly api: WildlifeApiService,
    private readonly fb: UntypedFormBuilder
  ) {}

  ngOnInit(): void {
    this.load();
  }

  getAnimalName(animalId: number | null): string {
    if (!animalId) {
      return 'Area report';
    }

    return this.animals.find((animal) => animal.id === animalId)?.name ?? `Animal #${animalId}`;
  }

  load(): void {
    this.isLoading = true;
    this.errorMessage = '';

    forkJoin({
      reports: this.api.getRangerReports(),
      animals: this.api.getAnimals()
    })
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (result) => {
          this.reports = result.reports;
          this.animals = result.animals;
        },
        error: () => {
          this.errorMessage = 'Unable to load ranger reports.';
        }
      });
  }

  createReport(): void {
    if (this.reportForm.invalid) {
      this.reportForm.markAllAsTouched();
      return;
    }

    const value = this.reportForm.getRawValue();
    this.api
      .createRangerReport({
        animalId: value.animalId || null,
        userId: value.userId,
        reportType: value.reportType,
        severity: value.severity,
        latitude: value.latitude,
        longitude: value.longitude,
        description: value.description,
        createdAt: localDateTimeInputToIso(value.createdAt)
      })
      .subscribe({
        next: () => {
          this.successMessage = 'Ranger report created.';
          this.reportForm.reset({
            animalId: null,
            userId: 1,
            reportType: 'Sighting',
            severity: 'Low',
            latitude: 0,
            longitude: 0,
            createdAt: toLocalDateTimeInputValue()
          });
          this.load();
        },
        error: () => {
          this.errorMessage = 'Unable to create ranger report.';
        }
      });
  }
}

import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { finalize, forkJoin } from 'rxjs';
import { Animal, Collar, collarStatusOptions } from '../../core/models/wildlife.models';
import { WildlifeApiService } from '../../core/services/wildlife-api.service';
import { localDateTimeInputToIso, toLocalDateTimeInputValue } from '../../core/utils/date-utils';

@Component({
  selector: 'app-collars',
  templateUrl: './collars.component.html',
  styleUrls: ['./collars.component.scss']
})
export class CollarsComponent implements OnInit {
  collars: Collar[] = [];
  animals: Animal[] = [];
  collarStatusOptions = collarStatusOptions;
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  collarForm = this.fb.group({
    serialNumber: ['', [Validators.required, Validators.maxLength(120)]],
    model: ['', Validators.maxLength(120)],
    manufacturer: ['', Validators.maxLength(120)],
    status: ['Available', Validators.required],
    notes: ['', Validators.maxLength(1000)]
  });

  assignmentForm = this.fb.group({
    animalId: [null, [Validators.required, Validators.min(1)]],
    collarId: [null, [Validators.required, Validators.min(1)]],
    assignedAt: [toLocalDateTimeInputValue(), Validators.required],
    reason: ['', Validators.maxLength(250)],
    notes: ['', Validators.maxLength(1000)]
  });

  unassignForm = this.fb.group({
    assignmentId: [null, [Validators.required, Validators.min(1)]],
    unassignedAt: [toLocalDateTimeInputValue()],
    reason: ['', Validators.maxLength(250)],
    notes: ['', Validators.maxLength(1000)]
  });

  constructor(
    private readonly api: WildlifeApiService,
    private readonly fb: UntypedFormBuilder
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.isLoading = true;
    this.errorMessage = '';

    forkJoin({
      collars: this.api.getCollars(),
      animals: this.api.getAnimals()
    })
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (result) => {
          this.collars = result.collars;
          this.animals = result.animals;
        },
        error: () => {
          this.errorMessage = 'Unable to load collars.';
        }
      });
  }

  createCollar(): void {
    if (this.collarForm.invalid) {
      this.collarForm.markAllAsTouched();
      return;
    }

    const value = this.collarForm.getRawValue();
    this.api
      .createCollar({
        serialNumber: value.serialNumber,
        model: value.model || null,
        manufacturer: value.manufacturer || null,
        status: value.status,
        notes: value.notes || null
      })
      .subscribe({
        next: () => {
          this.successMessage = 'Collar created.';
          this.collarForm.reset({ status: 'Available' });
          this.load();
        },
        error: () => {
          this.errorMessage = 'Unable to create collar.';
        }
      });
  }

  assignCollar(): void {
    if (this.assignmentForm.invalid) {
      this.assignmentForm.markAllAsTouched();
      return;
    }

    const value = this.assignmentForm.getRawValue();
    this.api
      .assignCollar({
        animalId: value.animalId,
        collarId: value.collarId,
        assignedAt: localDateTimeInputToIso(value.assignedAt),
        reason: value.reason || null,
        notes: value.notes || null
      })
      .subscribe({
        next: () => {
          this.successMessage = 'Collar assigned.';
          this.assignmentForm.reset({ assignedAt: toLocalDateTimeInputValue() });
          this.load();
        },
        error: () => {
          this.errorMessage = 'Unable to assign collar.';
        }
      });
  }

  unassignCollar(): void {
    if (this.unassignForm.invalid) {
      this.unassignForm.markAllAsTouched();
      return;
    }

    const value = this.unassignForm.getRawValue();
    this.api
      .unassignCollar(value.assignmentId, {
        unassignedAt: localDateTimeInputToIso(value.unassignedAt),
        reason: value.reason || null,
        notes: value.notes || null
      })
      .subscribe({
        next: () => {
          this.successMessage = 'Collar unassigned.';
          this.unassignForm.reset({ unassignedAt: toLocalDateTimeInputValue() });
          this.load();
        },
        error: () => {
          this.errorMessage = 'Unable to unassign collar.';
        }
      });
  }
}

import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { PageEvent } from '@angular/material/paginator';
import { catchError, finalize, map, Observable, of, Subject, takeUntil } from 'rxjs';
import { CreateUserRequest, PagedResult, UpdateUserAssignedAreaRequest, User, userRoleOptions } from '../core/models';
import { AuthService } from '../core/services/auth.service';
import { UserApiService } from '../core/services/user-api.service';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit, OnDestroy {
  users: Array<User> = [];
  selectedUser: User | null = null;
  userColumns: Array<string> = ['fullName', 'email', 'role', 'assignedArea', 'status', 'actions'];
  userRoleOptions = userRoleOptions;
  pageSizeOptions: Array<number> = [5, 10, 20];
  pageIndex = 0;
  pageSize = 10;
  totalCount = 0;
  workflowTabIndex = 0;
  isLoading = false;
  errorMessage = '';
  formErrorMessage = '';
  successMessage = '';
  private readonly destroy$ = new Subject<void>();

  userForm = this.fb.group({
    fullName: ['', [Validators.required, Validators.maxLength(160)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(200)]],
    password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(100)]],
    role: ['Ranger', Validators.required],
    isActive: [true],
    assignedLocationName: [''],
    assignedLatitude: [null, [Validators.min(-90), Validators.max(90)]],
    assignedLongitude: [null, [Validators.min(-180), Validators.max(180)]],
    assignedMapZoom: [11, [Validators.min(1), Validators.max(18)]]
  });

  assignedAreaForm = this.fb.group({
    assignedLocationName: [''],
    assignedLatitude: [null, [Validators.min(-90), Validators.max(90)]],
    assignedLongitude: [null, [Validators.min(-180), Validators.max(180)]],
    assignedMapZoom: [11, [Validators.min(1), Validators.max(18)]]
  });

  constructor(
    private readonly userApi: UserApiService,
    private readonly authService: AuthService,
    private readonly fb: UntypedFormBuilder
  ) {}

  ngOnInit(): void {
    this.loadData().pipe(takeUntil(this.destroy$)).subscribe();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  refresh(): void {
    this.loadData().pipe(takeUntil(this.destroy$)).subscribe();
  }

  onPageChanged(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.refresh();
  }

  loadData(): Observable<void> {
    this.isLoading = true;
    this.errorMessage = '';

    return this.userApi.getPaged({
      pageNumber: this.pageIndex + 1,
      pageSize: this.pageSize
    }).pipe(
      map((result) => this.mapLoadData(result)),
      catchError(() => {
        this.errorMessage = 'Unable to load users.';
        return of(void 0);
      }),
      finalize(() => (this.isLoading = false))
    );
  }

  createUser(): void {
    this.formErrorMessage = '';
    this.successMessage = '';

    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      this.formErrorMessage = 'Please fix the highlighted fields.';
      return;
    }

    this.userApi.create(this.mapCreateUserRequest())
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.successMessage = 'User created.';
          this.userForm.reset({
            fullName: '',
            email: '',
            password: '',
            role: 'Ranger',
            isActive: true,
            assignedLocationName: '',
            assignedLatitude: null,
            assignedLongitude: null,
            assignedMapZoom: 11
          });
          this.refresh();
        },
        error: () => {
          this.formErrorMessage = 'Unable to create user.';
        }
      });
  }

  selectUser(user: User): void {
    this.selectedUser = user;
    this.workflowTabIndex = 1;
    this.assignedAreaForm.reset({
      assignedLocationName: user.assignedLocationName ?? '',
      assignedLatitude: user.assignedLatitude,
      assignedLongitude: user.assignedLongitude,
      assignedMapZoom: user.assignedMapZoom ?? 11
    });
  }

  updateAssignedArea(): void {
    this.formErrorMessage = '';
    this.successMessage = '';

    if (!this.selectedUser) {
      this.formErrorMessage = 'Select a user first.';
      return;
    }

    if (this.assignedAreaForm.invalid) {
      this.assignedAreaForm.markAllAsTouched();
      this.formErrorMessage = 'Please fix the assigned area fields.';
      return;
    }

    this.userApi.updateAssignedArea(this.selectedUser.id, this.mapAssignedAreaRequest())
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (updatedUser) => {
          this.successMessage = 'Assigned area updated.';
          this.users = this.users.map((user) => user.id === updatedUser.id ? updatedUser : user);
          this.selectUser(updatedUser);
          this.refreshCurrentUserIfNeeded(updatedUser);
        },
        error: () => {
          this.formErrorMessage = 'Unable to update assigned area.';
        }
      });
  }

  getAssignedAreaLabel(user: User): string {
    if (user.assignedLocationName) {
      return user.assignedLocationName;
    }

    if (user.assignedLatitude !== null && user.assignedLongitude !== null) {
      return `${user.assignedLatitude}, ${user.assignedLongitude}`;
    }

    return '-';
  }

  private mapLoadData(result: PagedResult<User>): void {
    this.users = result.items;
    this.totalCount = result.totalCount;
    this.pageIndex = result.pageNumber - 1;
    this.pageSize = result.pageSize;
  }

  private mapCreateUserRequest(): CreateUserRequest {
    const value = this.userForm.getRawValue();
    return {
      fullName: value.fullName,
      email: value.email,
      password: value.password,
      role: value.role,
      isActive: value.isActive,
      assignedLocationName: value.assignedLocationName || null,
      assignedLatitude: value.assignedLatitude,
      assignedLongitude: value.assignedLongitude,
      assignedMapZoom: value.assignedMapZoom
    };
  }

  private mapAssignedAreaRequest(): UpdateUserAssignedAreaRequest {
    const value = this.assignedAreaForm.getRawValue();
    return {
      assignedLocationName: value.assignedLocationName || null,
      assignedLatitude: value.assignedLatitude,
      assignedLongitude: value.assignedLongitude,
      assignedMapZoom: value.assignedMapZoom
    };
  }

  private refreshCurrentUserIfNeeded(updatedUser: User): void {
    if (this.authService.currentUser?.id !== updatedUser.id) {
      return;
    }

    this.authService.refreshCurrentUser()
      .pipe(takeUntil(this.destroy$))
      .subscribe();
  }
}

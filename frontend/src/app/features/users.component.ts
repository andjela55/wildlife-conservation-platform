import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { PageEvent } from '@angular/material/paginator';
import { catchError, finalize, map, Observable, of, Subject, takeUntil } from 'rxjs';
import { CreateUserRequest, PagedResult, PermissionCodes, Role, UpdateUserAssignedAreaRequest, UpdateUserRequest, User } from '../core/models';
import { AuthService } from '../core/services/auth.service';
import { RoleApiService } from '../core/services/role-api.service';
import { UserApiService } from '../core/services/user-api.service';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../shared/confirm-dialog/confirm-dialog.component';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit, OnDestroy {
  users: Array<User> = [];
  selectedUser: User | null = null;
  userColumns: Array<string> = ['fullName', 'email', 'role', 'assignedArea', 'status', 'actions'];
  canManageUsers = false;
  canManageAllUsers = false;
  roleOptions: ReadonlyArray<Role> = [];
  createRoleOptions: ReadonlyArray<Role> = [];
  editRoleOptions: ReadonlyArray<Role> = [];
  roleLabels: Record<number, string> = {};
  assignedAreaLabels: Record<number, string> = {};
  editableUsers: Record<number, boolean> = {};
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
    roleIds: [[], Validators.required],
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

  editUserForm = this.fb.group({
    fullName: ['', [Validators.required, Validators.maxLength(160)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(200)]],
    password: ['', [Validators.minLength(8), Validators.maxLength(100)]],
    roleIds: [[], Validators.required],
    isActive: [true],
    assignedLocationName: [''],
    assignedLatitude: [null, [Validators.min(-90), Validators.max(90)]],
    assignedLongitude: [null, [Validators.min(-180), Validators.max(180)]],
    assignedMapZoom: [11, [Validators.min(1), Validators.max(18)]]
  });

  constructor(
    private readonly userApi: UserApiService,
    private readonly roleApi: RoleApiService,
    private readonly authService: AuthService,
    private readonly dialog: MatDialog,
    private readonly fb: UntypedFormBuilder
  ) {}

  ngOnInit(): void {
    this.canManageUsers = this.authService.hasPermission(PermissionCodes.UsersWrite);
    this.canManageAllUsers = !!this.authService.currentUser?.permissions.includes(PermissionCodes.Master);
    if (!this.canManageUsers) {
      this.userColumns = ['fullName', 'email', 'role', 'assignedArea', 'status'];
    }

    this.roleApi.getAll().pipe(takeUntil(this.destroy$)).subscribe({
      next: (roles) => {
        this.roleOptions = roles;
        this.updateRoleOptions();
      },
      error: () => this.errorMessage = 'Unable to load roles.'
    });
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
            roleIds: [],
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
    if (!this.editableUsers[user.id]) {
      return;
    }

    this.selectedUser = user;
    this.updateRoleOptions();
    this.workflowTabIndex = 1;
    this.editUserForm.reset({
      fullName: user.fullName, email: user.email, password: '', roleIds: user.roles.map((role) => role.id), isActive: user.isActive,
      assignedLocationName: user.assignedLocationName ?? '', assignedLatitude: user.assignedLatitude,
      assignedLongitude: user.assignedLongitude, assignedMapZoom: user.assignedMapZoom ?? 11
    });
  }

  selectUserArea(user: User): void {
    if (!this.editableUsers[user.id]) {
      return;
    }

    this.selectedUser = user;
    this.workflowTabIndex = 2;
    this.assignedAreaForm.reset({
      assignedLocationName: user.assignedLocationName ?? '',
      assignedLatitude: user.assignedLatitude,
      assignedLongitude: user.assignedLongitude,
      assignedMapZoom: user.assignedMapZoom ?? 11
    });
  }

  deleteUser(user: User): void {
    if (!this.editableUsers[user.id]) return;
    const dialogRef = this.dialog.open(ConfirmDialogComponent, { data: {
      title: 'Delete user', message: `Delete user "${user.fullName}"?`
    }, panelClass: 'confirm-dialog-panel' });
    dialogRef.componentInstance.getConfirm().pipe(take(1), takeUntil(this.destroy$)).subscribe(() => this.userApi.delete(user.id).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => { this.successMessage = 'User deleted.'; if (this.selectedUser?.id === user.id) this.cancelUserEdit(); this.refresh(); },
      error: () => this.errorMessage = 'Unable to delete user.'
    }));
  }

  updateUser(): void {
    this.formErrorMessage = '';
    this.successMessage = '';
    if (!this.selectedUser || this.editUserForm.invalid) {
      this.editUserForm.markAllAsTouched();
      this.formErrorMessage = 'Please fix the highlighted fields.';
      return;
    }
    this.userApi.update(this.selectedUser.id, this.mapUpdateUserRequest()).pipe(takeUntil(this.destroy$)).subscribe({
      next: (updatedUser) => {
        this.successMessage = 'User updated.';
        this.users = this.users.map((user) => user.id === updatedUser.id ? updatedUser : user);
        this.updateUserPresentation();
        this.refreshCurrentUserIfNeeded(updatedUser);
        this.selectedUser = null;
        this.updateRoleOptions();
        this.editUserForm.reset({ roleIds: [], isActive: true, assignedMapZoom: 11 });
        this.workflowTabIndex = 0;
      },
      error: () => this.formErrorMessage = 'Unable to update user.'
    });
  }

  cancelUserEdit(): void {
    this.selectedUser = null;
    this.updateRoleOptions();
    this.formErrorMessage = '';
    this.editUserForm.reset({ roleIds: [], isActive: true, assignedMapZoom: 11 });
    this.workflowTabIndex = 0;
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
          this.updateUserPresentation();
          this.refreshCurrentUserIfNeeded(updatedUser);
          this.assignedAreaForm.reset({ assignedLocationName: '', assignedLatitude: null, assignedLongitude: null, assignedMapZoom: 11 });
          this.selectedUser = null;
          this.updateRoleOptions();
          this.workflowTabIndex = 0;
        },
        error: () => {
          this.formErrorMessage = 'Unable to update assigned area.';
        }
      });
  }

  cancelAssignedArea(): void {
    this.selectedUser = null;
    this.updateRoleOptions();
    this.formErrorMessage = '';
    this.assignedAreaForm.reset({
      assignedLocationName: '',
      assignedLatitude: null,
      assignedLongitude: null,
      assignedMapZoom: 11
    });
    this.workflowTabIndex = 0;
  }

  private mapLoadData(result: PagedResult<User>): void {
    this.users = result.items;
    this.totalCount = result.totalCount;
    this.pageIndex = result.pageNumber - 1;
    this.pageSize = result.pageSize;
    this.updateUserPresentation();
  }

  private mapCreateUserRequest(): CreateUserRequest {
    const value = this.userForm.getRawValue();
    return {
      fullName: value.fullName,
      email: value.email,
      password: value.password,
      roleIds: value.roleIds,
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

  private mapUpdateUserRequest(): UpdateUserRequest {
    const value = this.editUserForm.getRawValue();
    return {
      fullName: value.fullName, email: value.email, password: value.password || null, roleIds: value.roleIds,
      isActive: value.isActive, assignedLocationName: value.assignedLocationName || null,
      assignedLatitude: value.assignedLatitude, assignedLongitude: value.assignedLongitude,
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

  private isAdministrativeUser(user: User): boolean {
    const permissions = user.roles.flatMap((role) => role.permissions);
    return permissions.includes(PermissionCodes.Master) || (
      permissions.includes(PermissionCodes.UsersWrite) &&
      permissions.includes(PermissionCodes.RolesWrite)
    );
  }

  private isAdministrativeRole(role: Role): boolean {
    return role.permissions.includes(PermissionCodes.Master) || (
      role.permissions.includes(PermissionCodes.UsersWrite) &&
      role.permissions.includes(PermissionCodes.RolesWrite)
    );
  }

  private updateRoleOptions(): void {
    this.createRoleOptions = this.canManageAllUsers
      ? this.roleOptions
      : this.roleOptions.filter((role) => !this.isAdministrativeRole(role));

    this.editRoleOptions = !this.canManageAllUsers && this.selectedUser?.id === this.authService.currentUser?.id
      ? this.roleOptions.filter((role) => !role.permissions.includes(PermissionCodes.Master))
      : this.createRoleOptions;
  }

  private updateUserPresentation(): void {
    const currentUserId = this.authService.currentUser?.id;
    this.roleLabels = Object.fromEntries(
      this.users.map((user) => [user.id, user.roles.map((role) => role.name).join(', ') || '-'])
    );
    this.assignedAreaLabels = Object.fromEntries(this.users.map((user) => [user.id,
      user.assignedLocationName || (
        user.assignedLatitude !== null && user.assignedLongitude !== null
          ? `${user.assignedLatitude}, ${user.assignedLongitude}`
          : '-'
      )
    ]));
    this.editableUsers = Object.fromEntries(this.users.map((user) => [user.id,
      this.canManageAllUsers || user.id === currentUserId || !this.isAdministrativeUser(user)
    ]));
  }
}

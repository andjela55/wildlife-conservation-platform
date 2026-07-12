import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { RoleGuard } from './core/guards/role.guard';
import { UserRoles } from './core/models';
import { AlertsComponent } from './features/alerts/alerts.component';
import { AnimalsComponent } from './features/animals/animals.component';
import { AuthComponent } from './features/auth.component';
import { CollarsComponent } from './features/collars/collars.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { LoginComponent } from './features/login.component';
import { ReportsComponent } from './features/reports/reports.component';
import { SpeciesComponent } from './features/species/species.component';
import { UsersComponent } from './features/users.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: '',
    component: AuthComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', pathMatch: 'full', component: DashboardComponent, canActivate: [RoleGuard], data: { roles: [UserRoles.Admin, UserRoles.Ranger, UserRoles.Researcher] } },
      { path: 'species', component: SpeciesComponent, canActivate: [RoleGuard], data: { roles: [UserRoles.Admin, UserRoles.Researcher] } },
      { path: 'animals', component: AnimalsComponent, canActivate: [RoleGuard], data: { roles: [UserRoles.Admin, UserRoles.Ranger, UserRoles.Researcher] } },
      { path: 'collars', component: CollarsComponent, canActivate: [RoleGuard], data: { roles: [UserRoles.Admin, UserRoles.Researcher] } },
      { path: 'reports', component: ReportsComponent, canActivate: [RoleGuard], data: { roles: [UserRoles.Admin, UserRoles.Ranger, UserRoles.Researcher] } },
      { path: 'alerts', component: AlertsComponent, canActivate: [RoleGuard], data: { roles: [UserRoles.Admin, UserRoles.Ranger, UserRoles.Researcher] } },
      { path: 'users', component: UsersComponent, canActivate: [RoleGuard], data: { roles: [UserRoles.Master] } }
    ]
  },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppRoutes, toNavigationRoute } from './app-route-definitions';
import { AuthGuard } from './core/guards/auth.guard';
import { RoleGuard } from './core/guards/role.guard';
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
  { path: AppRoutes.Login.Path, component: LoginComponent },
  {
    path: '',
    component: AuthComponent,
    canActivate: [AuthGuard],
    children: [
      { path: AppRoutes.Dashboard.Path, pathMatch: 'full', component: DashboardComponent, canActivate: [RoleGuard], data: { roles: AppRoutes.Dashboard.Roles, navigation: toNavigationRoute(AppRoutes.Dashboard) } },
      { path: AppRoutes.Animals.Path, component: AnimalsComponent, canActivate: [RoleGuard], data: { roles: AppRoutes.Animals.Roles, navigation: toNavigationRoute(AppRoutes.Animals) } },
      { path: AppRoutes.Species.Path, component: SpeciesComponent, canActivate: [RoleGuard], data: { roles: AppRoutes.Species.Roles, navigation: toNavigationRoute(AppRoutes.Species) } },
      { path: AppRoutes.Reports.Path, component: ReportsComponent, canActivate: [RoleGuard], data: { roles: AppRoutes.Reports.Roles, navigation: toNavigationRoute(AppRoutes.Reports) } },
      { path: AppRoutes.Alerts.Path, component: AlertsComponent, canActivate: [RoleGuard], data: { roles: AppRoutes.Alerts.Roles, navigation: toNavigationRoute(AppRoutes.Alerts) } },
      { path: AppRoutes.Collars.Path, component: CollarsComponent, canActivate: [RoleGuard], data: { roles: AppRoutes.Collars.Roles, navigation: toNavigationRoute(AppRoutes.Collars) } },
      { path: AppRoutes.Users.Path, component: UsersComponent, canActivate: [RoleGuard], data: { roles: AppRoutes.Users.Roles, navigation: toNavigationRoute(AppRoutes.Users) } }
    ]
  },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

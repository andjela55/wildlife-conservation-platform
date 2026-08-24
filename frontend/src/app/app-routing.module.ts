import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppRoutes, toNavigationRoute } from './app-route-definitions';
import { AuthGuard } from './core/guards/auth.guard';
import { PermissionGuard } from './core/guards/permission.guard';
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
      { path: AppRoutes.Dashboard.Path, pathMatch: 'full', component: DashboardComponent, canActivate: [PermissionGuard], data: { permission: AppRoutes.Dashboard.Permission, navigation: toNavigationRoute(AppRoutes.Dashboard) } },
      { path: AppRoutes.Animals.Path, component: AnimalsComponent, canActivate: [PermissionGuard], data: { permission: AppRoutes.Animals.Permission, navigation: toNavigationRoute(AppRoutes.Animals) } },
      { path: AppRoutes.Species.Path, component: SpeciesComponent, canActivate: [PermissionGuard], data: { permission: AppRoutes.Species.Permission, navigation: toNavigationRoute(AppRoutes.Species) } },
      { path: AppRoutes.Reports.Path, component: ReportsComponent, canActivate: [PermissionGuard], data: { permission: AppRoutes.Reports.Permission, navigation: toNavigationRoute(AppRoutes.Reports) } },
      { path: AppRoutes.Alerts.Path, component: AlertsComponent, canActivate: [PermissionGuard], data: { permission: AppRoutes.Alerts.Permission, navigation: toNavigationRoute(AppRoutes.Alerts) } },
      { path: AppRoutes.Collars.Path, component: CollarsComponent, canActivate: [PermissionGuard], data: { permission: AppRoutes.Collars.Permission, navigation: toNavigationRoute(AppRoutes.Collars) } },
      { path: AppRoutes.Users.Path, component: UsersComponent, canActivate: [PermissionGuard], data: { permission: AppRoutes.Users.Permission, navigation: toNavigationRoute(AppRoutes.Users) } }
    ]
  },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

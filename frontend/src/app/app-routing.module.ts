import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AlertsComponent } from './features/alerts/alerts.component';
import { AnimalsComponent } from './features/animals/animals.component';
import { CollarsComponent } from './features/collars/collars.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { ReportsComponent } from './features/reports/reports.component';
import { SpeciesComponent } from './features/species/species.component';

const routes: Routes = [
  { path: '', component: DashboardComponent },
  { path: 'species', component: SpeciesComponent },
  { path: 'animals', component: AnimalsComponent },
  { path: 'collars', component: CollarsComponent },
  { path: 'reports', component: ReportsComponent },
  { path: 'alerts', component: AlertsComponent },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

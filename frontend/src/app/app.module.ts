import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { SpeciesComponent } from './features/species/species.component';
import { AnimalsComponent } from './features/animals/animals.component';
import { CollarsComponent } from './features/collars/collars.component';
import { ReportsComponent } from './features/reports/reports.component';
import { AlertsComponent } from './features/alerts/alerts.component';

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    SpeciesComponent,
    AnimalsComponent,
    CollarsComponent,
    ReportsComponent,
    AlertsComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    ReactiveFormsModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }

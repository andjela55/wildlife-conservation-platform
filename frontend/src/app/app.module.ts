import { NgModule } from '@angular/core';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { SpeciesComponent } from './features/species/species.component';
import { AnimalsComponent } from './features/animals/animals.component';
import { CollarsComponent } from './features/collars/collars.component';
import { ReportsComponent } from './features/reports/reports.component';
import { AlertsComponent } from './features/alerts/alerts.component';
import { AuthComponent } from './features/auth.component';
import { LoginComponent } from './features/login.component';
import { UsersComponent } from './features/users.component';
import { EnumLabelPipe } from './core/pipes/enum-label.pipe';
import { MaterialModule } from './material.module';
import { AuthInterceptor } from './core/interceptors/auth.interceptor';
import { SearchableSelectComponent } from './shared/searchable-select/searchable-select.component';
import { TypingTimeDirective } from './shared/directives/typing-time.directive';
import { BelgradeDatePipe } from './core/pipes/belgrade-date.pipe';
import { ConfirmDialogComponent } from './shared/confirm-dialog/confirm-dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    SpeciesComponent,
    AnimalsComponent,
    CollarsComponent,
    ReportsComponent,
    AlertsComponent,
    AuthComponent,
    LoginComponent,
    UsersComponent,
    EnumLabelPipe,
    SearchableSelectComponent,
    TypingTimeDirective,
    BelgradeDatePipe,
    ConfirmDialogComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    ReactiveFormsModule,
    MaterialModule,
    AppRoutingModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

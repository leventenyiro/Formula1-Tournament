import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HeaderComponent } from './components/header/header.component';
import { LoginComponent } from './pages/login/login.component';
import { SeasonsComponent } from './pages/seasons/seasons.component';
import { AdminComponent } from './pages/admin/admin.component';
import { RegistrationComponent } from './pages/registration/registration.component';
import { MySeasonsComponent } from './pages/my-seasons/my-seasons.component';
import { SeasonComponent } from './pages/season/season.component';
import { ModalComponent } from './components/modal/modal.component';
import { DriverAllComponent } from './components/tables/driver-all/driver-all.component';
import { DriverResultComponent } from './components/tables/driver-result/driver-result.component';
import { TeamAllComponent } from './components/tables/team-all/team-all.component';
import { TeamResultComponent } from './components/tables/team-result/team-result.component';
import { RaceAllComponent } from './components/tables/race-all/race-all.component';
import { RaceResultComponent } from './components/tables/race-result/race-result.component';
import { SeasonFormComponent } from './components/season-form/season-form.component';
import { VerifyFormComponent } from './components/verify-form/verify-form.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    HeaderComponent,
    SeasonsComponent,
    AdminComponent,
    RegistrationComponent,
    MySeasonsComponent,
    SeasonComponent,
    ModalComponent,
    DriverAllComponent,
    DriverResultComponent,
    TeamAllComponent,
    TeamResultComponent,
    RaceAllComponent,
    RaceResultComponent,
    SeasonFormComponent,
    VerifyFormComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    ReactiveFormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }

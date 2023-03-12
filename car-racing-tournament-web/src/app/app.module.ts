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
import { CreateSeasonComponent } from './pages/create-season/create-season.component';
import { SeasonComponent } from './pages/season/season.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    HeaderComponent,
    SeasonsComponent,
    AdminComponent,
    RegistrationComponent,
    MySeasonsComponent,
    CreateSeasonComponent,
    SeasonComponent
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

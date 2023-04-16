import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { SeasonsComponent } from './pages/seasons/seasons.component';
import { RegistrationComponent } from './pages/registration/registration.component';
import { SeasonComponent } from './pages/season/season.component';
import { SettingsComponent } from './pages/settings/settings.component';
import { StatisticsComponent } from './pages/statistics/statistics.component';

const routes: Routes = [
  { path: 'seasons', component: SeasonsComponent },
  { path: 'settings', component: SettingsComponent },
  { path: 'login', component: LoginComponent },
  { path: 'registration', component: RegistrationComponent },
  { path: 'season/:id', component: SeasonComponent },
  { path: 'statistics', component: StatisticsComponent },
  { path: '**', redirectTo: 'seasons', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

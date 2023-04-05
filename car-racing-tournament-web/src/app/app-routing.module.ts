import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { SeasonsComponent } from './pages/seasons/seasons.component';
import { RegistrationComponent } from './pages/registration/registration.component';
import { SeasonComponent } from './pages/season/season.component';

const routes: Routes = [
  { path: 'seasons', component: SeasonsComponent },
  { path: 'login', component: LoginComponent },
  { path: 'registration', component: RegistrationComponent },
  { path: 'season/:id', component: SeasonComponent },
  { path: '**', redirectTo: 'seasons', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

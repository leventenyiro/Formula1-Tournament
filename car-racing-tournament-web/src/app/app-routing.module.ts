import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminComponent } from './pages/admin/admin.component';
import { LoginComponent } from './pages/login/login.component';
import { SeasonsComponent } from './pages/seasons/seasons.component';
import { RegistrationComponent } from './pages/registration/registration.component';
import { MySeasonsComponent } from './pages/my-seasons/my-seasons.component';
import { CreateSeasonComponent } from './pages/create-season/create-season.component';

const routes: Routes = [
  { path: 'seasons', component: SeasonsComponent },
  { path: 'login', component: LoginComponent},
  { path: 'registration', component: RegistrationComponent },
  { path: 'admin', component: AdminComponent },
  { path: 'my-seasons', component: MySeasonsComponent },
  { path: 'create', component: CreateSeasonComponent },
  // { path: 'season', component: SeasonComponent },
  { path: '**', redirectTo: 'seasons', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

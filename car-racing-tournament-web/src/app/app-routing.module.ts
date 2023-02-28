import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminComponent } from './admin/admin.component';
import { LoginComponent } from './login/login.component';
import { SeasonsComponent } from './seasons/seasons.component';
import { RegistrationComponent } from './registration/registration.component';
import { MySeasonsComponent } from './my-seasons/my-seasons.component';

const routes: Routes = [
  { path: 'seasons', component: SeasonsComponent },
  { path: 'login', component: LoginComponent},
  { path: 'registration', component: RegistrationComponent },
  { path: 'admin', component: AdminComponent },
  { path: 'my-seasons', component: MySeasonsComponent },
  { path: '**', redirectTo: 'seasons', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

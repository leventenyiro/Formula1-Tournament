import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'app/services/auth.service';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss']
})
export class RegistrationComponent implements OnInit {
  isFetching = false;
  error = "";

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    if (this.authService.getBearerToken() !== undefined) {
      this.router.navigate(['']);
    }
  }

  onRegistration(data: any) {
    this.isFetching = true

    this.authService.registration(data.value).subscribe({
      next: data => {
        this.router.navigate(['']);
      },
      error: err => this.error = err,
      complete: () => this.isFetching = false
    })

    this.isFetching = false;
  }

  usernamePattern() {
    return this.authService.usernamePattern();
  }

  emailPattern() {
    return this.authService.emailPattern();
  }

  passwordPattern() {
    return this.authService.passwordPattern();
  }

  passwordErrorMsg() {
    return this.authService.passwordErrorMsg();
  }

  login() {
    this.router.navigate(['/login'])
  }
}

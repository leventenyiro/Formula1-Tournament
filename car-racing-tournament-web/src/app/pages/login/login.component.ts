import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { Location } from '@angular/common';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  isFetching = false;
  error = "";
  
  constructor(private authService: AuthService, private router: Router, private location: Location) { }

  ngOnInit(): void {
    if (this.authService.getBearerToken() !== undefined) {
      this.router.navigate(['']);
    }
  }

  onLogin(data: any) {
    this.isFetching = true

    this.authService.login(data.value).subscribe({
      next: (data) => {
        document.cookie = `session=${data}`;
        this.authService.loggedIn.emit(true);
        this.isFetching = false;
        this.location.back();
      },
      error: error => {
        this.error = error
        this.isFetching = false;
      }
    })
  }

  registration() {
    this.router.navigate(['registration']);
  }
}

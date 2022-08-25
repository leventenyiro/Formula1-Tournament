import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoginService } from 'src/app/services/login.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  isFetching = false;
  error = "";

  constructor(private loginService: LoginService, private router: Router) { }

  ngOnInit(): void {
    // check if available
    if (document.cookie.split("token=")[1].split(";")[0]) {
      this.router.navigate(['/admin']);
    }
  }

  onLogin(data: any) {
    this.isFetching = true

    this.loginService.login(data.value).subscribe({
      next: (data) => {
        document.cookie = `token=${data}`;
        this.router.navigate(['/admin']);
      },
      error: err => this.error = err,
      complete: () => this.isFetching = false
    })

    this.isFetching = false;
  }
}

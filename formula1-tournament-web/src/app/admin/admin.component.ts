import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoginService } from '../services/login.service';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent implements OnInit {

  constructor(private loginService: LoginService, private router: Router) { }

  ngOnInit(): void {
    if (!this.loginService.isSessionValid(document.cookie)) {
      this.router.navigate(['/main']);
    }
  }

  logout() {
    document.cookie = "session=";
    this.router.navigate(['/main']);
  }

}

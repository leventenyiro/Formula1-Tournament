import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'app/services/auth.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {
  isLoggedIn = false;

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    this.authService.loggedIn.subscribe(
      (loggedIn: boolean) => this.isLoggedIn = loggedIn
    );
    this.isLoggedIn = this.authService.getBearerToken() !== undefined;
  }

  logout(): void {
    document.cookie = "session=";
    this.isLoggedIn = this.authService.getBearerToken() !== undefined;
    this.authService.loggedIn.emit(false);
  }
}

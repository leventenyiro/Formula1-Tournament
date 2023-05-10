import { Component, OnInit } from '@angular/core';
import { AuthService } from 'app/services/auth.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {
  isLoggedIn = false;

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    this.authService.loggedIn.subscribe(
      (loggedIn: boolean) => this.isLoggedIn = loggedIn
    );
    this.isLoggedIn = this.authService.getBearerToken() !== undefined;
    if (this.isLoggedIn === false)
      this.logout()
  }

  logout(): void {
    document.cookie = "session=";
    this.isLoggedIn = this.authService.getBearerToken() !== undefined;
    this.authService.loggedIn.emit(false);
  }

  collapseNavbar() {
    const navbarToggler = document.querySelector('.navbar-toggler');
    const navbarCollapse = document.querySelector('.navbar-collapse');
    navbarToggler!.classList.remove('show');
    navbarCollapse!.classList.remove('show');
  }
}

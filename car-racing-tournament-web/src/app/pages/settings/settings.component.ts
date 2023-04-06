import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { User } from 'app/models/user';
import { AuthService } from 'app/services/auth.service';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {
  user?: User;
  edit: boolean = false;
  error?: string = '';
  isFetching: boolean = false;
  modal: string = '';

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    this.isFetching = true;

    this.authService.loggedIn.subscribe(
      (loggedIn: boolean) => {
        if (!loggedIn) {
          this.router.navigate(['']);
        }
      }
    );

    this.authService.getUser().subscribe({
      next: user => this.user = user
    });
    this.isFetching = false;
  }

  setEdit(edit: boolean) {
    this.edit = edit;
  }

  usernamePattern() {
    return this.authService.usernamePattern();
  }

  emailPattern() {
    return this.authService.emailPattern();
  }

  updateUser() {

  }

  deleteUser() {

  }

  openModal(modal: string) {
    this.modal = modal;
  }

  closeModal() {
    this.modal = '';
  }
}

import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
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

  inputUsername = new FormControl('');
  inputEmail = new FormControl('');

  inputPasswordOld = new FormControl('');
  inputPassword = new FormControl('');
  inputPasswordAgain = new FormControl('');

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
      next: user => {
        this.user = user;
        this.inputUsername.setValue(user.username);
        this.inputEmail.setValue(user.email);
      }
    });
    this.isFetching = false;
  }

  onCancel() {
    this.inputUsername.setValue(this.user?.username);
    this.inputEmail.setValue(this.user?.email);
    this.error = '';
    this.setEdit(false);
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

  passwordPattern() {
    return this.authService.passwordPattern();
  }

  passwordErrorMsg() {
    return this.authService.passwordErrorMsg();
  }

  updateUser() {
    this.isFetching = true;
    this.authService.updateUser({ username: this.inputUsername.value, email: this.inputEmail.value }).subscribe({
      error: error => {
        this.error = error;
        this.isFetching = false;
      },
      complete: () => {
        this.isFetching = false;
        this.error = '';
        this.setEdit(false);
      }
    });
  }

  updatePassword() {
    this.isFetching = true;
    this.authService.updatePassword(
      this.inputPasswordOld.value,
      this.inputPassword.value,
      this.inputPasswordAgain.value
    ).subscribe({
      error: error => {
        this.inputPasswordOld.setValue('');
        this.inputPassword.setValue('');
        this.inputPasswordAgain.setValue('');
        
        this.error = error;
        this.isFetching = false;
      },
      complete: () => {
        this.error = '';
        this.closeModal();
        this.isFetching = false;
      }
    });
  }

  deleteUser() {
    this.isFetching = true;
    this.authService.deleteUser().subscribe({
      error: () => this.isFetching = false,
      complete: () => {
        this.isFetching = false;
        this.modal = '';
        this.authService.loggedIn.emit(false);
        document.cookie = "session=";
        this.router.navigate(['seasons']);
      }
    });
  }

  openModal(modal: string) {
    this.modal = modal;
  }

  closeModal() {
    this.modal = '';
  }
}

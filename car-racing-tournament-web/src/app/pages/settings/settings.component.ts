import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
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

  updatePasswordForm = new FormGroup({
    inputPasswordOld: new FormControl(''),
    inputPassword: new FormControl(''),
    inputPasswordAgain: new FormControl(''),
  });

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

    this.authService.checkIfLoggedIn(true);
    this.authService.getUser().subscribe({
      next: user => {
        this.user = user;
        this.inputUsername.setValue(user.username);
        this.inputEmail.setValue(user.email);
        this.isFetching = false;
      },
      error: error => {
        this.error = error;
        this.isFetching = false;
      }
    });
  }

  onCancel() {
    this.inputUsername.setValue(this.user?.username);
    this.inputEmail.setValue(this.user?.email);
    this.error = '';
    this.setEdit(false);
  }

  setEdit(edit: boolean) {
    this.authService.checkIfLoggedIn(true);
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
    this.authService.checkIfLoggedIn(true);
    this.authService.updateUser({ username: this.inputUsername.value, email: this.inputEmail.value }).subscribe({
      next: () => {
        this.isFetching = false;
        this.error = '';
        this.setEdit(false);
      },
      error: error => {
        this.error = error;
        this.isFetching = false;
      }
    });
  }

  updatePassword() {
    this.isFetching = true;
    this.authService.checkIfLoggedIn(true);
    this.authService.updatePassword(
      this.updatePasswordForm.get('inputPasswordOld')!.value,
      this.updatePasswordForm.get('inputPassword')!.value,
      this.updatePasswordForm.get('inputPasswordAgain')!.value,
    ).subscribe({
      next: () => {
        this.error = '';
        this.closeModal();
        this.isFetching = false;
      },
      error: error => {
        this.updatePasswordForm.get('inputPasswordOld')!.setValue('');
        this.updatePasswordForm.get('inputPassword')!.setValue('');
        this.updatePasswordForm.get('inputPasswordAgain')!.setValue('');
        
        this.error = error;
        this.isFetching = false;
      }
    });
  }

  deleteUser() {
    this.isFetching = true;
    this.authService.checkIfLoggedIn(true);
    this.authService.deleteUser().subscribe({
      next: () => {
        this.isFetching = false;
        this.modal = '';
        document.cookie = "session=";
        this.authService.loggedIn.emit(false);
        this.router.navigate(['seasons']);
      },
      error: error => {
        this.error = error;
        this.isFetching = false;
      }
    });
  }

  openModal(modal: string) {
    this.modal = modal;
  }

  closeModal() {
    this.error = '';
    this.modal = '';
  }

  removeError() {
    this.error = '';
  }
}

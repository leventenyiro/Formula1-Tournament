import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { EventEmitter, Injectable } from '@angular/core';
import { Registration } from 'app/models/registration';
import { User } from 'app/models/user';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import { Login } from '../models/login';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  loggedIn = new EventEmitter<boolean>();

  constructor(private http: HttpClient, private router: Router) { }

  login(login: Login) {
    return this.http
    .post(
      `${environment.backendUrl}/user/login`,
      {
        "usernameEmail": login.usernameEmail,
        "password": login.password
      },
      {
        responseType: 'text'
      }
    ).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error));
      })
    )
  }

  registration(registation: Registration) {
    return this.http
    .post(
      `${environment.backendUrl}/user/registration`,
      {
        "username": registation.username,
        "email": registation.email,
        "password": registation.password,
        "passwordAgain": registation.passwordAgain
      },
      {
        responseType: 'text'
      }
    ).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error));
      })
    )
  }

  getBearerToken() {    
    if (!document.cookie.includes('session=') || document.cookie.split('session=').length == 1) {
      // this.loggedIn.emit(false);
      return undefined;
    }
    const bearerToken = document.cookie.split("session=")[1].split(";")[0];
    
    if (!bearerToken) {
      return undefined;
    }
    return bearerToken;
  }

  getUser(): Observable<User> {
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    .set('Authorization', `Bearer ${this.getBearerToken()}`)
    return this.http
    .get<User>(
        `${environment.backendUrl}/user`,
        {
            headers: headers
        }
    ).pipe(
        tap(data => JSON.stringify(data)),
        catchError((error: HttpErrorResponse) => {
          return throwError(() => new Error(error.error));
        })
    );
  }

  updateUser(user: User) {
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    .set('Authorization', `Bearer ${this.getBearerToken()}`);

    return this.http.put(
      `${environment.backendUrl}/user`,
      {
        "username": user.username,
        "email": user.email
      },
      {
        headers: headers,
        responseType: 'text'
      }
    ).pipe(
      tap(data => data),
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error));
      })
    )
  }

  updatePassword(passwordOld: string, password: string, passwordAgain: string) {
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    .set('Authorization', `Bearer ${this.getBearerToken()}`);

    return this.http.put(
      `${environment.backendUrl}/user/password`,
      {
        "passwordOld": passwordOld,
        "password": password,
        "passwordAgain": passwordAgain
      },
      {
        headers: headers,
        responseType: 'text'
      }
    ).pipe(
      tap(data => data),
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error));
      })
    )
  }

  deleteUser() {
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    .set('Authorization', `Bearer ${this.getBearerToken()}`);

    return this.http.delete(
      `${environment.backendUrl}/user`,
      {
        headers: headers
      }
    ).pipe(
      tap(data => data),
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error));
      })
    )
  }

  public usernamePattern() {
    return environment.validation.nameRegex;
  }
  
  public emailPattern() {
    return environment.validation.emailRegex;
  }

  passwordPattern() {
    return environment.validation.passwordRegex;
  }

  passwordErrorMsg() {
    return environment.errorMessages.passwordFormat;
  }

  checkIfLoggedIn(navigate?: boolean) {
    if (this.getBearerToken() === undefined) {
      this.loggedIn.emit(false);
      if (navigate) this.router.navigate(['']);
    }
  }
}

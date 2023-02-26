import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { EventEmitter, Injectable } from '@angular/core';
import { Registration } from 'app/models/registration';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import { Login } from '../models/login';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  loggedIn = new EventEmitter<boolean>();

  constructor(private http: HttpClient) { }

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

  isSessionValid(documentCookie: string) {
    const bearerToken = documentCookie.split("session=")[1].split(";")[0];
    if (!bearerToken) {
      return false;
    }
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    .set('Authorization', `Bearer ${bearerToken}`)
    return this.http
    .get<Login>(
        `${environment.backendUrl}/user`,
        {
            headers: headers
        }
    ).pipe(
        tap(data => JSON.stringify(data)),
        catchError((error: HttpErrorResponse) => {
          return throwError(() => new Error(error.error));
        })
    ).subscribe({
      next: data => { return true },
      error: err => { return false }
    })
  }
}

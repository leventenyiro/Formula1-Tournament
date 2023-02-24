import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import { Login } from '../models/login';

@Injectable({
  providedIn: 'root'
})
export class LoginService {

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
      catchError(this.handleError)
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
        `${environment.backendUrl}/authentication`,
        {
            headers: headers
        }
    ).pipe(
        tap(data => JSON.stringify(data)),
        catchError(this.handleError)
    ).subscribe({
      next: data => { return true },
      error: err => { return false }
    })
  }

  private handleError(err: HttpErrorResponse): Observable<never> {
    let errorMessage = '';
    if (err.error instanceof ErrorEvent) {
      errorMessage = `An error occurred: ${err.error.message}`;
    } else {
      errorMessage = `Server returned code: ${err.status}, error message is: ${err.message}`;
    }

    return throwError(() => new Error(errorMessage));
  }
}

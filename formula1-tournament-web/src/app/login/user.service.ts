import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { catchError, Observable, Subject, tap, throwError } from "rxjs";
import { User } from "./login.model";

@Injectable({ providedIn: 'root' })
export class UserService {
    usersChanged = new Subject<User[]>();

    constructor(private http: HttpClient) {}

    getUsers(): Observable<User[]> {
        let headers = new HttpHeaders().set('content-type', 'application/json').set('Access-Control-Allow-Origin', '*')
        return this.http.get<User[]>(
            'https://advancedrestapi.azurewebsites.net/api/users',
            {
                headers: headers
            }
        ).pipe(
            tap(data => JSON.stringify(data)),
            catchError(this.handleError)
        )
    }

    addUser(user: User) {
        return this.http
        .post(
            'https://advancedrestapi.azurewebsites.net/api/users',
            user
        ).pipe(
            catchError(this.handleError)
        )
    }

    getUser(id: string) {
        let headers = new HttpHeaders().set('content-type', 'application/json').set('Access-Control-Allow-Origin', '*')
        return this.http
        .get<User>(
            `https://advancedrestapi.azurewebsites.net/api/users/${id}`,
            {
                headers: headers
            }
        ).pipe(
            tap(data => JSON.stringify(data)),
            catchError(this.handleError)
        )
    }

    // nincs error még
    updateUser(id: string, user: User) {
        return this.http
        .put(
            `https://advancedrestapi.azurewebsites.net/api/users/${id}`,
            user
        ).pipe(
            catchError(this.handleError)
        )
    }

    // nincs error még
    deleteUser(id: string) {
        return this.http
        .delete(
            `https://advancedrestapi.azurewebsites.net/api/users/${id}`
        ).pipe(
            catchError(this.handleError)
        )
    }

    // HIBAKEZELÉS
    private handleError(err: HttpErrorResponse): Observable<never> {
        // in a real world app, we may send the server to some remote logging infrastructure
        // instead of just logging it to the console
        let errorMessage = '';
        if (err.error instanceof ErrorEvent) {
          // A client-side or network error occurred. Handle it accordingly.
          errorMessage = `An error occurred: ${err.error.message}`;
        } else {
          // The backend returned an unsuccessful response code.
          // The response body may contain clues as to what went wrong,
          errorMessage = `Server returned code: ${err.status}, error message is: ${err.message}`;
        }

        return throwError(() => new Error(errorMessage));
      }
}
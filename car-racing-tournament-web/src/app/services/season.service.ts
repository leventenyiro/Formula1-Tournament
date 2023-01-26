import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Season } from '../models/season';

@Injectable({
  providedIn: 'root'
})
export class SeasonService {

  constructor(private http: HttpClient) { }

  getSeasons(): Observable<Season[]> {
    let headers = new HttpHeaders().set('content-type', 'application/json').set('Access-Control-Allow-Origin', '*')
    return this.http.get<Season[]>(
      `${environment.backendUrl}/season`,
        {
            headers: headers
        }
    ).pipe(
        tap(data => JSON.stringify(data)),
        catchError(this.handleError)
    )
  }

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

import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from 'app/models/user';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { environment } from '../../environments/environment'
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
        catchError((error: HttpErrorResponse) => {
          return throwError(() => new Error(error.error));
        })
    )
  }

  getSeasonsByUser(documentCookie: string): Observable<Season[]> {
    const bearerToken = documentCookie.split("session=")[1].split(";")[0];
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    .set('Authorization', `Bearer ${bearerToken}`);

    return this.http.get<Season[]>(
      `${environment.backendUrl}/season/user`,
        {
            headers: headers
        }
    ).pipe(
        tap(data => JSON.stringify(data)),
        catchError((error: HttpErrorResponse) => {
          return throwError(() => new Error(error.error));
        })
    )
  }

  createSeason(season: Season, documentCookie: string) {
    const bearerToken = documentCookie.split("session=")[1].split(";")[0];
    let headers = new HttpHeaders()
    .set('content-type', 'application/json')
    .set('Access-Control-Allow-Origin', '*')
    .set('Authorization', `Bearer ${bearerToken}`);

    return this.http.post(
      `${environment.backendUrl}/season`,
      {
        "name": season.name,
        "description": season.description
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
}

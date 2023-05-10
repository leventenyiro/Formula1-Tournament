import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { environment } from '../../environments/environment'
import { Season } from '../models/season';
import { Driver } from 'app/models/driver';
import { Result } from 'app/models/result';
import { Team } from 'app/models/team';
import { Race } from 'app/models/race';
import { AuthService } from './auth.service';
import { Statistics } from 'app/models/statistics';

@Injectable({
  providedIn: 'root'
})
export class SeasonService {

  constructor(private http: HttpClient, private authService: AuthService) { }

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

  getSeasonsByUser(): Observable<Season[]> {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', `Bearer ${this.authService.getBearerToken()}`);

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

  createSeason(season: Season) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', `Bearer ${this.authService.getBearerToken()}`);

    return this.http.post(
      `${environment.backendUrl}/season`,
      {
        "name": season.name,
        "description": season.description
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

  updateSeason(id: string, season: Season) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', `Bearer ${this.authService.getBearerToken()}`);

    return this.http.put(
      `${environment.backendUrl}/season/${id}`,
      {
        "name": season.name,
        "description": season.description
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

  archiveSeason(id: string) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', `Bearer ${this.authService.getBearerToken()}`);

    return this.http.put(
      `${environment.backendUrl}/season/${id}/archive`,
      null,
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

  deleteSeason(id: string) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', `Bearer ${this.authService.getBearerToken()}`);

    return this.http.delete(
      `${environment.backendUrl}/season/${id}`,
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

  getSeason(id: string) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
    return this.http.get<Season>(
      `${environment.backendUrl}/season/${id}`,
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

  deleteDriver(id: string) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', `Bearer ${this.authService.getBearerToken()}`);

    return this.http.delete(
      `${environment.backendUrl}/driver/${id}`,
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

  deleteResult(id: string) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', `Bearer ${this.authService.getBearerToken()}`);

    return this.http.delete(
      `${environment.backendUrl}/result/${id}`,
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

  deleteTeam(id: string) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', `Bearer ${this.authService.getBearerToken()}`);

    return this.http.delete(
      `${environment.backendUrl}/team/${id}`,
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

  deleteRace(id: string) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', `Bearer ${this.authService.getBearerToken()}`);

    return this.http.delete(
      `${environment.backendUrl}/race/${id}`,
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

  deletePermission(id: string) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', `Bearer ${this.authService.getBearerToken()}`);

    return this.http.delete(
      `${environment.backendUrl}/permission/${id}`,
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

  updatePermission(id: string) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', `Bearer ${this.authService.getBearerToken()}`);

    return this.http.put(
      `${environment.backendUrl}/permission/${id}`,
      null,
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

  createDriver(driver: Driver, seasonId: string) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', `Bearer ${this.authService.getBearerToken()}`);

    return this.http.post(
      `${environment.backendUrl}/season/${seasonId}/driver`,
      {
        "name": driver.name,
        "realName": driver.realName,
        "number": driver.number,
        "actualTeamId": driver.actualTeamId
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

  updateDriver(id: string, driver: Driver) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', `Bearer ${this.authService.getBearerToken()}`);

    return this.http.put(
      `${environment.backendUrl}/driver/${id}`,
      {
        "name": driver.name,
        "realName": driver.realName,
        "number": driver.number,
        "actualTeamId": driver.actualTeamId
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

  createResult(result: Result) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', `Bearer ${this.authService.getBearerToken()}`);

    return this.http.post(
      `${environment.backendUrl}/result`,
      {
        "type": result.type,
        "position": result.position,
        "point": result.point,
        "driverId": result.driverId,
        "teamId": result.teamId,
        "raceId": result.raceId
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

  updateResult(id: string, result: Result) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', `Bearer ${this.authService.getBearerToken()}`);

    return this.http.put(
      `${environment.backendUrl}/result/${id}`,
      {
        "type": result.type,
        "position": result.position,
        "point": result.point,
        "driverId": result.driverId,
        "teamId": result.teamId,
        "raceId": result.raceId
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

  createTeam(team: Team, seasonId: string) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', `Bearer ${this.authService.getBearerToken()}`);

    return this.http.post(
      `${environment.backendUrl}/season/${seasonId}/team`,
      {
        "name": team.name,
        "color": team.color
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

  updateTeam(id: string, team: Team) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', `Bearer ${this.authService.getBearerToken()}`);

    return this.http.put(
      `${environment.backendUrl}/team/${id}`,
      {
        "name": team.name,
        "color": team.color
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

  createRace(race: Race, seasonId: string) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', `Bearer ${this.authService.getBearerToken()}`);

    return this.http.post(
      `${environment.backendUrl}/season/${seasonId}/race`,
      {
        "name": race.name,
        "dateTime": race.dateTime
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

  updateRace(id: string, race: Race) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', `Bearer ${this.authService.getBearerToken()}`);

    return this.http.put(
      `${environment.backendUrl}/race/${id}`,
      {
        "name": race.name,
        "dateTime": race.dateTime
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

  createPermission(usernameEmail: string, seasonId: string) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/x-www-form-urlencoded')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', `Bearer ${this.authService.getBearerToken()}`);

    const params = new URLSearchParams();
    params.set('usernameEmail', usernameEmail);

    return this.http.post(
      `${environment.backendUrl}/season/${seasonId}/permission`,
      params.toString(),
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

  createFavorite(seasonId: string) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', `Bearer ${this.authService.getBearerToken()}`);

    return this.http.post(
      `${environment.backendUrl}/favorite/${seasonId}`,
      null,
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

  deleteFavorite(id: string) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', `Bearer ${this.authService.getBearerToken()}`);

    return this.http.delete(
      `${environment.backendUrl}/favorite/${id}`,
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

  getStatistics(name: string): Observable<Statistics> {
    let headers = new HttpHeaders()
      .set('content-type', 'application/x-www-form-urlencoded')
      .set('Access-Control-Allow-Origin', '*');

    return this.http.get<Statistics>(
      `${environment.backendUrl}/driver/statistics/${name}`,
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

  getFormattedDate(dateStr: Date, needSeconds: boolean) {
    const date = new Date(dateStr);
    const timeZoneOffset = date.getTimezoneOffset() * 60000;
    const localDate = new Date(date.getTime() - timeZoneOffset);
    return `${localDate.getFullYear()}-` +
      `${(Number(localDate.getMonth()) + 1).toString().padStart(2, '0')}-` +
      `${localDate.getDate().toString().padStart(2, '0')} ` +
      `${localDate.getHours().toString().padStart(2, '0')}:` +
      `${localDate.getMinutes().toString().padStart(2, '0')}` +
      `${needSeconds ? ':' + localDate.getSeconds().toString().padStart(2, '0') : ''}`;
  }

  resultConverter(data: any): any {
    if (data.position === 'DNF' || data.position === 'DSQ' || data.position === 'DNS') {
      data.type = data.position;
      data.position = 0;
    } else
      data.type = 'Finished';

    return data;
  }
}

import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Permission } from 'app/models/permission';
import { Season } from 'app/models/season';
import { User } from 'app/models/user';
import { AuthService } from 'app/services/auth.service';
import { SeasonService } from 'app/services/season.service';

@Component({
  selector: 'app-season',
  templateUrl: './season.component.html',
  styleUrls: ['./season.component.scss']
})
export class SeasonComponent implements OnInit {
  id!: string;
  season?: Season;
  error = '';
  isFetching = false;
  createdAt?: string;
  selectType = new FormControl('drivers');
  selectValue = new FormControl('all');
  isLoggedIn = false;
  user?: User;
  modal: string = '';
  selectedPermissionId?: string;

  constructor(
    private route: ActivatedRoute,
    private seasonService: SeasonService,
    private router: Router,
    private authService: AuthService,
  ) {
    this.selectType.valueChanges.subscribe(() => {
      this.selectValue.setValue('all');
    });
  }

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id')!;

    this.isLoggedIn = this.authService.getBearerToken() !== undefined;      
    this.onFetchData();

    if (this.route.snapshot.queryParamMap.get('type')) {
      this.selectType.setValue(this.route.snapshot.queryParamMap.get('type'));
      
      if (this.route.snapshot.queryParamMap.get('value')) {
        try {
          this.selectValue.setValue(this.route.snapshot.queryParamMap.get('value'));
        } catch {}
      }
    } else {
      this.onChange();
    }
  }

  onFetchData(): any {
    this.isFetching = true;

    if (this.isLoggedIn) {
      this.authService.getUser().subscribe({
        next: user => this.user = user,
        error: error => this.error = error
      });
    }

    this.seasonService.getSeason(this.id).subscribe({
      next: season => {
        this.season = season;
        this.createdAt = this.seasonService.getFormattedDate(this.season!.createdAt, true);
        this.isFetching = false;
      },
      error: error => {
        this.error = error;
        this.isFetching = false;
        this.router.navigate(['season']);
      }
    });
  }

  onChange() {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        type: this.selectType.value,
        value: this.selectValue.value
      },
      queryParamsHandling: 'merge'
    });
  }

  getUserPermission() {    
    return this.season?.permissions.find(x => x.userId === this.user?.id);
  }

  hasPermission() {
    this.authService.loggedIn.subscribe(
      (loggedIn: boolean) => {
        this.isLoggedIn = loggedIn;
      }
    );
    return this.isLoggedIn && (this.getUserPermission()?.type === 0 || this.getUserPermission()?.type === 1);
  }

  getDriverAll() {
    return this.season?.drivers.map(driver => {
      const actualTeam = this.season?.teams.find(team => team.id === driver.actualTeamId);

      const results = this.season?.races
        .map(race => race.results.find(result => result.driverId === driver.id))!
        .filter(x => x !== undefined);

      return {
        id: driver.id,
        name: driver.name,
        realName: driver.realName,
        nationality: driver.nationality,
        number: driver.number,
        actualTeam: {
          id: actualTeam?.id,
          name: actualTeam?.name,
          color: actualTeam?.color,
        },
        point: results!.length === 0
          ? 0
          : results!.map(result => result!.point).reduce((sum, current) => sum + current)
      }
    }).sort((a: any, b: any) => b.point - a.point);
  }

  getDriverById(id: string) {
    return this.season?.races.map(race => {
      const result = race.results.find(result => result.driverId === id);

      return result !== undefined ? {
        id: result.id,
        race: {
          id: race.id,
          name: race.name,
          dateTime: this.seasonService.getFormattedDate(race.dateTime, false)
        },
        team: this.season?.teams.find(team => team.id === result!.teamId),
        position: result!.type.toString() === 'Finished' ? result!.position : result!.type.toString(),
        type: result!.type.toString(),
        point: result!.point
      } : undefined
    }).filter(x => x !== undefined);
  }

  getTeamAll() {
    return this.season?.teams.map(team => {
      const results = this.season?.races
        .map(race => race.results.filter(result => result.teamId === team.id))!
        .flat()
        .filter(x => x !== undefined);

      return ({
        id: team.id,
        name: team.name,
        color: team.color,
        point: results!.length === 0
          ? 0
          : results!.map(result => result!.point).reduce((sum, current) => sum + current)
      })
    }).sort((a: any, b: any) => b.point - a.point);
  }

  getTeamById(id: string): any {
    return this.season?.races
      .map(race => {
        const results = race.results.filter(result => result.teamId === id);

        return {
          race: {
            name: race.name,
            dateTime: race.dateTime
          },
          point: results!.length === 0
          ? 0
          : results!.map(result => result!.point).reduce((sum, current) => sum + current)
        }
      })
      .filter(x => x !== undefined);
  }

  getRaceAll() {
    return this.season?.races.map(x => {
      const winnerResult = x.results.find(x => x.position === 1);
      const winnerDriver = this.season?.drivers.find(x => x.id === winnerResult?.driverId);
      const winnerTeam = this.season?.teams.find(x => x.id === winnerResult?.teamId);

      return {
        id: x.id,
        name: x.name,
        dateTime: this.seasonService.getFormattedDate(x.dateTime, false),
        winner: {
          name: winnerDriver?.name,
          realName: winnerDriver?.realName,
          team: {
            name: winnerTeam?.name,
            color: winnerTeam?.color
          }
        }
      }
    });
  }

  getRaceById(id: string) {
    return this.season?.races.find(x => x.id === id)?.results
    .map(result => ({
        id: result.id,
        driver: this.season?.drivers
          .find(driver => driver.id === result.driverId),
        team: this.season?.teams
          .find(team => team.id === result.teamId),
        position: result.type.toString() === 'Finished' 
          ? result.position 
          : result.type.toString(),
        type: result.type.toString(),
        point: result.point
    }))
    .sort((a: any, b: any) => {
      const posA = this.getPositionValue(a.position);
      const posB = this.getPositionValue(b.position);
      if (posA === posB) {
        return a.id - b.id;
      }
      return posA - posB;
    });
  }

  getPositionValue(position: any): number {
    switch (position) {
      case 'DNF':
        return 100;
      case 'DNS':
        return 101;
      case 'DSQ':
        return 102;
      default:
        return parseInt(position);
    }
  }

  getPermissions() {
    return this.season?.permissions.sort((a: Permission, b: Permission) => b.type - a.type);
  }

  archiveSeason() {
    this.isFetching = true;
    this.seasonService.archiveSeason(this.season!.id).subscribe({
      next: () => {
        this.closeModal();
        this.isFetching = false;
        this.onFetchData();
      },
      error: error => {
        this.error = error,
        this.isFetching = false;
      }
    });
  }

  deleteSeason() {
    this.isFetching = true;
    this.seasonService.deleteSeason(this.season!.id).subscribe({
      next: () => {
        this.closeModal();
        this.isFetching = false;
        this.router.navigate(['seasons'])
      },
      error: error => {
        this.error = error,
        this.isFetching = false;
      }
    });
  }

  deletePermission(id: string) {
    this.isFetching = true;
    this.seasonService.deletePermission(id).subscribe({
      next: () => {
        this.closeModal();
        this.isFetching = false;
        this.onFetchData();
      },
      error: error => {
        this.error = error;
        this.isFetching = false;
      }
    });
  }

  updatePermission(id: string) {
    this.isFetching = true;
    this.seasonService.updatePermission(id).subscribe({
      next: () => {
        this.closeModal();
        this.isFetching = false;
        this.onFetchData();
      },
      error: error => {
        this.error = error;
        this.isFetching = false;
      }
    });
  }

  createPermission(data: any) {
    this.isFetching = true;
    if (data.value.usernameEmail === '') {
      this.error = 'Username or e-mail is missing!';
      this.isFetching = false;
      return;
    }
    this.seasonService.createPermission(data.value.usernameEmail, this.season!.id).subscribe({
      next: () => {
        this.closeModal();
        this.isFetching = false;
        this.onFetchData();
      },
      error: error => {
        this.error = error;
        this.isFetching = false;
      }
    });
  }

  isFavorite(season: Season): boolean {
    if (!this.isLoggedIn || this.user === undefined || this.user.favorites === undefined)
      return false;
    return this.user.favorites.map(x => x.seasonId).includes(season?.id);
  }

  setFavorite(season: Season) {
    if (this.isLoggedIn) {      
      if (this.isFavorite(season)) {
        this.isFetching = true;
        this.seasonService.deleteFavorite(this.user!.favorites!.find(x => x.seasonId === season.id)!.id!).subscribe({
          next: () => {
            this.isFetching = false;
            this.onFetchData();
          },
          error: err => {
            this.error = err;
            this.isFetching = false;
          }
        });
      } else {
        this.seasonService.createFavorite(season.id).subscribe({
          next: () => {
            this.isFetching = false;
            this.onFetchData();
          },
          error: err => {
            this.error = err;
            this.isFetching = false;
          }
        });
      }
    }
  }

  openModal(modal: string, id?: string) {
    if (this.season?.isArchived)
      return;
    this.modal = modal;
    this.selectedPermissionId = id;
  }

  closeModal() {
    this.modal = '';
    this.error = '';
    this.selectedPermissionId = '';    
  }

  removeError() {
    this.error = '';
  }
}

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
    return this.season?.drivers.map(x => ({
      id: x.id,
      name: x.name,
      realName: x.realName,
      number: x.number,
      actualTeam: {
        id: x.actualTeam?.id,
        name: x.actualTeam?.name,
        color: x.actualTeam?.color,
      },
      point: x.results.length === 0
        ? 0
        : x.results.map(x => x.point).reduce((sum, current) => sum + current)
    })).sort((a: any, b: any) => b.point - a.point);
  }

  getDriverById(id: string) {
    return this.season?.drivers
      .find(x => x.id === id)?.results
      .sort((a: any, b: any) => a.dateTime - b.dateTime)
      .map(x => ({
        id: x.id,
        race: {
          id: x.race.id,
          name: x.race.name,
          dateTime: this.seasonService.getFormattedDate(x.race.dateTime, false)
        },
        team: x.team,
        position: x.type.toString() === 'Finished' ? x.position : x.type.toString(),
        type: x.type.toString(),
        point: x.point
      }));
  }

  getTeamAll() {
    return this.season?.teams.map(x => ({
      id: x.id,
      name: x.name,
      color: x.color,
      point: x.results.length === 0
        ? 0
        : x.results.map(x => x.point).reduce((sum, current) => sum + current)
    })).sort((a: any, b: any) => b.point - a.point);
  }

  getTeamById(id: string): any {
    const racePoints: any = this.season?.teams
      .find(x => x.id === id)?.results
      .reduce((sum: any, result: any) => {
        const { id, name, dateTime } = result.race;
        const existingRace = sum[id];
        if (!existingRace) {
          let formattedDateTime = this.seasonService.getFormattedDate(dateTime, false);
          sum[id] = { id, race: {name, dateTime: formattedDateTime}, point: result.point };
        } else {
          existingRace.point += result.point;
        }
        return sum;
      }, {});

    return Object.values(racePoints).sort((a: any, b: any) => a.dateTime - b.dateTime);
  }

  getRaceAll() {
    return this.season?.races.map(x => {
      const winnerResult = x.results.find(x => x.position === 1);
      return {
        id: x.id,
        name: x.name,
        dateTime: this.seasonService.getFormattedDate(x.dateTime, false),
        winner: {
          name: winnerResult?.driver.name,
          realName: winnerResult?.driver.realName,
          team: {
            name: winnerResult?.team.name,
            color: winnerResult?.team.color
          }
        }
      }
    });
  }

  getRaceById(id: string) {
    return this.season?.races.find(x => x.id === id)?.results
    .map(x => ({
      id: x.id,
      driver: x.driver,
      team: x.team,
      position: x.type.toString() === 'Finished' ? x.position : x.type.toString(),
      type: x.type.toString(),
      point: x.point
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
      case 'DSQ':
        return 101;
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
        this.seasonService.deleteFavorite(this.user!.favorites!.find(x => x.seasonId === season.id && x.userId === this.user!.id)!.id!).subscribe({
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
        this.seasonService.createFavorite(this.user!.id!, season.id).subscribe({
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
}

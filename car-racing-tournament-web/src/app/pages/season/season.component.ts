import { Time } from '@angular/common';
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
  error = "";
  isFetching = false;
  createdAt?: string;
  selectType = new FormControl('drivers');
  selectValue = new FormControl('all');
  isLoggedIn = false;
  user?: User;

  constructor(
    private route: ActivatedRoute,
    private seasonService: SeasonService,
    private router: Router,
    private authService: AuthService
  ) {
    this.selectType.valueChanges.subscribe(() => {
      this.selectValue.setValue('all');
    });
  }

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get("id")!;

    this.onFetchData();

    this.isFetching = true;
    this.authService.loggedIn.subscribe(
      (loggedIn: boolean) => {
        this.isLoggedIn = loggedIn;
      }
    );

    this.isLoggedIn = this.authService.isSessionValid(document.cookie) ? true : false;
    this.isFetching = false;

    if (this.isLoggedIn) {
      this.isFetching = true;
      this.authService.getUser(document.cookie).subscribe({
        next: user => this.user = user,
        error: () => {
          this.isFetching = false;
        }
      });
    }
  }

  onFetchData() {
    this.isFetching = true;
    this.seasonService.getSeason(this.id).subscribe({
      next: season => this.season = season,
      error: () => this.router.navigate(['season']),
      complete: () => {
        this.isFetching = false;
        this.createdAt = this.getFormattedDate(this.season!.createdAt);
      }
    });
  }

  getFormattedDate(dateStr: any) {
    const date = new Date(dateStr);
    return `${date.getFullYear()}-` +
    `${(Number(date.getMonth()) + 1).toString().padStart(2, '0')}-` +
    `${date.getDate().toString().padStart(2, '0')} ` +
    `${date.getHours().toString().padStart(2, '0')}:` +
    `${date.getMinutes().toString().padStart(2, '0')}:` +
    `${date.getSeconds().toString().padStart(2, '0')}`;
  }

  getUserPermission() {    
    return this.season?.permissions.find(x => x.userId === this.user?.id);
  }

  getDriverAll() {
    return this.season?.drivers.map(x => ({
      id: x.id,
      name: x.name,
      realName: x.realName,
      number: x.number,
      actualTeam: {
        name: x.actualTeam.name,
        color: x.actualTeam.color,
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
          name: x.race.name,
          dateTime: this.getFormattedDate(x.race.dateTime)
        },
        team: x.team,
        position: x.type.toString() === 'Finished' ? x.position : x.type.toString(),
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
          let formattedDateTime = this.getFormattedDate(dateTime);
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
        dateTime: this.getFormattedDate(x.dateTime),
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
    .sort((a: any, b: any) => b.point - a.point)
    .map(x => ({
      id: x.id,
      driver: x.driver,
      team: x.team,
      position: x.type.toString() === 'Finished' ? x.position : x.type.toString(),
      point: x.point
    }))
  }

  getPermissions() {
    return this.season?.permissions.sort((a: Permission, b: Permission) => b.type - a.type);
  }

  createDriver() {
    console.log("createDriver");
  }

  updateDriver(id: string) {
    console.log("updateDriver");
  }

  deleteDriver(id: string) {
    this.isFetching = true;
    this.seasonService.deleteDriver(id).subscribe({
      error: () => {},
      complete: () => {
        this.isFetching = false;
        this.onFetchData();
      }
    });
  }

  createResult() {
    console.log("createResult");
  }

  updateResult(id: string) {
    console.log("updateResult");
  }

  deleteResult(id: string) {
    this.isFetching = true;
    this.seasonService.deleteResult(id).subscribe({
      error: () => {},
      complete: () => {
        this.isFetching = false;
        this.onFetchData();
      }
    });
  }

  createTeam() {
    console.log("createTeam");
  }

  updateTeam(id: string) {
    console.log("updateTeam");
  }

  deleteTeam(id: string) {    
    this.isFetching = true;
    this.seasonService.deleteTeam(id).subscribe({
      error: () => {},
      complete: () => {
        this.isFetching = false;
        this.onFetchData();
      }
    });
  }

  createRace() {
    console.log("createRace");
  }

  updateRace(id: string) {
    console.log("updateRace");
  }

  deleteRace(id: string) {
    this.isFetching = true;
    this.seasonService.deleteRace(id).subscribe({
      error: () => {},
      complete: () => {
        this.isFetching = false;
        this.onFetchData();
      }
    });
  }

  updateSeason() {
    console.log("updateSeason");
  }

  archiveSeason() {
    console.log("archiveSeason");
  }

  deleteSeason() {
    /*this.isFetching = true;
    this.seasonService.deleteSeason(this.season?.id).subscribe({
      error: () => {},
      complete: () => {
        this.isFetching = false;
        this.router.navigate(['seasons'])
      }
    });*/
  }

  deletePermission(id: string) {
    this.isFetching = true;
    this.seasonService.deletePermission(id).subscribe({
      error: () => {},
      complete: () => {
        this.isFetching = false;
        this.onFetchData();
      }
    });
  }

  updatePermission(id: string) {
    console.log("updatePermission");
  }
}

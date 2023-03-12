import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'app/services/auth.service';
import { Subscription } from 'rxjs';
import { PermissionType } from '../../models/permission-type';
import { Season } from '../../models/season';
import { SeasonService } from '../../services/season.service';

@Component({
  selector: 'app-seasons',
  templateUrl: './seasons.component.html',
  styleUrls: ['./seasons.component.scss']
})
export class SeasonsComponent implements OnInit {
  seasons: Season[] = [];
  fetchedData: Season[] = [];
  subscription!: Subscription;
  isFetching = false;
  error = "";
  search = new FormControl('');
  isLoggedIn = false;

  constructor(
    private seasonService: SeasonService, 
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.onFetchData();
    
    this.authService.loggedIn.subscribe(
      (loggedIn: boolean) => {
        this.isLoggedIn = loggedIn;
      }
    );
    this.isLoggedInInit();
  }

  isLoggedInInit() {
    this.isLoggedIn = this.authService.isSessionValid(document.cookie) ? true : false;
  }

  onFetchData() {
    this.isFetching = true;

    this.seasonService.getSeasons().subscribe({
      next: seasons => {
        this.fetchedData = seasons;
        this.seasons = this.fetchedData;
      },
      error: err => this.error = err,
      complete: () => this.isFetching = false
    })
  }

  onSearch() {
    this.seasons = this.search.value !== '' ?
      this.seasons.filter(x => x.name.startsWith(this.search.value)) :
      this.seasons = this.fetchedData;
  }

  getAdminUsername(season: Season) {
    return season.permissions.find(x => x.type === PermissionType.Admin)?.username;
  }

  getFormattedDate(dateStr: Date) {
    const date = new Date(dateStr);
    return `${date.getFullYear()}-` +
    `${(Number(date.getMonth()) + 1).toString().padStart(2, '0')}-` +
    `${date.getDate().toString().padStart(2, '0')} ` +
    `${date.getHours().toString().padStart(2, '0')}:` +
    `${date.getMinutes().toString().padStart(2, '0')}:` +
    `${date.getSeconds().toString().padStart(2, '0')}`;
  }

  navigateSeason(id: string) {
    this.router.navigate([`season/${id}`]);
  }
}

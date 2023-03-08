import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { PermissionType } from 'app/models/permission-type';
import { Season } from 'app/models/season';
import { User } from 'app/models/user';
import { AuthService } from 'app/services/auth.service';
import { SeasonService } from 'app/services/season.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-my-seasons',
  templateUrl: './my-seasons.component.html',
  styleUrls: ['./my-seasons.component.scss']
})
export class MySeasonsComponent implements OnInit {
  seasons: Season[] = [];
  fetchedData: Season[] = [];
  subscription!: Subscription;
  isFetching = false;
  error = "";
  search = new FormControl('');
  selectRole = new FormControl('');
  user?: User;
  isLoggedIn = false;

  constructor(private seasonService: SeasonService, private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    this.isFetching = true

    this.authService.getUser(document.cookie).subscribe({
      next: user => this.user = user,
      error: () => {
        this.isFetching = false;
        this.router.navigate([''])
      }
    });

    this.onFetchData();
  }

  onFetchData() {
    this.isFetching = true;

    this.seasonService.getSeasonsByUser(document.cookie).subscribe({
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
      this.seasons.filter(x => x.name === this.search.value) :
      this.seasons = this.fetchedData;
  }

  onSelectRole() {
    switch (this.selectRole.value) {
      case 'all':
        this.seasons = this.fetchedData;
        break;
      case 'admin':
        this.seasons = this.fetchedData.filter(x => x.permissions.find(x => x.type === PermissionType.Admin)?.userId === this.user?.id);
        break;
      case 'moderator':
        this.seasons = this.fetchedData.filter(x => x.permissions.find(x => x.type === PermissionType.Admin)?.userId !== this.user?.id);
        break;
    }
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
}

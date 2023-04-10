import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'app/services/auth.service';
import { Subscription } from 'rxjs';
import { PermissionType } from '../../models/permission-type';
import { Season } from '../../models/season';
import { SeasonService } from '../../services/season.service';
import { User } from 'app/models/user';

@Component({
  selector: 'app-seasons',
  templateUrl: './seasons.component.html',
  styleUrls: ['./seasons.component.scss']
})
export class SeasonsComponent implements OnInit {
  seasons: Season[] = [];
  fetchedData: Season[] = [];
  fetchedMyData: Season[] = [];

  subscription!: Subscription;
  isFetching = false;
  error = "";
  search = new FormControl('');
  isLoggedIn = false;
  modal: boolean = false;
  user?: User;

  // filters
  checkBoxFavorites = new FormControl('');
  checkBoxAdmin = new FormControl('');
  checkBoxModerator = new FormControl('');

  constructor(
    private seasonService: SeasonService, 
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.authService.loggedIn.subscribe(
      (loggedIn: boolean) => {
        this.isLoggedIn = loggedIn;
        this.onFetchData();
      }
    );
    this.isLoggedIn = this.authService.getBearerToken() !== undefined;
      
    this.onFetchData();
    this.checkBoxFavorites.disable();
    this.checkBoxAdmin.setValue(false);
    this.checkBoxModerator.setValue(false);
  }

  onFetchData() {
    this.isFetching = true;

    if (this.isLoggedIn) {
      this.authService.getUser().subscribe({
        next: user => this.user = user,
        error: () => this.isFetching = false
      });

      this.seasonService.getSeasonsByUser().subscribe({
        next: seasons => {
          this.fetchedMyData = seasons;
        },
        error: err => this.error = err,
        complete: () => this.isFetching = false
      });
    }
    
    this.seasonService.getSeasons().subscribe({
      next: seasons => {
        this.fetchedData = seasons;
        this.onFilter();
      },
      error: err => this.error = err,
      complete: () => this.isFetching = false
    });
  }

  onSearch() {
    this.seasons = this.search.value !== '' ?
      this.seasons.filter(x => x.name.startsWith(this.search.value)) :
      this.seasons = this.fetchedData;
  }
  
  onFilter() {
    this.seasons = [];
    if (this.checkBoxAdmin.value) {
      this.seasons.push(...this.fetchedMyData.filter(x => x.permissions.find(x => x.type === PermissionType.Admin)?.userId === this.user?.id));
    }

    if (this.checkBoxModerator.value) {      
      this.seasons.push(...this.fetchedMyData.filter(x => x.permissions.find(x => x.type === PermissionType.Admin)?.userId !== this.user?.id));
    }

    if (!this.checkBoxAdmin.value && !this.checkBoxModerator.value) {
      this.seasons = this.fetchedData;
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

  navigateSeason(id: string) {
    this.router.navigate([`season/${id}`]);
  }

  setModal(modal: boolean) {
    this.modal = modal;
  }
}

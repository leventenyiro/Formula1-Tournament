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
    this.isFetching = true;
    this.authService.loggedIn.subscribe(
      (loggedIn: boolean) => {
        this.isLoggedIn = loggedIn;
        this.isFetching = false;
      }
    );
    this.isLoggedIn = this.authService.getBearerToken() !== undefined;
      
    this.onFetchData();
    this.checkBoxFavorites.setValue(false);
    this.checkBoxAdmin.setValue(false);
    this.checkBoxModerator.setValue(false);
  }

  onFetchData() {
    this.isFetching = true;

    if (this.isLoggedIn) {
      this.authService.getUser().subscribe({
        next: user => this.user = user
      });

      this.seasonService.getSeasonsByUser().subscribe({
        next: seasons => this.fetchedMyData = seasons
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

    if (this.checkBoxFavorites.value) {      
      const newSeasons = this.fetchedData.filter(x => this.user!.favorites!.map(x => x.seasonId).includes(x.id));
      this.seasons.push(...newSeasons.filter(x => !this.seasons.some(s => s.id === x.id)));
    }

    if (!this.checkBoxAdmin.value && !this.checkBoxModerator.value && !this.checkBoxFavorites.value) {
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

  isFavorite(season: Season): boolean {
    if (!this.isLoggedIn || this.user === undefined || this.user.favorites === undefined)
      return false;    
    return this.user.favorites.map(x => x.seasonId).includes(season.id);
  }

  setFavorite(event: MouseEvent, season: Season) {
    if (this.isLoggedIn) {
      event.stopPropagation();
      if (this.isFavorite(season)) {
        this.seasonService.deleteFavorite(this.user!.favorites!.find(x => x.seasonId === season.id && x.userId === this.user!.id)!.id!).subscribe({
          next: () => this.onFetchData(),
          error: err => this.error = err,
        });
      } else {
        this.seasonService.createFavorite(this.user!.id!, season.id).subscribe({
          next: () => () => this.onFetchData(),
          error: err => this.error = err,
        });
      }
    }
  }

  navigateSeason(id: string) {
    this.router.navigate([`season/${id}`]);
  }

  setModal(modal: boolean) {
    this.modal = modal;
  }
}

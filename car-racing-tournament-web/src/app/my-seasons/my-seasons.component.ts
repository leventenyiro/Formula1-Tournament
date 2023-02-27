import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { PermissionType } from 'app/models/permission-type';
import { Season } from 'app/models/season';
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

  constructor(private seasonService: SeasonService) { }

  ngOnInit(): void {
    this.onFetchData()
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
      this.seasons.filter(x => x.name === this.search.value) :
      this.seasons = this.fetchedData;
  }

  onSelectRole() {
    switch (this.selectRole.value) {
      case 'all':
        this.seasons = this.fetchedData;
        break;
      case 'admin':
        this.seasons = this.seasons.filter(x => x.permissions.find(x => x.type === PermissionType.Admin)?.user.id === ); // mi a userid-nk?
        break;
      case 'moderator':
        this.seasons = this.seasons.filter(x => x.permissions.find(x => x.type === PermissionType.Moderator)?.user.id === );
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

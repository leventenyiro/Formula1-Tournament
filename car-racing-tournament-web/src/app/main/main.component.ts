import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { Season } from '../models/season';
import { SeasonService } from '../services/season.service';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent implements OnInit {
  seasons: Season[] = [];
  fetchedData: Season[] = [];
  subscription!: Subscription;
  isFetching = false;
  error = "";
  search = new FormControl('');

  constructor(private seasonService: SeasonService, private router: Router) { }

  ngOnInit(): void {
    this.onFetchData();
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

  }
}

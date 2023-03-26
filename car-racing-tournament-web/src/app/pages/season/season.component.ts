import { Time } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ResultType } from 'app/models/result-type';
import { Season } from 'app/models/season';
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

  constructor(
    private route: ActivatedRoute,
    private seasonService: SeasonService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get("id")!;

    console.log(this.selectType.value);
    console.log(this.selectValue.value);
    

    this.isFetching = true;
    this.seasonService.getSeason(this.id).subscribe({
      next: season => this.season = season,
      error: () => this.router.navigate(['season']),
      complete: () => {
        this.isFetching = false;
        this.createdAt = this.getFormattedDate(this.season!.createdAt);
        console.log(this.getDriverAll()); // debug

      }
    });
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
      .sort((a: any, b: any) => a.dateTime - b.dateTime);
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

  getTeamById(id: string) {
    /*return this.season?.teams
      .find(x => x.id === id)?.results
      .map(x => ({
        id: x.id,
        name: x.race.name,
        
      }))
      .sort((a: any, b: any) => a.dateTime - b.dateTime);*/
  }
}

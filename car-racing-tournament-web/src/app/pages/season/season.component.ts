import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
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
        this.getDriverAll(); // debug
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
    // sort by sumpoints
    // name, realname, actualTeam, number, sumpoints
    interface AllDriver {
      id: string,
      name: string,
      realName: string,
      number: number,
      actualTeam: {
        name: string,
        color: string
      },
      point: number,
    };    

    const allDrivers: AllDriver[] = [];
    this.season?.drivers.forEach(x => {
      
      // console.log(x.results);
      
      // const map = x.results.length === 0 ? 0 : x.results.map(x => x.point).reduce((sum, current) => sum + current);
      // const point = x.results.map(x => x.points).reduce((total, point) => total + point)
      // const point = x.results.reduce((sum, result) => sum + result.point);
      // console.log(map);
      
      allDrivers.push({
        'id': x.id,
        'name': x.name,
        'realName': x.realName,
        'number': x.number,
        'actualTeam': {
          'name': x.actualTeam.name,
          'color': x.actualTeam.color,
        },
        // 'point': x.results.map(x => x.points).reduce((total, point) => total + point),
        'point': x.results.length === 0 
          ? 0 
          : x.results.map(x => x.point).reduce((sum, current) => sum + current)
      })
      
    });

    console.log(allDrivers);
    

    const example = [
      {
        name: "lewishamilton",
        realName: "Lewis Hamilton",
        number: 44,
        actualTeam: {
          name: "Mercedes",
          color: "#123133"
        },
        point: 32
      }
    ]
  }
}

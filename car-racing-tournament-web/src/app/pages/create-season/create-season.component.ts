import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'app/services/auth.service';
import { SeasonService } from 'app/services/season.service';

@Component({
  selector: 'app-create',
  templateUrl: './create-season.component.html',
  styleUrls: ['./create-season.component.scss']
})
export class CreateSeasonComponent implements OnInit {
  isFetching = false;
  error = "";

  constructor(
    private authService: AuthService,
    private seasonService: SeasonService,
    private router: Router
  ) { }

  ngOnInit(): void {
    if (!this.authService.isSessionValid(document.cookie)) {
      this.router.navigate(['']);
    }
  }

  onCreate(data: any) {
    this.isFetching = true;

    this.seasonService.createSeason(data.value, document.cookie).subscribe({
      next: (data) => {
        this.router.navigate([`season/${JSON.parse(data)}`]);
      },
      error: err => this.error = err,
      complete: () => {
        this.isFetching = false;
      }
    });




  }
}

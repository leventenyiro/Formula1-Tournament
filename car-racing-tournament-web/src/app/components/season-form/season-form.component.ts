import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Season } from 'app/models/season';
import { SeasonService } from 'app/services/season.service';

@Component({
  selector: 'app-season-form',
  templateUrl: './season-form.component.html',
  styleUrls: ['./season-form.component.scss']
})
export class SeasonFormComponent implements OnInit {
  @Input()
  selectedSeason?: Season;

  @Output()
  onCloseModalEmitter = new EventEmitter<undefined>();

  @Output()
  onFetchDataEmitter = new EventEmitter<undefined>();

  error: string = '';

  constructor(private seasonService: SeasonService) { }

  ngOnInit(): void {
  }

  createSeason(data: any) {
    this.seasonService.createSeason(data.value).subscribe({
      error: err => {
        this.error = err;
        this.onFetchDataEmitter.emit();
      },
      complete: () => {
        this.onFetchDataEmitter.emit();
        this.onCloseModalEmitter.emit();
      }
    });
  }

  updateSeason(id: string, data: any) {
    this.seasonService.updateSeason(id, data.value).subscribe({
      error: err => {
        this.error = err;
        this.onFetchDataEmitter.emit();
      },
      complete: () => {
        this.onFetchDataEmitter.emit();
        this.onCloseModalEmitter.emit();
      }
    });
  }

  closeModal() {
    this.onCloseModalEmitter.emit();
  }
}

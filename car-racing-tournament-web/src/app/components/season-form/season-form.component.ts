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
  title?: string;

  @Input()
  executeButtonText?: string;

  @Input()
  selectedSeason?: Season;

  @Output()
  onCloseModalEmitter = new EventEmitter<undefined>();

  @Output()
  onFetchDataEmitter = new EventEmitter<undefined>();

  error: string = '';
  isFetching: boolean = false;

  constructor(private seasonService: SeasonService) { }

  ngOnInit(): void {
  }

  createSeason(data: any) {
    this.isFetching = true;
    this.seasonService.createSeason(data.value).subscribe({
      next: () => {
        this.onCloseModalEmitter.emit();
        this.onFetchDataEmitter.emit();
      },
      error: err => {
        this.error = err
        this.isFetching = false
      }
    });
  }

  updateSeason(id: string, data: any) {
    this.isFetching = true;
    this.seasonService.updateSeason(id, data.value).subscribe({
      next: () => {
        this.onCloseModalEmitter.emit();
        this.onFetchDataEmitter.emit();
      },
      error: err => {
        this.error = err
        this.isFetching = false
      }
    });
  }

  closeModal() {
    this.onCloseModalEmitter.emit();
  }

  removeError() {
    this.error = '';
  }
}

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Race } from 'app/models/race';
import { Season } from 'app/models/season';
import { SeasonService } from 'app/services/season.service';

@Component({
  selector: 'app-race-all',
  templateUrl: './race-all.component.html',
  styleUrls: ['./race-all.component.scss']
})
export class RaceAllComponent implements OnInit {
  @Input()
  isLoggedIn!: boolean;

  @Input()
  season!: Season;

  @Input()
  raceAll?: any[];

  @Output()
  onFetchDataEmitter = new EventEmitter<undefined>();

  error: string = '';
  modal: boolean = false;
  selectedRace?: Race;

  constructor(private seasonService: SeasonService) { }

  ngOnInit(): void { }

  createRace(data: any) {
    this.seasonService.createRace(data.value, this.season?.id!).subscribe({
      error: err => {
        this.error = err;
        this.onFetchDataEmitter.emit();
      },
      complete: () => {
        this.onFetchDataEmitter.emit();
        this.closeModal();
      }
    });
  }

  updateRace(id: string, data: any) {
    this.seasonService.updateRace(id, data.value).subscribe({
      error: err => {
        this.error = err;
        this.onFetchDataEmitter.emit();
      },
      complete: () => {
        this.onFetchDataEmitter.emit();
        this.closeModal();
      }
    });
  }

  deleteRace(id: string) {
    this.seasonService.deleteRace(id).subscribe({
      error: () => this.onFetchDataEmitter.emit(),
      complete: () => this.onFetchDataEmitter.emit()
    });
  }

  openModal(selectedRace?: Race) {
    this.modal = true;
    console.log(selectedRace?.dateTime);
    
    this.selectedRace = selectedRace;
  }

  closeModal() {
    this.modal = false;
    this.error = '';
    this.selectedRace = undefined;
  }

  getCurrentDate() {
    return Date.now();
  }

  getSelectedDate() {
    // console.log(`${this.selectedRace?.dateTime.hours}:${this.selectedRace?.dateTime.minutes}`);
    
    return `${this.selectedRace?.dateTime.hours}:${this.selectedRace?.dateTime.minutes}`;
  }
}
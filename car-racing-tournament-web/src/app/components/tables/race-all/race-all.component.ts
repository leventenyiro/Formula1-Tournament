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
  season!: Season;

  @Input()
  raceAll?: any[];

  @Input()
  hasPermission: boolean = false;

  @Output()
  onFetchDataEmitter = new EventEmitter<undefined>();

  error: string = '';
  modal: string = '';
  selectedRace?: Race;
  isFetching: boolean = false;

  constructor(private seasonService: SeasonService) { }

  ngOnInit(): void { }

  createRace(data: any) {
    this.isFetching = true;
    data.value.dateTime = `${data.value.date}T${data.value.time}`;
    
    this.seasonService.createRace(data.value, this.season?.id!).subscribe({
      next: () => {
        this.closeModal();
        this.isFetching = false;
        this.onFetchDataEmitter.emit();
      },
      error: error => {
        this.error = error;
        this.isFetching = false;
      }
    });
  }

  updateRace(id: string, data: any) {
    this.isFetching = true;
    data.value.dateTime = `${data.value.date}T${data.value.time}`;

    this.seasonService.updateRace(id, data.value).subscribe({
      next: () => {
        this.closeModal();
        this.isFetching = false;
        this.onFetchDataEmitter.emit();
      },
      error: error => {
        this.error = error;
        this.isFetching = false;
      }
    });
  }

  deleteRace(id: string) {
    this.isFetching = true;
    this.seasonService.deleteRace(id).subscribe({
      next: () => {
        this.isFetching = false;
        this.onFetchDataEmitter.emit()
      },
      error: error => {
        this.error = error;
        this.isFetching = false;
      }
    });
  }

  openModal(modal: string, selectedRace?: Race) {
    this.modal = modal;
    this.selectedRace = selectedRace;
  }

  closeModal() {
    this.modal = '';
    this.error = '';
    this.selectedRace = undefined;
  }

  getCurrentDate() {    
    return new Date().toISOString().split('T')[0];
  }

  getCurrentTime() {
    return new Date().toISOString().split('T')[1].substring(0, 5);
  }
}

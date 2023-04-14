import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Result } from 'app/models/result';
import { Season } from 'app/models/season';
import { SeasonService } from 'app/services/season.service';

@Component({
  selector: 'app-driver-result',
  templateUrl: './driver-result.component.html',
  styleUrls: ['./driver-result.component.scss']
})
export class DriverResultComponent implements OnInit {
  @Input()
  season!: Season;

  @Input()
  driverId!: string;

  @Input()
  driverResults?: any[];

  @Input()
  hasPermission: boolean = false;

  @Output()
  onFetchDataEmitter = new EventEmitter<undefined>();

  isFetching: boolean = false;
  error: string = '';
  modal: string = '';
  selectedResult?: Result;

  constructor(private seasonService: SeasonService) { }

  ngOnInit(): void { }

  createResult(data: any) {
    if (data.value.position === 'DNF' || data.value.position === 'DSQ') {
      data.value.type = data.value.position;
      data.value.position = 0;
    } else
      data.value.type = 'Finished';
    data.value.driverId = this.driverId;

    this.seasonService.createResult(data.value).subscribe({
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

  updateResult(id: string, data: any) {
    if (data.value.position === 'DNF' || data.value.position === 'DSQ') {
      data.value.type = data.value.position;
      data.value.position = 0;
    } else
      data.value.type = 'Finished';
    data.value.driverId = this.driverId;  
    
    this.seasonService.updateResult(id, data.value).subscribe({
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

  deleteResult(id: string) {
    this.isFetching = true;
    this.seasonService.deleteResult(id).subscribe({
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

  openModal(modal: string, selectedResult?: Result) {
    this.modal = modal;    
    this.selectedResult = selectedResult;
  }

  closeModal() {
    this.modal = '';
    this.error = '';
    this.selectedResult = undefined;
  }

  availablePositions(): number[] {
    const positions = [];
    for (let i = 1; i <= 99; i++) {
      positions.push(i);
    }
    return positions;
  }

  getDriverActualTeam() {
    return this.season.drivers.find(x => x.id === this.driverId)?.actualTeam?.id;
  }
}

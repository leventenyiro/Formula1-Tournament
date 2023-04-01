import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Driver } from 'app/models/driver';
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
  isLoggedIn!: boolean;

  @Input()
  season!: Season;

  @Input()
  driverId!: string;

  @Input()
  driverResults?: any[];

  @Output()
  onFetchDataEmitter = new EventEmitter<undefined>();

  isFetching: boolean = false;
  error: string = '';
  modal: boolean = false;

  selectedResult?: Result;

  constructor(private seasonService: SeasonService) { }

  ngOnInit(): void {
  }

  createResult(data: any) {
    if (data.value.position === 'DNF' || data.value.position === 'DSQ') {
      data.value.type = data.value.position;
      data.value.position = 0;
    } else
      data.value.type = 'Finished';
    data.value.driverId = this.driverId;    

    this.seasonService.createResult(data.value).subscribe({
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

  updateResult(id: string, data: any) {
    if (data.value.position === 'DNF' || data.value.position === 'DSQ') {
      data.value.type = data.value.position;
      data.value.position = 0;
    } else
      data.value.type = 'Finished';
    data.value.driverId = this.driverId;  
    
    this.seasonService.updateResult(id, data.value).subscribe({
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

  deleteResult(id: string) {
    this.isFetching = true;
    this.seasonService.deleteResult(id).subscribe({
      error: () => this.onFetchDataEmitter.emit(),
      complete: () => this.onFetchDataEmitter.emit()
    });
  }

  openModal(selectedResult?: Result) {
    this.modal = true;    
    this.selectedResult = selectedResult;
  }

  closeModal() {
    this.modal = false;
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
}

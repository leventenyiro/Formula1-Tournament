import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Driver } from 'app/models/driver';
import { Season } from 'app/models/season';
import { SeasonService } from 'app/services/season.service';

@Component({
  selector: 'app-driver-all',
  templateUrl: './driver-all.component.html',
  styleUrls: ['./driver-all.component.scss']
})
export class DriverAllComponent implements OnInit {
  @Input()
  season!: Season;

  @Input()
  driverAll?: any[];

  @Input()
  hasPermission: boolean = false;

  @Output()
  onFetchDataEmitter = new EventEmitter<undefined>();

  error: string = '';
  modal: string = '';
  selectedDriver?: Driver;
  isFetching: boolean = false;

  constructor(private seasonService: SeasonService) { }

  ngOnInit(): void { }

  createDriver(data: any) {   
    this.isFetching = true; 
    this.seasonService.createDriver(data.value, this.season?.id!).subscribe({
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

  updateDriver(id: string, data: any) {
    this.isFetching = true;
    this.seasonService.updateDriver(id, data.value).subscribe({
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

  deleteDriver(id: string) {
    this.isFetching = true;
    this.seasonService.deleteDriver(id).subscribe({
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

  openModal(modal: string, selectedDriver?: Driver) {
    this.modal = modal;    
    this.selectedDriver = selectedDriver;
  }

  closeModal() {
    this.modal = '';
    this.error = '';
    this.selectedDriver = undefined;
  }
}

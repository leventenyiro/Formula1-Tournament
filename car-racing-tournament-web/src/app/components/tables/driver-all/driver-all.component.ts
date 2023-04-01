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
  isLoggedIn!: boolean;

  @Input()
  season!: Season;

  @Input()
  driverAll?: any[];

  @Output()
  onFetchDataEmitter = new EventEmitter<undefined>();

  error: string = '';
  modal: boolean = false;

  selectedDriver?: Driver;

  constructor(private seasonService: SeasonService) { }

  ngOnInit(): void {
  }

  createDriver(data: any) {    
    this.seasonService.createDriver(data.value, this.season?.id!).subscribe({
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

  updateDriver(id: string, data: any) {
    this.seasonService.updateDriver(id, data.value).subscribe({
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

  deleteDriver(id: string) {
    this.seasonService.deleteDriver(id).subscribe({
      error: () => this.onFetchDataEmitter.emit(),
      complete: () => this.onFetchDataEmitter.emit()
    });
  }

  openModal(selectedDriver?: Driver) {
    this.modal = true;    
    this.selectedDriver = selectedDriver;
  }

  closeModal() {
    this.modal = false;
    this.error = '';
    this.selectedDriver = undefined;
  }
}

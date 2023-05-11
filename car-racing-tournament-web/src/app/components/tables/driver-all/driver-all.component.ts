import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Driver, Nationality } from 'app/models/driver';
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
  nationalities: Nationality[] = [];

  inputName = new FormControl('');
  inputRealName = new FormControl('');
  inputNationality = new FormControl(null);
  inputNumber = new FormControl(1);
  inputActualTeamId = new FormControl(null);

  constructor(private seasonService: SeasonService) { }

  ngOnInit(): void { }

  getNationalities() {
    this.isFetching = true;
    this.seasonService.getNationalities().subscribe({
      next: data => {
        this.nationalities = data;
        this.isFetching = false;
      },
      error: error => {
        this.error = error;
        this.isFetching = false;
      }
    });
  }

  createDriver() {
    this.isFetching = true;
    if (this.inputName.value === '') {
      this.error = 'Driver name is missing!';
      this.isFetching = false;
      return;
    }
    if (this.inputNumber.value === '') {
      this.error = 'Driver number is missing!';
      this.isFetching = false;
      return;
    }
    const data = {
      'name': this.inputName.value,
      'realName': this.inputRealName.value,
      'number': this.inputNumber.value,
      'actualTeamId': this.inputActualTeamId.value,
    } as Driver;
    this.seasonService.createDriver(data, this.season?.id!).subscribe({
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

  updateDriver(id: string) {
    this.isFetching = true;
    if (this.inputName.value === '') {
      this.error = 'Driver name is missing!';
      this.isFetching = false;
      return;
    }
    if (this.inputNumber.value === '') {
      this.error = 'Driver number is missing!';
      this.isFetching = false;
      return;
    }
    const data = {
      'name': this.inputName.value,
      'realName': this.inputRealName.value,
      'number': this.inputNumber.value,
      'actualTeamId': this.inputActualTeamId.value,
    } as Driver;
    this.seasonService.updateDriver(id, data).subscribe({
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
        this.closeModal();
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
    if (modal !== "deleteDriver" && this.nationalities.length === 0) {
      this.getNationalities();
    }

    this.modal = modal;
    if (selectedDriver) {
      this.selectedDriver = selectedDriver;
      if (modal === 'updateDriver') {
        this.inputName.setValue(selectedDriver?.name);
        this.inputRealName.setValue(selectedDriver?.realName);
        this.inputNumber.setValue(selectedDriver?.number);
        this.inputActualTeamId.setValue(selectedDriver?.actualTeam?.id === undefined ? null : selectedDriver?.actualTeam?.id);
      }
    }
  }

  closeModal() {
    this.modal = '';
    this.error = '';
    this.selectedDriver = undefined;
  }

  openStatistics(name: string) {
    window.location.href = `statistics?name=${name}`;
  }

  actualTeamColor() {
    return this.season.teams.find(x => x.id === this.inputActualTeamId.value)?.color;
  }

  removeError() {
    this.error = '';
  }
}

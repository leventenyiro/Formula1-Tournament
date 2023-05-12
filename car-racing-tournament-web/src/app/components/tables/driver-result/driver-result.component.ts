import { Component, EventEmitter, Input, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormControl } from '@angular/forms';
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
  results: any[] = [];

  inputTeamId = new FormControl('');
  inputRaceId = new FormControl('');
  inputPosition = new FormControl('');
  inputPoint = new FormControl(0);

  constructor(private seasonService: SeasonService) { }

  ngOnInit(): void {
    this.results = this.driverResults!;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['driverId']) {
      this.inputTeamId.setValue(this.getDriverActualTeam() === undefined ? this.season!.teams[0].id : this.getDriverActualTeam());
      this.inputRaceId.setValue(this.season!.races[0].id);
      this.inputPosition.setValue(this.selectedResult === undefined ? 1 : (this.selectedResult.type.toString() === 'Finished' ? this.selectedResult.position : this.selectedResult.type));
    }
  }

  createResult() {
    this.isFetching = true;
    if (this.inputPoint.value === null) {
      this.error = 'Result point is missing!';
      this.isFetching = false;
      return;
    }
    const data = {
      'teamId': this.inputTeamId.value,
      'raceId': this.inputRaceId.value,
      'position': this.inputPosition.value,
      'point': this.inputPoint.value,
    } as Result

    const convertedResult = this.seasonService.resultConverter(data);
    data.type = convertedResult.type;
    data.position = convertedResult.position;
    data.driverId = this.driverId;

    this.seasonService.createResult(data).subscribe({
      next: id => {
        this.closeModal();

        data.id = id.replace(/"/g, '');
        this.results.push(data);

        this.isFetching = false;
      },
      error: error => {
        this.error = error;
        this.isFetching = false;
      }
    });
  }

  updateResult(id: string) {
    this.isFetching = true;
    if (this.inputPoint.value === null) {
      this.error = 'Result point is missing!';
      this.isFetching = false;
      return;
    }
    const data = {
      'teamId': this.inputTeamId.value,
      'raceId': this.inputRaceId.value,
      'position': this.inputPosition.value,
      'point': this.inputPoint.value,
    } as Result

    const convertedResult = this.seasonService.resultConverter(data);
    data.type = convertedResult.type;
    data.position = convertedResult.position;
    data.driverId = this.driverId;  
    
    this.seasonService.updateResult(id, data).subscribe({
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

  openModal(modal: string, selectedResult?: Result) {
    if (modal === 'createResult' && (this.season.teams.length === 0 || this.season.races.length === 0)) {
      this.error = 'There are not available teams or races!'
      return;
    }
    
    this.modal = modal;    
    if (selectedResult) {
      this.selectedResult = selectedResult;
      if (modal === 'updateResult') {
        this.inputTeamId.setValue(this.selectedResult.team.id);
        this.inputRaceId.setValue(this.selectedResult.race.id);
        this.inputPosition.setValue(this.selectedResult.position);
        this.inputPoint.setValue(this.selectedResult.point);
      }
    }
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
    return this.season.drivers.find(x => x.id === this.driverId)?.actualTeamId;
  }

  actualTeamColor() {
    return this.season.teams.find(x => x.id === this.inputTeamId.value)?.color;
  }

  removeError() {
    this.error = '';
  }
}

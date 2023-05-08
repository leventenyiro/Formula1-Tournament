import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Result } from 'app/models/result';
import { Season } from 'app/models/season';
import { SeasonService } from 'app/services/season.service';

@Component({
  selector: 'app-race-result',
  templateUrl: './race-result.component.html',
  styleUrls: ['./race-result.component.scss']
})
export class RaceResultComponent implements OnInit {
  @Input()
  season!: Season;

  @Input()
  raceId!: string;

  @Input()
  raceResults?: any[];

  @Input()
  hasPermission: boolean = false;

  @Output()
  onFetchDataEmitter = new EventEmitter<undefined>();

  isFetching: boolean = false;
  error: string = '';
  modal: string = '';
  selectedResult?: Result;
  inputTeamId = new FormControl('');
  inputDriverId = new FormControl('');

  constructor(private seasonService: SeasonService) { }

  ngOnInit(): void { }

  createResult(data: any) {
    this.isFetching = true;
    if (data.value.point === null) {
      this.error = 'Result point is missing!';
      this.isFetching = false;
      return;
    }
    const convertedResult = this.seasonService.resultConverter(data.value);
    
    data.value.type = convertedResult.type;
    data.value.position = convertedResult.position;

    data.value.raceId = this.raceId;
    data.value.driverId = this.inputDriverId.value;
    data.value.teamId = this.inputTeamId.value;

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
    this.isFetching = true;
    if (data.value.point === null) {
      this.error = 'Result point is missing!';
      this.isFetching = false;
      return;
    }
    const convertedResult = this.seasonService.resultConverter(data.value);
    data.value.type = convertedResult.type;
    data.value.position = convertedResult.position;
    
    data.value.raceId = this.raceId;
    data.value.driverId = this.inputDriverId.value;
    data.value.teamId = this.inputTeamId.value;
    
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
    if (modal === 'createResult' && (this.season.drivers.length === 0 || this.season.teams.length === 0)) {
      this.error = 'There are not available drivers or teams!'
      return;
    }
    
    this.modal = modal;
    this.selectedResult = selectedResult;

    this.inputDriverId.setValue(this.selectedResult === undefined ? 
      this.season!.drivers[0].id : 
      this.selectedResult.driver.id);

    this.setTeamId();
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

  setTeamId() {
    const actualTeamId = this.season.drivers.find(x => x.id === this.inputDriverId.value)?.actualTeamId;
    if (actualTeamId !== undefined)
      this.inputTeamId.setValue(actualTeamId);
    else
      this.inputTeamId.setValue(this.season.teams[0].id);
  }

  actualTeamColor() {
    return this.season.teams.find(x => x.id === this.inputTeamId.value)?.color;
  }

  removeError() {
    this.error = '';
  }
}

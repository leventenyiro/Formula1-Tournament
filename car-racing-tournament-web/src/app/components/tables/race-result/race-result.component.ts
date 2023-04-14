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
  teamId = new FormControl('');
  driverId = new FormControl('');

  constructor(private seasonService: SeasonService) { }

  ngOnInit(): void { }

  createResult(data: any) {
    this.isFetching = true;
    if (data.value.position === 'DNF' || data.value.position === 'DSQ') {
      data.value.type = data.value.position;
      data.value.position = 0;
    } else
      data.value.type = 'Finished';
    data.value.raceId = this.raceId;
    data.value.driverId = this.driverId.value;
    data.value.teamId = this.teamId.value;

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
    if (data.value.position === 'DNF' || data.value.position === 'DSQ') {
      data.value.type = data.value.position;
      data.value.position = 0;
    } else
      data.value.type = 'Finished';
    data.value.raceId = this.raceId;
    data.value.driverId = this.driverId.value;
    data.value.teamId = this.teamId.value;
    
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

    this.driverId.setValue(this.selectedResult === undefined ? 
      this.season!.drivers[0].id : 
      this.selectedResult.driver.id);

    const actualTeamId = this.season.drivers.find(x => x.id === this.driverId.value)?.actualTeam?.id;
    if (actualTeamId !== undefined)
      this.teamId.setValue(actualTeamId);
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
    const actualTeamId = this.season.drivers.find(x => x.id === this.driverId.value)?.actualTeam?.id;
    if (actualTeamId !== undefined)
      this.teamId.setValue(actualTeamId);
  }
}

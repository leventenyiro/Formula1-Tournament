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
  isLoggedIn!: boolean;

  @Input()
  season!: Season;

  @Input()
  raceId!: string;

  @Input()
  raceResults?: any[];

  @Output()
  onFetchDataEmitter = new EventEmitter<undefined>();

  isFetching: boolean = false;
  error: string = '';
  modal: boolean = false;
  selectedResult?: Result;
  teamId = new FormControl('');

  constructor(private seasonService: SeasonService) { }

  ngOnInit(): void {
  }

  createResult(data: any) {
    if (data.value.position === 'DNF' || data.value.position === 'DSQ') {
      data.value.type = data.value.position;
      data.value.position = 0;
    } else
      data.value.type = 'Finished';
    data.value.raceId = this.raceId;
    data.value.teamId = this.teamId.value;

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
    data.value.raceId = this.raceId;
    data.value.teamId = this.teamId.value;
    
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
    this.teamId.setValue(selectedResult === undefined ? this.season!.teams[0].id : selectedResult.team.id);
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

  setTeamId(driverId: string) {    
    const actualTeamId = this.season.drivers.find(x => x.id === driverId)?.actualTeam?.id;
    if (actualTeamId !== undefined)
      this.teamId.setValue(actualTeamId);
  }
}
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Season } from 'app/models/season';
import { Team } from 'app/models/team';
import { SeasonService } from 'app/services/season.service';

@Component({
  selector: 'app-team-all',
  templateUrl: './team-all.component.html',
  styleUrls: ['./team-all.component.scss']
})
export class TeamAllComponent implements OnInit {
  @Input()
  season!: Season;

  @Input()
  teamAll?: any[];

  @Input()
  hasPermission: boolean = false;

  @Output()
  onFetchDataEmitter = new EventEmitter<undefined>();

  error: string = '';
  modal: string = '';
  selectedTeam?: Team;
  isFetching: boolean = false;

  constructor(private seasonService: SeasonService) { }

  ngOnInit(): void {
  }

  createTeam(data: any) {
    this.isFetching = true;
    this.seasonService.createTeam(data.value, this.season?.id!).subscribe({
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

  updateTeam(id: string, data: any) {
    this.isFetching = true;
    this.seasonService.updateTeam(id, data.value).subscribe({
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

  deleteTeam(id: string) {
    this.isFetching = true;
    this.seasonService.deleteTeam(id).subscribe({
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

  openModal(modal: string, selectedTeam?: Team) {
    this.modal = modal;    
    this.selectedTeam = selectedTeam;
  }

  closeModal() {
    this.modal = '';
    this.error = '';
    this.selectedTeam = undefined;
  }
}

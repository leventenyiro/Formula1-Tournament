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
  isLoggedIn!: boolean;

  @Input()
  season!: Season;

  @Input()
  teamAll?: any[];

  @Output()
  onFetchDataEmitter = new EventEmitter<undefined>();

  error: string = '';
  modal: boolean = false;
  selectedTeam?: Team;

  constructor(private seasonService: SeasonService) { }

  ngOnInit(): void {
  }

  createTeam(data: any) {    
    this.seasonService.createTeam(data.value, this.season?.id!).subscribe({
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

  updateTeam(id: string, data: any) {
    this.seasonService.updateTeam(id, data.value).subscribe({
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

  deleteTeam(id: string) {
    this.seasonService.deleteTeam(id).subscribe({
      error: () => this.onFetchDataEmitter.emit(),
      complete: () => this.onFetchDataEmitter.emit()
    });
  }

  openModal(selectedTeam?: Team) {
    this.modal = true;    
    this.selectedTeam = selectedTeam;
  }

  closeModal() {
    this.modal = false;
    this.error = '';
    this.selectedTeam = undefined;
  }
}

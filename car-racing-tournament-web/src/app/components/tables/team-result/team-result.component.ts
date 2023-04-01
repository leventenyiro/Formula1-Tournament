import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-team-result',
  templateUrl: './team-result.component.html',
  styleUrls: ['./team-result.component.scss']
})
export class TeamResultComponent implements OnInit {
  @Input()
  teamResults?: any[];

  constructor() { }

  ngOnInit(): void {
  }

}

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-verify-form',
  templateUrl: './verify-form.component.html',
  styleUrls: ['./verify-form.component.scss']
})
export class VerifyFormComponent implements OnInit {
  @Input()
  title!: string;

  @Input()
  subtext?: string = 'Are you sure?';

  @Input()
  executeButtonText!: string;

  @Output()
  executionEmitter = new EventEmitter<undefined>();

  @Output()
  closeModalEmitter = new EventEmitter<undefined>();

  constructor() { }

  ngOnInit(): void { }
}

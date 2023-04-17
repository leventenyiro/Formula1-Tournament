import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.scss']
})
export class ModalComponent implements OnInit {
  @Input()
  type: string = 'loading';

  @Input()
  text?: string;

  @Output()
  onCloseModalEmitter = new EventEmitter<undefined>();

  constructor() { }

  ngOnInit(): void {
  }
}

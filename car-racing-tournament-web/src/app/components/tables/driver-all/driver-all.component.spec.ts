import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DriverAllComponent } from './driver-all.component';

describe('DriverAllComponent', () => {
  let component: DriverAllComponent;
  let fixture: ComponentFixture<DriverAllComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DriverAllComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DriverAllComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

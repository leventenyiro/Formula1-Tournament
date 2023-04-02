import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RaceAllComponent } from './race-all.component';

describe('RaceAllComponent', () => {
  let component: RaceAllComponent;
  let fixture: ComponentFixture<RaceAllComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RaceAllComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RaceAllComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

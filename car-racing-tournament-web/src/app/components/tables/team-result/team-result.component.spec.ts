import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TeamResultComponent } from './team-result.component';

describe('TeamResultComponent', () => {
  let component: TeamResultComponent;
  let fixture: ComponentFixture<TeamResultComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TeamResultComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TeamResultComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

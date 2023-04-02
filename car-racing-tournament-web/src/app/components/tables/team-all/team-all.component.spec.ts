import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TeamAllComponent } from './team-all.component';

describe('TeamAllComponent', () => {
  let component: TeamAllComponent;
  let fixture: ComponentFixture<TeamAllComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TeamAllComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TeamAllComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

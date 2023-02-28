import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MySeasonsComponent } from './my-seasons.component';

describe('MySeasonsComponent', () => {
  let component: MySeasonsComponent;
  let fixture: ComponentFixture<MySeasonsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MySeasonsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MySeasonsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

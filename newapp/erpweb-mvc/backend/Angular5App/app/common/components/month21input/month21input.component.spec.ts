import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { Month21inputComponent } from './month21input.component';

describe('Month21inputComponent', () => {
  let component: Month21inputComponent;
  let fixture: ComponentFixture<Month21inputComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Month21inputComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Month21inputComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

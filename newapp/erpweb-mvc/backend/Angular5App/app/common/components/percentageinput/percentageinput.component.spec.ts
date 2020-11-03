import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PercentageinputComponent } from './percentageinput.component';

describe('PercentageinputComponent', () => {
  let component: PercentageinputComponent;
  let fixture: ComponentFixture<PercentageinputComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PercentageinputComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PercentageinputComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

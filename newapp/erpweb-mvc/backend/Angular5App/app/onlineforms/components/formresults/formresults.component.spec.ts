import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FormresultsComponent } from './formresults.component';

describe('FormresultsComponent', () => {
  let component: FormresultsComponent;
  let fixture: ComponentFixture<FormresultsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FormresultsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FormresultsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

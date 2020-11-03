import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SalesforecastComponent } from './salesforecast.component';

describe('SalesforecastComponent', () => {
  let component: SalesforecastComponent;
  let fixture: ComponentFixture<SalesforecastComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SalesforecastComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SalesforecastComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FreightcostComponent } from './freightcost.component';

describe('FreightcostComponent', () => {
  let component: FreightcostComponent;
  let fixture: ComponentFixture<FreightcostComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FreightcostComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FreightcostComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PricingprojectComponent } from './pricingproject.component';

describe('PricingprojectComponent', () => {
  let component: PricingprojectComponent;
  let fixture: ComponentFixture<PricingprojectComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PricingprojectComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PricingprojectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

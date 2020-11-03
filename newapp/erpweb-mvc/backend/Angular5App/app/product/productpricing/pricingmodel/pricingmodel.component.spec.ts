import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PricingmodelComponent } from './pricingmodel.component';

describe('PricingmodelComponent', () => {
  let component: PricingmodelComponent;
  let fixture: ComponentFixture<PricingmodelComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PricingmodelComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PricingmodelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

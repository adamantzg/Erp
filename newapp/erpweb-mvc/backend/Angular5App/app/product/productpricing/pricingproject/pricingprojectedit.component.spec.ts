import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PricingprojecteditComponent } from './pricingprojectedit.component';

describe('PricingprojecteditComponent', () => {
  let component: PricingprojecteditComponent;
  let fixture: ComponentFixture<PricingprojecteditComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PricingprojecteditComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PricingprojecteditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

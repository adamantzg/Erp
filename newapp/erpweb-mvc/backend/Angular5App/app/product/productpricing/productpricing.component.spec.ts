import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductpricingComponent } from './productpricing.component';

describe('ProductpricingComponent', () => {
  let component: ProductpricingComponent;
  let fixture: ComponentFixture<ProductpricingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductpricingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductpricingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

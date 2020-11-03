import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductselectorComponent } from './productselector.component';

describe('ProductselectorComponent', () => {
  let component: ProductselectorComponent;
  let fixture: ComponentFixture<ProductselectorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductselectorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductselectorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

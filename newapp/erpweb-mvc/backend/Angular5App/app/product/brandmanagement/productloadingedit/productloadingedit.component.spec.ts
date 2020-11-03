import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductloadingeditComponent } from './productloadingedit.component';

describe('ProductloadingeditComponent', () => {
  let component: ProductloadingeditComponent;
  let fixture: ComponentFixture<ProductloadingeditComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductloadingeditComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductloadingeditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

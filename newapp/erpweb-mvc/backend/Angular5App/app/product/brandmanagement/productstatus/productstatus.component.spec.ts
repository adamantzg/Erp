import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductstatusComponent } from './productstatus.component';

describe('ProductstatusComponent', () => {
  let component: ProductstatusComponent;
  let fixture: ComponentFixture<ProductstatusComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductstatusComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductstatusComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

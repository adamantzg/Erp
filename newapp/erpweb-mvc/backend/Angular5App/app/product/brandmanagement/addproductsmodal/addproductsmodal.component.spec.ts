import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddproductsmodalComponent } from './addproductsmodal.component';

describe('AddproductsmodalComponent', () => {
  let component: AddproductsmodalComponent;
  let fixture: ComponentFixture<AddproductsmodalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddproductsmodalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddproductsmodalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

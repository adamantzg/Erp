import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderextradataComponent } from './orderextradata.component';

describe('OrderextradataComponent', () => {
  let component: OrderextradataComponent;
  let fixture: ComponentFixture<OrderextradataComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrderextradataComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrderextradataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

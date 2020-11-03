import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FactoryStockNewOrderModalComponent } from './factorystocknewordermodal.component';

describe('FactorystocknewordermodalComponent', () => {
  let component: FactoryStockNewOrderModalComponent;
  let fixture: ComponentFixture<FactoryStockNewOrderModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FactoryStockNewOrderModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FactoryStockNewOrderModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

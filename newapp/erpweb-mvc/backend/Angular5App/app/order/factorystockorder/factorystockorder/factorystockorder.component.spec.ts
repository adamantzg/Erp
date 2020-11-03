import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FactorystockorderComponent } from './factorystockorder.component';

describe('FactorystockorderComponent', () => {
  let component: FactorystockorderComponent;
  let fixture: ComponentFixture<FactorystockorderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FactorystockorderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FactorystockorderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

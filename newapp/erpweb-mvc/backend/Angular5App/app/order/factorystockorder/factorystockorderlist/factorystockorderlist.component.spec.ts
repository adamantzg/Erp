import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FactorystockorderlistComponent } from './factorystockorderlist.component';

describe('FactorystockorderlistComponent', () => {
  let component: FactorystockorderlistComponent;
  let fixture: ComponentFixture<FactorystockorderlistComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FactorystockorderlistComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FactorystockorderlistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

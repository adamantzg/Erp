import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FactorystockordereditComponent } from './factorystockorderedit.component';

describe('FactorystockordereditComponent', () => {
  let component: FactorystockordereditComponent;
  let fixture: ComponentFixture<FactorystockordereditComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FactorystockordereditComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FactorystockordereditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

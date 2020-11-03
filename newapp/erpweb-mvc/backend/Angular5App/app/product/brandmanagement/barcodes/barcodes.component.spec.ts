import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BarcodesComponent } from './barcodes.component';

describe('BarcodesComponent', () => {
  let component: BarcodesComponent;
  let fixture: ComponentFixture<BarcodesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BarcodesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BarcodesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

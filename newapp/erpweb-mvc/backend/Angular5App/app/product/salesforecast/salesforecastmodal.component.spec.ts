import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SalesforecastmodalComponent } from './salesforecastmodal.component';

describe('SalesforecastmodalComponent', () => {
  let component: SalesforecastmodalComponent;
  let fixture: ComponentFixture<SalesforecastmodalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SalesforecastmodalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SalesforecastmodalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

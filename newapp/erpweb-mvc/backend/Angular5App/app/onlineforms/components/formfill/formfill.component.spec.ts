import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FormFillComponent } from './formfill.component';

describe('FormfillComponent', () => {
  let component: FormFillComponent;
  let fixture: ComponentFixture<FormFillComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FormFillComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FormFillComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

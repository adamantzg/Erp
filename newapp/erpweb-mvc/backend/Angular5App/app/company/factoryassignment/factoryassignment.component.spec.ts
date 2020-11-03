import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FactoryassignmentComponent } from './factoryassignment.component';

describe('FactoryassignmentComponent', () => {
  let component: FactoryassignmentComponent;
  let fixture: ComponentFixture<FactoryassignmentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FactoryassignmentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FactoryassignmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

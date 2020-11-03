import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductbulkeditComponent } from './productbulkedit.component';

describe('ProductbulkeditComponent', () => {
  let component: ProductbulkeditComponent;
  let fixture: ComponentFixture<ProductbulkeditComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductbulkeditComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductbulkeditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

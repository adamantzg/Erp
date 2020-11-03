import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductPackagingExportComponent } from './productpackagingexport.component';

describe('ProductPackagingExportComponent', () => {
  let component: ProductPackagingExportComponent;
  let fixture: ComponentFixture<ProductPackagingExportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductPackagingExportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductPackagingExportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

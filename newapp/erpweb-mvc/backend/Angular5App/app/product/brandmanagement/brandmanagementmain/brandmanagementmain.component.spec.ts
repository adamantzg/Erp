import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BrandmanagementmainComponent } from './brandmanagementmain.component';

describe('BrandmanagementmainComponent', () => {
  let component: BrandmanagementmainComponent;
  let fixture: ComponentFixture<BrandmanagementmainComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BrandmanagementmainComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BrandmanagementmainComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

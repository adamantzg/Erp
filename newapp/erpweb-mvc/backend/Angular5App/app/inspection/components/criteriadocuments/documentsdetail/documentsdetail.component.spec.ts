import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentsdetailComponent } from './documentsdetail.component';

describe('DocumentsdetailComponent', () => {
  let component: DocumentsdetailComponent;
  let fixture: ComponentFixture<DocumentsdetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DocumentsdetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DocumentsdetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

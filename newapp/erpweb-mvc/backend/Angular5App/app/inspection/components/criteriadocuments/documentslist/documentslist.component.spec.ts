import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentslistComponent } from './documentslist.component';

describe('DocumentslistComponent', () => {
  let component: DocumentslistComponent;
  let fixture: ComponentFixture<DocumentslistComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DocumentslistComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DocumentslistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

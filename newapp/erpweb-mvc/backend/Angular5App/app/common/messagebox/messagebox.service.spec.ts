import { TestBed, inject } from '@angular/core/testing';

import { MessageboxService } from './messagebox.service';
import { BsModalService, ModalModule, ComponentLoaderFactory, PositioningService } from 'ngx-bootstrap';

describe('MessageboxService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [MessageboxService, BsModalService, ComponentLoaderFactory, PositioningService],
      imports: [ModalModule]
    });
  });

  it('should be created', inject([MessageboxService], (service: MessageboxService) => {
    expect(service).toBeTruthy();
  }));
});

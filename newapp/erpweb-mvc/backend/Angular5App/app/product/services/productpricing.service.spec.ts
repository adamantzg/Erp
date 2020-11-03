import { TestBed, inject } from '@angular/core/testing';

import { ProductpricingService } from './productpricing.service';

describe('ProductpricingService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ProductpricingService]
    });
  });

  it('should be created', inject([ProductpricingService], (service: ProductpricingService) => {
    expect(service).toBeTruthy();
  }));
});

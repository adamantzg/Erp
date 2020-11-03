import { TestBed } from '@angular/core/testing';

import { ProductPackagingExportService } from './productpackagingexport.service';

describe('ProductpackagingexportService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ProductPackagingExportService = TestBed.get(ProductPackagingExportService);
    expect(service).toBeTruthy();
  });
});

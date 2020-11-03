import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';


import { HttpService } from '../../common';
import { Settings } from '../settings';
import { bloomAdd } from '@angular/core/src/render3/di';

@Injectable({
  providedIn: 'root'
})
export class ProductPackagingExportService {

  constructor(private httpService: HttpService) { }

  getByCriteria(clientId, factoryId, categoryId, etaetd, dateFrom, dateTo) {
    return this.httpService.get('/Product/ProductPackagingMaterialExport',
                  { params: { clientId: clientId, factoryId: factoryId, categoryId: categoryId, etaetd: etaetd, dateFrom: dateFrom, dateTo: dateTo }
    , responseType: 'blob' as 'blob'});
  }

}

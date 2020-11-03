import { Injectable } from '@angular/core';
import { HttpService } from '../../common';
import { Settings } from '../settings';
import { MastProductsPackagingMaterial } from '../domainclasses';
import { HttpParams, HttpParameterCodec } from '@angular/common/http';

@Injectable()
export class ProductPackagingService {

  constructor(private httpService: HttpService) { }

  api = Settings.apiRoot + 'mastproduct/';

  getModel(allClients: boolean) {
      return this.httpService.get(this.api + 'getPackagingModel',
          {
             params: { allClients: allClients }
          }
      );
  }

  getByCriteria(factoryId: number, categoryId: number, clientId: number, productCode: string, pageNo: number, pageSize: number) {
    return this.httpService.get(this.api + 'getByCriteriaPaged',
    {
        params: this.getParamsForCriteria(factoryId, categoryId, clientId, productCode, pageNo, pageSize)
    });
  }

  getTotalForCriteria(factoryId: number, categoryId: number, clientId: number, productCode: string) {
      return this.httpService.get(this.api + 'getTotalForCriteria',
      {
          params: this.getParamsForCriteria(factoryId, categoryId, clientId, productCode)
      });
  }

  getParamsForCriteria(factoryId: number, categoryId: number, clientId: number, productCode: string, pageNo?: number, pageSize?: number) {
    const params = {
        factoryId: factoryId,
        categoryId: categoryId,
        clientId: clientId
    };
    if (productCode != null && productCode.length > 0) {
        params['productCode'] = encodeURIComponent(productCode);
    }
    if (pageNo != null) {
        params['pageNo'] = pageNo;
    }
    if (pageSize != null) {
        params['pageSize'] = pageSize;
    }
    return params;
  }

  updatePackaging(data: MastProductsPackagingMaterial) {
    return this.httpService.postNoBlock(this.api + 'updateProductPackaging', data);
  }

  deletePackaging(id: number) {
    return this.httpService.delete(this.api + 'deletePackaging?id=' + id);
  }
}

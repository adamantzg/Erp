import { Injectable } from '@angular/core';
import { HttpService, Currency, CurrencyCode, ContainerTypeValue } from '../../common';
import { Settings } from '../settings';

@Injectable()
export class BrandService {

  constructor(private httpService: HttpService) { }

  api = Settings.apiRoot + 'brands/';

  getAll() {
      return this.httpService.get(this.api);
  }
}

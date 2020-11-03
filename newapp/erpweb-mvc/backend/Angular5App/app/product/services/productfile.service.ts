import { Injectable } from '@angular/core';
import { HttpService } from '../../common';
import { ProductFile } from '../domainclasses';
import { Settings } from '../settings';


@Injectable()
export class ProductFileService {

  constructor(private httpService: HttpService) { }

  api = Settings.apiRoot + 'productfile/';

  create(file: ProductFile) {
      return this.httpService.post(this.api, file);
  }

  delete(id: number) {
      return this.httpService.delete(this.api + '?id=' + id, {});
  }

  getTypes() {
      return this.httpService.get(this.api + 'types');
  }

  update(file: ProductFile) {
      return this.httpService.putNoBlock(this.api, file);
  }
}

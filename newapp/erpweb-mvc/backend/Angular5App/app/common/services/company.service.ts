import { Injectable } from '@angular/core';
import { HttpService } from './http.service';
import { Observable } from 'rxjs/Observable';
import { Company } from '../domainclasses';
import { Settings } from '../settings';

@Injectable()
export class CompanyService {

  constructor(private httpService: HttpService) { }
  api = Settings.apiRoot + 'company/';

  getFactories(blockUI: boolean = false): Observable<Company[]> {
    if (blockUI) {
      return this.httpService.get(this.api + 'getFactories', null);
    }
    return this.httpService.getNoBlock(this.api + 'getFactories', null);
  }

  getClients(blockUI: boolean = false): Observable<Company[]> {
    if (blockUI) {
      return this.httpService.get(this.api + 'getClients', null);
    }
    return this.httpService.getNoBlock(this.api + 'getClients', null);
  }

  updateBulk(data: Company[]) {
    return this.httpService.put(this.api + 'updateBulk', data);
  }

    removeFile(companyId: number, fileId: number) {
        return this.httpService.delete(this.api + `removeFile?companyId=${companyId}&fileId=${fileId}`);
    }

    getByIds(ids) {
        return this.httpService.get(this.api + 'getByIds', {params: {ids: ids}});
    }

    getFactoriesForUser() {
        return this.httpService.get(this.api + 'getFactoriesForUser');
    }

    getFactoriesForClient(clientId: number) {
        return this.httpService.get(this.api + 'getFactoriesForClient', { params: { clientId: clientId}});
    }
  

}

import { Injectable } from '@angular/core';
import { HttpService } from '../../common';
import { Settings } from '../settings';
import { CustProduct } from '../domainclasses';

@Injectable()
export class TariffService {

    constructor(private httpService: HttpService) { }

    api = Settings.apiRoot + 'tariff/';

    getAllTariffs() {
        return this.httpService.get(this.api + 'getAllTariffs');
    }

    
}

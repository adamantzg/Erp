import {Injectable} from '@angular/core';
import { HttpService } from '../../common/index';
import { Settings } from '../../common/settings';
import { Order } from '../domainclasses';
import * as moment from 'moment';


@Injectable()
export class OrderService {

    constructor(
        private httpService: HttpService
    ) {

    }

    api = Settings.apiRoot + 'order/';

    getOrderExtraData(page: number, pageSize: number, includeAlreadySet: boolean, custpo: string, brand: boolean, isUK: boolean, monthsInPast?: number) {
        return this.httpService.get(this.api + 'getForExtraDataEntry', {
            params: {
                page: page,
                pageSize: pageSize,
                includeAlreadySet: includeAlreadySet,
                monthsInPast: monthsInPast,
                custpo: custpo,
                brand: brand,
                isUK: isUK
            }
        });
    }

    updateSaleData(o: Order) {
        o.sale_date = moment(o.sale_date).add(moment().utcOffset(), 'minutes').toDate();
        return this.httpService.postNoBlock(this.api + 'updateSaleData', o);
    }

}

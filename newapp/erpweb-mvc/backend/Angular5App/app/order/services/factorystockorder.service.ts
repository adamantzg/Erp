import {Injectable} from '@angular/core';
import { HttpService } from '../../common/index';
import { Settings } from '../../common/settings';
import { Order, FactoryStockOrder, FactoryStockOrderLine } from '../domainclasses';
import * as moment from 'moment';


@Injectable()
export class FactoryStockOrderService {

    constructor(private httpService: HttpService) {

    }

    api = Settings.apiRoot + 'factorystockorder/';

    getListModel() {
        return this.httpService.get(this.api);
    }

    getOrders(factoryId: number) {
        return this.httpService.get(this.api + 'getOrders', { params: { factoryId: factoryId}});
    }

    getEditModel(id: number) {
        return this.httpService.get(this.api + 'getEditModel', {params: {id: id}});
    }

    update(o: FactoryStockOrder) {
        return this.httpService.post(this.api, o);
    }

    delete(id: number) {
        return this.httpService.delete(this.api + '?id=' + id.toString());
    }

    moveLines(lines: FactoryStockOrderLine[], target: FactoryStockOrder) {
        return this.httpService.post(this.api + 'moveLines', { lines: lines, order: target} );
    }
}

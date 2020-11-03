import { FactoryStockOrder } from './domainclasses';
import { Company, Currency } from '../common';

export class FactoryStockOrderEditModel {
    order: FactoryStockOrder = new FactoryStockOrder();
    clients: Company[] = [];
    currencies: Currency[] = [];
    factories: Company[] = [];
    orders: FactoryStockOrder[] = [];
    clientId = null;
    currencyId = null;
    selectAllLines = false;
    orderId = null;
}

export class FactoryStockOrderListModel {
    factories: Company[];
    currencies: Currency[];
    factoryId: number;

}

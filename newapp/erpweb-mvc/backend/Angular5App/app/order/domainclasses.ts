import { Company, User } from '../common';
import { MastProduct, CustProduct } from '../product/domainclasses';

export class Order {
    orderid: number;
    req_eta: Date | null;
    custpo: string;
    client: Client;
    bdi_vat: number | null;
    freight_value: number | null;
    bdi_invoice: number | null;
    bdi_import_fees: number | null;
    sale_date: Date | null;
    freight_invoice_no: string;
    bdi_import_fees_invoice_no: string;
}

export class Client {
    user_id: number;
    customer_code: string;
}

export class FactoryStockOrder {
    id: number;
    factory_id: number | null;
    po_ref: string;
    etd: Date | string | null;
    datecreated: Date | string | null;
    creator_id: number | null;
    po_customer_ref: string;

    lines: FactoryStockOrderLine[] = [];
    factory: Company;
    creator: User;

    balance: number | null;
    balanceValue: number | null;
    currency: number | null;
    currencyText: string;
}

export class FactoryStockOrderLine {
    id: number;
    orderid: number | null;
    linedate: Date | string | null;
    mast_id: number | null;
    qty: number | null;
    price: number | null;
    currency: number | null;
    cprod_code1: string;

    mastProduct: MastProduct;
    product: CustProduct;

    selected: boolean;

}

import { Component, OnInit } from '@angular/core';
import { FactoryStockOrderService } from '../../services/factorystockorder.service';
import { CommonService, CompanyService, CurrencyCode } from '../../../common';
import { ActivatedRoute, Router } from '@angular/router';
import { FactoryStockOrderEditModel } from '../../models';
import { FactoryStockOrder, FactoryStockOrderLine } from '../../domainclasses';
import { CustProductsExtradata, MastProduct } from '../../../product/domainclasses';
import { Observable } from 'rxjs';
import { ProductService } from '../../../product/services/product.service';
import { mergeMap } from 'rxjs/operators';
import { TypeaheadMatch, BsModalService } from 'ngx-bootstrap';
import { FactoryStockNewOrderModalComponent } from '../factorystocknewordermodal/factorystocknewordermodal.component';

@Component({
  selector: 'app-factorystockorderedit',
  templateUrl: './factorystockorderedit.component.html',
  styleUrls: ['./factorystockorderedit.component.css']
})
export class FactoryStockOrderEditComponent implements OnInit {

  constructor(private factoryStockOrderService: FactoryStockOrderService,
    private commonService: CommonService, private activatedRoute: ActivatedRoute,
    private companyService: CompanyService, private productService: ProductService,
    private router: Router, private bsModalService: BsModalService) {

    }

    model: FactoryStockOrderEditModel = new FactoryStockOrderEditModel();
    errorMessage = '';
    prodObservables = {};


    newLineId = -1;

  ngOnInit() {
    const id = +this.activatedRoute.snapshot.params.id;
    this.factoryStockOrderService.getEditModel(id).subscribe(
        data =>  {
            if (data.order.etd != null) {
                data.order.etd = new Date(data.order.etd);
            }
            this.model = data;
            if (data.order.lines.length > 0) {
                this.model.currencyId = data.order.lines[0].currency;
            }
        },
        err => this.errorMessage = this.commonService.getError(err)
    );

    this.commonService.setBreadCrumb('Factory stock order');
  }

    deleteLine(l: FactoryStockOrderLine) {

    }

    addNewLine() {
        const line = new FactoryStockOrderLine();
        line.mastProduct = new MastProduct();
        line.currency = this.model.currencyId;
        line.orderid = this.model.order.id;
        line.id = this.newLineId--;
        line.linedate = new Date();
        line.qty = 0;
        line.price = 0;
        this.model.order.lines.push(line);
    }

    getTotal() {
        let sum = 0;
        for (let ix = 0; ix < this.model.order.lines.length; ix++) {
            const line = this.model.order.lines[ix];
            if (line.qty == null) {
                line.qty = 0;
            }
            if (line.price == null) {
                line.price = 0;
            }
            sum += line.qty * line.price;
        }
        return sum;
    }

    backToList() {
        this.router.navigate(['/factorystockorder']);
    }

    update() {
        const o = this.commonService.deepClone(this.model.order);
        for (let ix = 0; ix < o.lines.length; ix++) {
            const line = o.lines[ix];
            line.mast_id = line.mastProduct.mast_id;
            line.mastProduct = null;
        }
        this.factoryStockOrderService.update(o).subscribe(
            (data: FactoryStockOrder) => {
                this.model.order.id = data.id;
                for (let ix = 0; ix < this.model.order.lines.length; ix++) {
                    const line = this.model.order.lines[ix];
                    line.orderid = data.id;
                    line.id = data.lines[ix].id;
                }
            },
            err => this.errorMessage = this.commonService.getError(err)
        );
    }

    clientChanged() {
        this.loadFactories();
    }

    loadFactories() {
        this.companyService.getFactoriesForClient(this.model.clientId).subscribe(
            data => this.model.factories = data,
            err => this.errorMessage = this.commonService.getError(err)
        );
    }

    currencyChanged() {
        for (let ix = 0; ix < this.model.order.lines.length; ix++) {
            const line = this.model.order.lines[ix];
            line.currency = parseInt(this.model.currencyId, 10);
        }
    }

    custProducts(l: FactoryStockOrderLine) {
        if (!(l.id in this.prodObservables)) {
            this.prodObservables[l.id] = Observable.create((observer: any) => {
                // Runs on every search
                observer.next(l.cprod_code1);
              }).pipe(mergeMap((code: string) => this.productService.searchProductsCached(this.model.order.factory_id,
                null, null, null, null, null, code)));
        }
        return this.prodObservables[l.id];
    }

    onProductSelected(l: FactoryStockOrderLine, event: TypeaheadMatch) {
        l.cprod_code1 = event.item.cprod_code1;
        l.mastProduct = event.item.mastProduct;
        let currencyId = this.model.currencyId;
        if (this.model.currencyId == null) {
            currencyId = CurrencyCode.USD;
        }
        l.price = currencyId == CurrencyCode.USD ? l.mastProduct.price_dollar :
            currencyId == CurrencyCode.GBP ? l.mastProduct.price_pound :
            l.mastProduct.price_euro;
    }

    selectAllToggle() {
        this.model.order.lines.forEach(l => l.selected = this.model.selectAllLines);
    }

    getSelectedLines() {
        return this.model.order.lines.filter(l => l.selected);
    }

    getOrderDescription(o: FactoryStockOrder) {
        return o.po_customer_ref + ' (' + o.po_ref + ')';
    }

    moveLines() {
        const selected = this.getSelectedLines();
        let order = new FactoryStockOrder();
        order.id = this.model.orderId;
        const ids = selected.map(l => l.id);
        order.factory_id = this.model.order.factory_id;
        if (!order.id) {
            const modal = this.bsModalService.show(FactoryStockNewOrderModalComponent);
            const newOrderModel = new FactoryStockOrderEditModel();
            newOrderModel.factories = this.model.factories;
            newOrderModel.currencies = this.model.currencies;
            newOrderModel.order = order;
            modal.content.model = newOrderModel;
            modal.content.onOk.subscribe(o => {
                order = o;
                this.factoryStockOrderService.moveLines(selected, order).subscribe(
                    () => {
                        this.model.order.lines = this.model.order.lines.filter(l => ids.indexOf(l.id) < 0);
                    },
                    err => this.errorMessage = this.commonService.getError(err)
                );
            });
        } else {
            this.factoryStockOrderService.moveLines(selected, order).subscribe(
                () => {
                    this.model.order.lines = this.model.order.lines.filter(l => ids.indexOf(l.id) < 0);
                },
                err => this.errorMessage = this.commonService.getError(err)
            );
        }
        
    }

}

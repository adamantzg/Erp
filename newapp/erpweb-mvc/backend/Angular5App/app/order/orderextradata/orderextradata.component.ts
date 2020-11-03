import { Component, OnInit } from '@angular/core';
import { OrderService } from '../services/order.service';
import { CommonService } from '../../common/index';
import { Order } from '../domainclasses';
import * as moment from 'moment';
import { BsLocaleService, enGbLocale } from 'ngx-bootstrap';
import { defineLocale } from 'ngx-bootstrap/chronos';


@Component({
  selector: 'app-orderextradata',
  templateUrl: './orderextradata.component.html',
  styleUrls: ['./orderextradata.component.css']
})
export class OrderextradataComponent implements OnInit {

  constructor(
    private orderService: OrderService,
    private commonService: CommonService,
    private bsLocaleService: BsLocaleService
    ) {
      defineLocale('en-gb', enGbLocale);
      this.bsLocaleService.use('en-gb');
     }
  page = 1;
  lastPage = 1;
  pageSize = 50;
  isUk = true;
  brand = true;
  totalCount = 0;
  includeAlreadySet = false;
  custpo = '';
  errorMessage = '';
  orders: Order[] = [];
  validation = {};
  dateFormat = 'dd/MM/yyyy';
  activeOrder: Order = null;
  orderChanged = {};
  orderUpdated = {};
  numFormat = '1.2-2';
  updateTimer = null;
  loading = false;

  ngOnInit() {
    this.getData();
  }

  getData() {
    this.loading = true;
    this.orderService.getOrderExtraData(this.page - 1, this.pageSize, this.includeAlreadySet, this.custpo, this.brand, this.isUk).subscribe(data => {
      data.data.forEach(o => {
        o.req_eta = new Date(o.req_eta);
        if (o.sale_date != null) {
          o.sale_date = new Date(o.sale_date);
        }
      });
      this.orders = data.data;
      this.totalCount = data.totalCount;
      this.loading = false;
    },
    err => this.errorMessage = this.commonService.getError(err));
  }

  pageChanged(e: any) {
    this.page = e.page;
    if (this.page !== this.lastPage) {
      this.getData();
      this.lastPage = this.page;
    }

  }

  criteriaChanged() {
    this.getData();
  }

  checkDate(d: Date) {
    // this.validation = true;
    return d != null && moment(d).isValid();
  }

  getDateControlClass(o: Order) {
    let result = 'form-control';
    if (this.validation[o.orderid] && !this.checkDate(o.sale_date)) {
      result += ' has-error';
    }
    return result;
  }

  dateChanged(o: Order, $event: any) {
    if ($event != null && !this.loading && o.sale_date !== $event) {
      this.orderChanged[o.orderid] = true;
      this.orderUpdated[o.orderid] = false;
      this.validation[o.orderid] = true;
    }

  }

  enterControl(o: Order) {
    if (this.activeOrder == null || o.orderid !== this.activeOrder.orderid) {
      this.enterRow(o);
    }
  }

  enterRow(o: Order) {
    this.updateOrder(this.activeOrder);
    this.activeOrder = o;
    // this.orderChanged[o.orderid] = false;
  }

  update(o: Order) {
    this.orderService.updateSaleData(o).subscribe(data => {
      this.orderChanged[o.orderid] = false;
      this.orderUpdated[o.orderid] = true;
    },
    err => this.errorMessage = this.commonService.getError(err) );
  }

  valueChanged(o: Order) {
    clearTimeout(this.updateTimer);
    this.orderChanged[o.orderid] = true;
    this.orderUpdated[o.orderid] = false;
  }

  leaveControl(o: Order, property: string, $event: any) {
    if (o != null && property != null) {
      o[property] = $event.target.value;
    }
    this.updateTimer = setTimeout(() => { this.updateOrder(o); }, 2000);
  }

  updateOrder(o: Order) {
    if (o != null) {
      if (this.orderChanged[o.orderid]) {
        if (this.checkDate(o.sale_date)) {
          this.update(o);
        } else {
          this.validation[o.orderid] = true;
        }

      }
    }
  }

}

import { Component, OnInit } from '@angular/core';
import { FactoryStockOrderService } from '../../services/factorystockorder.service';
import { FactoryStockOrderListModel } from '../../models';
import { CommonService } from '../../../common';
import { FactoryStockOrder } from '../../domainclasses';
import { Router } from '@angular/router';
import { MessageboxService } from '../../../common/messagebox/messagebox.service';
import { MessageBoxType, MessageBoxCommand, MessageBoxCommandValue } from '../../../common/ModalDialog';

@Component({
  selector: 'app-factorystockorderlist',
  templateUrl: './factorystockorderlist.component.html',
  styleUrls: ['./factorystockorderlist.component.css']
})
export class FactoryStockOrderListComponent implements OnInit {

  constructor(private factoryStockOrderService: FactoryStockOrderService,
    private commonService: CommonService, private router: Router,
    private messageBoxService: MessageboxService) { }

  model: FactoryStockOrderListModel = new FactoryStockOrderListModel();
  errorMessage = '';
  orders: FactoryStockOrder[] = [];
  dateFormat = 'dd/MM/yyyy';

  ngOnInit() {
      this.factoryStockOrderService.getListModel().subscribe(
        (data) => {
            this.model = data;
        },
        err => this.errorMessage = this.commonService.getError(err)
      );

      this.commonService.setBreadCrumb('Factory stock order');
  }

  create() {
    this.router.navigate(['factorystockorder/edit/0']);
  }

  factoryChanged() {
    this.factoryStockOrderService.getOrders(this.model.factoryId).subscribe(
        data => {
            for (let d of data) {
                const curr = this.model.currencies.find(c => c.curr_code == d.currency);
                if (curr != null) {
                    d.currencyText = curr.curr_symbol;
                }
            }
            this.orders = data;
        },
        err => this.errorMessage = this.commonService.getError(err)
    );
  }

  deleteOrder(ix: number) {
    this.messageBoxService.openDialog('Delete order?', MessageBoxType.Yesno, 'Delete').subscribe(
        (res: MessageBoxCommand) => {
            if (res.value == MessageBoxCommandValue.Ok) {
                const order = this.orders[ix];
                this.factoryStockOrderService.delete(order.id).subscribe(
                    () => {
                        this.orders.splice(ix, 1);
                    },
                    err => this.errorMessage = this.commonService.getError(err)
                )
            }
        }
    )
  }

}

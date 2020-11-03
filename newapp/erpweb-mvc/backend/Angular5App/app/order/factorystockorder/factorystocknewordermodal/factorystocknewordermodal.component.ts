import { Component, OnInit, EventEmitter } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap';
import { CommonService } from '../../../common';
import { FactoryStockOrder } from '../../domainclasses';
import { FactoryStockOrderEditModel } from '../../models';


@Component({
  selector: 'app-factorystocknewordermodal',
  templateUrl: './factorystocknewordermodal.component.html',
  styleUrls: ['./factorystocknewordermodal.component.css']
})
export class FactoryStockNewOrderModalComponent implements OnInit {

    constructor(private bsRefModal: BsModalRef, private commonService: CommonService) { }

    public onOk: EventEmitter<any> = new EventEmitter();
    public onCancel: EventEmitter<any> = new EventEmitter();
    errorMessage = '';
    model = new FactoryStockOrderEditModel();

  ngOnInit() {
  }

  ok() {
    this.onOk.emit(this.model.order);
    this.bsRefModal.hide();
  }

  cancel() {
    this.onCancel.emit();
    this.bsRefModal.hide();
  }

}

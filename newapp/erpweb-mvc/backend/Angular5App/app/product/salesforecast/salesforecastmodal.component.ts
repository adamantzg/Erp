import { Component, OnInit, EventEmitter } from '@angular/core';
import { SalesForecast } from '../domainclasses';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CommonService, Month21 } from '../../common';


@Component({
  selector: 'app-salesforecastmodal',
  templateUrl: './salesforecastmodal.component.html',
  styleUrls: ['./salesforecastmodal.component.css']
})
export class SalesforecastmodalComponent implements OnInit {

  constructor(private bsRefModal: BsModalRef, private commonService: CommonService) { }

  public onOk: EventEmitter<any> = new EventEmitter();
  public onCancel: EventEmitter<any> = new EventEmitter();

  title = '';
  errorMessage = '';
  data: SalesForecast[] = [];
  cprod_id: number;
  allowMonthChange: boolean;

  public set Forecasts(value: SalesForecast[]) {
    this.data = this.commonService.deepClone(value);
    const start = Month21.Add(Month21.Now, 1);
    for (let i = 0; i < 12; i++) {
      const month21 = Month21.Add(start, i).Value;
      let forecast = this.data.find(f => f.month21 === month21);
      if (forecast == null) {
        forecast = new SalesForecast();
        forecast.cprod_id = this.cprod_id;
        forecast.month21 = month21;
        this.data.push(forecast);
      }
    }
  }

  ngOnInit() {
  }

  ok() {
    this.onOk.emit(this.data);
    this.bsRefModal.hide();
  }

  cancel() {
    this.onCancel.emit();
    this.bsRefModal.hide();
  }

}

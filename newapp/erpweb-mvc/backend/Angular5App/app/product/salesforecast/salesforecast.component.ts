import { Component, OnInit, Input } from '@angular/core';
import { SalesForecast } from '../domainclasses';


@Component({
  selector: 'app-salesforecast',
  templateUrl: './salesforecast.component.html',
  styleUrls: ['./salesforecast.component.css']
})
export class SalesforecastComponent implements OnInit {

  constructor() { }

  @Input()
  allowRemove = false;
  @Input()
  allowAdd = false;
  same = true;
  value: number;
  @Input()
  allowMonthChange = false;

  data: SalesForecast[] = [];

  @Input()
  public get forecasts(): SalesForecast[] {
    return this.data;
  }

  public set forecasts(value: SalesForecast[]) {
    this.data = value;
    let allSame = true;
    let first: number;
    if (value.length > 0) {
      first = +value[0].sales_qty;
      for (let i = 1; i < value.length; i++) {
        if (+value[i].sales_qty !== first) {
          allSame = false;
          break;
        }
      }
    }
    if (allSame) {
      this.value = first;
    }

  }

  ngOnInit() {
  }

  sameValueChanged() {
    if (this.same) {
      this.data.forEach(f => f.sales_qty = +this.value);
    }
  }


}

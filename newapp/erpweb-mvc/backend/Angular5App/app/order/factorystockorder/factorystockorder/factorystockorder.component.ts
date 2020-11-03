import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FactoryStockOrderEditModel } from '../../models';


@Component({
  selector: 'app-factorystockorder',
  templateUrl: './factorystockorder.component.html',
  styleUrls: ['./factorystockorder.component.css']
})
export class FactorystockorderComponent implements OnInit {

  constructor() { }

  @Input()
  model: FactoryStockOrderEditModel = new FactoryStockOrderEditModel();

  @Output()
  currencyChanged = new EventEmitter();


  ngOnInit() {
  }

  onCurrencyChanged() {
      this.currencyChanged.emit();
  }

}

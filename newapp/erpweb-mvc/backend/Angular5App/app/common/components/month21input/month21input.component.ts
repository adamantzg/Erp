import { Component, OnInit, OnChanges, Input, Output, EventEmitter } from '@angular/core';
import * as moment from 'moment';
import { Month21 } from '../month21';


@Component({
  selector: 'app-month21input',
  templateUrl: './month21input.component.html',
  styleUrls: ['./month21input.component.css']
})
export class Month21inputComponent implements OnInit {

  constructor() { }

  @Output() Month21Change = new EventEmitter<number>();

  data: number;
  protected month21: number;
  month: number;
  months: number[] = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
  year: number;
  @Input()
  disabled = false;

  ngOnInit() {

  }

  @Input()
  public get Month21() {
    return this.month21;
  }



  public set Month21(value: number) {
    const m21 = new Month21(null, value);
    this.year = m21.Date.getFullYear();
    this.month = m21.Date.getMonth() + 1;
  }

  change() {
    this.month21 = Month21.FromYearMonth(this.year, this.month).Value;
    this.Month21Change.emit(this.month21);
  }

}

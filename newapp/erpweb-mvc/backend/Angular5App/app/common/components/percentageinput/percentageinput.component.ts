import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-percentageinput',
  templateUrl: './percentageinput.component.html',
  styleUrls: ['./percentageinput.component.css']
})
export class PercentageinputComponent implements OnInit {

  constructor() { }

  private data: number;
  displayedData: number;
  @Output() DataChange = new EventEmitter<number>();

  @Input()
  public get Data() {
    return this.data;
  }

  public set Data(value: number) {
    this.data = value;
    if (!isNaN(value)) {
      this.displayedData = value * 100;
    }
  }

  ngOnInit() {
  }

  changed() {
    this.data = this.displayedData / 100;
    this.DataChange.emit(this.data);
  }

}

import { Component, OnInit, Input, Output, ComponentFactoryResolver,
  ViewContainerRef, Injector, ViewChild, EventEmitter } from '@angular/core';
import { GridColumn } from '../grid.component';



@Component({
  selector: 'app-customcolumn',
  templateUrl: './customcolumn.component.html',
  styleUrls: ['./customcolumn.component.css']
})
export class CustomcolumnComponent implements OnInit {

  constructor(private resolver: ComponentFactoryResolver,
    private injector: Injector,
    private vcr: ViewContainerRef ) { }

  @Input()
  row: any;
  @Input()
  column: GridColumn;
  component: any;
  @Output()
  ColumnClicked = new EventEmitter();

  @ViewChild('content', { read: ViewContainerRef }) content;

  ngOnInit() {
    const factory = this.resolver.resolveComponentFactory(this.column.customComponentType);
    this.component = this.content.createComponent(factory);
    this.component.instance.row = this.row;
    this.component.instance.column = this.column;
    this.component.instance.ColumnClicked.subscribe(data => this.ColumnClicked.emit(data));
  }

}

export interface ICustomGridColumnComponentContent {
  row: any;
  column: GridColumn;
  ColumnClicked: EventEmitter<any>;
}

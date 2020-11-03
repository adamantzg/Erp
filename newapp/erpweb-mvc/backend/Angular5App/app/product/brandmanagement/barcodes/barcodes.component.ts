import { Component, OnInit, Input } from '@angular/core';
import { CustProduct } from '../../domainclasses';
import { GridDefinition, GridColumn, GridColumnType} from '../../../common';

@Component({
  selector: 'app-barcodes',
  templateUrl: './barcodes.component.html',
  styleUrls: ['./barcodes.component.css']
})
export class BarcodesComponent implements OnInit {

  constructor() {
    this.gridDefinition.columns[0].headerStyle = {width: '120px', 'font-size': '0.9em'};
    this.gridDefinition.columns[1].headerStyle = {width: '120px', 'font-size': '0.9em'};
    this.gridDefinition.columns[2].headerStyle = {width: '500px', 'font-size': '0.9em'};
    this.gridDefinition.columns[3].headerStyle = {width: '250px', 'font-size': '0.9em'};
  }

  errorMessage = '';
  gridDefinition = new GridDefinition(
    [
      new GridColumn('Brand code', 'cprod_code1', GridColumnType.Label, 'cprod_code1', { width: '120px'}),
      new GridColumn('Factory', 'mastProduct.factory.factory_code' , GridColumnType.Label, 'mastProduct.factory.factory_code', { width: '120px'}),
      new GridColumn('Band description', 'cprod_name', GridColumnType.Label, 'cprod_name', { width: '500px'}),
      new GridColumn('Barcode', 'barcode', GridColumnType.Label, 'barcode', { width: '250px'})
  ], true );

  gridStyle = { height: '500px'};

  gridData: CustProduct[] = [];

  @Input()
  products: CustProduct[] = [];

  ngOnInit() {
    this.gridData = this.products;
  }
}

import { Component, OnInit, Input, EventEmitter } from '@angular/core';
import { ProductpricingService } from '../../services/productpricing.service';
import { Market, ProductPricingProject } from '../../domainclasses';
import { CommonService, GridColumn } from '../../../common';
import { Settings } from '../../settings';
import { ICustomGridColumnComponentContent } from '../../../common';


@Component({
  selector: 'app-reportfilter',
  templateUrl: './reportfilter.component.html',
  styleUrls: ['./reportfilter.component.css']
})
export class ReportfilterComponent implements OnInit, ICustomGridColumnComponentContent {


  constructor(private pricingService: ProductpricingService,
  private commonService: CommonService) { }

  markets: Market[] = [];
  row: ProductPricingProject;
  column: GridColumn;
  calculations = Settings.calculations;
  @Input()
  filterData: ReportFilterData = new ReportFilterData();
  showButton = true;
  showFilter = false;
  ColumnClicked = new EventEmitter();

  ngOnInit() {

    this.pricingService.getMarkets().subscribe(data => this.markets = data,
    err => this.commonService.getError(err));
  }

  onOver() {
    this.showFilter = true;
  }

  onLeave(event: MouseEvent) {
    this.showFilter = false;
  }

  showReport() {
    window.open(`${this.column.data.reportUrl}?projectId=${this.row.id}&market_id=${this.filterData.market_id}&calculation=${this.filterData.calculation_id}`);
  }

}

export class ReportFilterData {

  constructor() {}

  market_id: number;
  calculation_id: number;
}


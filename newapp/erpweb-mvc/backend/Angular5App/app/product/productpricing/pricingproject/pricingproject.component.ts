import { Component, OnInit } from '@angular/core';
import { ProductPricingProject } from '../../domainclasses';
import { Router } from '@angular/router';
import { ProductpricingService } from '../../services/productpricing.service';
import { CommonService, GridDefinition, GridColumn, GridColumnType, GridButtonEventData } from '../../../common';
import { ReportFilterData, ReportfilterComponent } from '../reportfilter/reportfilter.component';


@Component({
  selector: 'app-pricingprojects',
  templateUrl: './pricingproject.component.html',
  styleUrls: ['./pricingproject.component.css']
})
export class PricingprojectComponent implements OnInit {

  constructor(private router: Router,
  private productPricingService: ProductpricingService,
  private commonService: CommonService) { }

  projects: ProductPricingProject[] = [];
  errorMessage = '';
  filterData = new ReportFilterData();
  gridDefinition: GridDefinition;
  reportUrl = '/product/ProductPricingReport';

  ngOnInit() {
    this.gridDefinition = new GridDefinition(
      [
        new GridColumn('Name', 'name', GridColumnType.Label, '', {'width' : '20%'}),
        new GridColumn('Pricing model', 'pricingModel.name', GridColumnType.Label, ''),
        new GridColumn('Currency', 'currency.curr_symbol', GridColumnType.Label, '', {'width' : '20%'}),
        new GridColumn('', '', GridColumnType.Custom, 'report', {'text-align': 'right', width: '25%'},
        null, ReportfilterComponent, {reportUrl: this.reportUrl, showButton: true}),
        // new GridColumn('Report', '', GridColumnType.Button, 'report', {width: '80px'}),
        new GridColumn('Edit', '', GridColumnType.Button, 'edit', {width: '80px'})
      ]
    );
    this.productPricingService.getProjects().subscribe( data => this.projects = data,
    err => this.errorMessage = this.commonService.getError(err));
  }

  newProject() {
    this.router.navigate(['/productpricing/newproject']);
  }

  buttonClicked(b: GridButtonEventData) {
    if (b.name === 'edit') {
      this.router.navigate(['/productpricing/editproject/' + b.row.id]);
    } else if (b.name === 'report') {
      if (this.filterData.market_id == null || this.filterData.calculation_id == null) {
        this.errorMessage = 'Select market and calculation before opening a report.';
      } else {
        window.open(`${this.reportUrl}?projectId=${b.row.id}&market_id=${this.filterData.market_id}&calculation=${this.filterData.calculation_id}`);
      }

    }
  }


}

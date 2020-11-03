import { Component, OnInit } from '@angular/core';
import { ProductpricingService } from '../../services/productpricing.service';
import { Router } from '@angular/router';
import { Market } from '../../domainclasses';
import { CommonService, Currency} from '../../../common';

@Component({
  selector: 'app-market',
  templateUrl: './market.component.html',
  styleUrls: ['./market.component.css']
})
export class MarketComponent implements OnInit {

  constructor(private productPricingService: ProductpricingService,
    private router: Router,
    private commonService: CommonService) { }

  rows: Market[] = [];
  errorMessage = '';
  currencies: Currency[] = [];

  ngOnInit() {
    this.productPricingService.getMarkets().subscribe(data => this.rows = data,
      err => this.errorMessage = this.commonService.getError(err));
    this.productPricingService.getCurrencies().subscribe(data => this.currencies = data,
      err => this.errorMessage = this.commonService.getError(err));
  }

  update() {
    this.productPricingService.updateMarkets(this.rows).subscribe(data => this.rows = data,
    err => this.errorMessage = this.commonService.getError(err));
  }

  addRow() {
    this.rows.push(new Market());
  }

  removeRow(index: number) {
    this.rows.splice(index, 1);
  }

  back() {
    this.router.navigate(['/productpricing']);
  }

}

import { Component, OnInit, Input } from '@angular/core';
import { ProductpricingService } from '../../services/productpricing.service';
import { CommonService } from '../../../common';
import { ProductPricingSetting, ProductPricingSettingId } from '../../domainclasses';
import { Router } from '@angular/router';

@Component({
  selector: 'app-pricingsetting',
  templateUrl: './pricingsetting.component.html',
  styleUrls: ['./pricingsetting.component.css']
})
export class PricingsettingComponent implements OnInit {

  constructor(private productPricingService: ProductpricingService,
  private commonService: CommonService,
  private router: Router) { }

  @Input()
  settings: ProductPricingSetting[];
  @Input()
  autoLoad = true;
  @Input()
  showButtons = true;
  errorMessage = '';
  @Input()
  showTitle = true;
  settingIds = Object.assign({}, ProductPricingSettingId);


  ngOnInit() {
    if (this.autoLoad) {
      this.productPricingService.getSettings().subscribe(d => this.settings = d,
        err => this.errorMessage = this.commonService.getError(err));
    }
  }

  update() {
    this.productPricingService.updateSettings(this.settings).subscribe(null, err => this.errorMessage = this.commonService.getError(err));
  }

  back() {
    this.router.navigate(['productpricing']);
  }

  isPercentage(s: ProductPricingSetting): boolean {
    return s.id === this.settingIds.SageFreight || s.id === this.settingIds.Commision || s.id === this.settingIds.FiscalAgent;
  }

}

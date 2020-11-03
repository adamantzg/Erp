import { Component, OnInit, Input, Output } from '@angular/core';
import { ProductPricingModel, ProductPricingModelLevel, CustProduct, Market, MarketProduct } from '../../domainclasses';
import { ProductpricingService } from '../../services/productpricing.service';
import { Router } from '@angular/router';
import { CommonService } from '../../../common';


@Component({
  selector: 'app-pricingmodel',
  templateUrl: './pricingmodel.component.html',
  styleUrls: ['./pricingmodel.component.css']
})
export class PricingmodelComponent implements OnInit {

  constructor(private productPricingService: ProductpricingService,
  private router: Router,
  private commonService: CommonService) { }


  protected data: ProductPricingModel[] = [];
  levels: any[] = [];
  maxLevel = 0;
  errorMessage = '';
  @Input()
  prices: MarketProduct[] = null;
  @Input()
  mode: PricingmodelComponentMode = PricingmodelComponentMode.EditPercentages;
  showTitle = false;
  showButtons = false;
  showEditors = false;
  showMarkets = false;
  markets: Market[] = [];
  componentModes = Object.assign({}, PricingmodelComponentMode);
  numFormat = '1.2-2';

  @Input()
  public get models(): ProductPricingModel[] {
    return this.data;
  }

  public set models(value: ProductPricingModel[]) {
    this.data = value;
    this.buildMatrix();
  }


  ngOnInit() {
    this.showTitle = this.mode === PricingmodelComponentMode.EditPercentages;
    this.showButtons = this.mode === PricingmodelComponentMode.EditPercentages;
    this.showEditors = this.mode === PricingmodelComponentMode.EditPercentages;
    this.showMarkets = this.mode === PricingmodelComponentMode.EditPercentages;

    if (this.mode === PricingmodelComponentMode.EditPercentages) {
      this.productPricingService.getModels().subscribe(data => {
        this.data = data;
        this.buildMatrix();
       } );
      this.productPricingService.getMarkets().subscribe(data => this.markets = data);
    }
  }

  buildMatrix() {
    const allLevels = this.getAllLevels(this.data).map(l => l.level);
    if (allLevels.length > 0) {
      this.maxLevel = allLevels.reduce((l1, l2) => Math.max(l1, l2));
    }
    for (let i = 1; i <= this.maxLevel; i++) {
      this.levels.push({level: i });
    }
    this.insertMissingLevels(this.data);
  }

  insertMissingLevels(data: ProductPricingModel[]) {
    data.forEach(m => {
      for ( let i = 1; i <= this.maxLevel; i++) {
        if (m.levels.find(l => l.level === i) == null) {
          const ppLevel = new ProductPricingModelLevel();
          ppLevel.level = i;
          m.levels.push(ppLevel);
        }
      }

    });
  }

  getAllLevels(models: ProductPricingModel[]): ProductPricingModelLevel[] {
    const levelsFlat = models.map(m => m.levels);
    if (levelsFlat.length > 0) {
      return levelsFlat.reduce((a, b) => a.concat(b));
    }
    return [];
  }

  update() {
    const data: ProductPricingModel[] = [];
    this.data.forEach(m => {
      const d = new ProductPricingModel();
      d.name = m.name;
      d.market_id = m.market_id;
      d.id = m.id;
      d.levels = m.levels.filter(l => l.value != null && l.value > 0);
      data.push(d);
    });
    this.productPricingService.updateModels(data).subscribe(updated => {
      this.insertMissingLevels(updated);
      this.data = updated;
      this.errorMessage = null;
    },
      (err) => this.errorMessage = this.commonService.getError(err)
    );
  }

  back() {
    this.router.navigate(['productpricing']);
  }

  addLevel() {
    this.maxLevel++;
    this.levels.push({level: this.maxLevel});

    this.data.forEach(m => {
      const ppLevel = new ProductPricingModelLevel();
      ppLevel.level = this.maxLevel;
      m.levels.push(ppLevel);
    }
    );
  }

  addModel() {
    const m = new ProductPricingModel();
    for ( let i = 1; i <= this.maxLevel; i++) {
      const ppLevel = new ProductPricingModelLevel();
      ppLevel.level = i;
      m.levels.push(ppLevel);
    }
    this.data.push(m);
  }

  removeModel(index: number) {
    this.data.splice(index, 1);
  }

  calculateDiscount(model: ProductPricingModel, level: ProductPricingModelLevel) {
    const mp = this.prices.find(p => p.market_id === model.market_id);
    if (mp != null) {
      let totalDiscount = 1;
      for (let i = 1; i <= level.level; i++) {
        const l = model.levels.find(le => le.level === i);
        if (l != null && l.id > 0) {
          totalDiscount *= (1 - l.value);
        }
      }
      return mp.retail_price * totalDiscount;
    }
    return null;
  }


}

export enum PricingmodelComponentMode {
  EditPercentages = 0,
  CalculatePrices = 1
}

export class MarketPrice {
  market_id: number;
  price: number;
}

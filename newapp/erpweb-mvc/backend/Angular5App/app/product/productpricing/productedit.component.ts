import { Component, OnInit, EventEmitter } from '@angular/core';
import { ProductService } from '../services/product.service';
import { Category1, CustProduct, Category1Values,
  ProductPricingSettingId, Market, ProductPricingSetting,
  SalesForecast, ProductPricingModel, MastProductFileTypeValue, MastProductFile,
  ProductPricingProject, ProductPricingMastProductData} from '../domainclasses';
import { Company, ContainerTypeValue, CurrencyCode, Currency, ContainerType, Tariff, CommonService, Month21,
  CompanyService } from '../../common';
import { ActivatedRoute, Router} from '@angular/router';
import { ProductpricingService } from '../services/productpricing.service';
import { ProductPricingEditModel } from '../models';
import { PricingmodelComponentMode } from './pricingmodel/pricingmodel.component';
import { NgForm } from '@angular/forms/src/directives/ng_form';
import { BsModalService } from 'ngx-bootstrap';
import { SalesforecastmodalComponent } from '../salesforecast/salesforecastmodal.component';
import { UploadOutput, UploadInput, UploadFile, UploaderOptions, UploadStatus } from 'ngx-uploader';

@Component({
  selector: 'app-productedit',
  templateUrl: './productedit.component.html',
  styleUrls: ['./productedit.component.css']
})
export class ProducteditComponent implements OnInit {

  constructor(private companyService: CompanyService,
    private productService: ProductService,
    private productPricingService: ProductpricingService,
    private commonService: CommonService,
    private router: Router,
    private route: ActivatedRoute,
    private modalService: BsModalService) {
      this.files = []; // local uploading files array
      this.options = { concurrency: 1, allowedContentTypes: ['image/png', 'image/jpeg', 'image/gif'] };
      this.uploadInput = new EventEmitter<UploadInput>(); // input events, we use this to emit data to ngx-uploader
     }

  title = 'New product';
  categories: Category1[] = [];
  factories: Company[] = [];
  clients: Company[] = [];
  product: CustProduct = this.productService.createNew('D');
  model: ProductPricingEditModel = new ProductPricingEditModel();
  errorMessage = '';
  currency?: Currency = new Currency();
  currencyCodeValue: CurrencyCode;
  lmeRate?: number = null;
  shipmentType: ContainerType = new ContainerType();
  pricingModelMode: any = Object.assign({}, PricingmodelComponentMode);
  market: Market = {id: null, name: '', currency: {curr_code: null, curr_symbol: null}, internal_cost: null, currency_id: null, vat: null};
  settingId = Object.assign({}, ProductPricingSettingId);
  tariffs: Tariff[] = [];
  currencyDict: any = {};
  validationMessage = '';
  showValidation = false;
  numFormat = '1.2-2';
  // uploader
  options: UploaderOptions;
  formData: FormData;
  files: UploadFile[] = [];
  uploadInput: EventEmitter<UploadInput>;
  dragOver: boolean;
  uploadProgress = 0;
  uploading = false;
  projectId: number | null;

  ngOnInit() {

    let id = 0;
    const sId = this.route.snapshot.params.id;
    if (sId != null && sId.length > 0) {
      id = +sId;
    }
    const paramProjId = this.route.snapshot.params.project;
    if (paramProjId != null && paramProjId.length > 0) {
      this.projectId = +paramProjId;
    }

    this.commonService.setBreadCrumb('Product pricing');

    this.productPricingService.getEditModel(id, this.projectId).subscribe(m => {
      this.model = m;
      this.product = m.product;
      this.factories = m.factories;
      this.clients = m.clients;
      this.categories = m.categories;
      if (m.project != null) {
        this.model.settings = m.project.settings;
        if (this.product.cprod_id <= 0 && m.project.products.length > 0) {
          this.product.cprod_user = m.project.products[0].cprod_user;
          if (m.project.products[0].mastProduct != null) {
            this.product.mastProduct.factory = m.factories.find(f => f.user_id === m.project.products[0].mastProduct.factory_id);
            this.product.mastProduct.category1 = m.project.products[0].mastProduct.category1;
          }
        }
      }
      m.tariffs.forEach(t => this.tariffs.push(new Tariff(t)));
      if (id > 0) {
        this.title = 'Edit existing product';
        this.currency = this.model.currencies.find(c => c.curr_code === this.productService.getCurrencyCode(this.product));
        if (this.currency == null) {
          this.currency = this.model.currencies[0];
        }
        this.product.mastProduct.factory = this.factories.find(f => f.user_id === m.product.mastProduct.factory_id);
        this.product.mastProduct.tariff = this.tariffs.find(t => t.tariff_id === m.product.mastProduct.tariff_code);
        this.shipmentType =
          this.model.containerTypes.find(t => t.container_type_id === this.productService.getShipmentTypeId(this.product));
        if (this.product.cprod_status !== 'D' ) {
          this.errorMessage = 'Active product data cannot be modified';
        }
      } else {
        this.currency = this.model.currencies[0];
        this.shipmentType = this.model.containerTypes[0];
      }
      if (this.product.mastProduct.productPricingData == null) {
        this.product.mastProduct.productPricingData = new ProductPricingMastProductData();
        this.product.mastProduct.productPricingData.mastproduct_id = this.product.mastProduct.mast_id;
      }
      const setting = this.model.settings.find(s => s.id === ProductPricingSettingId.Lme);
      if (setting != null) {
        this.lmeRate = setting.numValue;
      }
      if (this.model.markets.length > 0) {
        this.market = this.model.markets[0];
      }
      this.updateMarketData();

      this.prepareMastProductPrices(this.product);

      this.currencyDict = this.buildCurrencyDict();
      this.model.markets.forEach(ma => ma.currency = this.model.currencies.find(c => c.curr_code === ma.currency_id));

    },
    err => this.errorMessage = this.commonService.getError(err));



  }

  back() {
    if (this.projectId == null) {
      this.router.navigate(['productpricing/']);
    }
    const route = 'productpricing/' + (this.projectId === 0 ? 'newproject' : `editproject/${this.projectId}`);
    this.router.navigate([route, {tab: 'products'}]);
  }

  updateMarketData() {
    this.model.markets.forEach (ma => {
      const mp = this.product.marketData.find(md => md.market_id === ma.id);
      if ( mp == null) {
        this.product.marketData.push({market_id: ma.id, cprod_id: this.product.cprod_id, retail_price: null, market: ma});
      } else {
        mp.market = ma;
      }
    });
  }

  checkDisabled(p: CustProduct): boolean {
    return p.cprod_id > 0 && p.cprod_status !== 'D';
  }

  checkLme(p: CustProduct) {
    return +p.mastProduct.category1 === Category1Values.brass;
  }

  getFreightCost() {

    if (this.shipmentType != null && this.market != null && this.product.mastProduct.factory != null) {
      const cost = this.model.freightCosts.find(c => c.container_id === this.shipmentType.container_type_id
        && c.market_id === this.market.id && c.location_id === this.product.mastProduct.factory.consolidated_port);
      if (cost != null) {
        return cost.value;
      }
    }
    return null;
  }

  getFreightCostPerUnit() {
    const cost = this.getFreightCost();
    let units = 0;
    switch (this.shipmentType.container_type_id) {
      case ContainerTypeValue.GP40:
        units = this.product.mastProduct.units_per_40pallet_gp;
        break;
      case ContainerTypeValue.GP20:
        units = this.product.mastProduct.units_per_20pallet;
        break;
      default:
        units = this.product.mastProduct.units_per_pallet_single;
        break;
    }
    if (units > 0) {
      return cost / units;
    }
    return null;
  }

  calculateLme(): number {
    if (this.lmeRate != null && this.product.mastProduct.lme > 0) {
      const price = this.getProductPrice();
      return +((((this.lmeRate - this.product.mastProduct.lme) * 0.000065) * price + price) + 0.0001).toFixed(2);
    }
    return this.getProductPrice();
  }

  getProductPrice(): number {
    return +this.productService.getPrice(this.product, this.currency.curr_code);
  }

  getFOBPrice() {
    if (this.checkLme(this.product)) {
      return this.calculateLme();
    }
    return this.getProductPrice();
  }

  update(editForm: NgForm) {
    if (editForm.valid) {
      const data = this.commonService.deepClone(this.product);
      if (data.mastProduct.factory != null) {
        data.mastProduct.factory_id = data.mastProduct.factory.user_id;
        data.mastProduct.factory = null;
      }
      if (data.mastProduct.tariff != null) {
        data.mastProduct.tariff_code = data.mastProduct.tariff.tariff_id;
        data.mastProduct.tariff = null;
      }
      // this.productService.cleanupPrices(data, this.currency.curr_code);
      const isNew = data.cprod_id <= 0;
      if (data.cprod_id <= 0 && this.projectId > 0) {
        const proj = new ProductPricingProject();
        proj.id = this.projectId;
        data.projects = [proj];
      }
      // eliminate empty prices
      data.mastProduct.prices = data.mastProduct.prices.filter(pr => pr.price > 0);
      // set default based on chosen currency
      if (data.mastProduct.prices.length > 0) {
        data.mastProduct.prices.forEach(p => p.isDefault = false);
        const mpp = data.mastProduct.prices.find(pr => +pr.currency_id === this.currency.curr_code);
        if (mpp != null) {
          mpp.isDefault = true;
        }
      }
      data.projects = [];

      this.productService.createOrUpdate(data).subscribe(p => {
        p.mastProduct.factory = this.factories.find(f => f.user_id === p.mastProduct.factory_id);
        p.mastProduct.tariff = this.tariffs.find(t => t.tariff_id === p.mastProduct.tariff_code);
        this.product = p;
        this.updateMarketData();
        this.prepareMastProductPrices(this.product);
        if (isNew) {
          const route = 'productpricing/editproduct/' + this.product.cprod_id;
          let params = {};
          if (this.projectId != null) {
            params = {project: this.projectId};
          }
          this.router.navigate([route, params]);
        }

      }, err => this.errorMessage = this.commonService.getError(err));
    } else {
      this.showValidation = true;
      this.validationMessage = 'Enter all required data before proceeding.';
    }

  }

  getSetting(id: number) {
    const sett = this.model.settings.find(s => s.id === id);
    if (sett != null) {
      return sett.numValue;
    }
    return null;
  }

  getImportDuty() {
    if (this.product.mastProduct.tariff != null) {
      return this.getFOBPrice() * this.product.mastProduct.tariff.tariff_rate;
    }
    return null;
  }

  buildCurrencyDict() {
    const gbp_usd = this.getSetting(ProductPricingSettingId.GbpUsdRate);
    const gbp_eur = this.getSetting(ProductPricingSettingId.GbpEurRate);
    const result = {};

    const currencies = [CurrencyCode.EUR, CurrencyCode.USD, CurrencyCode.GBP];
    result[CurrencyCode.EUR] = {};
    result[CurrencyCode.EUR][CurrencyCode.EUR] = 1;
    result[CurrencyCode.EUR][CurrencyCode.USD] = 1 / gbp_eur * gbp_usd;
    result[CurrencyCode.EUR][CurrencyCode.GBP] = 1 / gbp_eur;
    result[CurrencyCode.USD] = {};
    result[CurrencyCode.USD][CurrencyCode.EUR] = 1 / gbp_eur * gbp_usd;
    result[CurrencyCode.USD][CurrencyCode.USD] = 1;
    result[CurrencyCode.USD][CurrencyCode.GBP] = 1 / gbp_usd;
    result[CurrencyCode.GBP] = {};
    result[CurrencyCode.GBP][CurrencyCode.EUR] = gbp_eur;
    result[CurrencyCode.GBP][CurrencyCode.USD] = gbp_usd;
    result[CurrencyCode.GBP][CurrencyCode.GBP] = 1;
    return result;

  }

  toMarketCurrency(amount: number) {
    if (this.currency != null && this.market != null) {
      if (this.currencyDict[this.currency.curr_code] != null) {
        return this.currencyDict[this.currency.curr_code][this.market.currency_id] * amount;
      }
    }
    return 1;
  }

  getCommission() {
    return this.getFOBPrice() * this.getSetting(ProductPricingSettingId.Commision);
  }

  getLandedCostSage() {
    return this.toMarketCurrency(this.getFOBPrice() * (1 + this.getSetting(this.settingId.SageFreight)) +
    this.getCommission() + this.getImportDuty() );
  }

  getLandedCostGateway() {
    return this.toMarketCurrency((this.getFOBPrice() * (1 + this.getSetting(this.settingId.FiscalAgent))  + this.getFreightCostPerUnit() +
    + this.getImportDuty()) * (1 + this.market.internal_cost) + this.getCommission() );
  }

  getForecastSaleTotal() {
    let s = 0;
    this.product.salesForecast.filter(f => f.sales_qty != null).forEach(f => s += f.sales_qty);
    return s;
  }

  getTotalCostSage() {
    return this.getForecastSaleTotal() * this.getLandedCostSage();
  }

  getTotalCostGateway() {
    return this.getForecastSaleTotal() * this.getLandedCostGateway();
  }

  getDiscountedPrice(model: ProductPricingModel, market_id) {

  }


  showForecasts() {

    const modal = this.modalService.show(SalesforecastmodalComponent);
    modal.content.cprod_id = this.product.cprod_id;
    modal.content.Forecasts = this.commonService.deepClone(this.product.salesForecast);
    modal.content.allowMonthChange = false;
    modal.content.onOk.subscribe((data: SalesForecast[]) => {
      data.forEach(d => d.sales_qty = +d.sales_qty);
      Object.assign(this.product.salesForecast, data);
    });
  }

  getMarketPricingModels() {
    if (this.market != null) {
      return this.model.pricingModels.filter(m => m.market_id === this.market.id);
    }
    return [];
  }


  getPricingModelDiscountPrice(m?: ProductPricingModel) {
    /*if (index != null) {
      const models = this.getMarketPricingModels();
      if (models.length > 0) {
        return this.getPricingModelDiscountPrice(models[0]);
      }
      return null;
    }*/
    if (m != null && this.market != null) {
      const marketData = this.product.marketData.find(md => md.market_id === this.market.id);
      if (marketData != null) {
        let price = marketData.retail_price;
        m.levels.filter(l => l.value > 0).sort((a, b) => a.level - b.level).forEach(l => {
          price *= (1 - l.value);
        });
        return price;
      }
    }
    return null;
  }

  getTotalSales(m: ProductPricingModel): number {
    return this.getForecastSaleTotal() * this.getPricingModelDiscountPrice(m);
  }

  getImage() {
    const empty = '/images/no-image_wide.jpg';
    if (this.product != null && this.product.mastProduct != null) {
      const file = this.product.mastProduct.files.find(f => f.file_type_id === MastProductFileTypeValue.Picture);
      if (file != null) {
        if (file.file_id != null) {
          return this.commonService.getTempUrl() + '?id=' + file.file_id;
        }
        return file.filename;
      }
      return empty;
    }
    return empty;
  }

  onUploadOutput(output: UploadOutput): void {
    if (output.type === 'allAddedToQueue') { // when all files added in queue
      // uncomment this if you want to auto upload files when added
      const event: UploadInput = {
        type: 'uploadAll',
        url: this.commonService.getUploadUrl() + '?id=' + this.files[0].id,
        method: 'POST'
      };
      this.uploading = true;
      // setTimeout(t => this.uploadInput.emit(event), 500 );
      this.uploadInput.emit(event);

    } else if (output.type === 'addedToQueue'  && typeof output.file !== 'undefined') { // add file to array when added
      this.files.push(output.file);
    } else if (output.type === 'uploading' && typeof output.file !== 'undefined') {
      // update current data in files array for uploading file
      const index = this.files.findIndex(file => typeof output.file !== 'undefined' && file.id === output.file.id);
      this.files[index] = output.file;
      this.uploadProgress = output.file.progress.data.percentage;
    } else if (output.type === 'removed') {
      // remove file from array when removed
      this.files = this.files.filter((file: UploadFile) => file !== output.file);
    } else if (output.type === 'dragOver') {
      this.dragOver = true;
    } else if (output.type === 'dragOut') {
      this.dragOver = false;
    } else if (output.type === 'drop') {
      this.dragOver = false;
    } else if (output.type === 'done') {
      this.uploading = false;
      this.uploadProgress = 0;
      let file = this.product.mastProduct.files.find(f => f.file_type_id === MastProductFileTypeValue.Picture);
      if (file == null) {
        file = new MastProductFile();
        file.file_type_id = MastProductFileTypeValue.Picture;
        this.product.mastProduct.files.push(file);
      }
      file.file_id = output.file.id;
      file.filename = output.file.name;
      this.files = [];
    }
  }

  prepareMastProductPrices(p: CustProduct) {
    if (p.mastProduct.prices == null) {
      p.mastProduct.prices = [];
    }
    this.model.currencies.forEach(c => {
      if (p.mastProduct.prices.find(pr => pr.currency_id === c.curr_code) == null) {
        p.mastProduct.prices.push({currency_id : c.curr_code, isDefault : false, mastproduct_id: p.cprod_mast, id: null, price: null });
      }
    });
  }

}

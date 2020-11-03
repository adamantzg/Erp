import { Injectable } from '@angular/core';
import { HttpService, Currency, CurrencyCode, ContainerTypeValue } from '../../common';
import { Settings } from '../settings';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { of } from 'rxjs/observable/of';
import {
  CustProduct, Category1, MastProduct, MarketProduct,
  ProductPricingMastProductData, Product
} from '../domainclasses';



@Injectable()
export class ProductService {

  constructor(private httpService: HttpService) { }

  api = Settings.apiRoot + 'product/';
  mastprodapi = Settings.apiRoot + 'mastproduct/';
  fileapi = Settings.apiRoot + 'file/';

  cachedProducts: CustProduct[] = [];
  cachedMastProducts: MastProduct[] = [];
  lastCode = '';
  lastMastCode = '';
  lastSearchText = '';

  searchProducts(factoryId?: number, clientId?: number, category1Id?: number, status?: string, brand_cat_id?: number, brand_userid?: number,
    searchText?: string): Observable<CustProduct[]> {
        const cat1idParam = category1Id || '', statusParam = status || '', brand_cat_param = brand_cat_id || '',
            brand_useridParam = brand_userid || '', searchParam = searchText || '';
        return this.httpService.get(this.api +
         `searchProducts?factory_id=${factoryId}&client_id=${clientId}&category1_id=${cat1idParam}
         &status=${statusParam}&brand_cat_id=${brand_cat_param}&brand_userid=${brand_useridParam}&searchText=${searchParam}`);
  }
  searchSpare(code: string): Observable<Product[]> {

    return this.httpService.getNoBlock(this.api + 'SearchProductsForBrands', { params: { searchText: encodeURIComponent(code) } })
  }
 
  getProductsSpares(code: string):Observable<Product[]> {
    console.log("Get products spares: " +code);
    return this.httpService.get(this.api + 'GetSparesOrRelated', { params: { cprod_code: encodeURIComponent(code)}} );
  }

  getCategory1List(): Observable<Category1[]> {
    return this.httpService.get(this.api + 'getCategory1List');
  }

  createNew(status?: string): CustProduct {
    const p = new CustProduct();
    if (status != null) {
      p.cprod_status = status;
    }
    p.mastProduct = new MastProduct();
    p.mastProduct.files = [];
    p.mastProduct.prices = [];
    p.projects = [];
    p.mastProduct.productPricingData = new ProductPricingMastProductData();
    p.marketData = [];
    p.salesForecast = [];
    return p;
  }

  getCurrencyCode(product: CustProduct): number {
    if (product.mastProduct != null && product.mastProduct.prices != null && product.mastProduct.prices.length > 0) {
      /* if (product.mastProduct.price_dollar > 0) {
        return CurrencyCode.USD;
      } else if (product.mastProduct.price_pound > 0) {
        return CurrencyCode.GBP;
      } else if (product.mastProduct.price_euro > 0) {
        return CurrencyCode.EUR;
      } */
      let mpp = product.mastProduct.prices.find(pr => pr.isDefault === true);
      if (mpp == null) {
        mpp = product.mastProduct.prices[0];
      }
      return mpp.currency_id;
    }
    return null;
  }

  getPrice(product: CustProduct, currencyCode: number) {
    if (product.mastProduct != null && product.mastProduct.prices != null) {
      const mpp = product.mastProduct.prices.find(p => p.currency_id === currencyCode);
      if (mpp != null) {
        return mpp.price;
      }
      return null;
    }
    return null;
  }

  cleanupPrices(product: CustProduct, currencyCode: number) {
    if (product.mastProduct != null) {
      if (currencyCode === CurrencyCode.USD) {
        product.mastProduct.price_euro = null;
        product.mastProduct.price_pound = null;
      } else if (currencyCode === CurrencyCode.GBP) {
        product.mastProduct.price_euro = null;
        product.mastProduct.price_dollar = null;
      } else if (currencyCode === CurrencyCode.EUR) {
        product.mastProduct.price_dollar = null;
        product.mastProduct.price_pound = null;
      }
      return null;
    }
    return null;
  }

  getShipmentTypeId(product: CustProduct): number {
    if (product.mastProduct != null) {
      if (product.mastProduct.units_per_40pallet_gp > 0) {
        return ContainerTypeValue.GP40;
      } else if (product.mastProduct.units_per_20pallet > 0) {
        return ContainerTypeValue.GP20;
      } else {
        return null;
      }
    }
    return null;
  }

  

  getProduct(id: number): Observable<CustProduct> {
    return this.httpService.get(this.api + 'get', { params: { id: id } });
  }

  createOrUpdate(p: CustProduct): Observable<CustProduct> {
    const url = p.cprod_id > 0 ? 'update' : 'create';
    return this.httpService.post(this.api + url, p);
  }

  getMastProductSelectorModel() {
    return this.httpService.get(this.mastprodapi + 'getSelectorModel');
  }

  getMastProductByCriteria(factoryId: number, clientId?: number, categoryId?: number, onlyFiles?: boolean,
    salesMonthsInPast?: number, productCode?: string, custProducts?: boolean, packagingMaterials?: boolean,
    brand_cat_id?: number) {
    const params = {
        factoryId: factoryId,
        clientId: clientId,
        categoryId: categoryId,
        onlyFiles: onlyFiles,
        salesMonthsInPast: salesMonthsInPast,
        custProducts: custProducts,
        packagingMaterials: packagingMaterials,
        brand_cat_id: brand_cat_id
    }
    if(productCode != null && productCode.length > 0) {
        params['productCode'] = productCode;
    }
    return this.httpService.get(this.mastprodapi + 'getByCriteria',
      {
        params: params
      });
  }

  updateMastProductsBulk(products: MastProduct[]) {
    return this.httpService.put(this.mastprodapi + 'updatebulk', products);
  }

  updateCustProductsBulk(products: CustProduct[]) {
    return this.httpService.put(this.api + 'updatebulk', products);
  }

  removeMastProductFile(mastId: number, fileId: number) {
    return this.httpService.delete(this.mastprodapi + `removeFile?mastProductId=${mastId}&fileId=${fileId}`);
  }

  removeCustProductFile(cprodId: number, fileId: number) {
    return this.httpService.delete(this.api + `removeFile?productId=${cprodId}&fileId=${fileId}`);
  }

  searchMastProducts(code: string): Observable<MastProduct[]> {
    if (this.lastMastCode.length > 0 && code.length >= this.lastMastCode.length) {
        if (code.toLowerCase().startsWith(this.lastMastCode.toLowerCase())) {

          return of(this.cachedMastProducts
            .sort((a, b) => (a.factory_ref > b.factory_ref) ? 1 : ((b.factory_ref > a.factory_ref) ? -1 : 0))
            .filter(p => p.factory_ref.toLowerCase().indexOf(code.toLowerCase()) >= 0
             || p.custProducts.filter(cp => cp.cprod_code1.toLowerCase().indexOf(code.toLowerCase()) >= 0).length > 0 ));
        }
      }
      this.lastMastCode = code;
      return this.httpService.get(this.mastprodapi + 'searchByCode', { params: { code: code}}).pipe(
        tap((data) => {
             // const temp =  data.filter(item => item.code.toLowerCase() === code.toLowerCase() );
            this.cachedMastProducts = data
                .sort((a, b) => (a.factory_ref > b.factory_ref) ? 1 : ((b.factory_ref > a.factory_ref) ? -1 : 0));
            }
         )
      );

  }

  getMastProduct(id: number) {
    return this.httpService.get(this.mastprodapi + 'getData/?id=' + id);
  }

  getBrandCategories(brand_id: number) {
      return this.httpService.get(this.api + 'getBrandCategories', { params: {brand_id: brand_id}});
  }

  updateProductData(p: CustProduct) {
      const data = {};
      // create dictionary of only those values that exist
      for (const key in p) {
        if (p[key]) {
            data[key] = p[key];
        }
      }
      return this.httpService.post(this.api + 'updatePartial', data);
  }

    getTechPdfLink(prod: CustProduct, asproot: string, suffix: string) {

        if (prod.cprod_dwg || prod.mastProduct.prod_image3) {
            return `${asproot}/_client_application/client_PR_4_tech${suffix}_pdf.asp?prod_id=${prod.cprod_id}&cprod_code=${prod.cprod_code1}&loc1=${prod.cprod_user}`;
        }
        return '';
    }
    getDataPdfLink(field1: string, field2: string) {
        return field1 ? field1 : field2;
    }

    getDataPdfLink2(fields: string[]) {
        for (let i = 0; i < fields.length - 1; i++) {
            const field = this.getDataPdfLink(fields[i], fields[i + 1]);
            if(field) {
                return field;
            }
        }
        return '';
    }

    addProducts(brand_id: number, brand_category_id: number, mastIds: number[]) {
        return this.httpService.post(this.api + `addProducts?brand_id=${brand_id}&brand_category_id=${brand_category_id}`, mastIds);
    }

    updateTariffCode (mast_id: number, tariff_code: number) {
        return this.httpService.put(this.mastprodapi + `updateTariffCode?mast_id=${mast_id}&tariff_code=${tariff_code}`, null);
    }

    getCategoriesByBrand(brand_id: number) {
        return this.httpService.get(this.mastprodapi + 'getCategoriesByBrand', { params: { brand_id: brand_id}});
    }

    getFactoriesByCriteria(brand_id: number, category1_id?: number) {
        return this.httpService.get(this.mastprodapi + 'getFactoriesByCriteria', { params: { brand_id : brand_id, category1_id: category1_id}});
    }

    searchProductsCached(factoryId?: number, clientId?: number, category1Id?: number, status?: string, brand_cat_id?: number, brand_userid?: number,
        searchText?: string): Observable<CustProduct[]> {

            if (this.lastSearchText.length > 0 && searchText.length >= this.lastSearchText.length) {
                if (searchText.toLowerCase().startsWith(this.lastSearchText.toLowerCase())) {

                    return of(this.cachedProducts
                    .sort((a, b) => (a.cprod_code1 > b.cprod_code1) ? 1 : ((b.cprod_code1 > a.cprod_code1) ? -1 : 0))
                    .filter(p => p.cprod_code1.toLowerCase().indexOf(searchText.toLowerCase()) >= 0
                        || p.cprod_name.toLowerCase().indexOf(searchText.toLowerCase()) >= 0));
                }
            }

            const cat1idParam = category1Id || '', statusParam = status || '', brand_cat_param = brand_cat_id || '',
                brand_useridParam = brand_userid || '', searchParam = searchText || '';
            this.lastSearchText = searchText;
            return this.httpService.get(this.api +
             `searchProducts?factory_id=${factoryId}&client_id=${clientId}&category1_id=${cat1idParam}
             &status=${statusParam}&brand_cat_id=${brand_cat_param}&brand_userid=${brand_useridParam}&searchText=${searchParam}`).pipe(
                tap((data) => {
                   this.cachedProducts = data
                       .sort((a, b) => (a.cprod_code1 > b.cprod_code1) ? 1 : ((b.cprod_code1 > a.cprod_code1) ? -1 : 0));
                   }
                )
             );
      }

}

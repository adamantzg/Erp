import { Injectable } from '@angular/core';
import { HttpService, Location, ContainerType, FreighCost, Currency } from '../../common';
import { Settings } from '../settings';
import { Observable } from 'rxjs';
import { ProductPricingModel, ProductPricingSetting, Market, ProductPricingProject } from '../domainclasses';
import { FreightCostModel, ProductPricingEditModel, ProductPricingProjectEditModel } from '../models';
import { tap } from 'rxjs/operators';

@Injectable()
export class ProductpricingService {

  constructor(private httpService: HttpService) { }

  api = Settings.apiRoot + 'productpricing/';

  getModels(): Observable<ProductPricingModel[]> {
    return this.httpService.get(this.api + 'models');
  }

  updateModels(models: ProductPricingModel[]): Observable<ProductPricingModel[]> {
    return this.httpService.post(this.api + 'updateModels', models);
  }

  getSettings(): Observable<ProductPricingSetting[]> {
    return this.httpService.get(this.api + 'getSettings');
  }

  updateSettings(data: ProductPricingSetting[]) {
    return this.httpService.post(this.api + 'updateSettings', data);
  }

  getLocations(): Observable<Location[]> {
    return this.httpService.get(this.api + 'locations');
  }

  getMarkets(): Observable<Market[]> {
    return this.httpService.get(this.api + 'markets');
  }

  getContainers(): Observable<ContainerType[]> {
    return this.httpService.get(this.api + 'containers').pipe(
      tap(data => this.addPallet(data))
    );
  }

  getFreightCosts(): Observable<FreighCost[]> {
    return this.httpService.get(this.api + 'freightCosts');
  }

  getFreightCostsModel(): Observable<FreightCostModel> {
    return this.httpService.get(this.api + 'getfreightCostModel').pipe(
      tap(data => this.addPallet(data.containerTypes))
    );
  }

  updateFreightCosts(costs: FreighCost[]): Observable<FreighCost[]> {
    return this.httpService.post(this.api + 'updateFreightCosts', costs);
  }

  getEditModel(id?: number, projectId?: number): Observable<ProductPricingEditModel> {
    return this.httpService.get(this.api + 'getProductEditModel', {params: {id: id, projectId: projectId}}).pipe(
      tap(data => this.addPallet(data.containerTypes))
    );
  }

  addPallet(data: ContainerType[]) {
    data.push({container_type_id: null, container_type_desc: 'pallet', shortname: 'pallet' });
  }

  updateMarkets(data: Market[]): Observable<Market[]> {
    return this.httpService.post(this.api + 'updateMarkets', data);
  }

  getCurrencies(): Observable<Currency[]> {
    return this.httpService.get(this.api + 'currencies');
  }

  getProjects(): Observable<ProductPricingProject[]> {
    return this.httpService.get(this.api + 'projects');
  }

  getProjectEditModel(id: number): Observable<ProductPricingProjectEditModel> {
    return this.httpService.get(this.api + 'projectEditModel', {params: {id: id}});
  }

  createOrUpdateProject(p: ProductPricingProject): Observable<ProductPricingProject> {
    const url = p.id > 0 ? 'updateproject' : 'createproject';
    return this.httpService.post(this.api + url, p);
  }





}

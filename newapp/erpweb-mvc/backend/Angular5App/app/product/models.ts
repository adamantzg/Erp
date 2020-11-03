import { CustProduct, ProductPricingSetting, Market,
    ProductPricingModel, Category1, ProductPricingProject, Packaging, Material, MastProduct, MastProductsPackagingMaterial } from './domainclasses';
import { Location, ContainerType, FreighCost, Currency, Tariff, Company } from '../common';

export class FreightCostModel {
    locations: Location[];
    containerTypes: ContainerType[];
    markets: Market[];
    costs: FreighCost[];
}

export class ProductPricingEditModel {
    locations: Location[] = [];
    containerTypes: ContainerType[] = [];
    markets: Market[] = [];
    freightCosts: FreighCost[] = [];
    settings: ProductPricingSetting[] = [];
    pricingModels: ProductPricingModel[] = [];
    product: CustProduct;
    currencies: Currency[] = [];
    tariffs: Tariff[] = [];
    factories: Company[] = [];
    clients: Company[] = [];
    categories: Category1[] = [];
    project: ProductPricingProject;
    imagesRoot: string;
}

export class ProductPricingProjectEditModel {
    currencies: Currency[] = [];
    pricingModels: ProductPricingModel[] = [];
    project: ProductPricingProject;
}

export class ProductPackagingModel {
    factories: Company[] = [];
    packagings: Packaging[] = [];
    materials: Material[] = [];
    categories: Category1[] = [];
    clients: Company[] = [];
}

export class ProductPackagingExportModel {
    clients: Company[] = [];
    factories: Company[] = [];
    categories: Category1[] = [];
}


export class MastProductPackagingEntry {
    product: MastProduct;
    data: any;   // 2 dimensional array packagingsxmaterials
}



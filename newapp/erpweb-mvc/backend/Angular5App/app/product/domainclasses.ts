import { Company, Tariff, Checkable, ContainerType, ContainerTypeValue,
    Currency, CurrencyCode, FreighCost, Location, Lookup, File } from '../common';

export class MastProduct {
    mast_id: number;
    factory_ref: string;
    factory_id: number;
    asaq_ref: string;
    asaq_name: string;
    category1: number;
    price_dollar: number;
    price_euro: number;
    price_pound: number;
    lme: number;
    units_per_40pallet_gp: number;
    units_per_20pallet: number;
    units_per_pallet_single: number;
    factory: Company;
    tariff: Tariff;
    tariff_code: number;
    factory_moq: number;
    prod_image1: string;
    prod_nw: number;
    carton_GW: number;
    carton_length: number;
    carton_height: number;
    carton_width: number;
    pallet_weight: number;
    pallet_width: number;
    pallet_length: number;
    pallet_height: number;
    pack_GW: number;
    pack_height: number;
    pack_length: number;
    pack_width: number;
    product_group: string;
    prod_image3: string;
    prod_image2: string;
    prod_instructions: string;
    prod_width: number;
    prod_length: number;
    prod_height: number;
    units_per_carton: number;

    files: MastProductFile[];
    prices: MastProductPrice[];
    productPricingData: ProductPricingMastProductData;
    packagingMaterials: MastProductsPackagingMaterial[];
    // products: CustProduct[];
    otherFiles: File[];
    cprod_codes: string;
    productFiles: ProductFile[];
    custProducts: CustProduct[];
    category: Category1;

    selected: boolean;
    sizeFull: string;
}


export class CustProduct implements Checkable {
    cprod_stock_lvh: number | null;
    cprod_name: string;
    cprod_name_web_override: string;
    cprod_name2: string;
    whitebook_cprod_name: string;
    cprod_code1: string;
    cprod_code1_web_override: string;
    whitebook_cprod_code1: string;
    cprod_code2: string;
    cprod_image1: string;
    cprod_instructions2: string;
    cprod_instructions: string;
    cprod_label: string;
    cprod_packaging: string;
    cprod_dwg: string;
    cprod_spares: string;
    cprod_pdf1: string;
    cprod_status: string;
    cprod_status2: string;
    pack_image1: string;
    pack_image2: string;
    pack_image2b: string;
    pack_image2c: string;
    pack_image2d: string;
    pack_image3: string;
    pack_image4: string;
    insp_level_a: string;
    insp_level_D: string;
    insp_level_F: string;
    insp_level_M: string;
    client_image: string;
    cprod_track_image1: string;
    cprod_track_image2: string;
    cprod_track_image3: string;
    cprod_supplier: string;
    client_range: string;
    analysis_d: string;
    bin_location: string;
    cprod_opening_date: Date | string | null;
    cprod_stock_date: Date | string | null;
    cprod_pending_date: Date | string | null;
    EU_supplier: boolean | null;
    barcode: number | null;
    cprod_id: number;
    cprod_mast: number | null;
    cprod_user: number | null;
    cprod_cgflag: number | null;
    cprod_curr: number | null;
    cprod_opening_qty: number | null;
    cprod_oldcode: number | null;
    cprod_lme: number | null;
    cprod_brand_cat: number | null;
    cprod_brand_subcat: number | null;
    cprod_disc: number | null;
    cprod_seq: number | null;
    cprod_stock_code: number | null;
    brand_grouping: number | null;
    b_gold: number | null;
    cprod_loading: number | null;
    moq: number | null;
    WC_2011: number | null;
    cprod_stock: number | null;
    cprod_stock2: number | null;
    cprod_priority: number | null;
    aql_A: number | null;
    aql_D: number | null;
    aql_F: number | null;
    aql_M: number | null;
    criteria_status: number | null;
    cprod_confirmed: number | null;
    tech_template: number | null;
    tech_template2: number | null;
    cprod_returnable: number | null;
    client_cat1: number | null;
    client_cat2: number | null;
    bs_visible: number | null;
    original_cprod_id: number | null;
    cprod_range: number | null;
    cprod_combined_product: number | null;
    UK_production: number | null;
    report_exception: number | null;
    brand_userid: number | null;
    brand_id: number | null;
    buffer_stock_override_days: number | null;
    analytics_category: number | null;
    analytics_option: number | null;
    warning_report: number | null;
    cprod_special_payment_terms: number | null;
    product_type: number | null;
    pending_discontinuation: boolean | null;
    cwb_stock_type: number | null;
    pallet_grouping: number | null;
    dist_status: number | null;
    proposed_discontinuation: boolean | null;
    locked_sorder_qty: number | null;
    color_id: number | null;
    consolidated_port_override: number | null;
    stock_check: number | null;
    cust_product_range_id: number | null;
    discontinued_visible: number | null;
    cprod_price1: number | null;
    cprod_price2: number | null;
    cprod_price3: number | null;
    cprod_price4: number | null;
    cprod_retail: number | null;
    cprod_retail_uk: number | null;
    cprod_retail_pending: number | null;
    cprod_retail_web_override: number | null;
    cprod_old_retail: number | null;
    cprod_override_margin: number | null;
    days30_sales: number | null;
    cprod_pending_price: number | null;
    on_order_qty: number | null;
    sale_retail: number | null;
    wras: number | null;
    cprod_retail_pending_date: Date | string | null;
    product_group_id: number | null;

    checked: boolean;

    mastProduct: MastProduct;
    otherFiles: File[];
    marketData: MarketProduct[];
    salesForecast: SalesForecast[];
    projects: ProductPricingProject[];
    client: Company;
    productFiles: ProductFile[];

    extraData: CustProductsExtradata;
}

export class MarketProduct {
    market_id: number;
    cprod_id: number;
    retail_price: number;
    market: Market;
}

export class Category1 {

    category1_id: number;
    cat1_name: string;
}

export enum Category1Values {
    spares = 13,
    brass = 7
}

export class ProductPricingModelLevel {
    id: number;
    level: number;
    value?: number;
    model_id: number;
}

export class ProductPricingModel {
    id: number;
    name: string;
    market_id: number;
    levels: ProductPricingModelLevel[] =  [];

}

export class ProductPricingSetting {
    id: number;
    name: string;
    numValue: number;
}

export enum ProductPricingSettingId {
    Lme = 1,
    SageFreight = 2,
    FiscalAgent = 3,
    Commision = 4,
    GbpUsdRate = 5,
    GbpEurRate = 6
}

export class Market extends Lookup {
    internal_cost: number;
    vat: number;
    currency_id: number;
    currency: Currency;
}

export class SalesForecast {
    sales_unique: number;
    cprod_id: number | null;
    sales_qty: number | null;
    month21: number | null;

}

export class MastProductFile {
    id: number;
    filename: string;
    FileType: MastProductFileType;
    file_type_id: number | null;
    file_id: string;
}

export class MastProductPrice {
    id: number;
    mastproduct_id: number | null;
    currency_id: number | null;
    price: number | null;
    isDefault: boolean | null;
}

export class MastProductFileType {

    id: number;
    caption: string;
    extensions: string;
    isImage: boolean | null;
    path: string;
    IsUploadable: boolean | null;
}

export enum MastProductFileTypeValue {
    Picture = 1
}

export class ProductPricingProject {
    id: number;
    name: string;
    pricing_model_id: number | null;
    currency_id: number | null;

    pricingModel: ProductPricingModel;
    currency: Currency;

    products: CustProduct[] = [];
    settings: ProductPricingSetting[] = [];

}

export class ProductPricingMastProductData {
    mastproduct_id: number;
    tooling_cost: number | null;
    tooling_currency_id: number | null;
    initial_stock: number | null;
    display_qty: number | null;
}

export class Material {
    id: number;
    name: string;
}

export class Packaging {
    id: number;
    name: string;
}

export class MastProductsPackagingMaterial {
    id: number;
    mast_id: number;
    packaging_id: number;
    material_id: number;
    amount: number;

    constructor(id, mast_id, packaging_id, material_id, amount) {
        this.id = id;
        this.mast_id = mast_id;
        this.packaging_id = packaging_id;
        this.material_id = material_id;
        this.amount = amount;
    }
}

export class Product {
    cprod_id: number;
    cprod_code1: string;
    name: string;
    cprod_name: string;
    combined_name: string;
    isSpare: boolean;
}

export class ProductFile {
    id: number;
    file_name: string;
    cprod_id: number | null;
    mast_id: number | null;
    type_id: number | null;
    file_id: string;
    order_index: number | null;
}

export class ProductFileType {
    id: number;
    name: string;
    path: string;
    client_specific: boolean | null;
}

export class Brand
{
    brand_id: number;
    brandname: string;
    user_id: number | null;
    dealerstatus_view: string;
    code: string;
    image: string;
    eb_brand: number | null;
    category_flag: number | null;
    dealerstatus_manual: boolean | null;
    show_as_eb_default: boolean | null;
    show_as_eb_products: boolean | null;
    brand_group: number | null;
    dealersearch_view: string;

}

export class BrandCategory {
    brand_cat_id: number;
    brand_cat_desc: string;
    brand: number | null;
    unit_ordering: number | null;
    web_description: string;
    brand_cat_image: string;
    image_width: number | null;
    image_height: number | null;
    web_seq: number | null;
    unique_ordering: number | null;
    why_so_good: string;
    why_so_good_title: string;
    sale_retail_percentage: number | null;
    group_id: number | null;
}

export class CustProductsExtradata {
    cprod_id: number;
    lvh_terms: number | null;
    lvh_stock_type: number | null;
    analysis_e: string;
    removed_brochure: boolean | null;
    removed_website: boolean | null;
    removed_distributor: boolean | null;
}

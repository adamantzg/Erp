import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { BrandCategory, Category1, MastProduct, Brand } from '../../domainclasses';
import { Company, GridDefinition, GridColumn, GridColumnType, CommonService } from '../../../common';
import { BsModalRef } from 'ngx-bootstrap';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-addproductsmodal',
  templateUrl: './addproductsmodal.component.html',
  styleUrls: ['./addproductsmodal.component.css']
})
export class AddProductsModalComponent implements OnInit {

    constructor(private modalInstance: BsModalRef, private productService: ProductService,
        private commonService: CommonService) {
            for (const c of this.gridDefinition.columns) {
                c.headerStyle = c.style;
            }
        }

    errorMessage = '';
    type_id = null;
    factory_id = null;
    brand_category_id = null;
    products: any[] = [];
    gridStyle = { height: '300px'};
    shortenedTexts = {};
    headerStyle = { fontSize: '0.9em'};
    _brand_id = null;


    public get brand_id(): number {
        return this._brand_id;
    }

    public set brand_id(v: number) {
        this._brand_id = v;
        this.onBrandChanged();
    }


    gridDefinition = new GridDefinition(
        [
            new GridColumn('Factory Code', 'factory_ref', GridColumnType.Label, 'code', { width: '110px', fontSize: '0.9em'}),
            new GridColumn('Product Type', 'cat1_name', GridColumnType.Label, 'type', { width: '80px', fontSize: '0.9em'}),
            new GridColumn('BS Code', 'asaq_ref', GridColumnType.Label, 'bscode', { width: '90px', fontSize: '0.9em'}),
            new GridColumn('BS Description', 'asaq_name', GridColumnType.Label, 'name', {width: '230px', fontSize: '0.9em'}),
            new GridColumn('Size', 'sizeFull', GridColumnType.Label, 'size', { width: '150px', fontSize: '0.9em'}),
            new GridColumn('Factory FOB', 'fob', GridColumnType.Label, 'fob', { width: '80px', fontSize: '0.9em'}),
            new GridColumn('Existing records', 'existing', GridColumnType.Label, 'existing', { width: '80px', fontSize: '0.9em'}),
            new GridColumn('', 'selected', GridColumnType.Checkbox, 'selected', { width: '30px', fontSize: '0.9em'})
        ], true, true, '500px');


    @Input()
    brandCategories: BrandCategory[] = [];

    @Input()
    factories: Company[] = [];

    @Input()
    categories: Category1[] = [];

    @Input()
    brands: Brand[] = [];

    @Output()
    onOk = new EventEmitter();

    ngOnInit() {
        this.gridDefinition.columns[0].tooltipField = 'factory_ref_full';
        this.gridDefinition.columns[2].tooltipField = 'asaq_ref_full';
    }

    ok() {
        this.onOk.emit(new AddProductsModalOkEventArgs(this.selectedProducts()));
        this.modalInstance.hide();
    }

    cancel() {
        this.modalInstance.hide();
    }

    searchProducts() {
        this.productService.getMastProductByCriteria(this.factory_id, null, this.type_id, false, null, null, true, false, this.brand_category_id)
        .subscribe(
            data => this.products = this.transformProducts(data),
            err => this.commonService.getError(err)
        );
    }

    transformProducts(products: MastProduct[]): any[] {
        return products.map(p => {
            const prod: any = Object.assign({}, p);
            prod.factory_ref = this.correctLongText(p, 'factory_ref', 10);
            prod.factory_ref_full = p.factory_ref;
            prod.cat1_name = p.category.cat1_name;
            prod.asaq_ref = this.correctLongText(p, 'asaq_ref', 10);
            prod.sizeFull = `${p.prod_length || 0}X${p.prod_width || 0}X${p.prod_height || 0}`;
            prod.fob = this.getPrice(p);
            prod.existing = p.custProducts ? 'yes' : 'no';
            prod.selected = false;
            return prod;
        });
    }

    getPrice(p: MastProduct) {
        if (p.factory.user_curr == 0) {
            return p.price_dollar > 0 ? p.price_dollar : p.price_pound > 0 ? p.price_pound : p.price_euro;
        }
        if (p.factory.user_curr == 1) {
            return p.price_pound > 0 ? p.price_pound : p.price_dollar > 0 ? p.price_dollar : p.price_euro;
        }
        if (p.factory.user_curr == 2) {
            return p.price_euro > 0 ? p.price_euro : p.price_dollar > 0 ? p.price_dollar : p.price_pound;
        }
        return 0;
    }

    correctLongText(p: MastProduct, field: string, maxLength: number) {
        if (!(p.mast_id in this.shortenedTexts)) {
            this.shortenedTexts[p.mast_id] = {};
        }
        if (!(field in this.shortenedTexts[p.mast_id])) {
            const text = p[field] || '';
            let parts = text.split(' ');
            if (parts.length == 1) {
                parts = text.split('/');
            }
            this.shortenedTexts[p.mast_id][field] =  parts.map( x => x.length > maxLength ? x.substring(0, maxLength - 1) + '..' : x).join(' ');
        }
        return this.shortenedTexts[p.mast_id][field];
    }

    selectedProducts() {
        return this.products.filter(p => p.selected);
    }

    onBrandChanged() {
        this.type_id = null;
        this.factory_id = null;
        this.loadCategories();
        this.loadFactories();
    }

    loadCategories() {
        this.productService.getCategoriesByBrand(this.brand_id)
        .subscribe(
            data => {
                const allCat = new Category1();
                allCat.category1_id = null;
                allCat.cat1_name = 'All';
                data.splice(0, 0, allCat );
                this.categories = data;
            },
            err  => this.errorMessage = this.commonService.getError(err)
        );
    }

    loadFactories() {
        this.productService.getFactoriesByCriteria(this.brand_id, this.type_id)
        .subscribe(
            data => {
                const allFact = new Company();
                allFact.user_id = null;
                allFact.factory_code = 'All';
                data.splice(0, 0, allFact );
                this.factories = data;
            },
            err  => this.errorMessage = this.commonService.getError(err)
        );
    }

    onCategory1Changed() {
        this.loadFactories();
    }

}

export class AddProductsModalOkEventArgs {

    constructor(products: MastProduct[]) {
        this.products = products;
    }
    products: MastProduct[];
}

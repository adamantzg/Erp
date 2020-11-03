import { Component, OnInit, Input, IterableDiffers, DoCheck, IterableDiffer } from '@angular/core';
import { CustProduct, BrandCategory, Category1Values, MastProduct } from '../../domainclasses';
import { FormGroup, FormControl } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { CommonService, Company } from '../../../common';
import { Lightbox, IAlbum } from 'ngx-lightbox';

@Component({
  selector: 'app-productbulkedit',
  templateUrl: './productbulkedit.component.html',
  styleUrls: ['./productbulkedit.component.css']
})
export class ProductBulkEditComponent implements OnInit, DoCheck {

  constructor(private productService: ProductService, private commonService: CommonService,
    private _lightbox: Lightbox, private _iterableDiffers: IterableDiffers) {
        this.iterableDiffer = this._iterableDiffers.find([]).create(null);
     }

  formGroup: FormGroup;
  errorMessage = '';
  album = [];
  iconColumnStyle = { width: '50px'} ;
  shortenedTexts = {};
  iterableDiffer: IterableDiffer<CustProduct>;

  @Input()
  public get products(): CustProduct[] {
      return this._products;
  }

  public set products(prods: CustProduct[]) {
      this._products = prods;
      this.fixMissingData(prods);
      this.buildControls();
      this.album = prods.map(p => ({ src: p.mastProduct && p.mastProduct.prod_image1 ? p.mastProduct.prod_image1 : '/images/no-image_wide.jpg',
      thumb: p.mastProduct ? p.mastProduct.prod_image1 : '', caption: p.cprod_code1}));
  }

  _products: CustProduct[] = [];

  @Input()
  categories: BrandCategory[] = [];

  @Input()
  asproot = '';

  ngOnInit() {
  }

  ngDoCheck() {
    const changes = this.iterableDiffer.diff(this._products);
    if (changes) {
        // Add formcontrols
        for (let p of this._products) {
            if (!(this.getFormControlKey(p, 'cprod_code1') in this.formGroup.controls)) {
                this.addFormControl(p);
            }

        }
    }
  }

  buildControls() {
      const formGroup = {};
      for (let p of this._products) {
        formGroup[p.cprod_id + '_' + 'cprod_code1'] = new FormControl();
        formGroup[p.cprod_id + '_' + 'cprod_brand_cat'] = new FormControl();
        formGroup[p.cprod_id + '_' + 'cprod_name'] = new FormControl();
        formGroup[p.cprod_id + '_' + 'product_group'] = new FormControl();
      }
      this.formGroup = new FormGroup(formGroup);
  }

  addFormControl(p: CustProduct) {
    this.formGroup.addControl(this.getFormControlKey(p, 'cprod_code1'), new FormControl());
    this.formGroup.addControl(this.getFormControlKey(p, 'cprod_brand_cat'), new FormControl());
    this.formGroup.addControl(this.getFormControlKey(p, 'cprod_name'), new FormControl());
    this.formGroup.addControl(this.getFormControlKey(p, 'product_group'), new FormControl());
  }

  getFormControlKey(p: CustProduct, field: string) {
      return `${p.cprod_id}_${field}`;
  }

  checkUpdate(p: CustProduct, field: string) {
    const control = this.formGroup.controls[p.cprod_id + '_' + field];
    const data: any = Object.assign({}, p);
    delete data['brandCompany'];
    data.mastProduct = { product_group: p.mastProduct ?  p.mastProduct.product_group : ''};
    if (control.dirty) {
        this.productService.updateProductData(data).subscribe(
            () => {
                control.markAsPristine();
            },
            err => this.errorMessage = this.commonService.getError(err)
        );
    }
  }

  getTechDataLink(prod: CustProduct, suffix: string) {
      return this.productService.getTechPdfLink(prod, this.asproot, suffix);
  }

    getDataPdfLink(field1: string, field2: string) {
        return this.productService.getDataPdfLink(field1, field2);
    }

    getDataPdfLink2(fields: string[]) {
        return this.productService.getDataPdfLink2(fields);
    }

    getSparesLink(p: CustProduct) {
        if (!p.cprod_spares) {
            return '';
        }
        if (p.mastProduct.category1 == Category1Values.spares) {
            return `${this.asproot}/asaq_back/asaq_spares_product_upload.asp?cprod_id=${p.cprod_id}`;
        } else {
            return `${this.asproot}/asaq_back/asaq_spares_upload.asp?cprod_id=${p.cprod_id}`;
        }
    }

    openLightbox(index) {
        this._lightbox.open(this.album, index);
    }

    correctLongText(p: CustProduct) {
        if (p.mastProduct && p.mastProduct.factory_ref) {
            if (!(p.cprod_id in this.shortenedTexts)) {
                const parts = p.mastProduct.factory_ref.split(' ');
                this.shortenedTexts[p.cprod_id] = parts.map(x => x.length > 10 ? x.substring(0,9) + '..' : x).join(' ');
            }
            return this.shortenedTexts[p.cprod_id];
        }
        return '';

    }

    fixMissingData(products: CustProduct[]) {
        for (let p of products) {
            if(!p.mastProduct) {
                p.mastProduct = new MastProduct();
                p.mastProduct.factory = new Company();
            }
        }
    }

}

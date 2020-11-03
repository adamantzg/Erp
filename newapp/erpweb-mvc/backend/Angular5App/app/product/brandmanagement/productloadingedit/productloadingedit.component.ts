import { Component, OnInit, Input, DoCheck, IterableDiffer, IterableDiffers } from '@angular/core';
import { CustProduct } from '../../domainclasses';
import { ProductService } from '../../services/product.service';
import { CommonService } from '../../../common';
import { FormControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-productloadingedit',
  templateUrl: './productloadingedit.component.html',
  styleUrls: ['./productloadingedit.component.css']
})
export class ProductLoadingEditComponent implements OnInit, DoCheck {

  constructor(private productService: ProductService, private commonService: CommonService,
    private _iterableDiffers: IterableDiffers) {
    this.iterableDiffer = this._iterableDiffers.find([]).create(null);
   }

  _products: CustProduct[] = [];
  errorMessage = '';
  iterableDiffer: IterableDiffer<CustProduct>;

  @Input()
  public get products(): CustProduct[] {
      return this._products;
  }

  public set products(prods: CustProduct[]) {
      this._products = prods;
      this.buildControls();
  }

  formGroup = null;

  ngOnInit() {
  }

  ngDoCheck() {
    const changes = this.iterableDiffer.diff(this._products);
    if (changes) {
        // Add formcontrols
        for (let p of this._products) {
            if (!(this.getFormControlKey(p, 'cprod_moq') in this.formGroup.controls)) {
                this.addFormControl(p);
            }

        }
    }
  }


  buildControls() {
    const formGroup = {};
    for (const p of this._products) {
      formGroup[p.cprod_id + '_' + 'moq'] = new FormControl();
      formGroup[p.cprod_id + '_' + 'cprod_loading'] = new FormControl();
    }
    this.formGroup = new FormGroup(formGroup);
}

    addFormControl(p: CustProduct) {
        this.formGroup.addControl(this.getFormControlKey(p, 'moq'), new FormControl());
        this.formGroup.addControl(this.getFormControlKey(p, 'cprod_loading'), new FormControl());
    }

  getFormControlKey(p: CustProduct, field: string) {
      return `${p.cprod_id}_${field}`;
  }

checkUpdate(p: CustProduct, field?: string) {
    let dirty = true;
    let control = null;
    if (field) {
        control = this.formGroup.controls[p.cprod_id + '_' + field];
        dirty = control.dirty;
    }
    const data: any  = { cprod_id: p.cprod_id, moq: p.moq, cprod_loading: p.cprod_loading};
    if (dirty) {
        this.productService.updateProductData(data).subscribe(
            () => {
                if (control) {
                    control.markAsPristine();
                }
            },
            err => this.errorMessage = this.commonService.getError(err)
        );
    }
}

}


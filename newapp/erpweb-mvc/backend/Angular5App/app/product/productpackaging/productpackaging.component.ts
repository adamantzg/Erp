import { Component, OnInit } from '@angular/core';
import { ProductPackagingService } from '../services/productpackaging.service';
import { CommonService, BlockUIService } from '../../common';
import { ProductPackagingModel, MastProductPackagingEntry } from '../models';
import { MastProduct, MastProductsPackagingMaterial } from '../domainclasses';
import { FormGroup, FormControl } from '@angular/forms';

@Component({
  selector: 'app-productpackaging',
  templateUrl: './productpackaging.component.html',
  styleUrls: ['./productpackaging.component.css']
})
export class ProductPackagingComponent implements OnInit {

    constructor(private service: ProductPackagingService, private commonService: CommonService,
        private blockUIService: BlockUIService) {

    }

    model: ProductPackagingModel = new ProductPackagingModel();
    products: MastProductPackagingEntry[] = [];
    activePackagingId = null;
    form: FormGroup;
    page = 1;
    pageSize = 50;
    totals = { count: 0 };
    lastpage = 1; // last accessed page, to avoid unnecessary server trip

    filterParams = {
        factoryId: null,
        categoryId: null,
        clientId: null,
        code: null
    };
    errorMessage = null;

    ngOnInit() {
        this.service.getModel(false).subscribe((data) =>  {
            this.model = data;
            this.activePackagingId = this.model.packagings[0].id;
        },
        (err) => this.errorMessage = this.commonService.getError(err)
        );
    }

    getProducts() {

        this.service.getTotalForCriteria(this.filterParams.factoryId, this.filterParams.categoryId, this.filterParams.clientId,
            encodeURIComponent(this.filterParams.code))
            .subscribe(data => this.totals.count = data.count,
            err => this.errorMessage = this.commonService.getError(err));

        this.service.getByCriteria(this.filterParams.factoryId, this.filterParams.categoryId, this.filterParams.clientId,
            this.filterParams.code, this.page, this.pageSize )
        .subscribe(data => {
            this.blockUIService.startBlock();
            this.products = this.transformData(data);
            this.blockUIService.stopBlock();
        },
        err => this.errorMessage = this.commonService.getError(err)
        );
    }

    transformData(data: MastProduct[]): any {
        const result = [];
        const formGroup = {};
        for (let i = 0; i < data.length; i++) {
            const mp = data[i];
            mp.cprod_codes = this.getCustProductCodes(mp, this.filterParams.clientId);
            const item = new MastProductPackagingEntry();
            item.product = mp;
            item.data = new Array(this.model.materials.length);
            for (let j = 0;  j < this.model.materials.length; j++) {
                const m = this.model.materials[j];
                const packagings = new Array(this.model.packagings.length);
                item.data[j] = packagings;
                for (let k = 0; k < this.model.packagings.length; k++) {
                    const p = this.model.packagings[k];
                    let found = false;
                    let mpp = null;
                    if (mp.packagingMaterials != null) {
                        const ix = mp.packagingMaterials.findIndex(x => x.packaging_id === p.id && x.material_id === m.id);
                        if (ix >= 0) {
                            found = true;
                            mpp = mp.packagingMaterials[ix];
                        }
                    }
                    if (!found) {
                        mpp = new MastProductsPackagingMaterial(null, mp.mast_id, p.id, m.id, null);
                    }
                    formGroup[this.getControlName(mpp)] = new FormControl();
                    packagings[k] = mpp;
                }
            }
            result.push(item);
        }
        this.form = new FormGroup(formGroup);
        return result;
    }

    getCustProductCodes(mastProduct, client_id) {
        if (mastProduct.products == null) {
            return '';
        }
        return mastProduct.products.filter(cp => cp.brand_userid === parseInt(client_id, 10) || client_id == null).map(cp => cp.cprod_code1).join(', ');
    }

    checkMax(text: string, maxLen: number) {
        if (text.length < maxLen) {
            return text;
        }
        return text.substring(0, maxLen - 1);
    }

    calculateTotal(p: MastProductPackagingEntry) {
        let result = 0;
        const ix = this.model.packagings.findIndex(x => x.id === this.activePackagingId);
        for (let i = 0; i < p.data.length; i++) {
            const am = p.data[i][ix].amount;
            const amount = am != null ?  parseFloat(am.toString()) : 0;
            if (!isNaN(amount)) {
                result += amount;
            }
        }
        return result;
    }

    update(mmp: MastProductsPackagingMaterial) {
        const formControl = this.form.controls[this.getControlName(mmp)];
        if (formControl.dirty) {
            const amount = mmp.amount != null ? parseFloat(mmp.amount.toString()) : null;
            if ((amount == null || isNaN(amount))) {
                if (mmp.id) {
                    this.service.deletePackaging(mmp.id).subscribe(() => {},
                    err => this.errorMessage = this.commonService.getError(err)
                    );
                }
            } else {
                this.service.updatePackaging(mmp).subscribe((updated: MastProductsPackagingMaterial) => {
                    mmp.id = updated.id;
                },
                err => this.errorMessage = this.commonService.getError(err)
                );
            }
        }
    }

    getInputClass(mmp) {
        let result = 'form-control';
        if (mmp.packaging_id !== this.activePackagingId) {
            result += ' hidden';
        }
        return result;
    }

    getControlName(mmp: MastProductsPackagingMaterial) {
        return `${mmp.mast_id}_${mmp.material_id}_${mmp.packaging_id}`;
    }

    pageChanged(event: any) {
        this.page = event.page;
        if (this.page !== this.lastpage) {
            this.getProducts();
            this.lastpage = this.page;
        }

    }
}

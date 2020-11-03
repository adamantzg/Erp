import { Component, Input } from '@angular/core';
import { MastProduct, CustProduct, ProductFileType, ProductFile } from '../../domainclasses';
import { ProductFileService } from '../../services/productfile.service';

@Component({
    selector: 'app-productinfo',
    templateUrl: './productinfo.component.html',
    styleUrls: ['./productinfo.component.css']
})
export class ProductInfoComponent {

    mastProduct: MastProduct = null;
    custProduct: CustProduct = null;
    maxIndexes = {};

    constructor(private productFileService: ProductFileService) {

    }

    @Input()
    public get product(): any {
        return this.custProduct ? this.custProduct : this.mastProduct;
    }

    public set product(v: any) {
        if (v.mast_id) {
            this.mastProduct = v;
        } else {
            this.custProduct = v;
        }
    }

    @Input()
    types: ProductFileType[] = [];


    files(t: ProductFileType) {
        let result;
        if (this.mastProduct) {
            result = this.mastProduct.productFiles;
        } else {
            result = this.custProduct.productFiles;
        }
        return result.filter(f => f.type_id == t.id);
    }

    readOnlyFiles(t: ProductFileType) {
        return this.custProduct ? this.custProduct.mastProduct.productFiles.filter(f => f.type_id == t.id) : [];
    }

    onFileAdded(f: ProductFile) {
        let prodFiles;
        if (this.mastProduct) {
            f.mast_id = this.mastProduct.mast_id;
            prodFiles = this.mastProduct.productFiles;
        } else {
            f.cprod_id = this.custProduct.cprod_id;
            prodFiles = this.custProduct.productFiles;
        }
        const filesForType = prodFiles.filter(pf => pf.type_id == f.type_id);
        if (!(f.type_id in this.maxIndexes)) {
            this.maxIndexes[f.type_id] = filesForType.length > 0 ?
                Math.max.apply(Math, filesForType.map(pf => pf.order_index)) + 1  : 1;
        } else {
            this.maxIndexes[f.type_id] += 1;
        }
        f.order_index = this.maxIndexes[f.type_id];

        this.productFileService.create(f).subscribe( (data: ProductFile) => {
            f.id = data.id;
            f.file_name = data.file_name;
            prodFiles.push(f);
        });
    }

    onFileRemoved(f) {
        const productFiles = this.mastProduct ? this.mastProduct.productFiles : this.custProduct.productFiles;
        const index = productFiles.findIndex(pf => (f.file_id && pf.file_id == f.file_id) || pf.id == f.id);
        if (index >= 0) {
            if (f.id) {
                this.productFileService.delete(f.id).subscribe(() => {});
            }
            productFiles.splice(index, 1);
        }
    }

    onFileUpdated(f) {
        this.productFileService.update(f).subscribe(() => {});
    }

    filteredTypes(){
        if (this.custProduct) {
            return this.types;
        }
        return this.types.filter(t => !t.client_specific);
    }

    exportpdf() {
        window.open('/product/ProductSpecificationPdf/?' + (this.mastProduct ? 'mast_id=' + this.mastProduct.mast_id : 'cprod_id=' + this.custProduct.cprod_id));
    }

}


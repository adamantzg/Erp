import { Component, EventEmitter, Input, Output } from '@angular/core';

import { GridDefinition, GridColumn, GridColumnType, HeaderCheckboxEventData, FileType, FilesColumnComponent,
    FileColumnDeleteEventData } from '../../../common';
import { ProductDto } from './ProductDto';
import { ProductService } from '../../services/product.service';

@Component({
    selector: 'app-certproductlist',
    templateUrl: './productlist.component.html',
    styleUrls: ['./productlist.component.css']
  })
  export class ProductListComponent {

    constructor(private productService: ProductService) {

    }

    @Input()
    public set products(data: ProductDto[]) {
        this._products = data;
        this.getFilteredProducts();
    }
    private _products: ProductDto[] = [];

    headerData = {
        selectAll: false
    };
    @Output()
    HeaderCheckboxClicked = new EventEmitter();
    @Output()
    FileRemoved = new EventEmitter();
    @Input()
    showFilter = true;
    @Output()
    FilterApplied = new EventEmitter();

    productFilter = '';
    filteredProducts: ProductDto[] = [];


    gridDefinition = new GridDefinition([
        new GridColumn('', 'selected', GridColumnType.Checkbox, '', {width: '30px'}, null, null, null, true, 'selectAll'),
        new GridColumn('Ref', 'code', GridColumnType.Label, '', {width: '150px'}, null, null, null, null, null, null, 'childCodes'),
        new GridColumn('Name', 'name'),
        new GridColumn('Cert?', 'otherFiles', GridColumnType.Custom, 'cert', {width: '100px'}, null, FilesColumnComponent, {file_type: FileType.certificate} )
    ], true, true);

    onHeaderCheckBoxClicked(eventData: HeaderCheckboxEventData) {
        // this.HeaderCheckboxClicked.emit(eventData.checked);
        for (let i = 0; i < this.filteredProducts.length; i++) {
            this.filteredProducts[i].selected = eventData.checked ;
        }
    }

    onColumnClicked(eventData: FileColumnDeleteEventData) {
        this.FileRemoved.emit(new ProductListEventData(eventData.row.id, eventData.file.id));
    }

    getFilteredProducts() {
        if (this.productFilter.length > 0) {
            const term = this.productFilter.toLowerCase();
            this.filteredProducts = this._products.filter(p => p.code.toLowerCase().indexOf(term) >= 0 ||
            p.name.toLowerCase().indexOf(term) >= 0 || (p.childProducts != null &&
                    p.childProducts.find(cp => cp.code.toLowerCase().indexOf(term) >= 0)) );
        } else {
            this.filteredProducts = this._products;
        }
        this.FilterApplied.emit(this.filteredProducts);
    }

    getSelectedCount() {
        return this.filteredProducts.filter(p => p.selected).length;
    }
  }

  export class ProductListEventData {
      productId: number;
      fileId: number;

      constructor(productId: number, fileId: number) {
          this.productId = productId;
          this.fileId = fileId;
      }
  }

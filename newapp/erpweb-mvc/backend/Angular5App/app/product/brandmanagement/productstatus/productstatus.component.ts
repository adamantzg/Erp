import { Component, OnInit, IterableDiffer, Input } from '@angular/core';
import { CustProduct, CustProductsExtradata } from '../../domainclasses';
import { ProductService } from '../../services/product.service';
import { CommonService } from '../../../common';

@Component({
  selector: 'app-productstatus',
  templateUrl: './productstatus.component.html',
  styleUrls: ['./productstatus.component.css']
})
export class ProductstatusComponent implements OnInit {

  constructor(private productService: ProductService, private commonService: CommonService) { }

  @Input()
  products: CustProduct[] = [];
  errorMessage = '';


  ngOnInit() {
  }

  checkUpdate(p: CustProduct) {

    const data: any  = {
        cprod_id: p.cprod_id,
        extraData: Object.assign({}, p.extraData)
    };

    this.productService.updateProductData(data).subscribe(
        () => {
        },
        err => this.errorMessage = this.commonService.getError(err)
    );


  }

}

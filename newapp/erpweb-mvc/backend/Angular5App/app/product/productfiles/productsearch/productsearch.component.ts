import { Component, OnInit, EventEmitter, Input, Output } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { TypeaheadMatch } from 'ngx-bootstrap';
import { Observable } from 'rxjs/Observable';
import { MastProduct, CustProduct } from '../../domainclasses';
import { mergeMap } from 'rxjs/operators';
import { BlockUIService } from '../../../common';


@Component({
    selector: 'app-productsearch',
    templateUrl: './productsearch.component.html',
    styleUrls: ['./productsearch.component.css']
})
export class ProductSearchComponent {

    constructor(private productService: ProductService, private blockuiService: BlockUIService) {
        this.products = Observable.create((observer: any) => {
            // Runs on every search
            observer.next(this.code);
          }).pipe(mergeMap((code: string) => this.productService.searchMastProducts(code)));
    }

    products: Observable<MastProduct[]>;
    code = '';

    @Input()
    label = 'Find product';

    @Output()
    productSelected = new EventEmitter();

    onProductSelected(e: TypeaheadMatch) {
        this.productSelected.emit(e.item);
    }


}


import { Component, OnInit, EventEmitter } from '@angular/core';
import { ProductService } from '../services/product.service';
import { TypeaheadMatch } from 'ngx-bootstrap';
import { Observable } from 'rxjs/Observable';
import { Product } from '../domainclasses';
import { mergeMap } from 'rxjs/operators';

@Component({
    selector: 'app-spare',
    templateUrl: './spare.component.html',
    styleUrls: ['./spare.component.css']
  }
  )
  export class SpareComponent implements OnInit {
        code= '';
      product: Product;
      products: Observable<Product[]>;
      productsOrSpares:Product[];
      loadingMessage='Loading products...';
      loadingProducts = false;

      constructor(private productService: ProductService){
        this.products = Observable.create((observer: any) => {
            // Runs on every search
            observer.next(this.code);
          }).pipe(mergeMap((code: string) => this.productService.searchSpare(code)));
    }
    ngOnInit() {
        
    }
    onProductSelected(e: TypeaheadMatch) {
        this.product = e.item;
        this.code = this.product.cprod_code1;
        //this.showStock = false;
       // this.prices = null;
        //this.errorMessage = '';
       // this.showStockAndPrice(this.code);
       this.showProductSpares(this.code)
      }
      clickSearchProductSpares(code:string){
        this.productService.getProductsSpares(code).
          subscribe(data=>{
            console.log("DATA");
              console.log(data);
              this.productsOrSpares = data;
          });
      }
      
      showProductSpares(code:string){
      this.productService.getProductsSpares(this.product.cprod_code1).
        subscribe(data=>{
          console.log("DATA");
            console.log(data);
            this.productsOrSpares = data;
        });
    }

    getDescription() {
        if (this.productsOrSpares && this.productsOrSpares.length > 0) {
            return this.productsOrSpares[0].isSpare ? 'spares' : 'products';
        }
    }
  }
 
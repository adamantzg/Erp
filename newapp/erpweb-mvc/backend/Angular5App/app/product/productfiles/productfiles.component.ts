import { Component, OnInit, EventEmitter, Input, Output } from '@angular/core';
import { MastProduct, CustProduct } from '../domainclasses';
import { ProductService } from '../services/product.service';
import { ProductFileService } from '../services/productfile.service';
import { CompanyService, GridDefinition, GridColumn, GridColumnType, GridButtonEventData } from '../../common';

@Component({
    selector: 'app-productfiles',
    templateUrl: './productfiles.component.html',
    styleUrls: ['./productfiles.component.css']
})
export class ProductFilesComponent implements OnInit {
    

    constructor(private productService: ProductService,
        private productFileService: ProductFileService,
        private companyService: CompanyService) {

    }

    mastProduct: MastProduct = null;
    types = [];
    tabs = [];
    factories = [];
    factoryId = null;
    products: MastProduct[] = [];
    filteredProducts: MastProduct[] = [];
    gridStyle = {height: '300px'};
    productsHidden = false;
    lastFactoryId = null;

    gridDefinition = new GridDefinition([
        new GridColumn("Code", "factory_ref",GridColumnType.Label,"factory_ref",{width: '350px'}),
        new GridColumn("Name", "asaq_name"),
        new GridColumn("Select", null, GridColumnType.Button, "select", {width: '100px'},null,null,null,false)
    ], true, false);

    ngOnInit() {
        this.productFileService.getTypes().subscribe(data => this.types = data);
        this.companyService.getFactoriesForUser().subscribe(data => this.factories = data);
    }

    onProductSelected(product) {
        this.productService.getMastProduct(product.mast_id).subscribe(
            data => {
                this.mastProduct = data;
                this.tabs = [{title: 'Default', product: this.mastProduct}]
                if(data.custProducts != null && data.custProducts.length > 0) {
                    this.companyService.getByIds(data.custProducts.map(p => p.cprod_user).join(',')).subscribe(
                        clients => {
                            for (let p of this.mastProduct.custProducts) {
                                p.mastProduct = this.mastProduct;
                                if (p.cprod_status !== 'D') {
                                    p.client = clients.find(c => c.user_id == p.cprod_user);
                                    if (p.client != null) {
                                        this.tabs.push({ title: p.client.customer_code, product: p});
                                    }
                                }
                            }
                        }
                    );
                }                
            }
        );
    }

    showProducts() {
        if(this.lastFactoryId == null || this.lastFactoryId != this.factoryId) {
            this.lastFactoryId = this.factoryId;
            this.productService.getMastProductByCriteria(this.factoryId,null,null, false, null, null,false, false).subscribe(
                data => {
                    this.products = data;
                }
            );
        }
        this.productsHidden = false;
    }

    onGridButtonClicked(data: GridButtonEventData) {
        if(data.name == 'select') {
            this.onProductSelected(data.row);
        }
    }

    hideProducts() {
        this.productsHidden = true;
    }
}

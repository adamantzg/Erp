import { Component, OnInit } from '@angular/core';
import { BrandService } from '../../services/brand.service';
import { Brand, BrandCategory, CustProduct, Category1, MastProduct } from '../../domainclasses';
import { CommonService, Company, CompanyService } from '../../../common';
import { ProductService } from '../../services/product.service';
import { BsModalService } from 'ngx-bootstrap';
import { AddProductsModalComponent, AddProductsModalOkEventArgs } from '../addproductsmodal/addproductsmodal.component';
import { forkJoin} from 'rxjs';

@Component({
  selector: 'app-brandmanagementmain',
  templateUrl: './brandmanagementmain.component.html',
  styleUrls: ['./brandmanagementmain.component.css']
})
export class BrandManagementMainComponent implements OnInit {

  constructor(private brandService: BrandService,
    private commonService: CommonService, private productService: ProductService,
    private bsModalService: BsModalService, private companyService: CompanyService) { }


  brands: Brand[] = [];
  errorMessage = '';
  selectedBrandId = 2; // Burlington
  liveDisc = [ { value: 'N', name: 'Live'}, {value: 'D', name: 'Discontinued'}];
  liveDiscSelected = 'N';
  brandCategories: BrandCategory[] = [];
  selectedCategoryId = null;
  products: CustProduct[] = [];
  pageSize = 100;
  page = 1;
  lastPage = null;
  allProducts: CustProduct[] = [];
  asproot = '';
  searchText = '';
  categories: Category1[] = [];
  factories: Company[] = [];

  ngOnInit() {
      this.brandService.getAll().subscribe(
            data => {
                this.brands = data;
                this.onBrandChanged(this.selectedBrandId);
            },
            err => this.errorMessage = this.commonService.getError(err)
      );
      this.commonService.getSetting('aspsite_root').subscribe(
          data => this.asproot = data
      );
      /*this.companyService.getFactories().subscribe(
          data => this.factories = data
      );
      this.productService.getCategory1List().subscribe(
          data => this.categories = data
      );*/
  }

  onBrandChanged(brand_id) {
    /*const brandCatObs = this.productService.getBrandCategories(brand_id);
    brandCatObs.subscribe(
        data => {
            const allCat = new BrandCategory();
            allCat.brand_cat_id = null;
            allCat.brand_cat_desc = 'All';
            data.splice(0, 0, allCat );
            this.brandCategories = data;
            this.loadProducts();
        },
        err  => this.errorMessage = this.commonService.getError(err)
    );*/
    /*const category1Obs = this.productService.getCategoriesByBrand(brand_id);
    category1Obs.subscribe(
        data => {
            const allCat = new Category1();
            allCat.category1_id = null;
            allCat.cat1_name = 'All';
            data.splice(0, 0, allCat );
            this.categories = data;
            // this.loadProducts();
        },
        err  => this.errorMessage = this.commonService.getError(err)
    );
    const factoryObs = this.productService.getFactoriesByCriteria(brand_id, null);*/

    // forkJoin(brandCatObs, category1Obs).subscribe(() => this.loadProducts());
    this.productService.getBrandCategories(brand_id).subscribe(
        data => {
            const allCat = new BrandCategory();
            allCat.brand_cat_id = null;
            allCat.brand_cat_desc = 'All';
            data.splice(0, 0, allCat );
            this.brandCategories = data;
            this.loadProducts();
        },
        err  => this.errorMessage = this.commonService.getError(err)
    );
  }

  onCategoryChanged(cat_id) {
      this.loadProducts();
  }

  onLiveDiscChanged(value) {
      this.loadProducts();
  }

  loadProducts() {
    const brand = this.brands.find(b => b.brand_id == this.selectedBrandId);
    const brand_userid = brand.user_id;
    this.productService.searchProducts(null, null, null, this.liveDiscSelected, this.selectedCategoryId, brand_userid, this.searchText).subscribe(
        data => {
            this.handleMastProducts(data);
            this.allProducts = data;
            this.page = 1;
            this.paginate();
        },
        err => this.errorMessage = this.commonService.getError(err)
    );
  }

  handleMastProducts(products: CustProduct[]) {
      // Same mastproducts should be the same object (reference)
      const dictMastProducts = {};
      for (let p of products) {
        if (!(p.cprod_mast in dictMastProducts)) {
            dictMastProducts[p.cprod_mast] = p.mastProduct;
        } else {
            p.mastProduct = dictMastProducts[p.cprod_mast];
        }
      }
  }

  pageChanged(event: any) {
    this.page = event.page;
    if (this.page !== this.lastPage) {
        this.paginate();
        this.lastPage = this.page;
    }
  }

  paginate() {
      const start = (this.page - 1) * this.pageSize;
      this.products = this.allProducts.slice(start, start + this.pageSize - 1);
  }

  addProducts() {
    const modal = this.bsModalService.show(AddProductsModalComponent,
        Object.assign({}, { class: 'modal-lg' }));
    /*modal.content.categories = this.categories;
    modal.content.factories = this.factories;*/
    modal.content.brand_category_id = this.selectedCategoryId;
    modal.content.brandCategories = this.brandCategories;
    modal.content.brands = this.brands;
    modal.content.brand_id = this.selectedBrandId;
    modal.content.onOk.subscribe((event: AddProductsModalOkEventArgs) => {
        this.productService.addProducts(this.selectedBrandId, this.selectedCategoryId, event.products.map(p => p.mast_id)).
        subscribe(
            (data: CustProduct[]) => {
                this.handleMastProducts(data);
                for (let d of data) {
                    d.mastProduct = event.products.find(mp => mp.mast_id == d.cprod_mast);
                    this.allProducts.push(d);
                    this.products.push(d);
                }
                // this.paginate();
            },
            err => this.errorMessage = this.commonService.getError(err)
        );
      });
  }

}

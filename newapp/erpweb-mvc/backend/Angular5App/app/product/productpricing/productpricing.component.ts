import { Component, OnInit } from '@angular/core';
import { CustProduct } from '../domainclasses';
import { CompanyService, Company, CommonService } from '../../common';
import { ProductService } from '../services/product.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-productpricing',
  templateUrl: './productpricing.component.html',
  styleUrls: ['./productpricing.component.css']
})
export class ProductpricingComponent implements OnInit {

  constructor(private companyService: CompanyService,
    private productService: ProductService,
    private router: Router, private commonService: CommonService ) { }

  existingFilter: boolean;
  factories: Company[] = [];
  clients: Company[] = [];
  products: CustProduct[] = [];

  ngOnInit() {
    this.commonService.setBreadCrumb('Product pricing');
  }


  showFilter() {
    this.existingFilter = true;
  }

  newProject() {
    this.router.navigate(['/productpricing/new']);
  }



  pricingModels() {
    this.router.navigate(['productpricing/model']);
  }

  settings() {
    this.router.navigate(['productpricing/settings']);
  }

  freightCosts() {
    this.router.navigate(['productpricing/freightcost']);
  }

  markets() {
    this.router.navigate(['productpricing/markets']);
  }
}

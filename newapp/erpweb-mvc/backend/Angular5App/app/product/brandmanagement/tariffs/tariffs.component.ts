import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { CustProduct } from '../../domainclasses';
import { CommonService } from '../../../common';
import { Tariff } from '../../../common';
import { TariffService } from '../../services/tariff.service';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-producttariffs',
  templateUrl: './tariffs.component.html',
  styleUrls: ['./tariffs.component.css']
})
export class ProductTariffsComponent implements OnInit {

  constructor(private service: TariffService, private commonService: CommonService,
    private productService: ProductService) { }

  tariffs: Tariff[] = [];
  errorMessage = null;

  @Input()
  products: CustProduct[] = [];


  ngOnInit() {
    this.service.getAllTariffs().subscribe((data) => {
      this.tariffs = data;
    },
    (err) =>  this.errorMessage = this.commonService.getError(err)
    );
  }

  onChange(product: CustProduct) {
    this.productService.updateTariffCode(product.cprod_mast, product.mastProduct.tariff_code).subscribe(() => {
    },
    (err) => this.errorMessage = this.commonService.getError(err)
    );
  }
}

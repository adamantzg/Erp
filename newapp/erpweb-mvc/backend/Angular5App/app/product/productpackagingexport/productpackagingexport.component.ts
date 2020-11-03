import { Component, OnInit, Input } from '@angular/core';
import { ProductPackagingExportService } from '../services/productpackagingexport.service';
import { CommonService } from '../../common';
import { ProductPackagingService } from '../services/productpackaging.service';
import { ProductPackagingExportModel } from '../models';
import { saveAs } from 'file-saver';
import * as moment from 'moment';

@Component({
  selector: 'app-productpackagingexport',
  templateUrl: './productpackagingexport.component.html',
  styleUrls: ['./productpackagingexport.component.css']
})
export class ProductPackagingExportComponent implements OnInit {

  constructor(private service: ProductPackagingExportService, private productPackagingService: ProductPackagingService, private commonService: CommonService) {
  }

  model: ProductPackagingExportModel = new ProductPackagingExportModel();
  errorMessage = null;
  dateFormat = 'dd/MM/yyyy';

  filterParams = {
    factoryId: null,
    categoryId: null,
    clientId: null,
    etaetd: true,
    dateFrom: new Date(),
    dateTo: new Date()
  };

  onValueChange(value: Date): void {
    this.filterParams.dateFrom = value;
  }

  onValueChangeTo(value: Date): void {
    this.filterParams.dateTo = value;
  }

  ngOnInit() {
    this.productPackagingService.getModel(true).subscribe((data) => {
      this.model = data;
      this.filterParams.clientId = -1;
      this.filterParams.categoryId = -1;
      this.filterParams.factoryId = -1;
      this.filterParams.etaetd = true;
    },
    (err) => this.errorMessage = this.commonService.getError(err)
    );
  }

  getExport() {
    this.service.getByCriteria(this.filterParams.clientId, this.filterParams.factoryId, this.filterParams.categoryId,
                                  this.filterParams.etaetd, moment(this.filterParams.dateFrom).format('YYYY-MM-DD'), moment(this.filterParams.dateTo).format('YYYY-MM-DD'))
    .subscribe(data => {
      let file = new Blob([data], {type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'});
      // let fileURL = URL.createObjectURL(file);
      // window.open(fileURL);

      saveAs(file, 'materialsexport' + moment().format('DDMMYYYY') + '.xls', {type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'});
    },
    err => this.errorMessage = this.commonService.getError(err)
    );
  }

}

import { Component, OnInit } from '@angular/core';
import { ProductpricingService } from '../../services/productpricing.service';
import { Router, ActivatedRoute } from '@angular/router';
import { ProductPricingProject, ProductPricingModel, CustProduct } from '../../domainclasses';
import { Form } from '@angular/forms/src/directives/form_interface';
import { NgForm } from '@angular/forms/src/directives/ng_form';
import { ProductPricingProjectEditModel } from '../../models';
import { GridDefinition, GridColumn, GridColumnType, GridButtonEventData, CommonService, Currency } from '../../../common';

@Component({
  selector: 'app-pricingprojectedit',
  templateUrl: './pricingprojectedit.component.html',
  styleUrls: ['./pricingprojectedit.component.css']
})
export class PricingprojecteditComponent implements OnInit {

  constructor(private productPricingService: ProductpricingService,
  private commonService: CommonService,
  private router: Router,
  private route: ActivatedRoute ) { }

  title = 'New project';
  errorMessage = '';
  validationMessage = '';
  showValidation = false;
  updateButtonText = 'Create';
  editModel: ProductPricingProjectEditModel = new ProductPricingProjectEditModel();
  projectProductsTableDef: GridDefinition;
  active = 'data';
  showFilters = false;

  ngOnInit() {
    const id = +this.route.snapshot.params.id;
    this.editModel.project = new ProductPricingProject();
    this.productPricingService.getProjectEditModel(id).subscribe(p => this.editModel = p,
      err => this.errorMessage = this.commonService.getError(err));
    if (id > 0) {
      this.updateButtonText = 'Update';
      this.title = 'Edit project';
    }

    this.projectProductsTableDef = new GridDefinition(
      [
        new GridColumn('Code', 'cprod_code1', GridColumnType.Label, '', { width: '100px' }),
        new GridColumn('Description', 'cprod_name'),
        new GridColumn('', '', GridColumnType.Button, 'edit', { width: '30px' }, 'fa fa-edit'),
        new GridColumn('', '', GridColumnType.Button, 'remove', { width: '30px' }, 'fa fa-remove')
      ],
      true, false, '40vh'
    );
    this.projectProductsTableDef.columns[2].buttonTooltip = 'Edit';
    this.projectProductsTableDef.columns[3].buttonTooltip = 'Remove';

    const tab = this.route.snapshot.params.tab;
    if (tab != null) {
      this.active = tab;
    }
    this.commonService.setBreadCrumb('Product pricing');

  }

  back() {
    this.router.navigate(['/productpricing']);
  }

  update(editForm: NgForm) {
    if (editForm.valid) {
      const data = Object.assign({}, this.editModel.project);
      const isNew = data.id <= 0;
      this.productPricingService.createOrUpdateProject(data).subscribe(p => {
        this.editModel.project = p;
        if (isNew) {
          this.router.navigate(['/productpricing/editproject/' + p.id]);
        }

      }, err => this.errorMessage = this.commonService.getError(err));
    } else {
      this.showValidation = true;
      this.validationMessage = 'Enter all required data before proceeding.';
    }
  }

  onAddSelected(products: CustProduct[]) {
    products.forEach(p => {
      if (this.editModel.project.products.find(prod => prod.cprod_id === p.cprod_id) == null) {
        this.editModel.project.products.push(p);
      }
    });
  }

  onSelectedProductButton(button: GridButtonEventData) {
    if (button.name === 'remove') {
      const index = this.editModel.project.products.findIndex(p => p.cprod_id === button.row.cprod_id);
      if (index >= 0) {
        this.editModel.project.products.splice(index, 1);
      }
    } else if (button.name === 'edit') {
      this.router.navigate([`productpricing/editproduct/${button.row.cprod_id}`, { project: this.editModel.project.id}]);
    }
  }

  addNewProduct() {
    this.router.navigate([`productpricing/newproduct`, { project: this.editModel.project.id}]);
  }

  addExisting() {
    this.showFilters = true;
  }

}

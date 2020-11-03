import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppComponent } from './app.component';
import {BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { PricingprojecteditComponent, PricingmodelComponent, FreightcostComponent,
  PricingsettingComponent, MarketComponent, ProducteditComponent, ProductpricingComponent } from './product';
// import { DealersUsComponent } from './us-dealer/us-dealer.component';
import { DealersComponent } from './dealer/index';
import { DealerEditComponent } from './dealer/components/dealer-edit/dealer-edit.component';
import { OrderextradataComponent } from './order/orderextradata/orderextradata.component';
import { ProductPackagingComponent } from './product/productpackaging/productpackaging.component';
import { FormListComponent, FormFillComponent, FormResultsComponent } from './onlineforms/index';
import { CertificatesComponent } from './product/certificates/certificates.component';
import { ProductPackagingExportComponent } from './product/productpackagingexport/productpackagingexport.component';
import { SpareComponent } from './product/spares/spare.component';
import { ProductFilesComponent } from './product/productfiles/productfiles.component';
import { FactoryAssignmentComponent } from './company/factoryassignment/factoryassignment.component';
import { DocumentsListComponent } from './inspection/components/criteriadocuments/documentslist/documentslist.component';
import { DocumentsDetailComponent } from './inspection/components/criteriadocuments/documentsdetail/documentsdetail.component';
import { BrandManagementMainComponent } from './product/brandmanagement/brandmanagementmain/brandmanagementmain.component';
import { FactoryStockOrderListComponent } from './order/factorystockorder/factorystockorderlist/factorystockorderlist.component';
import { FactoryStockOrderEditComponent } from './order/factorystockorder/factorystockorderedit/factorystockorderedit.component';
// import { TestComponent } from './product/productpricing/pricingmodel/test.component';


const routes = [
  { path: '', component: AppComponent},
  { path: 'productpricing', component: ProductpricingComponent},
  { path: 'productpricing/newproject', component: PricingprojecteditComponent},
  { path: 'productpricing/editproject/:id', component: PricingprojecteditComponent},
  { path: 'productpricing/newproduct', component: ProducteditComponent},
  { path: 'productpricing/editproduct/:id', component: ProducteditComponent},
  { path: 'productpricing/model', component: PricingmodelComponent},
  // { path: 'productpricing/modeltest', component: TestComponent},
  { path: 'productpricing/settings', component: PricingsettingComponent },
  { path: 'productpricing/freightcost', component: FreightcostComponent},
  { path: 'productpricing/markets', component: MarketComponent},
  { path: 'dealers', component: DealersComponent},
  { path: 'dealers/create', component: DealerEditComponent},
  { path: 'dealers/edit/:id', component: DealerEditComponent},
  { path: 'order/orderextradata', component: OrderextradataComponent},
  { path: 'productpackaging', component: ProductPackagingComponent},
  { path: 'forms', component: FormListComponent },
  { path: 'forms/fill/:id', component: FormFillComponent},
  { path: 'forms/results/:name/:id', component: FormResultsComponent},
  { path: 'productcertificates', component: CertificatesComponent},
  { path: 'productpackagingexport', component: ProductPackagingExportComponent},
  { path: 'product/spares', component: SpareComponent},
  { path: 'product/productfiles', component: ProductFilesComponent},
  { path: 'company/factoryassignment', component: FactoryAssignmentComponent},
  { path: 'inspection/criteriadocuments', component: DocumentsListComponent},
  { path: 'inspection/criteriadocumentedit/:id', component: DocumentsDetailComponent},
  { path: 'product/brandmanagement', component: BrandManagementMainComponent},
  { path: 'factorystockorder', component: FactoryStockOrderListComponent},
  { path: 'factorystockorder/edit/:id', component: FactoryStockOrderEditComponent}
];

@NgModule({
  exports: [
    RouterModule
  ],
  imports: [
    RouterModule.forRoot(routes)
  ],
  declarations: []
})
export class AppRoutingModule {

}


import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {  FormsModule, ReactiveFormsModule } from '@angular/forms';
import { OrderModule } from 'ngx-order-pipe';
import { BsDatepickerModule, PaginationModule } from 'ngx-bootstrap';
import { ProductpricingComponent } from './productpricing/productpricing.component';
import { ProducteditComponent } from './productpricing/productedit.component';
import { PricingmodelComponent } from './productpricing/pricingmodel/pricingmodel.component';
import { FreightcostComponent } from './productpricing/freightcost/freightcost.component';
import { PricingsettingComponent } from './productpricing/pricingsetting/pricingsetting.component';
import { MarketComponent } from './productpricing/market/market.component';
import { SalesforecastComponent } from './salesforecast/salesforecast.component';
import { SalesforecastmodalComponent } from './salesforecast/salesforecastmodal.component';
import { PricingprojectComponent } from './productpricing/pricingproject/pricingproject.component';
import { PricingprojecteditComponent } from './productpricing/pricingproject/pricingprojectedit.component';
import { ProductselectorComponent } from './productselector/productselector.component';
import { ProductService } from './services/product.service';
import { ProductpricingService } from './services/productpricing.service';
import { CommonAppModule } from '../common/common.module';
import { NgxUploaderModule } from 'ngx-uploader';
import { ProgressbarModule, TabsModule, ModalModule, TypeaheadModule } from 'ngx-bootstrap';
import { ReportfilterComponent } from './productpricing/reportfilter/reportfilter.component';
import { ProductPackagingComponent } from './productpackaging/productpackaging.component';
import { ProductPackagingService } from './services/productpackaging.service';
import { CertificatesComponent } from './certificates/certificates.component';
import { MastProductSelectorComponent } from './mastproductSelector/mastproductSelector.component';
import { ProductListComponent } from './certificates/productlist/productlist.component';
import { FileSelectorComponent } from './certificates/fileselector/fileselector.component';
import { FactorylistComponent } from './certificates/factorylist/factorylist.component';
import { FileEditModalComponent } from './certificates/fileEdit/fileEditModal.Component';
import { ProductPackagingExportComponent } from './productpackagingexport/productpackagingexport.component';
import { SpareComponent } from './spares/spare.component';
import { ProductFilesComponent } from './productfiles/productfiles.component';
import { ProductSearchComponent } from './productfiles/productsearch/productsearch.component';
import { ProductInfoComponent } from './productfiles/productinfo/productinfo.component';
import { ProductFileService } from './services/productfile.service';
import { ProductFilesEditorComponent } from './productfiles/productfileseditor/productfileseditor.component';
import { LightboxModule } from 'ngx-lightbox';
import { BrandManagementMainComponent } from './brandmanagement/brandmanagementmain/brandmanagementmain.component';
import { BrandService } from './services/brand.service';
import { ProductTariffsComponent } from './brandmanagement/tariffs/tariffs.component';
import { BarcodesComponent } from './brandmanagement/barcodes/barcodes.component';
import { ProductBulkEditComponent } from './brandmanagement/productbulkedit/productbulkedit.component';
import { ProductLoadingEditComponent } from './brandmanagement/productloadingedit/productloadingedit.component';
import { AddProductsModalComponent } from './brandmanagement/addproductsmodal/addproductsmodal.component';
import { TariffService } from './services/tariff.service';
import { ProductstatusComponent } from './brandmanagement/productstatus/productstatus.component';

// import { SparesComponent } from './spares/spares.component';
// import { TestComponent } from './productpricing/pricingmodel/test.component';

@NgModule({
  imports: [
    CommonModule,
    CommonAppModule,
    FormsModule,
    OrderModule,
    ReactiveFormsModule,
    ModalModule.forRoot(),
    NgxUploaderModule,
    ProgressbarModule.forRoot(),
    TabsModule.forRoot(),
    BsDatepickerModule.forRoot(),
    PaginationModule.forRoot(),
    TypeaheadModule.forRoot(),
    LightboxModule
  ],
  declarations: [
    ProductpricingComponent,
    ProducteditComponent,
    PricingmodelComponent,
    FreightcostComponent,
    PricingsettingComponent,
    MarketComponent,
    SalesforecastComponent,
    SalesforecastmodalComponent,
    PricingprojectComponent,
    PricingprojecteditComponent,
    ProductselectorComponent,
    ReportfilterComponent,
    ProductPackagingComponent,
    CertificatesComponent,
    MastProductSelectorComponent,
    ProductListComponent,
    FileSelectorComponent,
    FactorylistComponent,
    FileEditModalComponent,
    ProductPackagingExportComponent,
   SpareComponent,
   ProductFilesComponent,
   ProductSearchComponent,
   ProductInfoComponent,
   ProductFilesEditorComponent,
   BrandManagementMainComponent,
   ProductTariffsComponent,
   BarcodesComponent,
   ProductBulkEditComponent,
   ProductLoadingEditComponent,
   AddProductsModalComponent,
   ProductstatusComponent

  ],
  exports: [
    ProductselectorComponent
  ],
  providers: [ProductService, ProductpricingService, ProductPackagingService, ProductFileService, BrandService, TariffService],
  entryComponents: [SalesforecastmodalComponent, ReportfilterComponent, FileEditModalComponent, AddProductsModalComponent]
})
export class ProductModule { }

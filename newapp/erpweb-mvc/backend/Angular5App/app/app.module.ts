import { BrowserModule } from '@angular/platform-browser';
import { NgModule} from '@angular/core';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { HttpClientModule } from '@angular/common/http';
import { OrderModule } from 'ngx-order-pipe';

import { ProductModule } from './product/product.module';
import { CommonAppModule } from './common/common.module';
import { DealerModule } from './dealer/dealer.module';
import {OrderModule as DataOrderModule} from './order/order.module';
import { OnlineFormsModule } from './onlineforms/onlineforms.module';
import { TabsModule, PaginationModule } from 'ngx-bootstrap';
import { CompanyModule } from './company/company.module';
import { InspectionModule } from './inspection/inspection.module';


@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    OrderModule,
    HttpClientModule,
    CommonAppModule,
    ProductModule,
    DealerModule,
    DataOrderModule,
    OnlineFormsModule,
    TabsModule.forRoot(),
    PaginationModule.forRoot(),
    CompanyModule,
    InspectionModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }

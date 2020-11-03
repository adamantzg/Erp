import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OrderextradataComponent } from './orderextradata/orderextradata.component';
import { OrderService } from './services/order.service';
import { PaginationModule, BsDatepickerModule, enGbLocale, TypeaheadModule } from 'ngx-bootstrap';
import { FactoryStockOrderListComponent } from './factorystockorder/factorystockorderlist/factorystockorderlist.component';
import { FactoryStockOrderService } from './services/factorystockorder.service';
import { RouterModule } from '@angular/router';
import { FactoryStockOrderEditComponent } from './factorystockorder/factorystockorderedit/factorystockorderedit.component';
import { CommonAppModule } from '../common/common.module';
import { FactoryStockNewOrderModalComponent } from './factorystockorder/factorystocknewordermodal/factorystocknewordermodal.component';
import { FactorystockorderComponent } from './factorystockorder/factorystockorder/factorystockorder.component';


@NgModule({
  imports: [
    CommonModule,
    CommonAppModule,
    FormsModule,
    RouterModule,
    BsDatepickerModule.forRoot(),
    PaginationModule.forRoot(),
    TypeaheadModule.forRoot()
  ],
  declarations: [OrderextradataComponent, FactoryStockOrderListComponent, FactoryStockOrderEditComponent, FactoryStockNewOrderModalComponent,
    FactorystockorderComponent],
  providers: [OrderService, FactoryStockOrderService],
  entryComponents: [FactoryStockNewOrderModalComponent]
})
export class OrderModule {

}

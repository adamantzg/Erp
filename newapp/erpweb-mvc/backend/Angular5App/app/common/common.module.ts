import { NgModule } from '@angular/core';
import { CommonModule, CurrencyPipe, DecimalPipe, DatePipe, PercentPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Company, Checkable, ContainerType, ContainerTypeValue, Currency, CurrencyCode, FreighCost
, Location, Lookup, Tariff } from './domainclasses';
import { CommonService } from './services/common.service';
import { HttpService } from './services/http.service';
import { BlockUIService } from './services/block-ui.service';
import { CompanyService } from './services/company.service';
import { OrderPipe } from 'ngx-order-pipe';
import { GridComponent, Month21inputComponent, PercentageinputComponent } from './components';
import { CustomcolumnComponent } from './components/grid/customcolumn/customcolumn.component';
import { ErrorMessageComponent } from './components/errorMessage/errorMessage';
import { FilesColumnComponent } from './components/filescolumn/filescolumn.component';
import { MessageboxService } from './messagebox/messagebox.service';
import { MessageboxComponent } from './messagebox/messagebox.component';
import { FileService } from './services/file.service';
import { UserService } from './services/user.service';
import { ColumnFormatPipe } from './components/grid/formatPipe';

@NgModule({
  imports: [
    CommonModule,
    FormsModule
  ],
  declarations: [
    GridComponent, Month21inputComponent, PercentageinputComponent, CustomcolumnComponent, ErrorMessageComponent,
    FilesColumnComponent, MessageboxComponent, ColumnFormatPipe
  ],
  exports: [
    GridComponent, Month21inputComponent, PercentageinputComponent, ErrorMessageComponent, FilesColumnComponent,
    MessageboxComponent
  ],
  providers: [CommonService, HttpService, BlockUIService, CompanyService, MessageboxService, FileService, UserService,
    CurrencyPipe, DecimalPipe, DatePipe, PercentPipe],
  entryComponents: [FilesColumnComponent, MessageboxComponent ]
})
export class CommonAppModule { }

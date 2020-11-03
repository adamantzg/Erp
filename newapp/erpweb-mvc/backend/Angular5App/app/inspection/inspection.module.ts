import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DocumentsListComponent } from './components/criteriadocuments/documentslist/documentslist.component';
import { DocumentsDetailComponent } from './components/criteriadocuments/documentsdetail/documentsdetail.component';
import { InstructionsNewService } from './services/instructionsnew.service';
import { CommonAppModule } from '../common/common.module';
import { ProductModule } from '../product/product.module';
import { NgxUploaderModule } from 'ngx-uploader';


@NgModule({
  declarations: [DocumentsListComponent, DocumentsDetailComponent],
  imports: [
    CommonModule,
    CommonAppModule,
    ProductModule,
    NgxUploaderModule
  ],
  providers: [InstructionsNewService]
})
export class InspectionModule { }

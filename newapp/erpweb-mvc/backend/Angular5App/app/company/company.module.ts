import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CommonAppModule } from '../common/common.module';
import { FactoryAssignmentComponent } from './factoryassignment/factoryassignment.component';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [FactoryAssignmentComponent],
  imports: [
    CommonModule,
    CommonAppModule,
    FormsModule
  ]  
})
export class CompanyModule { }

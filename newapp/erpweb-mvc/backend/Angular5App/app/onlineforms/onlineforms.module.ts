import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CommonAppModule } from '../common/common.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ModalModule, BsDatepickerModule } from 'ngx-bootstrap';
import { FormListComponent } from './components/formlist/formlist.component';
import { Router, RouterModule } from '@angular/router';
import { FormFillComponent } from './components/formfill/formfill.component';
import { FormResultsComponent } from './components/formresults/formresults.component';
import { OnlineFormService } from './services/onlineform.service';
import { FormSectionComponent } from './components/formsection/formsection.component';
import { QuestionInputComponent } from './components/question-input/question-input.component';
import { QuestionResultComponent } from './components/question-result/question-result.component';

@NgModule({
    imports: [
        CommonModule,
        CommonAppModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule,
        ModalModule.forRoot(),
        BsDatepickerModule.forRoot()
      ],
      declarations: [
          FormListComponent,
          FormFillComponent,
          FormResultsComponent,
          FormSectionComponent,
          QuestionInputComponent,
          QuestionResultComponent
      ],
      providers: [
          OnlineFormService
      ]
})

export class OnlineFormsModule { }

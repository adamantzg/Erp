import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

/********** NEDED FOR BREWSER LINK ***********************/
import {BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
/********************************************************/
// import { AgmCoreModule } from "@agm/core";
import { NguiMapModule } from "@ngui/map";
import { FormsModule } from '@angular/forms';

import { DealersComponent } from './components/dealers/dealers.component';
import { DealerService } from './services/dealer.service';
import { DealerFilterPipe } from './dealer-filter.pipe';
import { DealerEditComponent } from './components/dealer-edit/dealer-edit.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
// import { MatButtonModule, MatRadioModule } from "@angular/material";
import { Browser } from 'selenium-webdriver';
// import { ProgressbarModule, TabsModule, ModalModule } from 'ngx-bootstrap';
// import { PopoverModule } from 'ngx-bootstrap';
// import { Router } from '@angular/ router';

@NgModule({
  exports: [
    RouterModule
  ],
  imports: [
    CommonModule,
    FormsModule,
    BrowserModule,
    RouterModule,
    BrowserAnimationsModule,
    // MatButtonModule, MatRadioModule,
    // AgmCoreModule.forRoot({apiKey:'AIzaSyCleE5vbeb8HtYMso0gj5btzA2iD4li2nA'})
    NguiMapModule.forRoot({apiUrl: 'https://maps.google.com/maps/api/js?libraries=places&key=AIzaSyCleE5vbeb8HtYMso0gj5btzA2iD4li2nA'}),
    // PopoverModule.forRoot();
  ],
  declarations: [DealersComponent, DealerFilterPipe, DealerEditComponent],
  providers: [DealerService],
})
export class DealerModule { }

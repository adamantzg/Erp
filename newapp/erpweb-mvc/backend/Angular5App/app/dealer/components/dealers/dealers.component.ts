import { Component, OnInit, Input, ChangeDetectorRef } from '@angular/core';
import { Dealer } from '../../domainclasses';
import { DealerService } from '../../services/dealer.service';
// import {Router} from '@angular/router';
// import { DealerFilterPipe } from "../dealer-filter.pipe";

@Component({
  selector: 'dealers',
  templateUrl: './dealers.component.html',
  styleUrls: ['./dealers.component.css']
})
export class DealersComponent implements OnInit {

  dealers: Dealer[]= [];
  filtertext= '';
  constructor( private usDealerService: DealerService ) { }

  ngOnInit() {
    // let de= this.usDealerService.localDealer;
    console.log( 'PONOVO UČITAVAM: ', this.usDealerService.localStoredDealers.length, this.usDealerService.localStoredDealers);
    // disable style shit on index.cshtml
    document.styleSheets[4].disabled = true;
// if (this.usDealerService.localStoredDealers.length < 1) {
    this.usDealerService.getDealers().subscribe(
      dealers => this.dealers = dealers,
      err => console.error(`error in retriving dealers: ${ err }`),
      () => {
        console.log('Prije  spremanja');
        this.usDealerService.addToLocalDealers(this.dealers);
        console.log('Poslije spremanja: ' , this.usDealerService.getLocalDealers());
      }
    );
  // } else  {
  //   this.dealers = this.usDealerService.localStoredDealers;
  // }
   }

}

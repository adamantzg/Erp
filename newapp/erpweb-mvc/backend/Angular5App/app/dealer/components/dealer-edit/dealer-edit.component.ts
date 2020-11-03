import { Component, OnInit, Input, ChangeDetectorRef } from '@angular/core';
import { Location } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { Dealer } from '../../domainclasses';
import { DealerService } from '../../services/dealer.service';
import { GoogleMap } from '@agm/core/services/google-maps-types';
// import { Validator  } from "@angular/forms";


@Component({
  selector: 'app-dealer-edit',
  templateUrl: './dealer-edit.component.html',
  styleUrls: ['./dealer-edit.component.css']
})
export class DealerEditComponent implements OnInit {

  dealer: Dealer = new Dealer();
  createForm= false;
  idparam = +this.route.snapshot.paramMap.get('id');
  cssClass1 = 'flag-icon';
  cssClass2 = 'flag-icon-us';
  /************* MAP *********************************/
  mapsProps: any = {};
  allOptions = { center: { lat: 42.877742, lng: -97.380979 }, zoom: 4 };
  /************* END  MAP ******************************/
  address: any = {};
  test = 99;
  constructor(
    private dealerService: DealerService,
    private route: ActivatedRoute,
    private ref: ChangeDetectorRef,
    private location: Location,

  ) { }

  ngOnInit() {
    // document.styleSheets[5].disabled = true;
    /** FIX FOR SHOWING ICONS ON GOOGLE MAPS */
    /** MAX-WIDTH:NONE!IMPORTANT */
    document.styleSheets[4].disabled = true;
    if (this.idparam > 0 ) {
      this.getDealer();
    } else {
      this.createForm = true;
    }
  }
  setClasses() {
    if (!!this.dealer.user_country) {
      return `flag-icon flag-icon-${this.dealer.user_country.toLowerCase()}`;
    }    else {
      return '';
    }
  }
  getDealer(): void {
    this.dealerService.getDealer(this.idparam).subscribe(
      d => this.dealer = d,
      err => console.error('error error'),
      () => (
        console.log('CLASS: ', this.dealer.user_country),
        this.setClasses(),
        this.allOptions = {
          center: { lat: +this.dealer.latitude, lng: +this.dealer.longitude },
          zoom: 11
        }
      )
    );
  }

  save() {
      this.dealerService.saveDealer(this.dealer).subscribe(
        d => console.log('Saved: ', d),
        err => console.error('SAVING ERROR: ', err),
        () => this.goBack()
      );
  }
  goBack() {
    this.location.back();
  }
  refresh(): void {
    this.allOptions = {
      center: { lat: +this.dealer.latitude, lng: +this.dealer.longitude },
      zoom: 11
    };

  }

  /** Postavljanje oba pritisak na tipku enter(onEnter) i napuštanje input-a (onBlur)
   * kad sam testirao sam alert kad su oba aktivna stalno se pasi alert kad ka ugasiš
   * u beskonačnoj petlji
   ** FIXED onBlur and onEnter **
  */
  onClickMap(event) {
    const ev = event.latLng;
    // var lat =  typeof ev ==='undefined';
    if (typeof ev !== 'undefined') {
      this.dealer.latitude = ev.lat();
      this.dealer.longitude = ev.lng();
      this.allOptions.center = {
        lat: ev.lat(), lng: ev.lng()
      };
      event.target.panTo(event.latLng);
    }
  }
  onEnterRefreshMap($event) {
    $event.target.blur();
    $event.preventDefault();
  }
  refreshMap($event): void {
    // alert(`Refresh Map ${this.dealer.latitude}  ${this.dealer.longitude}`);
    $event.preventDefault();
    this.refresh();
  }
  placeChanged(place) {
    this.allOptions = {
      center: place.geometry.location,
      zoom: 11
    };
    for (let i = 0; i < place.address_components.length; i++) {

      const addressType = place.address_components[i].types[0];
      if (addressType === 'route') {
        this.address[addressType] = place.address_components[i].long_name;
      }      else {
        this.address[addressType] = place.address_components[i].short_name;
      }
    }
    console.log('NOVE KOORDINATE: ', place.geometry.location.lat());
    this.dealer.latitude = place.geometry.location.lat();
    this.dealer.longitude = place.geometry.location.lng();
    this.dealer.postcode = this.address['postal_code'];
    this.dealer.user_address1 = `${!this.address['street_number'] ? '' : this.address['street_number']} ${!this.address['route'] ? '' : this.address['route']}`;
    this.dealer.user_address2 = `${!this.address['locality'] ? '' : this.address['locality']}`;
    this.dealer.user_address3 = `${!this.address['administrative_area_level_2'] ? '' : this.address['administrative_area_level_2']}`;
    this.dealer.user_address4 = `${!this.address['administrative_area_level_1'] ? '' : this.address['administrative_area_level_1']}`;
    if (!!this.address['country']) {
      this.dealer.user_country = `${this.address['country']}`;
    }
    this.ref.detectChanges();
    // this.dealer.postcode=this.address['postal_code'];


  }

}


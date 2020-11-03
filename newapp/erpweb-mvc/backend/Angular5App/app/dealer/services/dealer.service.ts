import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { HttpService, } from '../../common';
// import { HttpService } from '../http.service';
import { Dealer } from '../domainclasses';
import { Settings } from '../settings';

@Injectable()
export class DealerService {

  constructor(private httpService: HttpService) { }
  api = Settings.apiRoot + 'dealers';
  localStoredDealers: Dealer []= [];

  getDealers(): Observable<Dealer[]> {
    return this.httpService.get<Dealer[]>(`${this.api}/getAll`, null);
  }
  getDealer(id): Observable<Dealer> {
    return this.httpService.get<Dealer>(`${this.api}/getDealer`, { params: {id: id}});
  }
  saveDealer(dealer): Observable<Dealer> {
    return this.httpService.post(`${this.api}/updateDealer`, dealer);
  }

  addToLocalDealers(dealers) {
    this.localStoredDealers = dealers;
  }
  getLocalDealers() {
    return this.localStoredDealers;
  }
}

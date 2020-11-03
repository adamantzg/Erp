import { Injectable } from '@angular/core';
import { HttpService, Currency, CurrencyCode, ContainerTypeValue } from '../../common';
import { Settings } from '../settings';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { of } from 'rxjs/observable/of';
import { InstructionsNew } from '../domainclasses';


@Injectable()
export class InstructionsNewService {

    constructor(private httpService: HttpService) { }

    api = Settings.apiRoot + 'instructions/';

    getAll(kind_id: number) {
        return this.httpService.get(this.api + 'getAll', { params: { kind_id: kind_id}});
    }

    getModel(id: number) {
        return this.httpService.get(this.api + 'getModel', { params: {id: id }});
    }

    create(data: InstructionsNew) {
        return this.httpService.post(this.api + 'create', data);
    }

    update(data: InstructionsNew) {
        return this.httpService.post(this.api + 'update', data);
    }

    delete(id: number) {
        return this.httpService.delete(this.api + 'delete?id=' + id);
    }

}

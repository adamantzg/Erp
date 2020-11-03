import { Injectable } from '@angular/core';
import { HttpService } from './http.service';
import { Observable } from 'rxjs/Observable';
import { File } from '../domainclasses';
import { Settings } from '../settings';

@Injectable()
export class FileService {

    constructor(private httpService: HttpService) { }
    api = Settings.apiRoot + 'file/';

    getFiles(type_id: number) {
        return this.httpService.get(this.api + 'getFiles', {params: {type_id: type_id}});
    }

    getFilesForCompanies(type_id: number) {
        return this.httpService.get(this.api + 'getFilesForCompanies', {params: {type_id: type_id}});
    }

    getFilesForMastProducts(type_id: number) {
        return this.httpService.get(this.api + 'getFilesForMastProducts', {params: {type_id: type_id}});
    }

    deleteFile(id: number) {
        return this.httpService.delete(this.api + 'delete?id=' + id);
    }

    updateFile(file: File) {
        return this.httpService.put(this.api + 'update', file);
    }



}

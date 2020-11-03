import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http/src/response';
import { Company } from '../domainclasses';
import { HttpService } from './http.service';

@Injectable()
export class CommonService {

  constructor(private httpService: HttpService) { }

  getError(err: HttpErrorResponse): string {
    if (err.error instanceof Error) {
      return err.error.message;
    }
    if (typeof(err.error) === 'string') {
      return err.error;
    }
    return err.message;
  }

  deepClone(obj: any): any {
    return JSON.parse(JSON.stringify(obj));
  }

  getUploadUrl() {
    return 'api/uploadImage';
  }

  getUploadFileUrl() {
      return 'api/uploadFile';
  }

  getTempUrl() {
    return 'api/getTempUrl';
  }

  uploadFile(url, file) {
    const formData: FormData = new FormData();
    formData.append('file', file, file.name);

    return this.httpService.post(url, formData);
  }

  getSetting(name: string) {
      return this.httpService.get('api/getSetting', { params: {name: name}});
  }


  setBreadCrumb(text) {
    const crumbs = document.getElementsByClassName('breadcrumb-item');
    if (crumbs != null && crumbs.length > 0) {
        crumbs[crumbs.length - 1].innerHTML = text;
    }
    document.title = 'Big company - ' + text;
}

 

}

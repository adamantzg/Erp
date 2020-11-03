import { Injectable } from '@angular/core';
import { HttpService } from './http.service';
import { Observable } from 'rxjs/Observable';
import { Company, AdminPermission } from '../domainclasses';
import { Settings } from '../settings';

@Injectable()
export class UserService {

  constructor(private httpService: HttpService) { }
  api = Settings.apiRoot + 'user/';

  getInspectors(location_id: number, includeAdminPermissions) {
      return this.httpService.get(this.api + 'getInspectors', { params: { location_id: location_id, includeAdminPermissions: includeAdminPermissions}});
  }

  addPermission(permission: AdminPermission) {
    return this.httpService.postNoBlock(this.api + 'addPermission', permission);
  }

  removePermission(permissionId: number) {
    return this.httpService.postNoBlock(this.api + 'removePermission?id=' + permissionId, null);
  }

  isCurrentUserQC() {
    return this.httpService.postNoBlock(this.api + 'isCurrentUserQC', null);
  }
}

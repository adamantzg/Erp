import { Component, OnInit } from '@angular/core';
import { CompanyService, Company, CommonService, User, GridDefinition, GridColumn, GridColumnType, GridButtonEventData, AdminPermission } from '../../common';
import { UserService } from '../../common/services/user.service';
import { SelectableUser } from '../modelclasses';

@Component({
  selector: 'app-factoryassignment',
  templateUrl: './factoryassignment.component.html',
  styleUrls: ['./factoryassignment.component.css']
})
export class FactoryAssignmentComponent implements OnInit {

  constructor(private companyService: CompanyService, 
    private userService: UserService, private commonService: CommonService) { }

  factoryId = null;
  factories: Company[] = [];
  dictUsers = {};
  users: SelectableUser[] = [];
  errorMessage = '';
  useAllLocations = false;

  gridDefinition = new GridDefinition([
      new GridColumn("Name", "userwelcome"),
      new GridColumn("Initials", "user_initials"),
      new GridColumn("Select", "checked", GridColumnType.Checkbox, "checked")
  ]);


  ngOnInit() {
      this.companyService.getFactoriesForUser().subscribe(data => this.factories = data);
  }

  factorySelected() {
    const factory = this.factories.find(f => f.user_id == this.factoryId);
    if (factory != null) {
        // Exception for location 4
        const location_id = factory.consolidated_port != 4 ? factory.consolidated_port : 2;
        if (!(location_id in this.dictUsers)) {
            this.userService.getInspectors(!this.useAllLocations ? location_id : null, true).subscribe(
                data => {
                    this.dictUsers[location_id] = this.transformUsers(data);
                    this.users = this.dictUsers[location_id];
                },
                err => this.errorMessage =  this.commonService.getError(err)
            );
        } else {
            this.users = this.dictUsers[location_id];
            if(!this.useAllLocations) {
                this.users = this.users.filter(u => u.consolidated_port == location_id);
            }
            for (let i = 0; i < this.users.length; i++) {
                const user = this.users[i];
                user.checked = user.adminPermissions.find(p => p.cusid == this.factoryId) != null;
            }
        }
    }
  }

  transformUsers(users: User[]) {
      const result = [];
      for (let i = 0; i < users.length; i++) {
          const user = users[i];
          const item = new SelectableUser();
          item.userid = user.userid;
          item.userwelcome = user.userwelcome;
          item.user_initials = user.user_initials;
          item.adminPermissions = user.adminPermissions;
          item.consolidated_port = user.consolidated_port;
          item.checked = user.adminPermissions.find(p => p.cusid == this.factoryId) != null;
          result.push(item);
      }
      return result;
  }

  onCheckBoxClicked(data: GridButtonEventData) {
      const checked = data.row.checked;
      if(checked) {
          const permission = new AdminPermission();
          permission.cusid = this.factoryId;
          permission.userid = data.row.userid;
          permission.feedbacks = 0;
          permission.processing = 0;
          permission.returns = 0;
          this.userService.addPermission(permission).subscribe(
              perm => {
                 data.row.adminPermissions.push(perm);
              }
          )
      } else {
          const index = data.row.adminPermissions.findIndex(p => p.cusid == this.factoryId);
            if(index >= 0) {
                const id = data.row.adminPermissions[index].permission_id;
                this.userService.removePermission(id).subscribe(() => {
                    data.row.adminPermissions.splice(index,1);
                })
            }
      }
  }

  onUseAllLocationsChanged() {
    const factory = this.factories.find(f => f.user_id == this.factoryId);
    if (this.useAllLocations) {
        delete this.dictUsers[factory.consolidated_port];
    }
    this.factorySelected();

  }



}

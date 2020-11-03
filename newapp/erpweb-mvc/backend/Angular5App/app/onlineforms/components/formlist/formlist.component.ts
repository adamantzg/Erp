import { Component, OnInit } from '@angular/core';
import { OnlineFormService } from '../../services/onlineform.service';
import { CommonService } from '../../../common';

@Component({
  selector: 'app-formlist',
  templateUrl: './formlist.component.html',
  styleUrls: ['./formlist.component.css']
})
export class FormListComponent implements OnInit {

  constructor(private formService: OnlineFormService, private commonService: CommonService) { }

  forms = [];
  errorMessage = '';

  ngOnInit() {
      this.formService.getAll().subscribe((data: any) => {
          this.forms = data;
        },
        (err) => this.errorMessage = this.commonService.getError(err)
        );
  }

}

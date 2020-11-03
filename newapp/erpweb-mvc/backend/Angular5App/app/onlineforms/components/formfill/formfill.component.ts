import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { OnlineFormService } from '../../services/onlineform.service';
import { CommonService } from '../../../common';
import { Form } from '../../domainclasses';
import { FormFillModel } from '../../models';

@Component({
  selector: 'app-formfill',
  templateUrl: './formfill.component.html',
  styleUrls: ['./formfill.component.css']
})
export class FormFillComponent implements OnInit {

  constructor(private formService: OnlineFormService,
    private route: ActivatedRoute, private commonService: CommonService,
    private router: Router ) { }

  model: FormFillModel = new FormFillModel();
  errorMessage = '';

  ngOnInit() {
      const id = +this.route.snapshot.paramMap.get('id');
      this.formService.getForm(id).subscribe(data => {
            this.model = data;
        },
        err => this.errorMessage = this.commonService.getError(err)
        );
  }

  submitForm() {
    this.formService.submitForm(this.model.form).subscribe(() => {
        this.router.navigate(['../..']);
    },
    err => this.errorMessage = this.commonService.getError(err)
    );
  }

}

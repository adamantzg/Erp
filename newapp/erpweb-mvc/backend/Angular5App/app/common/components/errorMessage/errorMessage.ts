import { Component, Input } from '@angular/core';

@Component({
    selector: 'app-errormessage',
    template: '<div *ngIf="errorMessage" class="alert alert-danger">{{errorMessage}}</div>',
    styleUrls: []
  })
  export class ErrorMessageComponent {

    @Input()
    errorMessage = '';

  }

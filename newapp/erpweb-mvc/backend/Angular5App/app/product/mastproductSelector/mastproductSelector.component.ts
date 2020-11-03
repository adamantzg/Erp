import { Component, OnInit, EventEmitter, Input, Output } from '@angular/core';
import { MastProductSelectorModel } from './mastProductSelectorModel';

@Component({
    selector: 'app-mastproductselector',
    templateUrl: './mastproductSelector.component.html',
    styleUrls: ['./mastproductSelector.component.css']
  })
  export class MastProductSelectorComponent implements OnInit {

    errorMessage = '';

    @Input()
    model: MastProductSelectorModel = new MastProductSelectorModel();

    @Input()
    showCategories = true;

    @Input()
    showReset = true;

    @Output()
    onAction: EventEmitter<string> = new EventEmitter();

    ngOnInit() {

    }

    onButtonClick(buttonName) {
        this.onAction.emit(buttonName);
    }

  }

import { Component } from '@angular/core';
import { BlockUIService } from './common';
import { OnInit } from '@angular/core/src/metadata/lifecycle_hooks';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'app';
  blockUI = 0;

  constructor(private blockUIService: BlockUIService) {

  }

  private blockUnBlockUI(event) {
      if (event) {
        this.blockUI++;
      } else {
          this.blockUI--;
      }

  }

  ngOnInit() {
    this.blockUIService.blockUIEvent.subscribe(event => this.blockUnBlockUI(event));
  }
}

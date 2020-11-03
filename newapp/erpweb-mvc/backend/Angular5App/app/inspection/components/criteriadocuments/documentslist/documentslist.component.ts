import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { InstructionsNew, InstructionsNewKind } from '../../../domainclasses';
import { InstructionsNewService } from '../../../services/instructionsnew.service';
import { CommonService, GridDefinition, GridColumn, GridColumnType, ColumnDataType, GridButtonEventData } from '../../../../common';
import { Router } from '@angular/router';
import { MessageboxService } from '../../../../common/messagebox/messagebox.service';
import { MessageBoxType, MessageBoxCommand, MessageBoxCommandValue } from '../../../../common/ModalDialog';
import { Subject ,  Observable ,  of } from 'rxjs';
import { debounceTime, switchMap, distinctUntilChanged } from 'rxjs/operators';
import { UserService } from '../../../../common/services/user.service';


@Component({
  selector: 'app-documentslist',
  templateUrl: './documentslist.component.html',
  styleUrls: ['./documentslist.component.css']
})
export class DocumentsListComponent implements OnInit {

  constructor(private instructionsNewService: InstructionsNewService, private commonService: CommonService,
    private router: Router, private messageBoxService: MessageboxService, private userService: UserService) {
        this.gridDefinition.columns[3].dataType = ColumnDataType.Date;
        this.gridDefinition.columns[3].format  =  { format: 'dd/MM/yyyy'};
        this.gridDefinition.columns[0].linkTarget = '_blank';

     }

  instructions: InstructionsNew[] = [];
  filterTexts: Subject<string> = new Subject<string>();
  filteredData$: Observable<InstructionsNew[]>;
  isQurrentUserQC;

  errorMessage = '';
  gridDefinition  = new GridDefinition([
    new GridColumn('Filename', 'filename', GridColumnType.Link, 'filename', { width: '250px'}, null, null, null, null, null, 'url' ),
    new GridColumn('Products', 'products', GridColumnType.List, 'products', null, null, null, null, null, null, 'factory_ref'),
    new GridColumn('Created By', 'createdBy.userwelcome', GridColumnType.Label, 'createdBy', { width: '120px'}),
    new GridColumn('Date created', 'dateCreated', GridColumnType.Label, 'dateCreated', { width: '120px'}),
    new GridColumn('Edit', 'edit', GridColumnType.Button, 'edit', { width: '100px'}),
    new GridColumn('Delete', 'delete', GridColumnType.Button, 'delete', { width: '100px'})
  ], true);

  gridStyle = { height: '500px'};
  gridData: InstructionsNew[];

  ngOnInit() {
      this.instructionsNewService.getAll(InstructionsNewKind.InspectionCriteria).subscribe(
          data =>  {
              this.instructions = data.instructions;
              this.gridData = data.instructions;
              for(let i of this.instructions) {
                  i.url = data.fileRootFolder + '/' +  i.filename;
              }
            },
          err => this.errorMessage = this.commonService.getError(err)
      );

      this.userService.isCurrentUserQC().subscribe(
        data => {
          this.isQurrentUserQC = data;

          if (this.isQurrentUserQC) {
            this.gridDefinition.columns[4].buttonCss = 'btn btn-default disabled';
            this.gridDefinition.columns[5].buttonCss = 'btn btn-default disabled';
          }
        },
        err => this.errorMessage = this.commonService.getError(err)
      );

      this.filteredData$ = this.filterTexts.pipe(debounceTime(400), distinctUntilChanged(),
      switchMap( (t: string) => this.filterProducts(t)));
      this.filteredData$.subscribe(d => this.gridData = d);
  }

  filterProducts(text: string): Observable<InstructionsNew[]> {
    if (text.length === 0) {
      return of(this.instructions);
    }
    const regExp = new RegExp(text, 'i');

    return of(this.instructions.filter(p => (p.filename != null && p.filename.search(regExp) >= 0)
                  || p.products.some(mp => mp.custProducts.some(cp => cp.cprod_code1.search(regExp) >= 0))
                        || p.products.some(fp => fp.factory.factory_code.search(regExp) >= 0) || p.products.some(f => f.factory_ref.search(regExp) >= 0) ));
  }

  createNew() {
    if (this.isQurrentUserQC == false) {
      this.router.navigate(['/inspection/criteriadocumentedit/0']);
    }
  }

  onButtonClicked(event: GridButtonEventData) {
      if (event.name === 'edit') {
        if (this.isQurrentUserQC == false) {
          this.router.navigate(['/inspection/criteriadocumentedit/' + event.row.id]);
        }
      }
      if (event.name === 'delete') {
        if (this.isQurrentUserQC == false) {
            this.messageBoxService.openDialog('Delete instruction?', MessageBoxType.Yesno, 'Delete', 'Yes', 'No').subscribe(
                (c: MessageBoxCommand) => {
                  if (c.value == MessageBoxCommandValue.Ok) {
                      this.instructionsNewService.delete(event.row.id).subscribe( () => {
                          const index = this.instructions.findIndex(i => i.id == event.row.id);
                          if(index >= 0) {
                              this.instructions.splice(index, 1);
                          }
                        },
                        err => this.errorMessage = this.commonService.getError(err));
                    }
                  }
            );
          }

      }
  }

  filterChange(value: string) {
    this.filterTexts.next(value);
  }


}

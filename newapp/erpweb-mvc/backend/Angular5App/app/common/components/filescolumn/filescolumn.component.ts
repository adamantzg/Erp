import { Component, OnInit, EventEmitter } from '@angular/core';
import { ICustomGridColumnComponentContent, GridColumn, File } from '../..';

@Component({
    selector: 'app-filescolumn',
    templateUrl: './filescolumn.component.html',
    styleUrls: ['./filescolumn.component.css']
  })
  export class FilesColumnComponent implements ICustomGridColumnComponentContent {
        row: any;
        column: GridColumn;
        ColumnClicked = new EventEmitter();

        files() {
            return this.row != null && this.row[this.column.field] ?
             this.row[this.column.field].filter((f: File) => f.type_id === this.column.data.file_type)
             : [];
        }

        onFileDelete(f: File) {
            this.ColumnClicked.emit(
                new FileColumnDeleteEventData(this.column, this.row, f)
            );
        }

        getFilesDescription(f: File) {
            let result = f.name;
            if (f.description) {
                result += ' (' + f.description + ')';
            }
            return result;
        }

  }

  export class FileColumnDeleteEventData {
      column: GridColumn;
      row: any;
      file: File;

      constructor(column: GridColumn, row: any, file: File) {
          this.column = column;
          this.row = row;
          this.file = file;
      }
  }


import { Component, EventEmitter, Input, Output, ViewChild, ElementRef, OnInit } from '@angular/core';
import { GridDefinition, GridColumn, GridColumnType, CommonService, File, GridButtonEventData } from '../../../common';
import { UploadOutput, UploadInput, UploadFile, UploaderOptions, UploadStatus } from 'ngx-uploader';

@Component({
    selector: 'app-fileselector',
    templateUrl: './fileselector.component.html',
    styleUrls: ['./fileselector.component.css']
  })
  export class FileSelectorComponent implements OnInit {


    constructor(private commonService: CommonService) {
        this.options = { concurrency: 1, allowedContentTypes: ['application/pdf', 'application/msword',
        'application/vnd.openxmlformats-officedocument.wordprocessingml.document'] };
        this.uploadInput = new EventEmitter<UploadInput>(); // input events, we use this to emit data to ngx-uploader


    }

    @Input()
    files: File[] = [];
    @Input()
    uploadedFiles: File[] = [];
    @Output()
    FileUploaded = new EventEmitter();
    @Output()
    FileDeleteClicked = new EventEmitter();
    @Input()
    showDescription = false;
    @Input()
    descriptionLabel = 'Description';
    @Output()
    FileEditClicked = new EventEmitter();


    description = '';
    @ViewChild('fileUpload') fileUploadRef: ElementRef;

    @Input()
    allowDescriptionEdit = false;

    // uploader
    options: UploaderOptions;
    formData: FormData;
    tempUploadedFiles: UploadFile[] = [];
    uploadInput: EventEmitter<UploadInput>;
    dragOver: boolean;
    uploadProgress = 0;
    uploading = false;


    gridDefinition = new GridDefinition([
        new GridColumn('', 'selected', GridColumnType.Checkbox, 'selected', {width: '30px'}),
        new GridColumn('Name', 'name', GridColumnType.Link, '', '', '', null, null, false, '', 'url', 'description'),
        new GridColumn('', '', GridColumnType.Button, 'delete', {width: '30px'}, '', null, null, false, '', '', '', 'remove', 'Remove file')
    ], true);

    ngOnInit(): void {
        if (this.allowDescriptionEdit) {
            this.gridDefinition.columns = this.gridDefinition.columns.slice(0, 2).concat([
                new GridColumn('', '', GridColumnType.Button, 'edit', {width: '30px'}, '',  null, null, false, '', '', '', 'edit', 'Edit file')
            ]).concat(this.gridDefinition.columns.slice(2));
        }
    }

    onUploadOutput(output: UploadOutput): void {
        if (output.type === 'allAddedToQueue') { // when all files added in queue
          // uncomment this if you want to auto upload files when added
          if (this.tempUploadedFiles.length > 0) {
            const event: UploadInput = {
                type: 'uploadAll',
                url: this.commonService.getUploadFileUrl() + '?id=' + this.tempUploadedFiles[this.tempUploadedFiles.length - 1].id,
                method: 'POST'
              };
              this.uploading = true;
              // setTimeout(t => this.uploadInput.emit(event), 500 );
              this.uploadInput.emit(event);
          }

        } else if (output.type === 'addedToQueue'  && typeof output.file !== 'undefined') { // add file to array when added
          this.tempUploadedFiles.push(output.file);
        } else if (output.type === 'uploading' && typeof output.file !== 'undefined') {
          // update current data in files array for uploading file
          const index = this.tempUploadedFiles.findIndex(file => typeof output.file !== 'undefined' && file.id === output.file.id);
          this.tempUploadedFiles[index] = output.file;
          this.uploadProgress = output.file.progress.data.percentage;
        } else if (output.type === 'removed') {
          // remove file from array when removed
          this.tempUploadedFiles = this.tempUploadedFiles.filter((file: UploadFile) => file !== output.file);
        } else if (output.type === 'dragOver') {
          this.dragOver = true;
        } else if (output.type === 'dragOut') {
          this.dragOver = false;
        } else if (output.type === 'drop' && typeof output.file !== 'undefined') {
          this.dragOver = false;
          this.tempUploadedFiles.push(output.file);
        } else if (output.type === 'done') {
          this.uploading = false;
          this.uploadProgress = 0;
            const upFile = new File();
            upFile.file_id = output.file.id;
            upFile.name = output.file.name;
            if (this.showDescription) {
                upFile.description = this.description;
            }
            this.uploadedFiles.push(upFile);
            this.FileUploaded.emit(upFile);

        }
      }

      onGridButtonClicked(data: GridButtonEventData) {
          if (data.name === 'delete') {
            this.FileDeleteClicked.emit(data.row);
          }
          if (data.name === 'edit') {
              this.FileEditClicked.emit(data.row);
          }
      }

      clearInputs() {
          this.description = '';
          this.fileUploadRef.nativeElement.value = '';
      }
  }

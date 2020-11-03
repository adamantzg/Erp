import { Component, OnInit, EventEmitter, Input, Output } from '@angular/core';
import { ProductFileType, ProductFile } from '../../domainclasses';
import { UploadOutput, UploadInput, UploadFile, UploaderOptions, UploadStatus } from 'ngx-uploader';
import { CommonService } from '../../../common';
import { Lightbox, IAlbum } from 'ngx-lightbox';

@Component({
    selector: 'app-productfileseditor',
    templateUrl: './productfileseditor.component.html',
    styleUrls: ['./productfileseditor.component.css']
})
export class ProductFilesEditorComponent {

    uploading = false;
    uploadProgress = 0;
    tempUploadedFiles: UploadFile[] = [];
    options: UploaderOptions;
    uploadInput: EventEmitter<UploadInput>;
    dragOver = false;
    album = [];

    @Input()
    files = [];
    @Input()
    readOnlyFiles = [];

    @Input()
    type: ProductFileType = null;

    @Output()
    FileAdded = new EventEmitter();

    @Output()
    FileRemoved = new EventEmitter();

    @Output()
    FileUpdated = new EventEmitter();

    constructor(private commonService: CommonService, private _lightbox: Lightbox) {
        this.options = { concurrency: 1, allowedContentTypes: ['image/png', 'image/jpeg', 'image/gif', 'application/pdf'] };
        this.uploadInput = new EventEmitter<UploadInput>(); // input events, we use this to emit data to ngx-uploader
    }

    onRemove(f) {
        this.FileRemoved.emit(f);
    }

    onUploadOutput(output: UploadOutput): void {
        if (output.type === 'allAddedToQueue') { // when all files added in queue
          // uncomment this if you want to auto upload files when added
          if (this.tempUploadedFiles.length > 0) {
              for (let f of this.tempUploadedFiles) {
                /*this.commonService.uploadFile(this.commonService.getUploadFileUrl() + '?id=' + f.id, f.nativeFile).subscribe(
                    () => {
                        this.uploading = false;
                        this.uploadProgress = 0;
                        const upFile = new ProductFile();
                        upFile.file_id = f.id;
                        upFile.type_id = this.type.id;
                        upFile.file_name = f.name;
                        this.files.push(upFile);
                        this.FileAdded.emit(upFile);
                    }
                );*/
                const event: UploadInput = {
                  type: 'uploadFile',
                    url: this.commonService.getUploadFileUrl() + '?id=' + f.id,
                    method: 'POST',
                    file: f
                };
                this.uploading = true;
                setTimeout(() => this.uploadInput.emit(event), 500 );
              }
              /*const event: UploadInput = {
                type: 'uploadAll',
                url: this.commonService.getUploadFileUrl() + '?id=' + this.tempUploadedFiles[this.tempUploadedFiles.length - 1].id,
                method: 'POST'
              };
              this.uploading = true;
              // setTimeout(t => this.uploadInput.emit(event), 500 );
              this.uploadInput.emit(event);*/

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
            const upFile = new ProductFile();
            upFile.file_id = output.file.id;
            upFile.type_id = this.type.id;
            upFile.file_name = output.file.name;
            this.files.push(upFile);
            this.FileAdded.emit(upFile);
            this.tempUploadedFiles = [];
        }
    }

    getImageUrl(f) {
        if (!f.id) {
            return this.commonService.getTempUrl() + '?id=' + f.file_id;
        }
        return `/images/upload/${this.type.path}/${f.file_name}`;
    }

    openLightbox(f) {
        const sortedFiles =  this.readOnlyFiles.concat(this.files.sort((a,b) => a.order_index - b.order_index));
        const album = sortedFiles.map(fi => ({ src: this.getImageUrl(fi), thumb: this.getImageUrl(fi), caption: fi.file_name}));
        const index = sortedFiles.findIndex(fi => fi.id == f.id);
        this._lightbox.open(album, index);
    }

    isFirst(f: ProductFile) {
        const minIndex = Math.min.apply(Math, this.files.map(pf => pf.order_index));
        return f.order_index == minIndex;
    }

    isLast(f: ProductFile) {
        const maxIndex = Math.max.apply(Math, this.files.map(pf => pf.order_index));
        return f.order_index == maxIndex;
    }

    moveLeft(f: ProductFile) {
        const orderIndexes = this.files.map(fi => fi.order_index).sort((a, b) => a - b);
        const arrIndex = orderIndexes.findIndex(i => i == f.order_index);
        if(arrIndex > 0) {
            const otherIndex = orderIndexes[arrIndex - 1];
            this.findAndSwap(otherIndex, f);
        }
    }

    findAndSwap(otherIndex: number, f: ProductFile) {
        const otherFile = this.files.find(fi => fi.order_index == otherIndex);
        // Swap them
        const swapBuffer = otherFile.order_index;
        otherFile.order_index = f.order_index;
        f.order_index = swapBuffer;
        this.FileUpdated.emit(f);
        this.FileUpdated.emit(otherFile);
    }

    moveRight(f: ProductFile) {
        const orderIndexes = this.files.map(fi => fi.order_index).sort((a, b) => a - b);
        const arrIndex = orderIndexes.findIndex(i => i == f.order_index);
        if (arrIndex < orderIndexes.length - 1) {
            const otherIndex = orderIndexes[arrIndex + 1];
            this.findAndSwap(otherIndex, f);
        }
    }
}

import { Component, Output, EventEmitter } from '@angular/core';
import { File, CommonService } from '../../../common';
import { BsModalRef } from 'ngx-bootstrap';

@Component({
    selector: 'app-fileeditdialog',
    styleUrls: [],
    templateUrl: './fileEditModal.Component.html'
})
export class FileEditModalComponent {

    constructor(private modalRef: BsModalRef,
        private commonService: CommonService) {

    }


    public get file(): File {
        return this._file;
    }

    public set file(f: File) {
        this._file = this.commonService.deepClone(f);
    }


    protected _file: File = new File();
    title = '';
    descriptionLabel = 'Description';
    errorMessage = '';

    @Output()
    OnOk = new EventEmitter();

    ok() {
        this.OnOk.emit(this._file);
        this.modalRef.hide();
    }

    cancel() {
        this.modalRef.hide();
    }
}

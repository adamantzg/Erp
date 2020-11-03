import { Component, OnInit, EventEmitter } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { InstructionsNewService } from '../../../services/instructionsnew.service';
import { InstructionsEditModel } from '../../../modelclasses';
import { CommonService, GridDefinition, GridColumn, GridColumnType, GridButtonEventData } from '../../../../common';
import { UploadInput, UploadOutput, UploadFile } from 'ngx-uploader';
import { CustProduct } from '../../../../product/domainclasses';
import { InstructionsNewKind } from '../../../domainclasses';

@Component({
  selector: 'app-documentsdetail',
  templateUrl: './documentsdetail.component.html',
  styleUrls: ['./documentsdetail.component.css']
})
export class DocumentsDetailComponent implements OnInit {

    constructor(private activatedRoute: ActivatedRoute, private instructionsService: InstructionsNewService,
    private commonService: CommonService, private router: Router) { }

    model: InstructionsEditModel = new InstructionsEditModel();
    errorMessage = '';
    buttonTitle = 'Create';
    title = 'Create new criteria document';
    options = { concurrency: 1, allowedContentTypes: ['application/pdf'] };
    uploadInput = new EventEmitter<UploadInput>();
    tempUploadedFile: UploadFile;
    uploading = false;
    uploadProgress = 0;
    dragOver = false;
    products: CustProduct[] = [];
    id = null;
    gridDefinition = new GridDefinition([
        new GridColumn('Factory ref', 'factory_ref'),
        new GridColumn('Name', 'asaq_name'),
        new GridColumn ('Factory', 'factory.factory_code'),
        new GridColumn('Cust. products', 'custProducts',GridColumnType.List, 'products', null, null, null, null, null, null, 'cprod_code1'),
        new GridColumn('Remove', '', GridColumnType.Button, 'remove', null, null, null, null, null, null, null, null, 'remove')
    ]);

    ngOnInit() {

        this.id = +this.activatedRoute.snapshot.params.id;
        if (this.id > 0) {
        this.buttonTitle = 'Update';
        this.title = 'Edit criteria document';
        } else {
            this.id = null;
        }
        this.instructionsService.getModel(this.id).subscribe(
            data => this.model = data,
            err => this.errorMessage = this.commonService.getError(err)
        );
    }

    onUploadOutput(output: UploadOutput): void {
        if (output.type === 'allAddedToQueue') { // when all files added in queue
        // uncomment this if you want to auto upload files when added
        const f = this.tempUploadedFile;

            const event: UploadInput = {
                type: 'uploadFile',
                url: this.commonService.getUploadFileUrl() + '?id=' + f.id,
                method: 'POST',
                file: f
            };
            this.uploading = true;
            setTimeout(() => this.uploadInput.emit(event), 500 );

        } else if (output.type === 'addedToQueue'  && typeof output.file !== 'undefined') { // add file to array when added
        this.tempUploadedFile = output.file;
        } else if (output.type === 'uploading' && typeof output.file !== 'undefined') {
        // update current data in files array for uploading file

        this.uploadProgress = output.file.progress.data.percentage;
        } else if (output.type === 'removed') {
        // remove file from array when removed
        this.tempUploadedFile = null;
        } else if (output.type === 'dragOver') {
        this.dragOver = true;
        } else if (output.type === 'dragOut') {
        this.dragOver = false;
        } else if (output.type === 'drop' && typeof output.file !== 'undefined') {
        this.dragOver = false;
        this.tempUploadedFile = output.file;
        } else if (output.type === 'done') {

            this.uploading = false;
            this.uploadProgress = 0;
            this.model.instruction.file_id = output.file.id;
            this.model.instruction.filename = output.file.name;
            this.tempUploadedFile = null;
        }
    }

    onAddProducts(products: CustProduct[]) {
        for (const p of products) {
            if (!this.model.instruction.products || this.model.instruction.products.find(prod => prod.mast_id === p.cprod_mast) == null) {
                const prodcopy = this.commonService.deepClone(p);
                const mast = this.commonService.deepClone(prodcopy.mastProduct);
                mast.custProducts = [prodcopy];
                if (!this.model.instruction.products) {
                    this.model.instruction.products = [];
                }
                this.model.instruction.products.push(mast);
            }
        }
    }

    save() {
        let observable;
        const data = this.commonService.deepClone(this.model.instruction);
        for (let p of data.products) {
            p.factory = null;
            p.custProducts = null;
        }
        data.kind_id = InstructionsNewKind.InspectionCriteria;
        data.type_id = 1;
        if (!this.id) {
            this.model.instruction.kind_id = InstructionsNewKind.InspectionCriteria;
            observable = this.instructionsService.create(data);
        } else {
            observable = this.instructionsService.update(data);
        }
        observable.subscribe(result =>  {
            Object.assign(this.model.instruction, result);
            this.backToList();
        },
        err => this.errorMessage = this.commonService.getError(err));
    }

    backToList() {
        this.router.navigate(['/inspection/criteriadocuments']);
    }

    onButtonClicked(event: GridButtonEventData) {
        if (event.name === 'remove') {
            const index = this.model.instruction.products.findIndex(p => p.mast_id === event.row.mast_id);
            if (index >= 0) {
                this.model.instruction.products.splice(index, 1);
            }
        }
    }

}

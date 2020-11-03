import { Component, OnInit, EventEmitter, ViewChild } from '@angular/core';
import { MastProductSelectorModel } from '../mastproductSelector/mastProductSelectorModel';
import { ProductService } from '../services/product.service';
import { CommonService, File, FileType, CompanyService, Company } from '../../common';
import { MastProduct, CustProduct } from '../domainclasses';
import { ProductDto } from './productlist/productDto';
import { MessageboxService } from '../../common/messagebox/messagebox.service';
import { MessageBoxType, MessageBoxCommand, MessageBoxCommandValue } from '../../common/ModalDialog';
import { FileService } from '../../common/services/file.service';
import { FileSelectorComponent } from './fileselector/fileselector.component';
import { BsModalService } from 'ngx-bootstrap';
import { FileEditModalComponent } from './fileEdit/fileEditModal.Component';
import { ProductListEventData } from './productlist/productlist.component';

@Component({
    selector: 'app-certificates',
    templateUrl: './certificates.component.html',
    styleUrls: ['./certificates.component.css']
  })
  export class CertificatesComponent implements OnInit {

    constructor(private productService: ProductService,
        private commonService: CommonService,
        private companyService: CompanyService,
        private fileService: FileService,
        private messageBoxService: MessageboxService,
        private modalService: BsModalService) {

    }

    selectorModel: MastProductSelectorModel = new MastProductSelectorModel();
    errorMessage = '';
    mastProducts: ProductDto[] = [];
    filteredMastProducts: ProductDto[] = [];
    mastProductsFull: MastProduct[] = [];
    custProducts: ProductDto[] = [];
    filteredCustProducts: ProductDto[] = [];
    custProductsFull: CustProduct[] = [];
    files: File[] = [];
    filesFactories: File[] = [];
    uploadedFiles: File[] = [];
    uploadedFilesFactories: File[] = [];
    filters = {
        productFilter: '',
        factoryFilter: ''
    };
    salesMonthsInPast = 24;
    showCustProducts = false;
    @ViewChild('factoryFileSelector') factoryFileSelector: FileSelectorComponent;

    ngOnInit(): void {
        this.productService.getMastProductSelectorModel().subscribe(
            data => this.selectorModel = data,
            err => this.commonService.getError(err));
        this.fileService.getFilesForMastProducts(FileType.certificate).subscribe(
            data => {
                this.files = data;
            } ,
            err => this.errorMessage = this.commonService.getError(err)
        );
        this.fileService.getFilesForCompanies(FileType.certificate).subscribe(
            data => {
                this.filesFactories = data;
            } ,
            err => this.errorMessage = this.commonService.getError(err)
        );
    }

    onSelectorAction(event) {
        if (event === 'filter') {
            this.productService.getMastProductByCriteria(this.selectorModel.factoryId, null, this.selectorModel.categoryId, true, 
                this.salesMonthsInPast, null)
                .subscribe(data => {
                    this.mastProductsFull = data;
                    this.mastProducts = data.map( (d: MastProduct) => new ProductDto(d.mast_id, d.factory_ref, d.asaq_name, d.otherFiles,
                        d.custProducts.map(cp => new ProductDto(cp.cprod_id, cp.cprod_code1, cp.cprod_name, cp.otherFiles, null))));
                    this.custProductsFull = data.map(p => p.custProducts)
                        .reduce((a: CustProduct[], c: CustProduct[]) => a.concat(c));
                    this.custProducts = this.mastProducts.map(p => p.childProducts)
                    .reduce((a: ProductDto[], c: ProductDto[]) => a.concat(c));
                } ,
                err => this.errorMessage = this.commonService.getError(err));
        }
    }

    getSelectedMastProductsCount() {
        return this.filteredMastProducts.filter(p => p.selected).length;
    }

    getSelectedCustProductsCount() {
        return this.filteredCustProducts.filter(p => p.selected).length;
    }

    getSelectedFactoriesCount() {
        return this.selectorModel.factories.filter(f => f.selected).length;
    }

    getSelectedFilesCount() {
        return this.files.filter(f => f.selected).length;
    }

    getSelectedFilesFactoriesCount() {
        return this.filesFactories.filter(f => f.selected).length;
    }

    getUploadedFilesCount() {
        return this.uploadedFiles.length;
    }

    getUploadedFilesFactoriesCount() {
        return this.uploadedFilesFactories.length;
    }

    onMastProductListFilterApplied(filteredProducts: ProductDto[]) {
        this.filteredMastProducts = filteredProducts;
    }

    onCustProductListFilterApplied(filteredProducts: ProductDto[]) {
        this.filteredCustProducts = filteredProducts;
    }

    assignFiles() {
        const selectedMastProducts = this.filteredMastProducts.filter(p => p.selected);
        const selectedCustProducts = this.filteredCustProducts.filter(p => p.selected);
        const mastProducts = this.mastProductsFull.filter(mp => selectedMastProducts.find(s => s.id === mp.mast_id) != null );
        const custProducts = this.custProductsFull.filter(cp => selectedCustProducts.find(s => s.id === cp.cprod_id) != null );
        // take only last uploadedFile
        const upFiles = this.uploadedFiles.slice(-1);
        for (let i = 0; i < upFiles.length; i++) {
            upFiles[i].type_id = FileType.certificate;
        }
        const selectedFiles = this.files.filter(f => f.selected).concat(upFiles);
        for (let i = 0; i < mastProducts.length; i++) {
            const mp = mastProducts[i];
            if (mp.otherFiles == null) {
                mp.otherFiles = [];
            }
            mp.otherFiles = mp.otherFiles.concat(selectedFiles);
        }
        for (let i = 0; i < custProducts.length; i++) {
            const cp = custProducts[i];
            if (cp.otherFiles == null) {
                cp.otherFiles = [];
            }
            cp.otherFiles = cp.otherFiles.concat(selectedFiles);
        }
        if (mastProducts.length > 0) {
            this.productService.updateMastProductsBulk(mastProducts)
            .subscribe((data: MastProduct[]) => {
                for (let i = 0; i < data.length; i++) {
                    const d = data[i];
                    const mpDto = selectedMastProducts.find(p => p.id === d.mast_id);
                    if (mpDto != null) {
                        mpDto.otherFiles = d.otherFiles;
                    }
                }
                // Add newly uploaded files
                for (let i = 0; i < data[0].otherFiles.length; i++) {
                    const file = data[0].otherFiles[i];
                    if (this.files.find(f => f.id === file.id) == null) {
                        this.files.push(file);
                        this.filesFactories.push(this.commonService.deepClone(file));
                    }
                }
                // Reset selections
                this.uploadedFiles = [];
                for (let i = 0; i < this.files.length; i++) {
                    this.files[i].selected = false;
                }
                for (let i = 0; i < this.mastProducts.length; i++) {
                    this.mastProducts[i].selected = false;
                }
            },
            err => this.errorMessage =  this.commonService.getError(err));
        }

        // cust products
        if (custProducts.length > 0) {
            this.productService.updateCustProductsBulk(custProducts)
            .subscribe((data: CustProduct[]) => {
                for (let i = 0; i < data.length; i++) {
                    const d = data[i];
                    const cpDto = selectedCustProducts.find(p => p.id === d.cprod_id);
                    if (cpDto != null) {
                        cpDto.otherFiles = d.otherFiles;
                    }
                }
                // Add newly uploaded files
                if (mastProducts.length === 0) {
                    for (let i = 0; i < data[0].otherFiles.length; i++) {
                        const file = data[0].otherFiles[i];
                        if (this.files.find(f => f.id === file.id) == null) {
                            this.files.push(file);
                            this.filesFactories.push(this.commonService.deepClone(file));
                        }
                    }
                    // Reset selections
                    this.uploadedFiles = [];
                    for (let i = 0; i < this.files.length; i++) {
                        this.files[i].selected = false;
                    }
                }

                for (let i = 0; i < this.custProducts.length; i++) {
                    this.custProducts[i].selected = false;
                }
            },
            err => this.errorMessage =  this.commonService.getError(err));
        }

    }

    assignFilesFactories() {
        const selected = this.selectorModel.factories.filter(p => p.selected);
        const upFiles = this.uploadedFilesFactories.slice(-1);
        for (let i = 0; i < upFiles.length; i++) {
            upFiles[i].type_id = FileType.certificate;
        }

        const selectedFiles = this.filesFactories.filter(f => f.selected).concat(upFiles);
        for (let i = 0; i < selected.length; i++) {
            const f = selected[i];
            if (f.files == null) {
                f.files = [];
            }
            f.files = f.files.concat(selectedFiles);
        }
        this.companyService.updateBulk(selected)
            .subscribe((data: Company[]) => {
                for (let i = 0; i < data.length; i++) {
                    const d = data[i];
                    const f = selected.find(p => p.user_id === d.user_id);
                    if (f != null) {
                        f.files = d.files;
                    }
                }
                // Add newly uploaded files
                for (let i = 0; i < data[0].files.length; i++) {
                    const file = data[0].files[i];
                    if (this.files.find(f => f.id === file.id) == null) {
                        this.filesFactories.push(file);
                        this.files.push(this.commonService.deepClone(file));
                    }
                }
                // Reset selections
                this.uploadedFilesFactories = [];
                for (let i = 0; i < this.filesFactories.length; i++) {
                    this.filesFactories[i].selected = false;
                }
                for (let i = 0; i < this.selectorModel.factories.length; i++) {
                    this.selectorModel.factories[i].selected = false;
                }
                this.factoryFileSelector.clearInputs();
            },
            err => this.errorMessage =  this.commonService.getError(err));
    }

    onFileDeleteClicked(data: File) {
        this.messageBoxService.openDialog('Delete file ' + data.name + ' ?', MessageBoxType.Yesno, 'Warning', 'Delete')
        .subscribe((result: MessageBoxCommand) => {
            if (result.value === MessageBoxCommandValue.Ok) {
                this.fileService.deleteFile(data.id)
                    .subscribe(() => {
                        let index = this.files.findIndex(f => f.id === data.id);
                        if (index >= 0) {
                            this.files.splice(index, 1);
                        }
                        index = this.filesFactories.findIndex(f => f.id === data.id);
                        if (index >= 0) {
                            this.filesFactories.splice(index, 1);
                        }
                        for (let i = 0; i < this.mastProducts.length; i++) {
                            const mpDto = this.mastProducts[i];
                            index = mpDto.otherFiles.findIndex(f => f.id === data.id);
                            if (index >= 0) {
                                mpDto.otherFiles.splice(index, 1);
                            }
                            const mp = this.mastProductsFull[i];
                            index = mp.otherFiles.findIndex(f => f.id === data.id);
                            if (index >= 0) {
                                mp.otherFiles.splice(index, 1);
                            }
                        }
                        for (let i = 0; i < this.selectorModel.factories.length; i++) {
                            const factory = this.selectorModel.factories[i];
                            index = factory.files.findIndex(f => f.id === data.id);
                            if (index >= 0) {
                                factory.files.splice(index, 1);
                            }
                        }
                    },
                    err => this.errorMessage = this.commonService.getError(err));
            }
        });
    }

    onFileEditClicked(data: File) {
        const editDialog = this.modalService.show(FileEditModalComponent);
        editDialog.content.file = data;
        editDialog.content.title = 'Edit file';
        editDialog.content.descriptionLabel = 'Name';
        editDialog.content.OnOk.subscribe((file) => {
            this.fileService.updateFile(file).subscribe(() => Object.assign(data, file));
        });
    }

    onMastProductFileRemoved(eventData: ProductListEventData) {
        const productId = eventData.productId;
        const fileId = eventData.fileId;
        this.productService.removeMastProductFile(productId, fileId)
        .subscribe(() => {
            const mp = this.mastProducts.find(f => f.id === productId);
            if (mp != null) {
                const index = mp.otherFiles.findIndex(f => f.id === fileId);
                if (index >= 0) {
                    mp.otherFiles.splice(index, 1);
                }
            }
        });
    }

    onCustProductFileRemoved(eventData: ProductListEventData) {
        const productId = eventData.productId;
        const fileId = eventData.fileId;
        this.productService.removeCustProductFile(productId, fileId)
        .subscribe(() => {
            const cp = this.custProducts.find(f => f.id === productId);
            if (cp != null) {
                const index = cp.otherFiles.findIndex(f => f.id === fileId);
                if (index >= 0) {
                    cp.otherFiles.splice(index, 1);
                }
            }
        });
    }

  }

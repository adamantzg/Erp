import { Component, OnInit, EventEmitter, Input } from '@angular/core';
import { Company, GridDefinition, GridColumn, GridColumnType, FileType, CompanyService, FilesColumnComponent, FileColumnDeleteEventData } from '../../../common';

@Component({
    selector: 'app-certfactorylist',
    templateUrl: './factorylist.component.html',
    styleUrls: ['./factorylist.component.css']
  })
  export class FactorylistComponent  {

    constructor(private companyService: CompanyService) {

    }

    @Input()
    factories: Company[] = [];



    gridDefinition = new GridDefinition([
        new GridColumn('', 'selected', GridColumnType.Checkbox, 'selected', {width: '30px'}),
        new GridColumn('Code', 'factory_code', GridColumnType.Label, '', {width: '120px'}),
        new GridColumn('Name', 'user_name'),
        new GridColumn('Cert?', 'files', GridColumnType.Custom, 'cert', {width: '100px'}, null, FilesColumnComponent, {file_type: FileType.certificate} )
    ], true);

    onColumnClicked(eventData: FileColumnDeleteEventData) {
        this.companyService.removeFile(eventData.row.user_id, eventData.file.id)
        .subscribe(() => {
            const factory = this.factories.find(f => f.user_id === eventData.row.user_id);
            if (factory != null) {
                const index = factory.files.findIndex(f => f.id === eventData.file.id);
                if (index >= 0) {
                    factory.files.splice(index, 1);
                }
            }
        });
    }
  }

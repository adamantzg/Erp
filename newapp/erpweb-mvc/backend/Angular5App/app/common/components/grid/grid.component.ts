import { Component, OnInit, Input, Output, EventEmitter, ComponentFactoryResolver, ViewContainerRef, Type, AfterViewInit, 
    ViewChild, ElementRef } from '@angular/core';

@Component({
  selector: 'app-grid',
  templateUrl: './grid.component.html',
  styleUrls: ['./grid.component.css']
})
export class GridComponent implements OnInit, AfterViewInit {

  constructor(private componentFactoryResolver: ComponentFactoryResolver,
    private viewContainerRef: ViewContainerRef) { }

    @ViewChild('bodytable')
    bodyTable: ElementRef;

    @ViewChild('headtable')
    headTable: ElementRef;

    _data: any[];

    @Input()
    public get data(): any[] {
        return this._data;
    }

    public set data(v: any[]) {
        this._data = v;
        setTimeout(() => this.fixColumnWidths(), 0);
    }


    @Input()
    definition: GridDefinition;
    @Output()
    ButtonClicked = new EventEmitter();
    columnTypes = Object.assign({}, GridColumnType);
    fixedHeaderStyle: any;
    @Input()
    style = {};
    @Input()
    filter: string;
    @Input()
    headerData: any;
    @Output()
    HeaderCheckBoxClicked = new EventEmitter();
    @Output()
    ColumnClicked = new EventEmitter();
    @Output()
    CheckboxClicked = new EventEmitter();
    @Input()
    headerStyle = null;


  ngOnInit() {
    this.fixedHeaderStyle = { height: this.calculateScrollableHeight(this.style['height']), 'overflow-y': 'auto'};

  }


  ngAfterViewInit() {

  }

  buttonClicked(d: any, c: GridColumn) {
    this.ButtonClicked.emit(new GridButtonEventData(c.name, d));
  }

  calculateScrollableHeight(styleHeight: string) {
    if ( styleHeight != null) {
      const num = +styleHeight.replace('px', '');
      if (num > 0) {
        return (num - 40).toString() + 'px';
      }
    }
    return '';
  }

  getValue(row: any, c: GridColumn, field?: string) {

    if (!field) {
        field = 'field';
    }
    if (c.fieldValueCallback == null) {
      let obj = row;
      const parts = c[field].split('.');
      for (let i = 0; i < parts.length - 1; i++) {
        if (obj[parts[i]] != null) {
          obj = obj[parts[i]];
        }
      }
      if (obj[parts[parts.length -1 ]] != null) {
        return obj[parts[parts.length - 1]];
      } else {
        return '';
      }
    }
    return c.fieldValueCallback(row);
  }

  onHeaderCheckBoxClicked(c: GridColumn, event: any) {
      this.HeaderCheckBoxClicked.emit(new HeaderCheckboxEventData(c, event.target.checked));
  }

  columnClicked(event) {
    this.ColumnClicked.emit(event);
  }

  checkboxClicked(d: any, c: GridColumn) {
    this.CheckboxClicked.emit(new GridButtonEventData(c.name, d));
  }

  joinListValue(row: any, col: GridColumn) {
    const list = this.getValue(row, col);
    if (list.length) {
        return list.map(l => l[col.linkField]).join(', ');
    }
    return '';
  }

  fixColumnWidths() {
    if (this.definition.fixedHeaders) {
        const trHeadTable = this.headTable.nativeElement.firstChild.firstChild;
        const tbodyBodyTable = this.bodyTable.nativeElement.querySelector('tbody');
        let diff = 0;
        if (tbodyBodyTable.children.length > 0) {
            const firstRow = tbodyBodyTable.querySelector('tr');
            for (let i = 0; i < firstRow.children.length; i++) {
                const td = firstRow.children[i];
                const th = trHeadTable.children[i];
                diff += th.clientWidth - td.clientWidth;
            }
        }
        if (diff > 0) {
            const th = document.createElement('th');
            th.style.width = diff + 'px';
            trHeadTable.appendChild(th);
        }
    }
  }

  

}

export class GridDefinition {
  columns: GridColumn[];
  fixedHeaders = false;
  multiSelect = false;
  scrollHeight: string;

  constructor(columns: GridColumn[], fixedHeaders?: boolean, multiSelect?: boolean, scrollHeight?: string) {
    this.columns = columns;
    if (fixedHeaders != null) {
      this.fixedHeaders = fixedHeaders;
    }
    if (multiSelect != null) {
      this.multiSelect = multiSelect;
    }
    if (scrollHeight != null) {
      this.scrollHeight = scrollHeight;
    }
  }
}

export class GridColumn {
  title: string;
  field: string;
  name: string;
  style: any;
  headerStyle: any;
  buttonCss: string;
  customComponentType: Type<any>;
  type: GridColumnType;
  data: any;
  showInHeader: boolean;
  headerSelectField: string;
  linkField: string;
  tooltipField: string;
  buttonIcon: string;
  buttonTooltip: string;
  dataType: ColumnDataType;
  format: any;
  fieldValueCallback: any;
  linkTarget: string;

  constructor(title: string, field: string, type?: GridColumnType, name?: string, style?: any, buttonCss?: string,
    customComponentType?: Type<any>, data?: any, showInHeader?: boolean, headerSelectField?: string, linkField?: string,
    toolTipField?: string, buttonIcon?: string, buttonToolTip?: string, dataType?: ColumnDataType, format?: any, fieldValueCallback?: any,
    headerStyle?: any) {
    this.title = title;
    this.field = field;
    this.name = name;
    if (type != null) {
      this.type = type;
    } else {
      this.type = GridColumnType.Label;
    }
    this.style = style;
    this.headerStyle = headerStyle;
    if(this.headerStyle == null) {
        this.headerStyle = this.style;
    }
    this.buttonCss = buttonCss;
    if (name == null) {
      this.name = this.title;
    }
    this.customComponentType = customComponentType;
    this.data = data;
    this.showInHeader = showInHeader;
    this.headerSelectField = headerSelectField;
    this.linkField = linkField;
    this.tooltipField = toolTipField;
    this.buttonIcon = buttonIcon;
    this.buttonTooltip = buttonToolTip;
    this.dataType = dataType;
    if (this.dataType == null) {
        this.dataType = ColumnDataType.Text;
      }
    this.format = format;
    this.fieldValueCallback = fieldValueCallback;
  }

}

export class GridButtonEventData {
  name: string;
  row: any;

  constructor(name: string, row: any) {
    this.name = name;
    this.row = row;
  }
}

export class CustomColumnEventData {
  name: string;
  row: any;
}

export class HeaderCheckboxEventData {
    column: GridColumn;
    checked: boolean;

    constructor(column: GridColumn, checked: boolean) {
        this.column = column;
        this.checked = checked;
    }
}

export enum GridColumnType {
  Label = 0,
  Checkbox = 1,
  Button = 2,
  Custom = 3,
  Checkmark = 4,
  ButtonGroup = 5,
  Link = 6,
  LinkList = 7,
  List = 8
}

export enum ColumnDataType {
    Text,
    Numeric,
    Date,
    Currency,
    Percent
  }



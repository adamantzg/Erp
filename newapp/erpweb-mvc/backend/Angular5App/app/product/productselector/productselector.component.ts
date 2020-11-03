import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { CustProduct } from '../domainclasses';
import { CompanyService, Company, GridDefinition, GridColumn, GridColumnType, GridButtonEventData } from '../../common';
import { ProductService } from '../services/product.service';
import { Subject ,  Observable ,  of } from 'rxjs';
import { debounceTime, switchMap, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-productselector',
  templateUrl: './productselector.component.html',
  styleUrls: ['./productselector.component.css']
})
export class ProductselectorComponent implements OnInit {


  constructor(private companyService: CompanyService,
  private productService: ProductService) { }

  @Input()
  factories: Company[] = [];
  @Input()
  clients: Company[] = [];
  @Input()
  loadDropdowns = true;
  @Input()
  multiSelect = false;
  @Input()
  showEdit = false;
  @Input()
  showRemove = false;
  @Input()
  style = {};

  products: CustProduct[] = [];
  searchParams: any = {factory_id: null, client_id: null};
  gridStyle = {};
  filterText = '';
  filterTexts: Subject<string> = new Subject<string>();
  filteredData$: Observable<CustProduct[]>;
  gridData: CustProduct[];

  gridDefinition: GridDefinition;

  @Output()
  EditProduct = new EventEmitter<any>();
  @Output()
  AddSelected = new EventEmitter<any>();

  ngOnInit() {
    if (this.loadDropdowns) {
      this.companyService.getClients().subscribe(data => this.clients = data);
      this.companyService.getFactories().subscribe(data => this.factories = data);
    }
    this.gridDefinition = new GridDefinition(
      [
        new GridColumn('Code', 'cprod_code1', GridColumnType.Label, 'code', { width: '100px'}),
        new GridColumn('Description', 'cprod_name')
      ],
      this.style['height'] != null, this.multiSelect
    );
    
    if (this.multiSelect) {
      this.gridDefinition.columns.push(new GridColumn('', 'checked', GridColumnType.Checkbox, '', { width: '30px' }));
    }
    if (this.showEdit) {
      this.gridDefinition.columns.push(new GridColumn('Edit', '', GridColumnType.Button, 'edit', { width: '100px' }));
    }
    // subtract height of dropdowns
    this.gridStyle = { height: this.calculateScrollableHeight(this.style['height']) };
    this.filteredData$ = this.filterTexts.pipe(debounceTime(400), distinctUntilChanged(),
    switchMap( (t: string) => this.filterProducts(t)));
    this.filteredData$.subscribe(d => this.gridData = d);
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

  filterProducts(text: string): Observable<CustProduct[]> {
    if (text.length === 0) {
      return of(this.products);
    }
    const regExp = new RegExp(text, 'i');
    return of(this.products.filter(p => p.cprod_code1.search(regExp) >= 0 || p.cprod_name.search(regExp) >= 0));
  }


  searchProducts() {
    this.productService.searchProducts(this.searchParams.factory_id, this.searchParams.client_id).subscribe(data => {
      this.products = data;
      this.gridData = data;
    });
  }

  editProduct(p: CustProduct) {
    this.EditProduct.emit(p);
  }

  addSelected() {
    this.AddSelected.emit(this.checkedProducts());
    this.products.forEach(p => p.checked = false);
  }

  onButtonClicked(data: GridButtonEventData) {
    if (data.name === 'edit') {
      this.editProduct(data.row);
    }
  }

  checkedProducts() {
    return this.products.filter(p => p.checked);
  }

  filterChange(value: string) {
    this.filterTexts.next(value);
  }


}

import { PipeTransform, Pipe } from '@angular/core';
import {CurrencyPipe, DecimalPipe, DatePipe, PercentPipe} from '@angular/common';
import { GridColumn, ColumnDataType } from './grid.component';

@Pipe({name: 'columnFormat'})
export class ColumnFormatPipe implements PipeTransform {
    constructor(private cp: CurrencyPipe, private np: DecimalPipe, private dp: DatePipe, private pp: PercentPipe ) {
    }

    transform(value: any, type: ColumnDataType, formatObj?: any) {
        const locale = formatObj && formatObj.locale || null;
        const format = formatObj && formatObj.format || undefined;
        const digits = formatObj && formatObj.digits || null;
        switch (type) {
            case ColumnDataType.Text:
                return value;
            case ColumnDataType.Currency:
                const currencyCode = formatObj && formatObj.currencyCode || null;
                const display = formatObj && formatObj.display || null;
                return this.cp.transform(value, currencyCode, display, digits, locale);
            case ColumnDataType.Date:
                return this.dp.transform(value, format, null, locale);
            case ColumnDataType.Numeric:
                return this.np.transform(value, digits, locale);
            case ColumnDataType.Percent:
                return this.pp.transform(value, digits, locale);
        }
    }
}

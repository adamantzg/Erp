import * as moment from 'moment';

export class Month21 {
    private value: number;
    public get Value(): number {
        return this.value;
    }
    public get Date(): Date {
        return Month21.GetDate(this.Value);
    }
    constructor(date?: Date, num?: number) {
        if (num != null) {
            this.value = num;
        } else {
            this.value = Month21.GetMonth21FromDate(date);
        }
    }

    public static get Now(): Month21 {
        return new Month21(new Date());
    }
    public static FromDate(date: Date): Month21 {
        return new Month21(date);
    }
    public static FromYearMonth(year: number, month: number): Month21 {
        return new Month21(new Date(year, month, 1));
    }
    public static GetDate(value: number): Date {
        return Month21.GetDateFromMonth21(value);
    }

    public static GetMonth21FromDate(date: Date): number {
        return (date.getFullYear() - 2000) * 100 + date.getMonth() + 1;
    }

    public static GetDateFromMonth21(month21: number): Date {
        return new Date(2000 + (month21 / 100), month21 % 100 - 1, 1);
    }

    public static Add(m: Month21, offset: number): Month21 {
        return new Month21(moment(Month21.GetDateFromMonth21(m.Value)).add(offset, 'month').toDate());
    }
    public static SubtractOffset(m: Month21, offset: number): Month21 {
        return new Month21(moment(Month21.GetDateFromMonth21(m.Value)).add(-1 * offset, 'month').toDate());
    }
    public static Subtract(m: Month21, mToSubtract: Month21): number {
        const date1 = Month21.GetDateFromMonth21(m.Value);
        const date2 = Month21.GetDateFromMonth21(mToSubtract.Value);
        return (date1.getFullYear() - date2.getFullYear()) * 12 + date1.getMonth() + 1 - (date2.getMonth() + 1);
    }
    public static Lt(first: Month21, second: Month21): boolean {
        return first.Value < second.Value;
    }
    public static Gt(first: Month21, second: Month21): boolean {
        return first.Value > second.Value;
    }
    public static Le(first: Month21, second: Month21): boolean {
        return first.Value <= second.Value;
    }
    public static Ge(first: Month21, second: Month21): boolean {
        return first.Value >= second.Value;
    }
    public static EqualNum(first: Month21, second: number): boolean {
        return first.Value === second;
    }
    public static NotEqualNum(first: Month21, second: number): boolean {
        return first.Value !== second;
    }
    public static NumEqual(first: number, second: Month21): boolean {
        return first === second.Value;
    }
    public static NumNotEqual(first: number, second: Month21): boolean {
        return first !== second.Value;
    }


}

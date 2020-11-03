import { Company } from '../../common';
import { Category1 } from '../domainclasses';

export class MastProductSelectorModel {
    factories: Company[] = [];
    categories: Category1[] = [];
    factoryId: number;
    categoryId: number;
}

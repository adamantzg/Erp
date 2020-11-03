import { File } from '../../../common';

export class ProductDto {
    id: number;
    code: string;
    name: string;
    selected: boolean;
    otherFiles: File[];
    childProducts: ProductDto[];
    childCodes: string;


    constructor(id: number, code: string, name: string, otherFiles: File[], childProducts: ProductDto[]) {
        this.id = id;
        this.name = name;
        this.code = code;
        this.otherFiles = otherFiles;
        this.childProducts = childProducts;
        if (childProducts != null) {
            this.childCodes = childProducts.map(p => p.code).join(', ');
        }
    }
}

/*export class CustProductDto {
    cprod_id: number;
    cprod_code1: string;
    cprod_name: string;
    brand_userid: number;

    constructor(cprod_id: number, cprod_code1: string, cprod_name: string, brand_userid: number) {
        this.cprod_id = cprod_id;
        this.cprod_code1 = cprod_code1;
        this.cprod_name = cprod_name;
        this.brand_userid = brand_userid;
    }
}*/

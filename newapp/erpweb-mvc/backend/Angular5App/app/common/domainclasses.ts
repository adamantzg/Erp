export interface Checkable {
    checked: boolean;
}

export class Company {
    user_id: number;
    user_name: string;
    customer_code: string;
    factory_code: string;
    consolidated_port: number;
    selected: boolean;
    files: File[];
    user_curr: number;
}



export class Lookup {
    id: number;
    name: string;
}


export class Location extends Lookup {

}

export class ContainerType {
    container_type_id: number;
    container_type_desc: string;
    shortname: string;
}

export class FreighCost {
    id: number;
    market_id: number;
    location_id: number;
    container_id: number;
    value: number;
}

export enum ContainerTypeValue {
    GP40 = 0,
    GP20 = 1
}

export class Currency {
    curr_code: number;
    curr_symbol: string;
}

export enum CurrencyCode {
    USD = 0,
    GBP = 1,
    EUR = 2
}

export class Tariff {
    tariff_id: number;
    tariff_code: string;
    tariff_desc: string;
    tariff_rate: number | null;

    constructor(initData?: Tariff) {
        if (initData != null) {
            Object.assign(this, initData);
        }
    }

    public get Description() {
        return this.tariff_code + '(' + (this.tariff_rate * 100).toFixed(2) + '%) ' + this.tariff_desc;
    }
}

export enum FileType {
    certificate = 5
}

export class File {
    id: number;
    name: string;
    type_id: number;
    description: string;
    selected: boolean;
    url: string;
    file_id: string;
}

export class User {
    userid: number;        
    username: string;
    userpassword: string;
    userwelcome: string;
    company_id: number;
    user_level: number | null;
    session: number | null;
    user_email: string;
    admin_type: number | null;
    consolidated_port: number | null;
    inspection_plan_admin: number | null;
    restrict_ip: number | null;
    ip_address: string;
    ip_address1b: string;
    ip_address1c: string;
    ip_address2: string;
    mobilea: string;
    mobileb: string;
    email_pwd: string;
    skype: string;
    manager: number | null;
    user_initials: string;
    status_flag: number | null;
    restricted: number | null;
    qc_technical: number | null;
    after_sales: boolean | null;
    newdesign: boolean | null;
    login_restriction_from: any;
    login_restriction_to: any;
    login_restriction_days: string;

    adminPermissions: AdminPermission[] = [];
}

export class AdminPermission {
    permission_id: number;
    userid: number | null;
    cusid: number | null;
    agent: number | null;
    clientid: number | null;
    returns: number | null;
    processing: number | null;
    feedbacks: number | null;
}




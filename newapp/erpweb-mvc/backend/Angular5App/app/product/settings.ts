export enum CalculationId {
    Sage = 1,
    Gateway = 2
}

export class Settings {
    static apiRoot = '/api/';
    
    static calculations = [{id: CalculationId.Sage, name: 'Sage' }, { id: CalculationId.Gateway, name: 'Gateway'}];
}



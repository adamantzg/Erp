import { MastProduct } from '../product/domainclasses';

export class InstructionsNew {
        id: number;
        filename: string;
        language_id: number | null;
        dateCreated: Date | string | null;
        created_by: number | null;
        type_id: number | null;
        kind_id: number | null;
        products: MastProduct[] = [];

        file_id: string;
        url: string;

}

export enum InstructionsNewKind {
    Instructions = 1,
    InspectionCriteria = 2
}

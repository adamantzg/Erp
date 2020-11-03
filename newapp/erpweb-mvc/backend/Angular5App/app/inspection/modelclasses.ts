import { Company } from '../common';
import { InstructionsNew } from './domainclasses';

export class InstructionsEditModel {
    factories: Company[] = [];
    clients: Company[] = [];
    fileRootFolder: string;
    instruction: InstructionsNew;
}

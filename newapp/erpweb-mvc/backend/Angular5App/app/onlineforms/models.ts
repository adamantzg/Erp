import { Form, FormQuestionRendermethod, FormQuestionType } from './domainclasses';

export class FormFillModel {
    form: Form;
    renderMethods: FormQuestionRendermethod[];
    types: FormQuestionType[];
}

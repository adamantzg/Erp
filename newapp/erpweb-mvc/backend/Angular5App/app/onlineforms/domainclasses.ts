export class Form {
    id: number;
    title: string;

    sections: FormFormSection[];
    submissions: FormSubmission[];
}

export class FormChoice {
    id: number;
    name: string;
    selected: boolean;
}

export class FormChoiceGroup {
    id: number;
    name: string;
    choices: FormChoice[];
}

export class FormFormSection {
    form_id: number;
    section_id: number;
    sequence: number | null;

    form: Form;
    section: FormSection;
}

export class FormQuestion {
    id: number;
    question_text: string;
    description: string;
    question_type: number | null;
    color: string;
    choice_group_id: number | null;
    render_id: number | null;
    has_comment: boolean | null;
    comment_label: string;
    label_editable: boolean | null;

    questionType: FormQuestionType;
    choiceGroup: FormChoiceGroup;
    questionRenderMethod: FormQuestionRendermethod;
    questionGroups: FormQuestiongroupQuestion[];


}

export class FormQuestionAnswer {
        id: number;
        form_submission_answer_id: number | null;
        question_id: number | null;
        intValue: number | null;
        textValue: string;
        dateValue: Date | string | null;
        comment_text: string;

        question: FormQuestion;
        choices: FormChoice[];
}

export class FormQuestionGroup {
    id: number;
    name: string;
    description: string;
    questions: FormQuestiongroupQuestion[];

}

export class FormQuestionRendermethod {
    id: number;
    name: string;
}

export class FormQuestionType {
    id: number;
    name: string;
    default_render_id: number | null;

    renderMethod: FormQuestionRendermethod;
}

export class FormQuestiongroupQuestion {
    group_id: number;
    question_id: number;
    sequence: number | null;
    numRepeat: number | null;
    group: FormQuestionGroup;
    question: FormQuestion;
}

export class FormSection {
    id: number;
    title: string;
    questions: FormSectionQuestion[];

}

export class FormSectionQuestion {
    id: number;
    section_id: number | null;
    question_group_id: number | null;
    question_id: number | null;
    sequence: number | null;

    section: FormSection;
    questionGroup: FormQuestionGroup;
    question: FormQuestion;
    answer: FormSubmissionAnswer;

}

export class FormSubmission {
    id: number;
    dateCreated: Date | string | null;
    user_id: number | null;
    form_id: number | null;

    form: Form;
    user: any;
    answers: FormSubmissionAnswer[];
}

export class FormSubmissionAnswer {
    id: number;
    submission_id: number | null;
    formsection_question_id: number | null;

    questionAnswers: FormQuestionAnswer[];
    submission: FormSubmission;
    sectionQuestion: FormSectionQuestion;
}

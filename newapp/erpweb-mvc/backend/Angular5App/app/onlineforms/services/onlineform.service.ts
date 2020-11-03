import { Injectable } from '@angular/core';
import { HttpService } from '../../common';
import { Settings } from '../settings';
import { FormSubmission, FormSubmissionAnswer, Form, FormSectionQuestion, FormQuestionAnswer } from '../domainclasses';

@Injectable()
export class OnlineFormService {

    constructor(private httpService: HttpService) { }

    api = Settings.apiRoot;

    getAll () {
        return this.httpService.get(this.api + 'getAll');
    }

    getForm(id) {
        return this.httpService.get(this.api + 'getForm', { params: { id: id } });
    }

    submitForm (form: Form) {
        const submission = new FormSubmission();
        submission.form_id = form.id;
        submission.answers = form.sections.reduce((prev, curr) => prev.concat(curr.section.questions.map(q => q.answer)), []);

        /*form.Sections.forEach(s => {
            s.Section.Questions.forEach(q => {
                const answer = new FormSubmissionAnswer();
                answer.formsection_question_id = q.id;
                answer.QuestionAnswers = [];

                const questions = q.Question != null ? [q.Question] : q.QuestionGroup.Questions.map(x => x.Question);

                questions.forEach(x => {
                    if (x.ChoiceGroup != null && x.question_type === QuestionTypeEnum.multiplechoice) {
                        answer.Choices = x.ChoiceGroup.choices.filter(c => c.selected);
                    }
                    answer.QuestionAnswers.push(x.Answer);
                });
                submission.Answers.push(q.Question.Answer);
            });
        });*/

        return this.httpService.post(this.api + 'submitForm', submission);
    }

    getResults(id) {
        return this.httpService.get(this.api + 'submissions', { params: { form_id: id } });
    }

    getResult(id) {
        return this.httpService.get(this.api + 'submission', { params: { id: id } });
    }
}

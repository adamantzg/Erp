import { Component, OnInit, Input } from '@angular/core';
import { FormQuestion, FormQuestionType, FormQuestionRendermethod, FormSubmissionAnswer, FormQuestionAnswer } from '../../domainclasses';
import { QuestionTypeEnum, RenderMethodEnum} from '../../enums';

@Component({
  selector: 'app-question-input',
  templateUrl: './question-input.component.html',
  styleUrls: ['./question-input.component.css']
})
export class QuestionInputComponent implements OnInit {

    constructor() { }

    private question: FormQuestion;
    private answer: FormSubmissionAnswer;
    questionAnswer: FormQuestionAnswer;

    @Input()
    public get Question(): FormQuestion {
        return this.question;
    }

    public set Question(q: FormQuestion) {
        this.question = q;
        if (this.answer != null && this.answer.questionAnswers) {
            this.answer.questionAnswers.forEach(qa => qa.question_id = q.id);
        }
    }

    @Input()
    public get Answer(): FormSubmissionAnswer {
        return this.answer;
    }

    public set Answer(a: FormSubmissionAnswer) {
        if (a != null && a.questionAnswers != null && a.questionAnswers.length > 0) {
            this.questionAnswer = a.questionAnswers[0];
        } else {
            if (a == null) {
                a = new FormSubmissionAnswer();
            }
            if (a.questionAnswers == null) {
                a.questionAnswers = [];
            }
            this.questionAnswer = new FormQuestionAnswer();
            if (this.question != null) {
                this.questionAnswer.question_id = this.question.id;
            }
            a.questionAnswers.push(this.questionAnswer);
        }
    }

    @Input()
    types: FormQuestionType[] = [];
    @Input()
    renderMethods: FormQuestionRendermethod[] = [];

    questionTypes = Object.assign({}, QuestionTypeEnum);
    renderMethodTypes = Object.assign({}, RenderMethodEnum);



  ngOnInit() {
  }

}

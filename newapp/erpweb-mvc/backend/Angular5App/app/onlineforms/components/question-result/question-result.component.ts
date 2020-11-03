import { Component, OnInit, Input } from '@angular/core';
import { FormSectionQuestion, FormQuestionAnswer, FormSubmissionAnswer, FormQuestionType } from '../../domainclasses';

@Component({
  selector: 'app-question-result',
  templateUrl: './question-result.component.html',
  styleUrls: ['./question-result.component.css']
})
export class QuestionResultComponent implements OnInit {

  constructor() { }

  @Input()
  Question: FormSectionQuestion;
  @Input()
  Answer: FormSubmissionAnswer;
  @Input()
  types: FormQuestionType[] = [];

  ngOnInit() {
  }

}

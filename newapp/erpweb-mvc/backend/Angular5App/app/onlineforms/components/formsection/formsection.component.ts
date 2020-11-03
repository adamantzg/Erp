import { Component, OnInit, Input } from '@angular/core';
import { FormSectionQuestion, FormQuestionType, FormQuestionRendermethod } from '../../domainclasses';

@Component({
  selector: 'app-formsection',
  templateUrl: './formsection.component.html',
  styleUrls: ['./formsection.component.css']
})
export class FormSectionComponent implements OnInit {

  constructor() { }

  @Input()
  source: FormSectionQuestion;

  @Input()
  types: FormQuestionType[] = [];
  @Input()
  renderMethods: FormQuestionRendermethod[] = [];

  @Input()
  last: boolean;
  @Input()
  showResult: boolean;

  ngOnInit() {
  }

  getLabel(s: FormSectionQuestion) {
    if (s.question != null) {
        return s.question.question_text;
    }
    if (s.questionGroup != null) {
        return s.questionGroup.name;
    }
  }

  getDescription(s: FormSectionQuestion) {
    if (s.question != null) {
        return s.question.description;
    }
    if (s.questionGroup != null) {
        return s.questionGroup.description;
    }
    return null;
  }

}

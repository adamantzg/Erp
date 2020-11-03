angular.module('app').component('formSectionElement', {
    templateUrl: '/AngularApps/Forms/views/formSectionElement.html',
    controller: formSectionElementController,
    bindings: {
        source: '=',
        types: '<',
        renderMethods: '<',
        last: '<',
        showResult: '<'
    }
});

function formSectionElementController() {
    var ctrl = this;

    /*ctrl.source.answer = {
        formsection_question_id: ctrl.source.id, question_id: ctrl.source.question_id, question_group_id: ctrl.source.question_group_id,
        questionAnswers: []
    };
    if (ctrl.source.question_id != null)
        ctrl.source.answer.questionAnswers.push({ intValue: null, textValue: null, dateValue: null, question_id: ctrl.source.question_id });
    else
        if (ctrl.source.question_group_id != null)
            ctrl.source.questionGroup.questions.forEach(function (q) {
                ctrl.source.answer.questionAnswers.push({ intValue: null, textValue: null, dateValue: null, question_id: q.question_id });
            });*/
    
    ctrl.getLabel = function ()
    {
        if (ctrl.source.question != null)
            return ctrl.source.question.question_text;
        if (ctrl.source.questionGroup != null)
            return ctrl.source.questionGroup.name;
    }
}

angular.module('app').component('questionInput', {
    templateUrl: '/AngularApps/Forms/views/questionInput.html',
    controller: formQuestionInputController,
    bindings: {
        question: '=',        
        types: '<',
        renderMethods: '<'
    }
});

function formQuestionInputController()
{
    var ctrl = this;

    ctrl.popupOpened = false;
    ctrl.openPopup = function () {
        ctrl.popupOpened = !ctrl.popupOpened;
    };
    
    ctrl.question.answer = { intValue: null, textValue: null, dateValue: null, question_id: ctrl.question.id };

    ctrl.answer = ctrl.question.answer;

    ctrl.type = ctrl.question != null ? ctrl.question.question_type : null;
    ctrl.QuestionTypeEnum = QuestionTypeEnum;
    ctrl.RenderMethodEnum = RenderMethodEnum;

    if (ctrl.question != null) {
        if (ctrl.question.render_id == null) {
            var questionTypeObj = _.find(ctrl.types, { id: ctrl.question.question_type });
            if (questionTypeObj != null)
                ctrl.render_id = questionTypeObj.default_render_id;
        }
        else
            ctrl.render_id = ctrl.question.render_id;
        if (ctrl.question.choiceGroup != null)
            ctrl.choices = ctrl.question.choiceGroup.choices;

    }
}

angular.module('app').component('questionResult', {
    templateUrl: '/AngularApps/Forms/views/questionResult.html',
    controller: formQuestionResultController,
    bindings: {
        question: '<',
        types: '<',
        answer: '<'
    }
});

function formQuestionResultController() {
    var ctrl = this;

    ctrl.QuestionTypeEnum = QuestionTypeEnum;

    if (ctrl.question.choiceGroup != null)
        ctrl.choices = ctrl.question.choiceGroup.choices;

    ctrl.getSingleChoice = function (id) {
        var c = _.find(ctrl.choices, { id: id });
        if (c != null)
            return c.name;
        return '';
    };
}
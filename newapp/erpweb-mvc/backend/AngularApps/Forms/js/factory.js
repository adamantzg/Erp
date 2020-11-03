angular.module('app').factory('factory', ['$http', function ($http) {
    var factory = {};
    var baseUrl = 'api/forms/';

    factory.getAll = function () {
        return $http.get(baseUrl + 'getAll');
    };

    factory.getForm = function(id)
    {
        return $http.get(baseUrl + 'getForm', { params: { id: id } });
    }

    factory.submitForm = function(form)
    {
        var submission = {form_id: form.id, Answers: []};
        form.sections.forEach(function (s) {
            s.elements.forEach(function (e) {
                var answer = { formsection_question_id: e.id };
                var questions = e.question != null ? [e.question] : e.questionGroup.questions;
                answer.questionAnswers = [];
                questions.forEach(function (q) {
                    
                    if (q.choiceGroup != null && q.question_type == QuestionTypeEnum.multiplechoice)
                        q.answer.choices = _.filter(q.choiceGroup.choices, { selected: true });
                    answer.questionAnswers.push(q.answer);
                });
                
                submission.Answers.push(answer);
            });
        });

        return $http.post(baseUrl + 'submitForm', submission);
    }

    factory.getResults = function (id) {
        return $http.get(baseUrl + 'submissions', { params: { form_id: id } });
    };

    factory.getResult = function (id) {
        return $http.get(baseUrl + 'submission', { params: { id: id } });
    };

    return factory;
}]);
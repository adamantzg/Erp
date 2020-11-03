angular.module('app', ['ui.bootstrap', 'ui.router', 'logToServer'])
    .config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {

        // default route
        $urlRouterProvider.otherwise('/');

        $stateProvider
            .state('home', {
                url: '/',
                templateUrl: '/AngularApps/Forms/views/Index.html',
                controller: 'HomeCtrl'
            })
            .state('fill', {
                url: '/fill/{id:[0-9]{1,5}}',
                templateUrl: '/AngularApps/Forms/views/Fill.html',
                controller: 'FillCtrl'
            })
            .state('results', {
                url: '/results/{name}/{id:[0-9]{1,5}}',
                templateUrl: '/AngularApps/Forms/views/Results.html',
                controller: 'ResultCtrl'
            })
            .state('result', {
                url: '/result/{id:[0-9]{1,5}}',
                templateUrl: '/AngularApps/Forms/views/Result.html',
                controller: 'ResultCtrl'
            })

        /*
            .state('results', {
                url: '/results',
                templateUrl: '/AngularApps/Forms/views/results.html',
                controller: 'ResultCtrl'
            })*/
        ;

    }]);

var QuestionTypeEnum = {
    shorttext: 1,
    longtext: 2,
    singlechoice: 3,
    multiplechoice: 4,
    yesno: 5,
    date: 6
};

var RenderMethodEnum = {
    radio: 1,
    dropdown: 2,
    checkbox: 3
};
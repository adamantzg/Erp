angular.module('app', ['ui.bootstrap', 'ui.router', 'logToServer', 'datatables', 'angular-plupload'])
    .config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {

        // default route
        $urlRouterProvider.otherwise('/');

        $stateProvider
            .state('home', {
                url: '/',
                templateUrl: '/AngularApps/Instructions/views/index.html',
                controller: 'homeCtrl'
            })
            .state('edit', {
                url: '/edit/{id:[0-9]{1,5}}',
                templateUrl: '/AngularApps/Instructions/views/edit.html',
                controller: 'editCtrl'
            })
            .state('create', {
                url: '/create',
                templateUrl: '/AngularApps/Instructions/views/edit.html',
                controller: 'editCtrl'
            })
            

        /*
            .state('results', {
                url: '/results',
                templateUrl: '/AngularApps/Forms/views/results.html',
                controller: 'ResultCtrl'
            })*/
        ;

    }]);

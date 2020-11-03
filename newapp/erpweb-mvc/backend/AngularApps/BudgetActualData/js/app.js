angular.module('app', ['ui.bootstrap', 'ui.router', 'logToServer'])
    .config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {

        // default route
        $urlRouterProvider.otherwise('/');

        $stateProvider
            .state('home', {
                url: '/',
                templateUrl: '/AngularApps/BudgetActualData/views/index.html',
                controller: 'homeCtrl'
            })
            .state('edit', {
                url: '/edit/{month21:[0-9]{1,5}}',
                templateUrl: '/AngularApps/BudgetActualData/views/edit.html',
                controller: 'editCtrl'
            })
            .state('distributors', {
                url: '/distributors',
                templateUrl: '/AngularApps/BudgetActualData/views/distributors.html',
                controller: 'homeCtrl'
            })
            .state('create', {
                url: '/create',
                templateUrl: '/AngularApps/BudgetActualData/views/edit.html',
                controller: 'editCtrl'
            })
        ;

    }]);

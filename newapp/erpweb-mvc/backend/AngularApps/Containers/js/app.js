angular.module('app', ['ui.bootstrap', 'ui.router', 'logToServer'])
    .config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {

        // default route
        $urlRouterProvider.otherwise('/create');

        $stateProvider
            .state('create', {
                url: '/create',
                templateUrl: '/AngularApps/Containers/views/Create.html',
                controller: 'CreateCtrl'
            })
            .state('calculate', {
                url: '/calculate',
                templateUrl: '/AngularApps/Containers/views/Calculate.html',
                controller: 'CalculateCtrl'
            })            
        ;

    }]);


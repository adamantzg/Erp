var claim_type = 8;

angular.module('app').config(['$urlRouterProvider', '$stateProvider', function($urlRouterProvider, $stateProvider) {

    // default route
    $urlRouterProvider.otherwise('/');

    $stateProvider
        .state('home', {
            url: '/',
            templateUrl: '/AngularApps/Claims/views/qa/Index.html',
            controller: 'controller'
        })
        .state('create', {
            url: '/create',
            templateUrl: '/AngularApps/Claims/views/qa/edit.html',
            controller: 'controller'
        })
        .state('edit', {
            url: '/edit/{id:[0-9]{1,5}}',
            templateUrl: '/AngularApps/Claims/views/qa/edit.html',
            controller: 'controller'
        })
        ;

}]);
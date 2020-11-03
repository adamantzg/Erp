//var claim_type = 8;

angular.module('app').config(['$urlRouterProvider', '$stateProvider', function($urlRouterProvider, $stateProvider) {

    // default route
    $urlRouterProvider.otherwise('/');

    $stateProvider
        .state('home', {
            url: '/',
            templateUrl: '/AngularApps/Claims/views/All.html',
            controller: 'controller'
        });

}]);
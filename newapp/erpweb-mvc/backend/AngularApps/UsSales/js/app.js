(function () {
    'use strict';

    angular.module('app', [
        // Angular modules 
        'ui.bootstrap', 'datatables', 'ui.router', 'angular-plupload', 'logToServer'

        // Custom modules 

        // 3rd Party Modules

    ])
    .constant("version", { "v": 1 })
    .config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {

        // default route
        $urlRouterProvider.otherwise('/');

        $stateProvider
            .state('home', {
                url: '/',
                templateUrl: '/AngularApps/UsSales/views/Index.html',
                controller: 'controller'
            })
            .state('editGoodsIn', {
                url: '/editGoodsIn/{id:[0-9]{1,5}}',
                templateUrl: '/AngularApps/UsSales/views/editGoodsIn.html',
                controller: 'controller'
            })
            .state('editSales', {
                url: '/editSales/{order_no}',
                templateUrl: '/AngularApps/UsSales/views/editSales.html',
                controller: 'controller'
            })
        ;

    }]);
    
})();
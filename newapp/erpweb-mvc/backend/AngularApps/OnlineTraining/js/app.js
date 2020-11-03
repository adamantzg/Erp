(function () {
    'use strict';

    var app = angular.module('app', [
        // Angular modules 
        'ui.bootstrap', 'datatables', 'angular-plupload', 'ui.router', 'logToServer'

        // Custom modules 

        // 3rd Party Modules
        
    ]);
    app.config(function (pluploadOptionProvider) {
        /* Global settings*/
        pluploadOptionProvider.setOptions({
            flash_swf_url: '/Scripts/plupload-2.0.0/js/Moxie.swf',
            silverlight_xap_url: '/Scripts/plupload-2.0.0/js//Moxie.xap',
            max_file_size: '50mb'
        });
    });
    app.config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {

        // default route
        $urlRouterProvider.otherwise('/OnlineTraining/');

        $stateProvider
            .state('home', {
                url: '/OnlineTraining/',
                templateUrl: '/AngularApps/OnlineTraining/views/Index.html',
                controller: 'controller'
            })
            .state('upload', {
                url: '/OnlineTraining/upload',
                templateUrl: '/AngularApps/OnlineTraining/views/upload.html',
                controller: 'controller'
            })
            .state('edit', {
                url: '/OnlineTraining/edit/{id:[0-9]{1,5}}',
                templateUrl: '/AngularApps/OnlineTraining/views/upload.html',
                controller: 'controller'
            })
            .state('publish', {
                url: '/OnlineTraining/publish/{id:[0-9]{1,5}}',
                templateUrl: '/AngularApps/OnlineTraining/views/upload.html',
                controller: 'controller'
            });

        //.state('customer.detail.contact', {
        //    url: '^/customer/detail/contact/{customerId:[0-9]{1,5}}',
        //    templateUrl: 'app/customer/contact.html',
        //    controller: 'customerContactCtrl'
        //})
    }]);
    
})();
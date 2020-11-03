(function () {
    'use strict';

    var app = angular.module('app', [
        // Angular modules 
        'ui.bootstrap', 'datatables', 'ui.router', 'angular-plupload', 'logToServer'

        // Custom modules 

        // 3rd Party Modules

    ]);

    app.config(function (pluploadOptionProvider) {
        /* Global settings*/
        pluploadOptionProvider.setOptions({
            flash_swf_url: '/Scripts/plupload-2.0.0/js/Moxie.swf',
            silverlight_xap_url: '/Scripts/plupload-2.0.0/js//Moxie.xap',
            max_file_size: '10mb'
        });
    });

    app.constant("version", { "v": 0 });

    app.config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {

        // default route
        $urlRouterProvider.otherwise('/');

        $stateProvider
            .state('home', {
                url: '/',
                templateUrl: '/AngularApps/Manual/manuals/listManualsView.html',
                controller: 'listManualsCtrl',
                controllerAs: 'vm'
            })
            .state('edit', {
                url: '/edit/{id:[0-9]{1,5}}',
                templateUrl: '/AngularApps/Manual/manuals/editManualView.html',
                controller: 'editManualCtrl',
                controllerAs: 'vm'
            })
            .state('create', {
                url: '/create/',
                templateUrl: '/AngularApps/Manual/manuals/editManualView.html',
                controller: 'editManualCtrl',
                controllerAs:'vm'
            })
    }]);

})();
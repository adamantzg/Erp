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

    app.constant("version", { "v": 3 });
    app.constant("STATUS", {
        "NOTICE": {
            0: { "name": "Pending", "id": 0 },
            1: { "name": "Resolved", "id": 1 },
            2: { "name": "N/A", "id": 2 }
        }
    });
    app.constant("IMAGES", { "ROWS": 1, "COLUMNS": 6 });
    app.config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {

            // default route
            $urlRouterProvider.otherwise('/');

            $stateProvider
                .state('home', {
                    url: '/',
                    templateUrl: '/AngularApps/ChangeNoticeV2/views/index.html',
                    controller: 'controller'
                })
                .state('create', {
                    url: '/create/{id:[0-9]{1,5}}',
                    templateUrl: '/AngularApps/ChangeNoticeV2/views/edit.html',
                    controller: 'controller'
                })
                .state('edit', {
                    url: '/edit/{id:[0-9]*}',
                    templateUrl: '/AngularApps/ChangeNoticeV2/views/edit.html',
                    controller: 'controller'
                })
                .state('view', {
                    url: '/View/{id:[0-9]{1,5}}',
                    templateUrl: '/AngularApps/ChangeNoticeV2/views/view1.html',
                    controller: 'controllerView',
                      controllerAs:'vm'
                });
        }]);

})();
(function () {
    'use strict';

    var app = angular.module('app', [
        // Angular modules 
        'ui.bootstrap', 'datatables', 'angular-plupload', 'ui.router', 'bootstrapLightbox', 'logToServer'

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

    /*angular.module('app').config(function (LightboxProvider) {
        LightboxProvider.getImageUrl = function (image) {
            return image.insp_image;
        };
        
    });*/

    app.config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {

        // default route
        $urlRouterProvider.otherwise('/');

        $stateProvider
            .state('home', {
                url: '/',
                templateUrl: '/AngularApps/InspectionSample/views/Index.html',
                controller: 'controller'
            })
            .state('create', {
                url: '/create',
                templateUrl: '/AngularApps/InspectionSample/views/edit.html',
                controller: 'controller'
            })
            .state('edit', {
                url: '/edit/{id:[0-9]{1,5}}',
                templateUrl: '/AngularApps/InspectionSample/views/edit.html',
                controller: 'controller'
            })
        ;

    }]);

    app.directive('childline', renderChildLine);

    /*app.run(function ($rootScope, $templateCache) {
        $rootScope.$on('$viewContentLoaded', function () {
            $templateCache.removeAll();
        });
    });*/

    function renderChildLine($compile) {
        
        var directive = {};

        directive.restrict = 'A';
        directive.templateUrl = '/AngularApps/InspectionSample/views/linedetail.html';
        directive.transclude = true;
        directive.link = function (scope, element, attrs) {

        }
        return directive;
    }


})();
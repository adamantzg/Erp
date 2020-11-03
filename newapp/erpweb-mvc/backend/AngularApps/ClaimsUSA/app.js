(function () {
    'use strict';

    angular.module('app', [
        // Angular modules 
        //'ngRoute'

        // Custom modules 
        'ngAnimate',
        //'ngSanitize',
        // 3rd Party Modules
         'ui.router',
        'ui.bootstrap',
        'angular-plupload', 'logToServer'
    ])
    .config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {
        $urlRouterProvider.otherwise('/');
        console.log("ROUTER PROVIDER");
        $stateProvider
          .state('create', {
              url:'/create',
              views: {
                  'report@': {
                      templateUrl: '/AngularApps/ClaimsUSA/feedbacks/returnFeedbacksView.html',
                      controller: 'returnFeedbacks'
                  }
              }
          })
        .state('list', {
            url: '/',
            views: {
                'report@': {
                    templateUrl: '/AngularApps/ClaimsUSA/feedbacks/listFeedbacksView.html',
                    controller: 'listFeedbacks'
                }
            }
        })
       

    }])
    .config(function (pluploadOptionProvider) {
        pluploadOptionProvider.setOptions({
            flash_swf_url: '/Scripts/plupload-2.0.0/js/Moxie.swf',
            silverlight_xap_url: '/Scripts/plupload-2.0.0/js//Moxie.xap',
            max_file_size: '10mb'
        });
    });
    
})();
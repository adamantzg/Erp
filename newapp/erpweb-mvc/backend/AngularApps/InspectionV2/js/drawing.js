
angular.module('app').config(['$urlRouterProvider', '$stateProvider',  function ($urlRouterProvider, $stateProvider) {
    
    $urlRouterProvider.otherwise('/');

    
    $stateProvider
        .state('uploadDrawing', {
            url: '/:id',
            templateUrl: '/AngularApps/InspectionV2/views/uploaddrawing.html',
            controller: 'uploadCtrl'            
        });

}]);
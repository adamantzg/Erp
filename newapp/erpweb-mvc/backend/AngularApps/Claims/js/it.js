var claim_type = 6;
//var param = paramId;

angular.module('app').config(['$urlRouterProvider', '$stateProvider', '$provide', function ($urlRouterProvider, $stateProvider, $provide) {
    // default route
    /** Getting from MVC View ITFedbacks.cshtml  **/
    $provide.constant('feedbackId', paramId)
    //$provide.constant('baseUrl', '/api/it/')
    //$provide.constant('importance', { 'low': 5, 'medium': 6, 'high': 7 })
    $provide.constant('feedbackTypeIt', 6);

    //$urlRouterProvider.otherwise('/');
    $urlRouterProvider.otherwise(function ($injector, $location) {
        var $state = $injector.get("$state");
        $state.go('home');
    });

    console.log("Parametar: ", paramId);

    $stateProvider
        .state('home', {
            url: '/',
            templateUrl: '/AngularApps/Claims/views/it/itFeedbackAllView.html',
            controller: ['$state', '$stateParams', 'feedbackId', function ($state, $stateParams, feedbackId) {
                location.href = '/Claims/Itfeedbacks';
                event.preventDefault();

            }],
            controllerAs: 'vm'
        })
        .state('create', {
            url: '/create',
            templateUrl: '/AngularApps/Claims/views/it/itFeedbackCreateView.html',
            controllerAs: 'vm',
            controller: 'controller'

        })
        .state('edit', {
            url: '/edit/{id: [0-9]{1,5}}',
            templateUrl: '/AngularApps/Claims/views/it/itFeedbackEditView.html',
            controller: 'controller',
            controllerAs:'vm'
        })
        ;

}]);
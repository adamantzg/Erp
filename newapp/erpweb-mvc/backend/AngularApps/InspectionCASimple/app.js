(function () {
    'use strict'
    angular.module('app', ['angular-plupload', 'ui.bootstrap', 'ui.router', 'ngAnimate','logToServer'])
        .constant('MAX', 4)
        .constant('MIN', 0)
        .constant('FEEDBACK_CATEGORY', { 'INSPECTION_FINDINGS': 6, 'CLIENT_FEEDBACK': 7, 'CLIENT_CLIME': 8 })
        .constant('DEFAULT_PATH', '/AngularApps/InspectionCASimple/')
        .constant("FILE_CATEGORY", {
            "REJECTION": null,
            "RETURNED_BY_FACTORY": 1,
            "RECHEK_PHOTOS": 2
        })
        .constant("RECHECK_STATUS", {
            "OK": 1,
            "NO": 0,
            "NA":null
        })
        .config(['$urlRouterProvider', '$stateProvider','DEFAULT_PATH', function ($urlRouterProvider, $stateProvider,DEFAULT_PATH) {
            $urlRouterProvider.otherwise('/create');
            $stateProvider
                .state('create', {
                    url: '/create',
                    views: {
                        'navigation@': {
                            templateUrl: DEFAULT_PATH + 'Navigation/navigatonView.html',
                            controller: 'navigationController',
                            controllerAs:'nav'
                        },
                        'create@': {
                            templateUrl: DEFAULT_PATH + 'Inspections/createInpspectionCAView.html',
                            controller: 'createCACtrl',
                            controllerAs:'vm'
                        }
                    }
                })
                .state('rechecks', {
                    url: '/rechecks',
                    views: {
                        'navigation@': {
                            templateUrl: DEFAULT_PATH + 'Navigation/navigatonView.html',
                            controller: 'navigationController',
                            controllerAs: 'nav'
                        },
                        'create@': {
                            templateUrl: DEFAULT_PATH + 'Recheck/listRechecksView.html',
                            controller: 'listRechecksController',
                            controllerAs:'vm'
                        }
                    }
                })
                .state('rechechekEdit', {
                    url: '/recheks/edit/:id',
                    views: {
                        'navigation@': {
                            templateUrl: DEFAULT_PATH + 'Navigation/navigatonView.html',
                            controller: 'navigationController',
                            controllerAs: 'nav'
                        },
                        'create@': {
                            templateUrl: DEFAULT_PATH + 'Recheck/editRecheckView.html',
                            controller: 'editRecheckController',
                            controllerAs: 'vm'
                        }
                    }
                })
                .state('resolved', {
                    url: '/resolved',
                    views: {
                        'navigation@': {
                            templateUrl: DEFAULT_PATH + 'Navigation/navigatonView.html',
                            controller: 'navigationController',
                            controllerAs: 'nav'
                        },
                        'create@': {
                            templateUrl: DEFAULT_PATH + 'Resolved/listResolvedView.html',
                            controller: 'listResolvedController',
                            controllerAs: 'vm'
                        }
                    }
                })
                .state('resolvedDetail', {
                    url: '/resolved/detail/:id',
                    views: {
                        'navigation@': {
                            templateUrl: DEFAULT_PATH + 'Navigation/navigatonView.html',
                            controller: 'navigationController',
                            controllerAs: 'nav'
                        },
                        'create@': {
                            templateUrl: DEFAULT_PATH + 'Resolved/detailResolved.html',
                            controller: 'detailResolvedController',
                            controllerAs: 'vm'
                        }
                    }
                })
        }])
        //.config(function (pluploadOptionProvider) {
        //    // global setting
        //    pluploadOptionProvider.setOptions({
        //        flash_swf_url: '/Scripts/plupload-2.0.0/js/Moxie.swf',
        //        silverlight_xap_url: '/Scripts/plupload-2.0.0/js//Moxie.xap',
        //        max_file_size: '10mb'
        //    });
        //});


})();
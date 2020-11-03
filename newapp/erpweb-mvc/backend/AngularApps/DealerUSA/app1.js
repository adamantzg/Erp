(function () {
    'use strict';

    angular.module('app', [
        // Angular modules


        // Custom modules
        'ngAnimate',
        'ngSanitize',
        // 3rd Party Modules
        'ui.router',
        'ui.bootstrap',
        'datatables',
        'angular-plupload',
        'logToServer',
        'angularjs-dropdown-multiselect',
        'mgcrea.ngStrap'
    ])
    .constant("defaultPaths",{
        "DEALER" : {"URL":"/AngularApps/DealerUSA/" },
        "CLAIMS" :{ "FOLDER"  :"claims"}
    }
    )
    .constant("version", {"v":9})
    .constant("types", { /** the walues is the same in database **/
        "calls": {"in":1,"out":2},
        "chat": { "phone": 1, "mail": 2 },
        "status": { "closed": 1, "open": 0 },
        "showTest":false
    })
    .config(['$urlRouterProvider', '$stateProvider','version','defaultPaths', function ($urlRouterProvider, $stateProvider,version,defaultPaths) {
        var claims = defaultPaths.CLAIMS.FOLDER;
        
        $urlRouterProvider.otherwise('/');        
        $stateProvider
            .state('list', {
                url: '/:brsLink',
                views: {
                    'dealers@': {
                        // url: '/',

                        templateUrl: defaultPaths.DEALER.URL + 'dealers/dealerIndexView1.html?p=' + version.v,
                        controller: 'dealerListCtrl'
                    },
                    'navigation@': {
                        templateUrl: defaultPaths.DEALER.URL + 'navigation-tpl.html?p=' + version.v,
                        controller: 'navigationCtrl'
                    }
                },
                resolve: {
                    common: function (common) {
                        common.nav = " Dealer List";
                        common.title = "DEALER MANAGEMENT";
                        return common;
                    },
                    //factory:"factory",
                    dealers: function (factory, common) {
                        var d = [];
                        if (common.getDealers().length > 0) {
                            return common.getDealers();
                        } else {
                            return factory.getDealers().then(
                                function (request) {
                                    common.setDealers(request.data);
                                    return request.data;
                                }
                            );
                        }
                    }
                }
            })
            .state('detail', {
                url: '/detail/:id',
                views: {
                    'dealers@': {
                        // url: '/',
                        templateUrl: defaultPaths.DEALER.URL + 'dealers/dealerDetailView8.html?p='+version.v,
                        controller: 'dealerDetailCtrl',


                    },
                    'navigation@': {
                        templateUrl: defaultPaths.DEALER.URL + 'navigation-tpl.html?p='+version.v,
                        controller: 'navigationCtrl'
                    }
                },
                resolve: {
                    common: function (common) {
                        common.root = "details";
                        common.nav = " Dealer details";
                        common.title = "DEALER MANAGEMENT";
                        return common;
                    }


                }

                //controllerAs: 'vm'
            })
            .state('detailAlpha', {
                url: '/detailalpha/:id',
                views: {
                    'dealers@': {
                        // url: '/',
                        templateUrl: defaultPaths.DEALER.URL + 'dealers/dealerDetailView8.html?p='+version.v,
                        controller: 'dealerDetailAlphaCtrl',


                    },
                    'navigation@': {
                        templateUrl: defaultPaths.DEALER.URL + 'navigation-tpl.html?p='+version.v,
                        controller: 'navigationCtrl'
                    }
                },
                resolve: {
                    common: function (common) {
                        common.root = "details";
                        common.nav = " Dealer details";
                        common.title = "DEALER MANAGEMENT";
                        return common;
                    }


                }

                //controllerAs: 'vm'
            })
            .state('create', {
                url: '/create-claim/:dealerId',
                views: {
                    'dealers@': {
                        templateUrl: defaultPaths.DEALER.URL + claims +'/returnFeedbacksView.html?p='+version.v,
                        controller: 'returnFeedbacks'
                    },
                    'navigation@': {
                        templateUrl: defaultPaths.DEALER.URL + 'navigation-tpl.html?p=' + version.v,
                        controller: 'navigationCtrl'
                    }
                },
                resolve: {
                    common: function (common) {
                        common.root="claims"
                        common.nav = " Create Claim";
                        common.title = "CLAIMS / FEEDBACKS"
                        return common;
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
        })
    .config(function ($popoverProvider) {
        angular.extend($popoverProvider.defaults, {
            html: true
        });
    })
})();
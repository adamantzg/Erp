(function () {
    'use strict';

    var app = angular.module('app', [
        // Angular modules 
        'ui.bootstrap', 'datatables', 'ui.router'

    ]).config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {

        // default route
        $urlRouterProvider.otherwise('/');

        $stateProvider
            .state('home', {
                url: '/',
                templateUrl: '/AngularApps/Claims/views/statistics/index.html',
                controller: 'controller'
            })
            .state('summary', {
                url: '/summary',
                templateUrl: '/AngularApps/Claims/views/statistics/summary.html',
                controller: 'controller'
            });

    }]);
    
    
})();


(function () {
    'use strict';
    
    angular
        .module('app')
        .controller('controller', controller);

    controller.$inject = ['$scope', '$timeout', '$compile','$state', '$stateParams', 'factory'];

    function controller($scope, $timeout, $compile, $state, $stateParams, factory) {

        var startOffset = 1, endOffset = 1;
        if ($state.current.name == 'summary')
        {
            startOffset = 24;
            endOffset = 0;
            $scope.tabSelection = {0: true};
            for (var i = 1; i < 7; i++)
                $scope.tabSelection[i] = false;
            $scope.activeTab = 0;
            
        }

        $scope.monthStart = moment().subtract(startOffset, 'month').startOf('month').toDate();
        $scope.monthEnd = moment().subtract(endOffset, 'month').endOf('month').toDate();
                

        $scope.showData = function () {
            $scope.monthEnd = moment($scope.monthEnd).endOf('month').toDate();
            factory.getStatistics($scope.monthStart, $scope.monthEnd).then(function (response) {
                $scope.data = response.data.data;
                factorySubData = {};
            });
        };

        var factorySubData = {};

        $scope.dtInstance = {};
        $scope.dtOptions = { paging: false, searching: false };

        $scope.monthSelectorOptions = {
            datepickerMode: 'month',
            minMode: 'month',
            monthColumns: 4
        };

        $scope.format = function (value, decimals) {
            if (typeof decimals === 'undefined')
                decimals = 2;
            return number_format(value, decimals);
        };

        $scope.expand = function (l, index) {

            l.expanded = l.expanded == null ? true : !l.expanded;
            var dtRow = $scope.dtInstance.DataTable.row(index);
            if (l.expanded) {
                var html = '';
                if (l.brand in factorySubData)
                    html = factorySubData[l.brand];
                else
                {
                    var scope = $scope.$new(true);
                    scope.data = l.subData;
                    scope.otherValue = l.otherValue;
                    scope.format = $scope.format;
                    scope.dtOptions = {
                        paging: false,
                        searching: false,
                        order: [2, 'desc'],
                        columnDefs: [
                        {
                            orderable: false,
                            searchable: false,
                            targets: 0
                        }]
                    };
                    scope.dtInstance = {};
                    html = $compile('<div brandline></div>')(scope);
                    factorySubData[l.brand] = html;
                }
                
                dtRow.child(html).show();
            }
            else {
                dtRow.child().hide();
            }

        };

        if($state.current.name == 'home')
            $scope.showData();

        $scope.summaryTabSelected = function(index)
        {
            if (!$scope.tabSelection[index])
            {
                loadSummaryTab(index);
                $scope.tabSelection[index] = true;
            }

            $scope.activeTab = index;
                
        }

        $scope.sum = function(arr, iteratee)
        {
            return _.sumBy(arr, iteratee);
        }

        function loadSummaryTab(index)
        {
            switch(index)  
            {
                case 0:
                    loadBrands();
                    $scope.brandChartUrl = factory.getChartUrl(0,$scope.monthStart, $scope.monthEnd);
                    break;
                case 1:
                    loadReasons();
                    $scope.reasonChartUrl = factory.getChartUrl(1, $scope.monthStart, $scope.monthEnd);
                    break;
                case 2:
                    loadDecisions();
                    $scope.decisionByMonthChartUrl = factory.getChartUrl(2, $scope.monthStart, $scope.monthEnd,$scope.filter.brand_id, $scope.filter.reason_id, $scope.filter.decision_id);
                    break;
                case 3:
                    loadBrandByMonth();
                    $scope.brandByMonthChartUrl = factory.getChartUrl(3, $scope.monthStart, $scope.monthEnd, $scope.filter.brand_id, $scope.filter.reason_id, $scope.filter.decision_id);
                    break;
                case 4:
                    loadReasonByMonth();
                    $scope.reasonByMonthChartUrl = factory.getChartUrl(4, $scope.monthStart, $scope.monthEnd, $scope.filter.brand_id, $scope.filter.reason_id, $scope.filter.decision_id);
                    break;
                case 5:
                    loadDistributorPercentageData();
                    $scope.percentageByDistributorChartUrl = factory.getChartUrl(5, $scope.monthStart, $scope.monthEnd);
                    break;
                case 6:
                    loadFactoryPercentageData();
                    $scope.percentageByfactoryChartUrl = factory.getChartUrl(6, $scope.monthStart, $scope.monthEnd);
                    break;
            }
        }

        $scope.showSummaryData = function () {
            loadSummaryDropdowns();
            for(var key in $scope.tabSelection)
            {
                var iKey = parseInt(key);
                if ($scope.tabSelection[iKey])
                    loadSummaryTab(iKey);
            }
        };

        function loadBrands()
        {
            $scope.monthEnd = moment($scope.monthEnd).endOf('month').toDate();
            factory.getBrandStats($scope.monthStart, $scope.monthEnd).then(function (response) {
                $scope.brandStats = response.data;                
            });
        }

        function loadReasons() {
            $scope.monthEnd = moment($scope.monthEnd).endOf('month').toDate();
            factory.getReasonStats($scope.monthStart, $scope.monthEnd).then(function (response) {
                $scope.reasonStats = response.data;
            });
        }

        function loadDecisions() {
            $scope.monthEnd = moment($scope.monthEnd).endOf('month').toDate();
            factory.getDecisionStats($scope.monthStart, $scope.monthEnd, $scope.filter.brand_id, $scope.filter.reason_id).then(function (response) {
                $scope.decisionMonthlyStats = response.data;
            });
        }

        function loadBrandByMonth() {
            $scope.monthEnd = moment($scope.monthEnd).endOf('month').toDate();
            factory.getBrandByMonthStats($scope.monthStart, $scope.monthEnd, $scope.filter.reason_id, $scope.filter.decision_id).then(function (response) {
                $scope.brandMonthlyStats = response.data;
            });
        }

        function loadReasonByMonth() {
            $scope.monthEnd = moment($scope.monthEnd).endOf('month').toDate();
            factory.getReasonByMonthStats($scope.monthStart, $scope.monthEnd,$scope.filter.brand_id, $scope.filter.decision_id).then(function (response) {
                $scope.reasonMonthlyStats = response.data;
            });
        }

        function loadDistributorPercentageData() {
            $scope.monthEnd = moment($scope.monthEnd).endOf('month').toDate();
            factory.getDistributorPercentageStats($scope.monthStart, $scope.monthEnd).then(function (response) {
                $scope.distributorPercentageStats = response.data;
            });
        }

        function loadFactoryPercentageData() {
            $scope.monthEnd = moment($scope.monthEnd).endOf('month').toDate();
            factory.getFactoryPercentageStats($scope.monthStart, $scope.monthEnd).then(function (response) {
                $scope.factoryPercentageStats = response.data;
            });
        }

        if ($state.current.name == 'summary')
        {
            loadSummaryTab(0);
            loadSummaryDropdowns();

            $scope.filter = {brand_id: null, reason_id: null, decision_id: 1};

            $scope.disableBrandDropdown = function () {
                var active = $scope.activeTab;
                return active != 2 && active != 4;
            };

            $scope.disableReasonDropdown = function () {
                var active = $scope.activeTab;
                return active != 2 && active != 3;
            };

            $scope.disableDecisionDropdown = function () {
                var active = $scope.activeTab;
                return active != 3 && active != 4;
            };
            $scope.exportData = function () {
                $scope.monthEnd = moment($scope.monthEnd).endOf('month').toDate();
                location.href = '/Claims/GetClaimRawData/?dateFrom=' + moment($scope.monthStart).format('YYYY-MM-DD') + '&dateTo='
                    + moment($scope.monthEnd).format('YYYY-MM-DD') + '&reason_id=' + $scope.filter.reason_id + '&decision_id=' + $scope.filter.decision_id + '&brand_id=' + $scope.filter.brand_id;
            };
        }

        function loadSummaryDropdowns() {
            factory.getBrands($scope.monthStart, $scope.monthEnd).then(function (response) {
                $scope.brands = response.data;
                $scope.brands.splice(0, 0, { brand_id: null, brandname: '<Any>' });
            });

            factory.getReasons($scope.monthStart, $scope.monthEnd).then(function (response) {
                $scope.reasons = response.data;
                $scope.reasons.splice(0, 0, { returncategory_id: null, category_name: '<Any>' });
            });

            factory.getDecisions($scope.monthStart, $scope.monthEnd).then(function (response) {
                $scope.decisions = response.data;
                $scope.decisions.splice(0, 0, { code: null, description: '<Any>' });
            });
        }
            
    }

})();

angular.module('app').directive('brandline', renderBrandLine);

/*app.run(function ($rootScope, $templateCache) {
    $rootScope.$on('$viewContentLoaded', function () {
        $templateCache.removeAll();
    });
});*/

function renderBrandLine($compile) {

    var directive = {};

    directive.restrict = 'A';
    directive.templateUrl = '/AngularApps/Claims/views/statistics/brandline.html';
    directive.transclude = true;
    directive.link = function (scope, element, attrs) {
        
        var productSubData = {};        

        scope.expand = function (l, index) {

            l.expanded = l.expanded == null ? true : !l.expanded;
            var dtRow = scope.dtInstance.DataTable.row(index);
            if (l.expanded) {
                var html = '';
                if (l.factory_code in productSubData)
                    html = productSubData[l.factory_code];
                else
                {
                    var newScope = scope.$new(true);
                    newScope.data = l.subData;
                    newScope.otherValue = l.otherValue;
                    newScope.dtOptions =
                        {
                            paging: false,
                            searching: false,
                            order: [3, 'desc'],
                        };
                    
                    newScope.format = scope.format;
                    html = $compile('<div factoryline></div>')(newScope);
                    productSubData[l.factory_code] = html;
                }
                dtRow.child(html).show();
            }
            else {
                dtRow.child().hide();
            }

        };
    }
    return directive;
}

angular.module('app').directive('factoryline', renderFactoryLine);

/*app.run(function ($rootScope, $templateCache) {
    $rootScope.$on('$viewContentLoaded', function () {
        $templateCache.removeAll();
    });
});*/

function renderFactoryLine($compile) {

    var directive = {};

    directive.restrict = 'A';
    directive.templateUrl = '/AngularApps/Claims/views/statistics/factoryline.html';
    directive.transclude = true;
    directive.link = function (scope, element, attrs) {

    }
    return directive;
}
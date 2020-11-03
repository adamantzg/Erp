(function () {
    'use strict';

    angular
        .module('app')
        .controller('controller', controller);

    controller.$inject = ['$scope', '$location', '$timeout', 'factory', 'usSpinnerService'];

    function controller($scope, $location, $timeout, factory, usSpinnerService) {
        $scope.factories = [];
        var end_date = moment();
        var start_date = moment().add(-12, 'M');
        $scope.from = { month: start_date.month()+1, year: start_date.year() };
        $scope.to = { month: end_date.month()+1, year: end_date.year() };
        $scope.years = [];
        $scope.months = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
        $scope.checkAll = true;
        $scope.chartSrc = '';
        $scope.byOrderDate = true;
        $scope.byETD = true;
        $scope.byETA = true;
        $scope.clientsLoading = false;
        $scope.productsLoading = false;

        factory.getFactories().then(function () {
            $scope.factories = _.filter(factory.factories, function (elem) {
                return elem.factory_code.length > 0;
            });
        });

        $scope.factorySelected = function ()
        {
            $scope.clientsLoading = true;
            usSpinnerService.spin('spinner-1');
            factory.getClients($scope.factory.user_id).then(function () {
                $scope.clientsLoading = false;
                usSpinnerService.stop('spinner-1');
                var allClient = { user_id: null, customer_code: '(All)' };
                factory.clients.splice(0, 0, allClient);
                $scope.clients = factory.clients;
                $scope.client = allClient;
            });
        }

        $scope.showProducts = function () {
            $scope.productsLoading = true;
            usSpinnerService.spin('spinner-2');
            factory.getProducts($scope.factory.user_id, $scope.client.user_id, $scope.category1 != null ? $scope.category1.category1_id : null).then(function () {
                $scope.productsLoading = false;
                usSpinnerService.stop('spinner-2');
                $scope.products = factory.products;
            });
        };

        $scope.checkShowProducts = function () {
            return $scope.factory != null && $scope.factory.user_id > 0 &&
                $scope.client != null;
        };

        $scope.checkShowGenerateChart = function () {
            return $scope.products != null && _.filter($scope.products, 'selected').length > 0;
        };

        $scope.generateChart = function () {
            $scope.showChart = true;
            usSpinnerService.spin('spinner-3');
            var prodIds = _.map(_.filter($scope.products, { selected: true }), $scope.client.user_id != null ? 'cprod_id' : 'cprod_mast').join(',');
            var chartSrc = '/Report/GenerateOrdersChart/?';
            var params = [];
            params.push('productIds=' + prodIds);
            params.push('m21From=' + toMonth21($scope.from.year, $scope.from.month));
            params.push('m21To=' + toMonth21($scope.to.year, $scope.to.month));
            params.push('showByOrderDate=' + $scope.byOrderDate.toString());
            params.push('showByETD=' + $scope.byETD.toString());
            params.push('showByETA=' + $scope.byETA.toString());
            if ($scope.client.user_id == null)
                params.push('factoryProducts=true');
            chartSrc += params.join('&');
            $scope.chartSrc = chartSrc;
            
        };

        $scope.chartLoaded = function () {
            usSpinnerService.stop('spinner-3');
        };

        $scope.toggleCheckAll = function () {
            
            var checkAll = this.checkAll;
            $scope.products.forEach(function (p) {
                p.selected = checkAll;
            });
        };

        factory.getCategories().then(function () {
            
            var allCats = { category1_id: null, cat1_name: '(All)' };
            factory.categories.splice(0, 0, allCats);
            $scope.categories = factory.categories;            
            $scope.category1 = allCats;
        });

        for (var i = 1990; i < end_date.year()+3; i++) {
            $scope.years.push(i);
        }

        $scope.productTableOptions =
        {
            'scrollY': 400,
            processing: true,
            paging: false,
            order: [1, 'asc'],
            columnDefs: [
            {
                orderable: false,
                searchable: false,
                targets: 0,
                className: 'dt-body-center'
            }]
        };
        

    }
})();

angular.module('app').controller('CreateCtrl', ['$scope', '$timeout','factory', function ($scope, $timeout,factory) {
    $scope.popupOpened = { 1: false, 2: false };
    $scope.openPopup = function (which) {
        $scope.popupOpened[which] = !$scope.popupOpened[which];
    };

    $scope.progress = 0;
    $scope.inProgress = false;
    $scope.orderCount = null;
    $scope.calculations = [];
    $scope.ordersLoading = false;
    $scope.allowSave = false;

    $scope.from = moment().subtract(6, 'month').toDate();
    $scope.to = moment().toDate();
    var currentOrder = 0;

    $scope.getCalculations = function () {
        $scope.ordersLoading = true;
        factory.getOrders($scope.from, $scope.to).then(function (response) {
            $scope.ordersLoading = false;
            $scope.orders = response.data;
            $scope.orderCount = $scope.orders.length;
            startCalculations();
        });
    };

    $scope.clearCalculations = function()
    {
        $scope.calculations = [];
    }

    $scope.formatDate = function(d)
    {
        return moment(d).format('DD/MM/YYYY');
    }

    $scope.format = function(num)
    {
        return number_format(num, 2);
    }

    $scope.stopCalculations = function () {
        $scope.inProgress = false;
    };

    function startCalculations()
    {
        currentOrder = 0;
        $scope.inProgress = true;
        $timeout(getCalculation, 500);
        
    }

    function getCalculation()
    {
        if (currentOrder < $scope.orders.length) {
            factory.getCalculation($scope.orders[currentOrder].orderid, $scope.allowSave).then(function (response) {
                $scope.calculations.push(response.data);
                $scope.progress = $scope.calculations.length / $scope.orders.length * 100;
                if ($scope.inProgress) {
                    currentOrder++;            
                    getCalculation();
                }
            });
        }
        else
        {
            if (currentOrder > $scope.orders.length)
                currentOrder = $scope.orders.length;
            $scope.inProgress = false;
        }
            
    }

    $scope.getProgressCaption = function () {
        if($scope.orders != null)
            return (currentOrder + 1).toString() + '/' + $scope.orders.length.toString();
        return '';
    };
}]);

angular.module('app').controller('CalculateCtrl', ['$scope', '$timeout', 'factory', function ($scope, $timeout, factory) {
    $scope.getOrdersByText = function (text) {
        return factory.getOrdersByText(text).then(function (response) {
            return response.data;
        });
    };

    $scope.calculate = function () {
        factory.calculateForOrder($scope.order.orderid).then(function (response) {
            $scope.calculation = response.data;
        });
    };
}]);
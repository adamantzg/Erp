angular.module('app').factory('factory', ['$http', function ($http) {
    var factory = {};
    var baseUrl = '/api/order/';

    factory.getOrders = function (from, to) {
        return $http.get(baseUrl + 'getByCriteria', { params: { from: from, to: to } });
    };

    factory.getCalculation = function(id,allowSave)
    {
        return $http.get(baseUrl + 'getCalculation', { params: { orderid: id,save: allowSave } });
    }

    factory.getOrdersByText = function(text)
    {
        return $http.get(baseUrl + 'getOrdersByText', { params: { text: text } });
    }

    factory.calculateForOrder = function(orderid)
    {
        return $http.get(baseUrl + 'calculateForOrder', { params: { orderid: orderid } });
    }

    return factory;
}]);
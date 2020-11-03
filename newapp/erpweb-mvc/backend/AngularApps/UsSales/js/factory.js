(function () {
    'use strict';

    
    angular
        .module('app')
        .factory('factory', factory);

    factory.$inject = ['$http'];

    function factory($http) {
        var baseUrl = '/api/ussales/';
        var service = {};

        service.getGoodsInOrders = function () {
            return $http.get(baseUrl + 'getGoodsInOrders').then(function (response) {
                /*response.data.forEach(function (o) {
                    if (o.req_eta != null)
                        o.req_eta = moment(o.req_eta).toDate();
                    if (o.orderdate != null)
                        o.orderdate = moment(o.orderdate).toDate();
                });*/
                service.goodsInOrders = response.data;
            });
        };

        service.getGoodsInOrder = function (orderid) {
            return $http.get(baseUrl + 'getGoodsInOrder', { params: { orderid: orderid } }).then(function (response) {
                service.goodsInOrder = response.data;
            });
        };

        service.updateGoodsInOrder = function (order) {
            return $http.post(baseUrl + 'updateGoodsInOrder', order).then(function (response) {
            });
            
        };

        service.getSalesOutOrders = function () {
            return $http.get(baseUrl + 'getSalesOutOrders').then(function (response) {
                service.salesOutOrders = response.data;
            });
        };

        service.getSalesOrder = function (order_no) {
            return $http.get(baseUrl + 'getSalesOrder', { params: { order_no: order_no } }).then(function (response) {
                service.salesOrder = response.data;
            });
        };

        service.updateSalesOrder = function(order)
        {
            return $http.post(baseUrl + 'updateSalesOrder', order).then(function (response) {
            });
        }


        return service;
    }

})();
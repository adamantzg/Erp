(function () {
    'use strict';

    angular
        .module('app')
        .factory('factory', factory);

    factory.$inject = ['$http'];

    function factory($http) {
        var service = {};
        var base = '/';
        

        service.getFactories = function () {
            return $http.get(base + 'Order/GetFactories').then(function (response) {
                service.factories = response.data;
            });
        };
        service.getClients = function (factory_id) {
            return $http.get(base + 'Order/GetClientsForFactory', { params: { factory_id: factory_id } }).then(function (response) {
                service.clients = response.data;
            });
        };
        service.getCategories = function () {
            return $http.get(base + 'Order/GetCategories').then(function (response) {
                service.categories = response.data;
            });
        };

        service.getProducts = function (factory_id, client_id, category_id) {
            return $http.get(base + 'Report/GetProductsByCriteria', {
                params: {
                    factory_id: factory_id,
                    client_id: client_id,
                    category_id: category_id
                }
            }).then(function (response) {
                response.data.forEach(function (elem) {
                    elem.selected = true;
                });
                service.products = response.data;

            });
        };

        

        return service;

        
    }
})();
(function () {
    'use strict';

    angular
        .module('app')
        .factory('factory', factory);

    factory.$inject = ['$http'];

    function factory($http) {
        var baseUrl = '/api/tcdocument/';
        var service = {};
        
        service.getDocuments = function () {
            return $http.get(baseUrl + 'get').then(function (response) {
                service.documents = response.data;
            });
        }

        service.getDocument = function (id) {
            return $http.get(baseUrl + 'getById', { params: {id: id}}).then(function (response) {
                service.documents = response.data;
            });
        }

        service.create = function (documents) {
            return $http.post(baseUrl + 'create', documents);
        };

        service.update = function (documents) {
            return $http.post(baseUrl + 'update', documents);
        };


        service.getUsers = function () {
            return $http.get(baseUrl + 'users').then(function (response) {
                service.users = response.data;
            });
        };

        service.delete = function (id) {
            
            return $http.delete(baseUrl + 'delete?id=' + id);
        };

        return service;
    }
})();
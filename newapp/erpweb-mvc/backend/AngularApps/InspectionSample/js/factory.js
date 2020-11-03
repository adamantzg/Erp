(function () {
    'use strict';

    
    angular
        .module('app')
        .factory('factory', factory);

    factory.$inject = ['$http'];

    function factory($http) {
        var baseUrl = '/api/inspection/';
        var service = {};

        service.new_line_id = -1;

        service.getModel = function (state,id) {
            return $http.get(baseUrl + 'getSIModel', { params: { state: state, id: id } }).then(function (response) {
                service.model = response.data;
            });
        };

        service.getFactories = function () {
            return $http.get(baseUrl + 'getFactories').then(function (response) {
                service.factories = response.data;
            });
        };

        service.getClients = function () {
            return $http.get(baseUrl + 'getClients').then(function (response) {
                service.clients = response.data;
            });
        };

        service.getSubjects = function () {
            return $http.get(baseUrl + 'getSubjects').then(function (response) {
                service.subjects = response.data;
            });
        };

        service.createInspection = function () {
            return {
                factory_id: null, client_id: null, dateCreated: new Date(), type: 6, si_subject_id: null, startdate: null, days: 1, insp_status: 0,
                controllers: [], lines: []
            };
        };

        service.createLine = function () {
            return { id: service.new_line_id--, insp_mastproduct_code: '', insp_custproduct_code: '', insp_custproduct_name : '', orderqty: 0, cprod_id: null};
        };

        service.getInspection = function (id) {
            return $http.get(baseUrl + 'get', { params: { id: id } }).then(function (response) {
                service.insp = response.data;
            });
        };

        service.getInspections = function (date_from,date_to,factory_id, client_id) {
            return $http.get(baseUrl + 'getbycriteria', { params: {types: '6',dateFrom: date_from,dateTo: date_to, factory_id: factory_id, client_id: client_id}}).then(function (response) {
                service.inspections = response.data;
            });
        }

        service.getControllers = function (prefixText) {
            return $http.get(baseUrl + 'getControllers', { params: { prefixText: prefixText } }).then(function (response) {
                service.controllers = response.data;
            });
        };

        service.create = function (insp) {
            return $http.post(baseUrl + 'create', insp).then(function (response) {
                service.insp = response.data;
            });
        };

        service.update = function (insp) {
            return $http.post(baseUrl + 'update', insp).then(function (response) {
                service.insp = response.data;
            });
        };

        service.delete = function (id) {
            return $http.delete(baseUrl + 'delete?id=' + id);
        };

        service.getProducts = function (term) {
            return $http.get(baseUrl + 'getProducts', { params: { prefixText: term } }).then(function (response) {
                service.products = response.data;
            });
        };

        service.getRole = function () {
            return $http.get(baseUrl + 'getRole').then(function (response) {
                service.role = response.data;
            });
        };

        service.changeStatus = function (id) {
            return $http.put(baseUrl + 'changeStatus?id=' + id.toString()).then(function (response) {
                service.status = response.data;
            });
        };


        return service;
    }

})();
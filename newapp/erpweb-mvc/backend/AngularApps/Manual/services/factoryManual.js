(function () {
    'use strict';

    angular
        .module('app')
        .factory('factoryManual', ['$http', factoryManual]);

    function factoryManual($http) {
        var defaultURL = '/api/manualv2/';
        var service = {
            getModel: getModel,
            getManuals: getManuals,
            getManual: getManual,
            copyManual: copyManual,
            deleteManual: deleteManual,
            createManual: createManual,
            updateManual: updateManual,
            insertManualNode: insertManualNode,
            updateManualNode: updateManualNode,
            deleteManualNode: deleteManualNode
        };

        function getModel(state, id) {
            return $http.get(defaultURL + 'GetManualV2Model', { params: {state: state, id: id}}).then(function (response) {
                service.model = response.data;
            });
        }

        function getManuals() {
            return $http.get(defaultURL + 'GetManuals').then(function (response) {
                return response.data;
            });
        }

        function getManual(manual_id) {
            return $http.get(defaultURL + 'GetManual', { params: { manual_id: manual_id } }).then(function (response) {
                service.manual = response.data;
            });
        }

        function copyManual(manual) {
            return $http.post(defaultURL + 'CopyManual', manual).then(function (response) {
                service.manual = response.data;
            });
        }

        function deleteManual(manual) {
            return $http.post(defaultURL + 'DeleteManual', manual).then(function (response) {
            });
        }

        function createManual(manual) {
            return $http.post(defaultURL + 'CreateManual', manual).then(function (response) {
                service.manual = response.data;
            });
        }

        function updateManual(manual) {
            return $http.put(defaultURL + 'UpdateManual', manual).then(function (response) {
                service.manual = response.data;
            });
        }

        function insertManualNode(manual,node) {
            return $http.post(defaultURL + 'InsertManualNode', manual, node).then(function (response) {
                service.node = response.data;
            });
        }

        function deleteManualNode(manual, node) {
            return $http.post(defaultURL + 'DeleteManualNode', manual, node).then(function (response) {

            });
        }

        function updateManualNode(manual, node) {
            return $http.post(defaultURL + 'UpdateManualNode', manual, node).then(function (response) {
                service.node = response.data;
            });
        }

        return service;
    }
})();

    
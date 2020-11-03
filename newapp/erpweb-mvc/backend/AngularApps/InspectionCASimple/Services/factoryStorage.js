(function () {
    'use strict';

    angular
        .module('app')
        .factory('factoryStorage', ['$http', factoryStore]);

    //factoryInspectionCASample.$inject = ['$http'];

    function factoryStore($http) {
        var defaultUrl = '/api/claims/';
        var modelProducts = [];
        var modelRecheck = {};
        var modelReseolved = {};
        var service = {
           
            test: 'TEST IDE',
            setModelProducts: setModelProducts,
            getModelProducts: getModelProducts,
            setRecheck: setRecheck,
            getRecheck: getRecheck,
            setResolved: setResolved,
            getResolved:getResolved

        };

        return service;

        function setModelProducts(products) {
            modelProducts = products;
        }
        function getModelProducts() {
            return modelProducts;
        }

        function setRecheck(recheck) {
            console.log("FACTORY: ", recheck);
            modelRecheck = angular.copy( recheck);
        }
        function getRecheck() {
            return modelRecheck;
        }

        function setResolved(resolved) {
            console.info("FACTORY: ", resolved);
            modelRecheck = angular.copy(resolved);
        }
        function getResolved() {
            return modelRecheck;
        }
    }
})();
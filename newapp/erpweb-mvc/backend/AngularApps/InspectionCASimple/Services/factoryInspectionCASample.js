(function () {
    'use strict';

    angular
        .module('app')
        .factory('factoryInspectionCASample',['$http', factoryInspectionCASample]);

    //factoryInspectionCASample.$inject = ['$http'];

    function factoryInspectionCASample($http) {
        var defaultUrl = '/api/claims/';
        var service = {
            getData: getData,
            getReturnNo: getReturnNo,
            getClientOrders: getClientOrders,
            getProducts:getProducts,
            getCategories: getCategories,
            getCprod: getCprod,
            saveNewCA: saveNewCA,
            getUser: getUser,
            getReturnCategories: getReturnCategories, 
            getReasonTemplates: getReasonTemplates,
            defaultUrl: defaultUrl,
            deleteTempFile: deleteTempFile,
            getRechecks: getRechecks,
            getFeedbackImages: getFeedbackImages,
            saveRecheck: saveRecheck,
            getImportances: getImportances,
            getResolved: getResolved,
            getRecheck: getRecheck,
            getRefSequence:getRefSequence
            
        };

        return service;
        function getImportances() {
           
            return $http.get(defaultUrl + 'importances').then(
                function (response) {
                    return response.data
                }
            )
        }

        function getRechecks() {
            
            return $http.get(defaultUrl + 'getRecheks').then(
                function (response) {
                    return response.data
                }
            )
        }
        function getResolved(month) {
            
            return $http.get(defaultUrl + 'getResolved', { params: { m: month } }).then(
                function (response) {
                    return response.data
                }
            )
        }
        function getRefSequence(fc) {
            console.log("Factory code: ", fc);
            return $http.get(defaultUrl + 'getRefSequence',{ params: { factory: fc} }).then(
                function (response) {
                    return response.data
                }
            );
        }

        function deleteTempFile(name) {
            console.log("service delete: ");
            return $http.post(defaultUrl + 'deleteTempFile?name='+name).then(
                function (response) {
                    return response.data
                }
            );
        }
        function getReasonTemplates() {
            return $http.get(defaultUrl + 'getReasonTemplates').then(
                function (response) {
                    return response.data
                }
            );
        }

        function getReturnCategories() {
            return $http.get(defaultUrl + 'getReturnCategories').then(
                function (response) {
                    return response.data
                }
            );
        }
        function getData() {
            return $http.get(defaultUrl + 'test').then(
                function (response) {
                    return response.data
                }
            );
        }
        function getReturnNo() {
            return $http.get(defaultUrl + 'getReference').then(
                function (response) {
                    return response.data
                }
            );
        }
        function getClientOrders() {
            return $http.get(defaultUrl + 'getClientOrders').then(
                function (response) {
                    return response.data
                }
            );
        }

        function getProducts(inspId,newInspId) {
            return $http.post(defaultUrl + 'getProducts?inspId='+inspId+'&newInspId='+newInspId).then(
                function (response) {
                    return response.data
                }
            );
        }

        function getCategories() {
            return $http.get(defaultUrl + 'getCategories').then(
                function (response) {
                    return response.data
                }
            );
        }
        
        function getCprod(cprodCode) {
            return $http.get(defaultUrl + 'getCprod', { params: { cprodCode: cprodCode } }).then(
                function (response) {
                    return response.data
                }
            );
        }
        
        function saveNewCA(r) {
            return $http.post(defaultUrl + 'createCA', r).then(
                function (response) {
                    return response.data
                }
            );
        }
        function getUser() {
            return $http.get(defaultUrl + 'getUser').then(
                function (response) {
                    return response.data
                }
            );
        }
        function getFeedbackImages(feedId,type) {
            return $http.get(defaultUrl + 'getFeedbackImages?returnId='+feedId+'&imageType='+type).then(
                function (response) {
                    return response.data
                }
            );
        }
        function saveRecheck(r) {
            return $http.post(defaultUrl + 'createRecheck', r).then(
                function (response) {
                    return response.data;
                }
            )
        }
        function getRecheck(returnsid) {
            return $http.get(defaultUrl + 'getRecheck', { params: { returnsid: returnsid } }).then(
                function (response) {
                    return response.data;
                }
            )
        }
    }
})();
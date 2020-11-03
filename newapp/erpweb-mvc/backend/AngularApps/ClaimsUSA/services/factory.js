(function () {
    'use strict';

    angular
        .module('app')
        .factory('factory', factory);

    factory.$inject = ['$http'];

    function factory($http) {
        var baseUrl = 'api/dealerUS/';
        var defaultUrl = '../api/claimsUS/';
        var notAuth = "";
        var service = {
            getTest: getTest,
            getProducts: getProducts,
            getDealers: getDealers,
            getReference: getReference,
            saveFeedback: saveFeedback,
            getFeedbacks: getFeedbacks,
            getUser: getUser,
            deleteClaim: deleteClaim,
            deleteImage: deleteImage
            

            //getDealers: getDealers,
            //getDealerOrders: getDealerOrders,
            //getDealerLogs: getDealerLogs,
            //getLogs: getLogs,
            
            //createLog: createLog,
            //getDealerPersons: getPersons,
            //updateLog:updateLog,
            //notAuth:notAuth,
            //getLogCategories: getLogCategories,
        };

        return service;

        function getTest() {
            return $http.get(defaultUrl + 'test');
        }
        function getUser() {
            return $http.get(defaultUrl + 'getUser').then(
                    function (response) {
                        return response.data;
                    }
                )
        }

        function getProducts(therm, optAmaraOrCrossoptAmaraOrCross) {
            return $http.get(defaultUrl + 'getProducts', { params: { word: therm, option: optAmaraOrCrossoptAmaraOrCross } })
                    .then(function (response) {
                        return response.data;
                    });
        }
        function getDealers() {
            return $http.get(defaultUrl + "get").then(
                    function (response) {
                        return response.data;
                    }
                )
        }

        function getReference(refId) {
            return $http.get(defaultUrl + "getReference", { params: { rId: refId } }).then(
                    function (response) {
                        return response.data;
                    }
                )
        }
        function saveFeedback(model) {
            return $http.post(defaultUrl + 'createReturn', model)
        }
        function deleteClaim(model) {
            return $http.post(defaultUrl + 'delete', model);
        }
        function deleteImage(image) {
            return $http.post(defaultUrl + 'deleteImage', image);           
        }
        function getFeedbacks(_date) {
            return $http.get(defaultUrl + "getFeedbacks", {params:{date:_date}}).then(
                    function (response) {
                        return response.data;
                    }
                )
        }

      //*********************** za obrisat kad sve završim ----- **********
        //function getDealerOrders(dealer_id) {
        //    return $http.get(baseUrl + 'getOrders', { params: { customer: dealer_id } });
        //}
        //function getDealerLogs(dealer_id) {
        //    return $http.get(baseUrl + 'getDealerLogs', { params: { customer: dealer_id } });
        //}
        //function getLogs() {
        //    return $http.get(baseUrl + 'getLogs');
        //}
        //function getLogCategories(){
        //    return $http.get(baseUrl + 'getCategories');
        //}
        //function getUser() {
        //    //return $http.get(baseUrl + 'getUser')
        //}
        //function getPersons(dealer) {

        //    return $http.get(baseUrl + "getDealerPersons", { params: {dealer_code:dealer}})
        //}
        //function createLog(log) {
        //    return $http.post(baseUrl + 'createLog',log)
        //}
        //function updateLog(log) {
        //    return $http.post(baseUrl + 'updateLog',log)
        //}

    }
})();
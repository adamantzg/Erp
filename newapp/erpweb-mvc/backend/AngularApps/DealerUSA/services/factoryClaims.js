(function () {
    'use strict';

    angular
        .module('app')
        .factory('factoryClaims', factory);

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
            deleteImage: deleteImage,
            getDealerFeedbacks: getDealerFeedbacks
            

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
        function getDealerFeedbacks(customer) {
            return $http.get(defaultUrl + "getDealerFeedbacks", { params: { customer: customer } }).then(
                    function (response) {
                        return response.data;
                    }
                )
        }

      
    }
})();
(function () {
    'use strict';

    angular
        .module('app')
        .factory('factory', factory);

    factory.$inject = ['$http'];

    function factory($http) {
        var baseUrl = 'api/dealerUS/';
        var notAuth = "";
        var service = {
            getDealers: getDealers,
            getDealerOrders: getDealerOrders,
            getDealerLogs: getDealerLogs,
            getLog:getLog,
            getLogs: getLogs,
            getUser: getUser,
            createLog: createLog,
            getDealerPersons: getPersons,
            updateLog:updateLog,
            notAuth:notAuth,
            getLogCategories: getLogCategories,
            getRangeLogs: getRangeLogs,
            getOrdersAll: getOrdersAll,
            createShippingsRef: createShippingsRef,
            searchOrders: searchOrders,
            getOrdersLength: getOrdersLength,
            deleteRef: deleteRef,
            getDealerOrdersAlpha:getDealerOrdersAlpha,
            getDealerLogsAlpha:getDealerLogsAlpha,
            getDealersAlpha: getDealersAlpha,
            getOrdersSize: getOrdersSize,
            getLogsSize: getLogsSize,
            deleteImage: deleteImage,
            getDealerPersonsObj: getDealerPersonsObj,
            updateShowHideNames: updateShowHideNames
            
        };


        return service;

        function getOrdersSize(cust) {           
            return $http.get(baseUrl + 'getOrdersSize', { params: { customer: cust } }).then(
                function (response) {
                    return response.data;
                }
            );
        }

        function getDealers() {
            return $http.get(baseUrl + 'get');
        }

        
        function getDealersAlpha(cust){
            return $http.get(baseUrl + 'getDealersAlpha',{ params: {  customer: cust} }).then(
                function (response) {
                    return response.data;
                }
            );
        }
        function getOrdersLength(filter1,filter2) {
            return $http.get(baseUrl + 'getOrdersLength',{ params: {  filter1: filter1, filter2: filter2 } }).then(
                function (response) {
                    return response.data;
                }
            );
        }
        function getLogsSize(cust, filterByCustomer) {
            return $http.get(baseUrl + 'getLogsSize', { params: { customer: cust, filterByCustomer: filterByCustomer} }).then(
                function (response) {
                    return response.data;
                }
            );
        }
        function getDealerOrders(dealer_id) {
            return $http.get(baseUrl + 'getOrders', { params: { customer: dealer_id } });
        }
        function getDealerOrdersAlpha(dealer_id, page) {
            return $http.get(baseUrl + 'getOrdersAlpha', { params: { customer: dealer_id,page:page } });
        }
        function getDealerLogs(dealer_id) {
            return $http.get(baseUrl + 'getDealerLogs', { params: { customer: dealer_id } });
        }
        function getLog(id) {
            return $http.get(baseUrl + 'getLog', { params: { id : id } });
        }
        function getDealerLogsAlpha(dealer_id, page, filterByCustomer) {
            return $http.get(baseUrl + 'getDealerLogsAlpha', { params: { customer: dealer_id, page: page, filterByCustomer: filterByCustomer } });
                }
        function getLogs(status) {
            return $http.get(baseUrl + 'getLogs', { params: { takeNum : 100 , status: status } });
        }
        function getLogCategories(){
            return $http.get(baseUrl + 'getCategories');
        }
        function getUser() {
            return $http.get(baseUrl + 'getUser')
        }
        function getPersons(dealer) {

            var model = $http.get(baseUrl + "getDealerPersons", { params: { dealer_code: dealer } });
            model.then(function (R) {
                console.log('MODEL DATA: '+'dealer: '+dealer, R.data);
            }
            )

            return model

        }
        function getDealerPersonsObj(dealer) {

            return $http.get(baseUrl + "getDealerPersonsObj", { params: { dealer_code: dealer } })
        }
        function createLog(log) {
            return $http.post(baseUrl + 'createLog',log)
        }
        function updateLog(log) {
            return $http.post(baseUrl + 'updateLog',log)
        }
        function getRangeLogs(from, to) {
            return $http.get(baseUrl + "getRangeLogs",{ params: {from:from,to:to}})
        }
        function getOrdersAll(take, skip,filter1,filter2) {
            return $http.get(baseUrl + "getOrdersAll", { params: { take:take, skip:skip,filter1:filter1,filter2:filter2 } })
        }
        function createShippingsRef(shippingRef) {
            return $http.post(baseUrl + "createRef", shippingRef)
        }
        function updateShowHideNames(personsObj) {
            return $http.post(baseUrl + "updateShowHideNames", personsObj)
        }
        function searchOrders(textSearch) {
            return $http.get(baseUrl + "searchOrders", { params: { text: textSearch } }).then(
                function (response) {
                    return response.data
                }
            )
        }
        
        function deleteRef(shippingRef) {
            return $http.post(baseUrl + "deleteRef", shippingRef)
        }

        function deleteImage(image) {
            return $http.post(baseUrl + 'deleteImage', image);
        }
    }
})();
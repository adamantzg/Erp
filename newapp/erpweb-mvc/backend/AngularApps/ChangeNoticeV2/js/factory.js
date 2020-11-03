(function () {
    'use strict';


    angular
        .module('app')
        .factory('factory', factory);

    factory.$inject = ['$http'];

    function factory($http) {
        var baseUrl = '/api/changenoticev2/';
        var service = {
            getTest: getTest,
            getModel: getModel
            
        };

        service.searchProducts = function(client_id, factory_id, category1_id) {
            return $http.get(baseUrl + 'searchProducts', { params: { client_id: client_id, factory_id: factory_id, category1_id: category1_id } }).then(function (response) {
                service.products = response.data;
            });
        };

        service.loadClientsOrdersForProduct = function (cprod_id, readyDate) {
            return $http.get(baseUrl + 'loadClientsOrdersForProduct', { params: { cprod_id: cprod_id, readyDate: readyDate } });
        };

        service.loadClientsOrdersForProducts = function (cprod_ids, readyDate,orders) {
            return $http.get(baseUrl + 'loadClientsOrdersForProducts', { params: { cprod_ids: cprod_ids.join(','), readyDate: readyDate, orders: orders } });
        };

        service.create = function (n) {
            return $http.post(baseUrl + 'create', stripNotice(n)).then(function (response) {
                updateNoticeAfterSave(n, response.data);
            });
        };

        service.update = function (n) {
            return $http.put(baseUrl + 'update', stripNotice(n)).then(function (response) {
                updateNoticeAfterSave(n, response.data);
            });
        };

        service.deleteChangeNoticeDocument = function (n) {
            return $http.put(baseUrl + 'deleteChangeNoticeDocument', stripNotice(n)).then(function (response) {
                updateNoticeAfterSave(n, response.data);
                n.document.formatted_change_doc_id = response.data.document.formatted_change_doc_id;
                n.document.formatted_change_doc = response.data.document.formatted_change_doc;                    
            });
        };

        service.getUploadUrl = function () {
            return baseUrl + 'UploadImage';
        };
        service.getTempFileUrl = function (name, id) {
            return baseUrl + 'getTempUrl?file_id=' + id;
        };

        service.getNotices = function (factory_id, client_id, status, product_code) {
            if (product_code == '')
                product_code = null;
            return $http.get(baseUrl + "getNotices", { params: {factory_id: factory_id, client_id: client_id, status: status, product_code: product_code}});
        };

        service.getListModel = function () {
            return $http.get(baseUrl + "getListModel").then(function (response) {
                service.listModel = response.data;
            });
        };

        function updateNoticeAfterSave(n, updatedData)
        {
            n.id = updatedData.id;
            n.allocations.forEach(function (a) {
                var updAlloc = _.find(updatedData.allocations, { cprod_id: a.cprod_id });
                if (a.id == null || a.id <= 0)
                {
                    if (updAlloc != null)
                    {
                        a.id = updAlloc.id;
                        a.dateAllocated = updAlloc.dateAllocated;
                    }                        
                }
            });
        }

        function stripNotice(n)
        {
            var sn = clone(n);
            sn.allocations.forEach(function (a) {
                a.product = null;
                a.orders.forEach(function (o) {
                    o.orderList = null;
                });
            });
            return sn;
        }
        
        return service;

        function getTest() {
            return $http.get(basUrl + 'test');
        }

        function getModel(state, id) {
            return $http.get(baseUrl + 'GetChangeNoticeV2Model', { params: { state: state, id: id } }).then(function (response) {
                service.model = response.data;
            });
        }   
    }


})();
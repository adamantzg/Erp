(function () {
    'use strict';


    angular
        .module('app')
        .factory('factory', factory);

    factory.$inject = ['$http','$q'];

    function factory($http,$q) {
        var baseUrl = '/api/claims/';
        

        factory.get = function(type) {
            return $http.get(baseUrl, { params: { type: type } });
        };

        factory.getDefaultSubscribers = function (claim_type) {
            return $http.get(baseUrl + 'getDefaultSubscribers', { params: { claim_type: claim_type } }).then(
                function (response) {
                    return response.data
                }
            );
        }

        factory.getModel = function(id, type) {
            return $http.get(baseUrl + 'getModel', { params: { id: id, type: type } }).then(function(response) {
                factory.model = response.data;
            });
        };

        
        factory.create = function (c) {
            return $http.post(baseUrl + 'create', strip(c)).then(function (response) {
                updateAfterSave(c, response.data);
            });
        };

        factory.update = function (c) {
            return $http.put(baseUrl + 'update', strip(c)).then(function (response) {
                updateAfterSave(c, response.data);
            });
        };

        var searchProductsText = '';
        var products = [];

        factory.searchProductsByText = function(text) {
            return $q(function(resolve) {
                if (searchProductsText.length > 0 && searchProductsText.length <= text.length && text.substring(0, searchProductsText.length).toLowerCase() == searchProductsText.toLowerCase()) {
                    var regExp = new RegExp(text, "i");
                    resolve(_.filter(products, function(p) {
                        return p.cprod_code1.match(regExp) || p.cprod_name.match(regExp) || p.factory_ref.match(regExp);
                    }));
                }
                else
                {
                    searchProductsText = text;
                    $http.get(baseUrl + 'searchProductsByText', { params: { prefixText: text } }).then(function(response) {
                        products = response.data;
                        resolve(response.data);
                    });
                }
            });
        };

        factory.searchProductsByCriteria = function(client_id, factory_id) {
            return $http.get(baseUrl + 'searchProductsByCriteria', { params: { client_id: client_id, factory_id: factory_id } });
        };
  

        var searchUsersText = '';
        var users = [];

        factory.searchUsers = function(text) {
            return $q(function(resolve) {
                if (searchUsersText.length > 0 && searchUsersText.length <= text.length && text.substring(0, searchUsersText.length).toLowerCase() == searchUsersText.toLowerCase()) {
                    var regExp = new RegExp(text, "i");
                    resolve(_.filter(users, function(u) {
                        return u.username.match(regExp) || u.userwelcome.match(regExp);
                    }));
                }
                else {
                    searchUsersText = text;
                    $http.get(baseUrl + 'searchUsers', { params: { prefixText: text } }).then(function(response) {
                        users = response.data;
                        resolve(response.data);
                    });
                }
            });
        };

        factory.getFileUrl = function(obj,field) {
            if (obj.file_id != null)
                return baseUrl + 'getTempUrl?file_id=' + obj.file_id + '&name=' + obj[field];
            else
                if(factory.model != null)
                    return factory.model.imagesRoot + '/' +  obj[field];
            return '';
        };

        factory.getUploadUrl = function() {
            return baseUrl + 'uploadFile';
        };


        function updateAfterSave(c, updatedData)
        {
            c.returnsid = updatedData.returnsid;
            c.request_date = updatedData.request_date;

            if (c.subscriptions != null) {
                c.subscriptions.forEach(function (s) {
                    var upd = _.find(updatedData.subscriptions, { subs_useruserid: s.subs_useruserid });
                    if (s.subs_id == null || s.subs_id <= 0) {
                        if (upd != null) {
                            s.subs_id = upd.subs_id;
                        }
                    }
                });
            }
        }

        factory.createComment = function (returnsid, comment)
        {
            comment.return_id = returnsid;
            return $http.post(baseUrl + 'createComment', comment);
        }

        function strip(c)
        {
            var sc = clone(c);

            if (sc.subscriptions != null) {
                sc.subscriptions.forEach(function (s) {
                    s.user = null;
                });
            }   

            return sc;
        }

        factory.getModelAll = function () {
            return $http.get(baseUrl + 'getModelAll');
        };

        factory.getNextMonth = function (month21) {
            return $http.get(baseUrl + 'getNextMonth', { params: { month21: month21 }});
        };

        factory.getPreviousMonth = function (month21) {
            return $http.get(baseUrl + 'getPreviousMonth', { params: { month21: month21 }});
        };

        factory.getCompletedClaims = function (month21) {
            return $http.get(baseUrl + 'getCompleted', { params: { month21: month21 } });
        };

        factory.getEditPageUrl = function (asproot,r) {
            if (r.claim_type < 5)
                return asproot + '/backend/asaq_credit_requests_update_qa.asp?returnid=' + r.returnsid.toString();
            if(r.claim_type == 5)
                //Product feedback
                return asproot + '/backend/asaq_feedback_requests_update_v3.asp?returnid=' + r.returnsid.toString();
                //return '/claims/ProductFeedbackEdit?return_id=' + r.returnsid.toString();
            if(r.claim_type == 7)
                //ca
                return '/claims/EditCA/' + r.returnsid.toString();
            if (r.claim_type == 8)
                //qa
                return '/claims/qa#/edit/' + r.returnsid.toString();
        };

        factory.getCreatePageUrl = function (claim_type) {
            if (claim_type == 7)
                //ca
                return '/claims/CreateCA/';
            if (claim_type == 8)
                return '/claims/qa#/create';
        };

        factory.getStatistics = function (dateFrom, dateTo) {
            return $http.get(baseUrl + 'getStatisticsData', { params: { from: dateFrom, to: dateTo } });
        };

        factory.getBrandStats = function (dateFrom, dateTo) {
            return $http.get(baseUrl + 'getBrandsStatistics', { params: { from: dateFrom, to: dateTo } });
        };

        factory.getReasonStats = function (dateFrom, dateTo) {
            return $http.get(baseUrl + 'getReasonsStatistics', { params: { from: dateFrom, to: dateTo } });
        };

        factory.getDecisionStats = function (dateFrom, dateTo, brand_id, reason_id) {
            return $http.get(baseUrl + 'getDecisionStatistics', { params: { from: dateFrom, to: dateTo, brand_id: brand_id, reason_id: reason_id } });
        };

        factory.getBrandByMonthStats = function (dateFrom, dateTo, reason_id, decision_id) {
            return $http.get(baseUrl + 'getBrandByMonthStatistics', { params: { from: dateFrom, to: dateTo, decision_id: decision_id, reason_id: reason_id } });
        };
        factory.getReasonByMonthStats = function (dateFrom, dateTo, brand_id, decision_id) {
            return $http.get(baseUrl + 'getReasonByMonthStatistics', { params: { from: dateFrom, to: dateTo, brand_id: brand_id, decision_id: decision_id } });
        };

        factory.getDistributorPercentageStats = function (dateFrom, dateTo) {
            return $http.get(baseUrl + 'getDistributorPercentageStats', { params: { from: dateFrom, to: dateTo } });
        };

        factory.getFactoryPercentageStats = function (dateFrom, dateTo) {
            return $http.get(baseUrl + 'getFactoryPercentageStats', { params: { from: dateFrom, to: dateTo } });
        };

        factory.getChartUrl = function (type, dateFrom, dateTo, brand_id, reason_id, decision_id) {
            if (brand_id == null)
                brand_id = '';
            if (reason_id == null)
                reason_id = '';
            if (decision_id == null)
                decision_id = '';
            return baseUrl + 'getChart?type=' + type + '&from=' + moment(dateFrom).format('YYYY-MM-DD') + '&to=' + moment(dateTo).format('YYYY-MM-DD') + '&brand_id=' + brand_id + '&reason_id=' + reason_id + '&decision_id=' + decision_id;
        };

        factory.getBrands = function (dateFrom, dateTo) {
            return $http.get(baseUrl + 'getBrands', { params: { from: dateFrom, to: dateTo } });
        };

        factory.getReasons = function (dateFrom, dateTo) {
            return $http.get(baseUrl + 'getReasons', { params: { from: dateFrom, to: dateTo } });
        };

        factory.getDecisions = function (dateFrom, dateTo) {
            return $http.get(baseUrl + 'getDecisions', { params: { from: dateFrom, to: dateTo } });
        };

        factory.removeClaim = function (id) {
            return $http.post(baseUrl + 'deactivate?id=' + id);
        };

        factory.openClose = function (id, value) {
            return $http.post(baseUrl + 'openclose?id=' + id + '&value=' + value);
        };

        factory.authorize = function(id)
        {
            return $http.post(baseUrl + 'authorize?id=' + id);
        }

        factory.saveSubscription = function (sub) {
            return $http.post(baseUrl + 'saveSubscription', sub);
        };

        factory.removeSubscription = function (sub) {
            return $http.post(baseUrl + 'removeSubscription', sub);
        };

        return factory;       
        
    }


})();
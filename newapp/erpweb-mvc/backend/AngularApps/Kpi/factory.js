(function () {
    'use strict';

    angular
        .module('app')
        .factory('factory', factory);

    factory.$inject = ['$http'];

    function factory($http) {
        var inspBaseUrl = '/api/inspection/';
        var claimsBaseUrl = '/api/claims/';
        var service = {};

        service.getControllers = function () {
            return $http.get(inspBaseUrl + 'getControllers');
        };

        service.getKpiModel = function () {
            return $http.get(inspBaseUrl + 'getKpiModel');
        };

        service.getInspections = function (qc_id, monthStart) {
            return $http.get(inspBaseUrl + 'getForKpi', { params: { qc_id: qc_id, monthStart: monthStart } });
        };

        service.getClaims = function (qc_id, monthStart) {
            return $http.get(claimsBaseUrl + 'getByCriteria', { params: { monthStart: monthStart, qc_id: qc_id, products: true, currentUserOnly : false} });
        };

        service.getClaimUrl = function (asproot, r) {
            if (r.claim_type < 5)
                return asproot + '/backend/asaq_credit_requests_update_qa.asp?returnid=' + r.returnsid.toString();
            if (r.claim_type == 5)
                //Product feedback
                return asproot + '/backend/asaq_feedback_requests_update_v3.asp?returnid=' + r.returnsid.toString();
            //return '/claims/ProductFeedbackEdit?return_id=' + r.returnsid.toString();
            if (r.claim_type == 7)
                //ca
                return '/claims/EditCA/' + r.returnsid.toString();
            if (r.claim_type == 8)
                //qa
                return '/claims/qa#/edit/' + r.returnsid.toString();
        };

        return service;

        
    }
})();
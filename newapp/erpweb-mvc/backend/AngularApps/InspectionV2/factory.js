(function () {
    'use strict';

    angular
        .module('app')
        .factory('factory', factory);

    factory.$inject = ['$http'];

    function factory($http) {
        var baseUrl = '/InspectionV2/';
        var insp2ApiBaseUrl = '/api/inspectionv2/';
        var service = {};

        service.getTrackingNumbersData = function (id) {
            return $http.get(baseUrl + 'GetTrackingNumbersDataJSON',
            {
                params: { id: id }
            }).then(function (response) {
                service.trackingNumbersData = response.data;
            });
        };

        service.saveTrackingNumbers = function(list)
        {
            return $http.post(baseUrl + 'SaveTrackingNumbers',list).then(function (response) {
                service.trackingNumbersData.trackingNumbers = response.data;
            });
        }

        service.sendAutoEmail = function (id) {
            return $http.get(baseUrl + 'TrackingNumbersMail', { params: { id: id } });
        };

        service.getDrawingUploadUrl = function () {
            return insp2ApiBaseUrl + 'Images';
        };

        service.getEditModel = function (id) {
            return $http.get(insp2ApiBaseUrl + 'getEditModel', { params: { id: id } }).then(function (response) {
                service.rootFolder = response.data.rootFolder;
                return response;
            });
        };

        service.getFileUrl = function(insp,field)
        {
            if (insp.file_id)
                return insp2ApiBaseUrl + 'getTempUrl?id=' + insp.file_id + '&name=' + insp[field];
            return CombineUrls(service.rootFolder, insp[field]);
        }

        service.updateDrawing = function (insp) {
            return $http.post(insp2ApiBaseUrl + 'updateDrawing', insp);
        };

        return service;

        
    }
})();
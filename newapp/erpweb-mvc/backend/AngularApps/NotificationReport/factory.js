(function () {
    'use strict';

    angular
        .module('app')
        .factory('factory', factory);

    factory.$inject = ['$http'];

    function factory($http) {
        var service = {};
        var baseUrl = '/api/nr/';
        service.getCreateModel = function(id) {
            return $http.get(baseUrl + 'getNRCreateModel',
            {
                params: {insp_id: id}
            }).then(function(response) {
                service.model = response.data;
                },
                function (error) {
                }
            );
        }

        service.getEditModel = function(id) {
            return $http.get(baseUrl + 'GetNREditModel',
            {
                params: { id: id }
            }).then(function(response) {
                service.model = response.data;
            });
        };

        service.getEditReportModel = function(id) {
            return $http.get(baseUrl + 'GetNrEditReportModel',
            {
                params: { id : id }
            }).then(function (response) {
                service.model = response.data;
            });
        };

        service.update = function (nrHeader, IsFromOldRecord)
        {
            return $http.post(baseUrl + 'UpdateNr?isOldRecord=' + IsFromOldRecord, nrHeader)
                .then(function (response) {
                    service.nrHeader = response.data;
                }, function (error) {
                    alert(error);
                });
        }

        service.updateReport = function (nrHeader,submit) {
            return $http.post(baseUrl + 'UpdateNrReport?submit=' + submit, nrHeader)
                .then(function (response) {
                    service.nrHeader = response.data;
                }, function (error) {
                    alert(error);
                });
        }

        service.getUploadUrl = function() {
            return baseUrl + 'UploadImage';
        };
        service.getTempFileUrl = function(name, id) {
            return baseUrl + 'getTempUrl?file_id=' + id;
        };

        service.getNtListModel = function (from, readyDate) {
            return $http.get(baseUrl + 'getNtList', { params: { from: from, etdReadyDate: readyDate } });
        };


        return service;

        
    }
})();
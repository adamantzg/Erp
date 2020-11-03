angular.module('app').factory('factory', ['$http', function ($http) {
    var factory = {};
    
    var baseUrl = 'api/instructions/';

    factory.getAll = function () {
        return $http.get(baseUrl + 'getAll').then(function (response) {
            factory.fileRootFolder = response.data.fileRootFolder;
            return response;
        });
    };

    factory.getModel = function (id) {
        return $http.get(baseUrl + 'getModel', { params: { id: id } }).then(function (response) {
            factory.fileRootFolder = response.data.fileRootFolder;
            return response;
        });
    };

    factory.getUploadUrl = function () {
        return '/' + baseUrl + 'upload';
    };

    factory.getFileUrl = function (i) {
        if (i.file_id != null)
            return baseUrl + 'getTempUrl?file_id=' + i.file_id;
        return CombineUrls(factory.fileRootFolder, i.filename);
    };

    factory.searchProducts = function (factory_id, client_id) {
        return $http.get(baseUrl + 'searchProducts', { params: { factory_id: factory_id, client_id: client_id } });
    };

    factory.update = function (i) {
        var url = i.id > 0 ? 'update' : 'create';
        return $http.post(baseUrl + url, i);
    };

    return factory;
}]);
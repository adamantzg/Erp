angular.module('app').factory('factory', ['$http', function ($http) {
    var factory = {};
    
    var baseUrl = '/api/budget/';

    factory.getAll = function () {
        return $http.get(baseUrl);
    };

    factory.getModel = function (month21, date) {
        return $http.get(baseUrl + 'getModel', { params: { month21: month21, date: date } });
    };
       

    factory.update = function (data) {
        return $http.post(baseUrl + 'update', data);
    };

    factory.getDistributors = function () {
        return $http.get(baseUrl + 'distributors');
    };

    factory.updateDistributor = function (id, show) {
        return $http.post(baseUrl + 'updatedist?id=' + id + '&show=' + show);
    };

    return factory;
}]);
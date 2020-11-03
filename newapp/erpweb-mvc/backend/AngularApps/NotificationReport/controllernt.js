(function () {
    'use strict';

    angular
        .module('app')
        .controller('controller', controller);

    controller.$inject = ['$scope','$location','factory']; 

    function controller($scope, $location, factory) {
        var queryString = loadQueryString();
        var from, readyDate;
        if ('from' in queryString)
            from = queryString['from'];
        if ('readyDate' in queryString)
            readyDate = queryString['readyDate'];
        factory.getNtListModel(from, readyDate).then(function (response) {
            $scope.model = response.data;            
        });

        $scope.create = function (r) {
            var h = _.find($scope.model.nrHeaders, { insp_v2_id: r.insp_id });
            if (h != null)
                factory.update(h, false).then(function () {
                    _.remove($scope.model.inspectionRows, { insp_id: r.insp_id });
                });
        };

        $scope.formatDate = function (d) {
            return fromDateFormatted(d);
        };
    };
})();
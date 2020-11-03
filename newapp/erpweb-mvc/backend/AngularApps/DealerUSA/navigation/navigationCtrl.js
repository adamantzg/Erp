(function () {
    'use strict';

    angular
        .module('app')
        .controller('navigationCtrl', navigationCtrl);

    navigationCtrl.$inject = ['$scope','common'];

    function navigationCtrl($scope, common) {
        $scope.root = common.root;
        $scope.title = common.title;
        $scope.breadLink=common.nav

        activate();

        function activate() { }
    }
})();

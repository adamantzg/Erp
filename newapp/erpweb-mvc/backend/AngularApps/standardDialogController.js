(function () {
    'use strict';

    angular
        .module('app')
        .controller('standardDialogController', stdController);

    stdController.$inject = ['$scope', '$uibModalInstance', 'message'];

    function stdController($scope, $uibModalInstance, message) {
        $scope.message = message;
        $scope.close = function () {
            $uibModalInstance.close();
        };
    }
})();
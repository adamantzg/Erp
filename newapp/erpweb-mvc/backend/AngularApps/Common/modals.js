angular.module('app').factory('modalService', ['$uibModal', function ($uibModal) {
    var service = {};

    service.openDialog = function (message, title) {
        if (title == null)
            title = 'Message';

        var instance = $uibModal.open({
            templateUrl: '/AngularApps/common/templates/modal.html',
            controller: modalController,
            resolve: {
                params: function () {
                    return {
                        title: title,
                        message: message
                    };
                }
            }
        });

        return instance.result;
    }

    return service;

}]);

function modalController($uibModalInstance, $scope, params) {
    $scope.title = params.title;
    $scope.message = params.message;
    $scope.ok = function () {
        $uibModalInstance.close('ok');
    };

    $scope.cancel = function () {
        $uibModalInstance.close('cancel');
    }
}
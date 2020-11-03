(function () {
    'use strict';

    angular
        .module('app')
        .controller('listRechecksController', ['factoryInspectionCASample', 'factoryStorage', createRecheckController]);

    //createRecheckController.$inject = ['$scope'];

    function createRecheckController(factoryInspectionCASample, factoryStorage) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'createRecheckController';
        vm.rechecks = [];
        vm.showLoadingDialog = true;
        activate();
        
        function activate() {
            factoryInspectionCASample.getRechecks().then(
                function (data) {
                    vm.showLoadingDialog = false;
                    vm.rechecks = data;

                }, function (error) {
                    console.error(error);
                }
            );
        }
        vm.saveForEdit = function (recheck) {
            console.info("RECHECK IN LISTCONTROLLER: ", recheck)
            factoryStorage.setRecheck(recheck)
        }
    }
})();

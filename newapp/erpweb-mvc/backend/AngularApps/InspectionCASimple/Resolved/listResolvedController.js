(function () {
    'use strict';

    angular
        .module('app')
        .controller('listResolvedController', listResolvedController);

    listResolvedController.$inject = ['$location', 'factoryInspectionCASample', 'RECHECK_STATUS','factoryStorage'];

    function listResolvedController($location, factoryInspectionCASample, RECHECK_STATUS, factoryStorage) {
        /* jshint validthis:true */
        var vm = this;
        vm.showLoadingDialog = true;
        var lastMonths = -1;
        vm.title = 'listResolvedController';

        activate();

        function activate() {
            getResolved();
        }

        function getResolved(m) {
            factoryInspectionCASample.getResolved(lastMonths).then(
                function (data) {
                    vm.showLoadingDialog = false;
                    vm.resolved = data;
                },
                function (err) {
                    console.error("Get Resolved error: ", err);
                }
            )
        }
        vm.getStatus = function (status) {
            return status == RECHECK_STATUS.OK?"OK":"NO"
        }
        vm.saveResolvedDetail = function (resolved) {  
                factoryStorage.setResolved(resolved)
           
        }
    }
})();

(function () {
    'use strict';

    angular
        .module('app')
        .controller('logsStatus', logsStatus);

    logsStatus.$inject = ['$location', '$scope', 'factory', 'types'];

    function logsStatus($location, $scope ,factory, types) {
        /* jshint validthis:true */
        var vm = this;
        $scope.ctrlName = 'logsStatus';

        $scope.types = types;

        $scope.dtOptions = { paging: false, searching: false, order: [], }
        $scope.tableOptions = { order: [] };
        $scope.tableOptionsCallLogs = {
            dom: 'Bfrtip',
            order: [[1, 'desc']],
            buttons: [
                { extend: 'excelHtml5', text: '<i class="fa fa-file-excel-o"></i>', titleAttr: 'Excel' },
                { extend: 'print', text: '<i class="fa fa-print"></i>', titleAttr: 'print' }
            ]
        };
        $scope.ordersTake = 10;
        $scope.ordersSkip = 0;
        $scope.detailAlpha = function (customer) {
        console.log("CUSTOMER: ", customer)
            $state.go('detailAlpha', { id: customer }, { reload: true });
        };
        activate();

        function activate() {
            $scope.logs = factory.getLogs();
            console.log("LOGS: ", $scope.logs);

            factory.getLogs(0).then(
                function (req) {
                    $scope.logs = req.data;
                    $scope.logs.forEach(function (l) {
                        if (l.in_out == types.calls.in) {
                            l.from = l.person;
                            l.to = l.user.userwelcome;
                        } else {
                            l.to = l.person;
                            l.from = l.user.userwelcome;
                        }

                    });
                }
            );
        }
    }
})();

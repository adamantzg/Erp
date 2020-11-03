(function () {
    'use strict';
    
    angular
        .module('app')
        .controller('controller', controller);

    controller.$inject = ['$scope', '$state', '$stateParams', '$timeout', '$compile','factory', 'Lightbox','modalService'];

    function controller($scope, $state, $stateParams, $timeout, $compile, factory, Lightbox,modalService) {
        $scope.state = $state.current.name;

        $scope.datatables = { dtPending: {}, dtCompleted: {}};

        if ($scope.state == 'home')
            factory.getModelAll().then(function (response) {
                $scope.pending = response.data.pending;                
                $scope.completed = response.data.completed;
                $scope.user_id = response.data.user_id;
                $scope.month = response.data.month;
                $scope.asproot = response.data.asproot;
                $scope.types = response.data.types;
                $scope.criteria = {selectedType : "all"};
            });

        $scope.pendingStatuses = [{ value: 1, text: 'Pending' }, { value: 2, text: 'Awaiting response / recheck' }];

        $scope.pendingStatuses.forEach(function (s) {
            $scope.datatables.dtPending[s.value] = {};
        });

        $scope.claimsFilter = function (status) {
            return function (r) {
                return (r.status1 == 1 && (status == null || checkStatus(r) == status)) /*&& ($scope.criteria.selectedType == "all" || r.type == $scope.criteria.selectedType)*/;
            }
        }

        $scope.typeFilter = function (r) {
            return $scope.criteria.selectedType == "all" || r.type == $scope.criteria.selectedType;
        };

        $scope.showPreviousMonth = function () {
            factory.getPreviousMonth($scope.month.month21).then(function (response) {
                $scope.month = response.data;
                getCompleted();
            });
        };

        $scope.showNextMonth = function () {
            factory.getNextMonth($scope.month.month21).then(function (response) {
                $scope.month = response.data;
                getCompleted();
            });
        };

        function getCompleted() {
            factory.getCompletedClaims($scope.month.month21).then(function (response) {
                $scope.completed = response.data;
                if ($scope.criteria.selectedType != 'all')
                {
                    $timeout(function () {
                        filterTables($scope.criteria.selectedType, true);
                    }, 500);
                }
                    
            });
        }

        function checkStatus(r) {
            if (r.lastCommenterId == $scope.user_id)
                return 2;
            return 1;            
        }

        $scope.getEditUrl = function (r) {
            return factory.getEditPageUrl($scope.asproot, r);
        };

        $scope.create = function (type) {
            location.href = factory.getCreatePageUrl(type);
        };

        $scope.$watch('criteria.selectedType', function (newVal, oldVal) {
            filterTables(newVal,false);
        });

        $scope.removeClaim = function (array,c,dt) {
            modalService.openDialog('Delete this record?').then(function (response) {
                if (response == 'ok')
                {
                    factory.removeClaim(c.returnsid).then(function () {
                        c.status1 = 2;
                        $timeout(function () {
                            dt.DataTable.draw();
                        }, 500);
                        
                    });
                }                
            });            
        };

        function filterTables(newVal, completedOnly)
        {
            var search = '';
            var regex = false;
            if (newVal != 'all') {
                search = "^" + newVal + "$";
                regex = true;
            }
            if (!completedOnly)
            {
                for (var key in $scope.datatables.dtPending) {
                    if ($scope.datatables.dtPending[key].DataTable != null)
                        $scope.datatables.dtPending[key].DataTable.columns([0]).search(search, regex, false).draw();
                }
            }
            
            if ($scope.datatables.dtCompleted.DataTable != null)
                $scope.datatables.dtCompleted.DataTable.columns([0]).search(search, regex, false).draw();
        }

        $scope.tableOptions = {
            order: [3, "desc"]/*,
            paging: false,
            scrollY: 400,
            scrollCollapse: true,*/
        };
    }
})();
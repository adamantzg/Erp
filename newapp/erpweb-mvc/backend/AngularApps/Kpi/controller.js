(function () {
    'use strict';

    angular
        .module('app')
        .controller('controller', controller);

    controller.$inject = ['$scope','$location','$timeout','factory']; 

    function controller($scope, $location,$timeout, factory) {
        
        

        $scope.monthSelectorOptions = {
            datepickerMode: 'month',
            minMode: 'month',
            monthColumns: 6
        };

        $scope.model = {};
        

        factory.getKpiModel().then(function (response) {
            $scope.model = response.data;
            $scope.model.inspDtOptions = {};
            $scope.model.inspDtInstance = {};
        });

        $scope.showData = function () {
            factory.getInspections($scope.model.controller_id, $scope.model.monthStart).then(function (response) {
                $scope.model.inspections = response.data;
                factory.getClaims($scope.model.controller_id, $scope.model.monthStart).then(function (response) {
                    $scope.model.claimsSections = [
                    {
                        title: 'Claims',
                        subtitle: 'Accepted claims within given month for products inspected by selected QC',
                        groups: [
                        {
                            text: ' accepted claims that were inspected by QC',
                            data: _.filter(response.data, function (c) {
                                return c.decision_final == true && c.order_id > 0 && c.order_id != 9999;
                            }),
                            show: false
                        },
                        {
                            text: ' accepted claims that may have been inspected by QC',
                            data: _.filter(response.data, function (c) {
                                return c.decision_final == true && (c.order_id == 0 || c.order_id == null || c.order_id == 9999);
                            }),
                            show: false
                        }
                        ]                        
                    },
                    {
                        title: 'Feedbacks',
                        subtitle: 'Feedbacks within given month for products inspected by selected QC',
                        groups: [
                        {
                            text: ' product feedbacks that were inspected by QC',
                            data: _.filter(response.data, function (c) {
                                return c.claim_type == 5 && c.order_id > 0 && c.order_id != 9999;
                            }),
                            show: false
                        },
                        {
                            text: ' product feedbacks that may have been inspected by QC',
                            data: _.filter(response.data, function (c) {
                                return c.claim_type == 5 && (c.order_id == 0 || c.order_id == null || c.order_id == 9999);
                            }),
                            show: false
                        }
                        ]                         
                    }
                    ];
                });
            });
        };

        $scope.getClaimUrl = function (c) {
            return factory.getClaimUrl($scope.model.asproot, c);
        };
        

        $scope.formatDate = function (d) {
            if (d == null)
                return '';
            return fromDateFormatted(d.toString());
        }

        $scope.getInspectionUrl = function (i) {
            if (i.new)
                return '/InspectionV2/LoadingReport2/' + i.insp_id.toString();
            return '/Inspection/Report/' + i.insp_id.toString();
        };

        $scope.showHideGroup = function (g) {
            g.show = !g.show;
        };

        
    }
    

    
})();

function File(id, name, percent, size) {
    this.id = id;
    this.name = name;
    this.percent = percent;
    this.size = size;
}

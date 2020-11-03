(function () {
    'use strict';

    angular
        .module('app')
        .controller('controller', controller);

    controller.$inject = ['$scope', '$state', '$stateParams', '$timeout', '$compile', 'factory'];

    function controller($scope, $state, $stateParams, $timeout, $compile, factory) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'controller';
        $scope.dt = {};

        $scope.newShipping = {id: 0,refnumber: '', weight: 0 };
        $scope.newShippingId = -1;
        
        //$scope.dt.moment('DD/MM/YYYY');
        $.fn.dataTable.moment('DD/MM/YYYY');
        

        $scope.state = $state.current.name;

        $scope.dtSaleOptions = {
            order: [[2, 'asc']]
        };

        $scope.dtGoodsInOptions = {
            order: [[1, 'asc']],
            columnDefs: [{width: '20%', targets: [3,4]}]
        };

        $scope.dtOrders = {
            columnDefs: [
            {
                targets: [7],
                visible: false
            }],
            order: [[7, 'asc']]
        };

        $scope.addCallback = function (ship) {
            $scope.fileUpload.callbacks[ship.id] =
            {
                filesAdded: function (uploader, files) {

                    $timeout(function () {
                        uploader.start();
                    }, 1);
                },
                uploadProgress: function (uploader, file) {
                    ship.progress = file.percent;
                },
                beforeUpload: function (uploader, file) {
                    uploader.settings.multipart_params = { id: file.id };
                },
                fileUploaded: function (uploader, file, response) {

                    if (ship.id == 0) {
                        $scope.addShipping(file);
                    }
                    else
                    {
                        ship.document = file.name;
                        ship.file_id = file.id;
                    }
                    

                },
                error: function (uploader, error) {

                    alert(error.message);
                }
            };
        }

        $scope.addShipping = function(file) {
            var s = clone($scope.newShipping);
            s.id = $scope.newShippingId--;
            if (file != null) {
                s.file_id = file.id;
                s.document = file.name;
                s.file_id = file.id;    
            }
            $scope.salesOrder.shippings.push(s);
            $scope.addCallback(s);
            $scope.newShipping.refnumber = '';
            $scope.newShipping.weight = 0;
        };


        $scope.formatDate = function (d) {
            if (d == null)
                return '';
            return fromDateFormatted(d.toString());
        }

        $scope.editOrderIn = function(o)
        {
            $state.go('editGoodsIn', { id: o.orderid });
        }

        $scope.copyQty = function(lines)
        {
            lines.forEach(function (l) {
                l.received_qty = l.orderqty;
            });
        }

        $scope.copyQtySales = function (lines) {
            lines.forEach(function (l) {
                l.despatched_qty = l.order_qty;
            });
        }

        $scope.updateGoodsInOrder = function () {
            factory.updateGoodsInOrder($scope.goodsInOrder).then(function (response) {
                $state.go('home');
            });
        };

        $scope.isOutstanding = function(o)
        {
            return o.received_qty < o.orderqty;
        }

        $scope.isCompleted = function (o) {
            return o.received_qty >= o.orderqty;
        }

        $scope.isSalesOutstanding = function (o) {
            return o.despatched_qty < o.orderqty;
        }

        $scope.isSalesCompleted = function (o) {
            return o.despatched_qty >= o.orderqty;
        }

        $scope.limitInput = function(l)
        {
            if ('received_qty' in l)
            {
                if (l.received_qty == null || l.received_qty > l.orderqty)
                    l.received_qty = l.orderqty;
            }
            if('despatched_qty' in l)
            {
                if (l.despatched_qty == null || l.despatched_qty > l.order_qty)
                    l.despatched_qty = l.order_qty;
            }
            
        }

        $scope.editOrderOut = function(o)
        {
            $state.go('editSales', {order_no: o.order_no});
        }

        $scope.updateSalesOrder = function () {
            if ($scope.salesOrder.delivered != null && !$scope.shippingsMissing()) {
                factory.updateSalesOrder($scope.salesOrder).then(function (response) {
                    $state.go('home');
                });    
            }
        };

        $scope.removeShipping = function (s) {
            _.remove($scope.salesOrder.shippings, {id: s.id});
        };

        $scope.getShippingLink = function (s) {
            if (s.id < 0)
                return '/api/ussales/getTempUrl?file_id=' + s.file_id;
            else
                return '/files/salesordershipping/' + s.document;
        };

        $scope.fileUpload = {
            url: '/api/ussales/uploadshipping',
            options:
                {
                    multi_selection: false,
                    max_file_size: '50mb'
                },
            callbacks: {}            
        }

        $scope.downloadPackingList = function(o, $event) {

        };

        if ($scope.state == 'home') {
            factory.getGoodsInOrders().then(function (response) {
                $scope.goodsInOrders = factory.goodsInOrders;
            });
            factory.getSalesOutOrders().then(function (response) {
                $scope.salesOutOrders = factory.salesOutOrders;
            });
        }
        else if ($scope.state == 'editGoodsIn') {
            var orderid = $state.params.id;
            factory.getGoodsInOrder(orderid).then(function (response) {
                $scope.goodsInOrder = factory.goodsInOrder;
                $scope.goodsInOrder.lines.forEach(function(l) {
                    l.orig_received_qty = l.received_qty;
                });
            });
        }
        else if ($scope.state == 'editSales') {
            var order_no = $state.params.order_no;
            $scope.addCallback($scope.newShipping);
            factory.getSalesOrder(order_no).then(function (response) {
                $scope.salesOrder = factory.salesOrder;
                $scope.salesOrder.shippings.forEach(function (s) {
                    $scope.addCallback(s);
                });
                $scope.salesOrder.lines.forEach(function (l) {
                    l.orig_despatched_qty = l.despatched_qty;
                });
            });
        }

        $scope.shippingsMissing = function () {
            if ($scope.salesOrder == null)
                return false;
            return $scope.salesOrder.delivered == 2 &&
                _.filter($scope.salesOrder.shippings,
                    function(s) {
                        return s.refnumber.length > 0;
                    }).length == 0;
        }


            
    }
})();

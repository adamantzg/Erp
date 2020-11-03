  (function () {
    'use strict';

    angular
        .module('app')
        .controller('dealerListCtrl', dealerListCtrl);

    dealerListCtrl.$inject = ['$scope', '$location', 'factory', 'dealers', '$uibModal', 'version', 'DTOptionsBuilder', 'DTColumnDefBuilder', 'defaultPaths', 'types', '$timeout','PagerService','shared','$state','common','$sce','$popover'];

    function dealerListCtrl($scope, $location, factory, dealers, $uibModal, version, DTOptionsBuilder, DTColumnDefBuilder, defaultPaths, types, $timeout, PagerService,shared,$state,common,$sce,$popover) {
        /* jshint validthis:true */
        $scope.ctrlName = "dealerListCtrl.js";
    

        $scope.popover = {
        "title":"Title", "content":"Ovo je content"
        }; 

        $scope.getText = function (addr) {
            var text = "<table class='table-more-addr'>";
           
            for (var i = 0; i < addr.length; i++) {
                //text += "<tbody>"
                if(i==0)
                    text += "<tr>"
                else
                    text += "<tr style='border-top:1px solid #555'>"
                
                //ion-key
                text += "<td class='icon-cell'> <i class='ion ion-ios7-person'></i> </td><td style='padding:0'>" + addr[i].customer + "</td>"
                text += "</tr><tr>"
                text += "<td class='icon-cell'><i class='ion ion-home'></i></td><td style='padding:0'> " + addr[i].address1 + " - " + addr[i].town_city + "</td>";
                text += "</tr>"
                if (addr[i].address5 !=='' ){
                    text += "<tr>"
                    text += "<td class='icon-cell'><i class='ion ion-ios7-telephone'></i></td><td style='padding:0'> " + addr[i].address5 + "</td>";
                    text += "</tr>"
                }
                text += "</tbody>"
            }
            //text +="</table>"
            return text
        }
        $scope.dtOptions = { paging: false, searching: false, order: [],  }
        $scope.tableOptions = { order:[] };
        $scope.tableOptionsCallLogs = {
            dom: 'Bfrtip',
            order: [[1, 'desc']],
            buttons: [
                { extend: 'excelHtml5', text: '<i class="fa fa-file-excel-o"></i>', titleAttr: 'Excel' },
                { extend: 'print', text: '<i class="fa fa-print"></i>', titleAttr: 'print' }
                ]
        };
        $scope.definiran = typeof $scope.dealerDetail === 'undefined';
        $scope.types = types;
        $scope.ordersTake = 10;
        $scope.ordersSkip = 0;
        $scope.gotoState = function (customer,name,alpha) {
            common.setDealer({customer:customer, name:name, alpha:alpha});
            $state.go("detailAlpha", {id:customer})
        }

        //$scope.getPopoverAddress = function (text) {return  $sce.trustAsHtml('<h2>test</h2>'); }

        //    function (getPopoverAddress) {
        //    return $sce.trustAsHtml('test')
        //};

        var brsLink = $location.search().brsLink;
        if (brsLink != null)
            shared.brsLink = brsLink;
        
        $scope.brsLink = shared.brsLink;
        $scope.isUndefined = function (val) {
            
                return angular.isUndefined(val) || val === null
            
        }
        $scope.deleteRef = function(ref,order){

            factory.deleteRef(ref).then(
                function () {
                    for (var i = 0; i < $scope.orders.length; i++) {
                        if ($scope.orders[i].header_id === order.header_id) {
                            for (var n = 0; n < $scope.orders[i].shippings.length; n++) {
                                if ($scope.orders[i].shippings[n].id === ref.id)
                                    $scope.orders[i].shippings.splice(n, 1);
                            }
                        }
                    }
                },
                function () {
                });
        }

        /**Filter**/
        $scope.filter1 = 'all';
        $scope.filter2='oustanding';

        $scope.search = {};
        $scope.search.text = '';
        $scope.getFilter2 = function (filterby) {

            $scope.filter2 = filterby;
            $scope.search.text = '';
            factory.getOrdersLength($scope.filter1, $scope.filter2).then(
                function (number) {
                    $scope.isSearched = false;
                    
                    $scope.number = number;
                    paging();

                });
           
        }
        $scope.getFilter = function (filterby) {

            $scope.filter1 = filterby;
            $scope.search.text = '';
            factory.getOrdersLength($scope.filter1, $scope.filter2).then(
                function (number) {
                    $scope.isSearched = false;
                   

                    $scope.number = number;
                    paging();

                });
           
        }

        /****/
        /** PAGING **/
        var paging = function () {

            //$scope.dummyItems = _.range(1, $scope.orders.length);
            $scope.dummyItems = $scope.number;
            $scope.pager = {};
            $scope.setPage = setPage;
            /* init */
            $scope.setPage(1);
            /* end init*/

            function setPage(page) {
                if (page < 1 || page > $scope.pager.totalPages) {
                    return;
                }
                //get pager object from service
                $scope.pager = PagerService.GetPager($scope.dummyItems, page);
                //get current page of items
              
                  factory.getOrdersAll($scope.ordersTake, $scope.pager.endIndex > 8 ? $scope.pager.endIndex - 9 : 0,$scope.filter1, $scope.filter2).then(
                        function (request) {
                            $scope.orders = request.data;

                        })
                

// $scope.dummyItems.slice($scope.pager.startIndex, $scope.pager.endIndex + 1);
            }
        }
        /** END PAGING **/

        /**CALENDAR**/
        $scope.open1 = function () {
            $scope.popup1.opened = true;
        };
        $scope.open2 = function () {
            $scope.popup2.opened = true;
        };
        $scope.popup1 = {
            opened: false
        };
        $scope.popup2 = {
            opened: false
        };
        $scope.PreselectTo = function () {
            $scope.dtTo = new Date();
        }
        /**END CALENDAR **/
        $scope.dtInstance = {};
        $scope.dealers = dealers;
        activate();

        function activate() {            
            factory.getUser().then(
                function(req) {
                    
                    $scope.currentUser = req.data;
                },
                function(err) {
                    $scope.ERR = err;
                });

            //factory.getOrdersLength($scope.filter1,$scope.filter2).then(
            //    function (number) {
            //        $scope.number = number;
            //    }
            //);
            
        }
        $scope.getLogs = function() {
            factory.getLogs().then(
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

        $scope.getUsaZone = function (hours) {
            var date = new Date();

        }
        /* DIALOG BOX */
        $scope.openNewClaim = function (claim, customer) {
            $uibModal.open({
                templateUrl: $scope.error ? defaultPaths.DEALER.URL + defaultPaths.CLAIMS.FOLDER + "/returnError.html?v="+version.v : defaultPaths.DEALER.URL + defaultPaths.CLAIMS.FOLDER + "/returnFeedbacksView.html?v="+version.v,
                controller: 'ClaimsPopupCtrl',//claim !== null ? 'ClaimsPopupCtrl' : 'returnFeedbacks',
                size: "lg",
                animation: true,
                resolve: {
                    params: function () {
                        return {
                            claim: claim,
                            customer: customer,
                            error: $scope.ERROR
                        };
                    }
                }
            });
        }
        $scope.open = function (log, newLog) {
            /* OPEN DIALOG BOX */
            var modalInstance = $uibModal.open({
                templateUrl: "AngularApps/DealerUSA/dealers/myModalContent1.html?p="+version.v,
                controller: "dealerDetailEditCtrl",
                size: "lg",
                animation: true,
                backdrop: 'static',
                keyboard: false,
                resolve: {
                    params: function () {
                        return {
                            dealer: null,
                            userid: $scope.currentUser.userid,
                            newLog: newLog
                        };
                    }
                }
            });
            /* END OPEN DIALG BOX */

            /* RETRIEVE CLOSE - CANCEL - OK */
            modalInstance.result.then(
                    function (selected) { /* SAVE */
                        selected.userwelcome = $scope.currentUser.username;
                        selected.usaDate = Date.now();

                        var exist = _.some($scope.logs, function (checkLog) {
                            return checkLog.id === selected.id;
                        });
                        if (!exist)
                            $scope.logs.unshift(selected);
                            //$scope.logs.push(selected);
                    },
                    function (backLog) {/* CANCEL */
                        /* backLog is like one step undo */
                    }
                )
            /* END RETRIEVE CLOSE - CANCEL - OK */
        }
        /* END DIALOG BOX*/

        $scope.getRange = function (from, to) {
            factory.getRangeLogs(from,to).then(
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
            )
        }

        /* ORDERS */
        $scope.getOrders = function () {
            factory.getOrdersAll($scope.ordersTake, $scope.ordersSkip,$scope.filter1, $scope.filter2).then(
                function (req) {
                    $scope.orders = req.data;
                    paging();
                    
                },
                function(error) {
                    $scope.error = error;
                }
            )
        }
        $scope.orderShippingRefNumbers = function (order) {
            var refNum = "";
            for (var i = 0; i < order.header.shippings.length; i++) {
                refNum += order.header.shippings[i].refnumber;
            }
            return refNum;
        }
        $scope.dealerName = function (customer) {
            var name = "";
            for (var i = 0; i < $scope.dealers.length; i++) {
                if ($scope.dealers[i].customer === customer) {
                    name = $scope.dealers[i].name;
                    break;
                }
            }
            return name;
        }
        $scope.orderItems = function (order) {
            return _.reduce(order, function (result, item) {
                return result + item.order_qty;
            }, 0);
        }
        $scope.orderItemsLines = function (order) {
            return _.reduce(order.lines, function (result, item) {
                return result + item.order_qty;
            }, 0);
        }
        $scope.orderValue = function (order) {
            return _.reduce(order, function (result, item) {
                return result + item.value
            }, 0)
        }
        $scope.orderValueLines = function (order) {
            return _.reduce(order.lines, function (result, item) {
                return result + item.value
            }, 0)
        }
        $scope.shippings = [];
        $scope.novalidInput = [];
        $scope.loadingNewRF = false;
        $scope.createShippings = function ($index, order) {
            if (typeof $scope.shippings[$index] === 'undefined' || $scope.shippings[$index].refnumber==='') {
                $scope.novalidInput[$index] = true;
            }
            else {
                //$scope.createShippings = true;
                $scope.shippings[$index].header_id = order.header_id;
                //$scope.shippings[$index].order_no = order.order_no;
               // $scope.novalidInput[$index] = true;

                factory.createShippingsRef($scope.shippings[$index]).then(
                    function () {
                        $scope.novalidInput[$index] = false;
                        var p = angular.copy($scope.shippings[$index]);
                        $scope.orders[$index].shippings.push(p);
                        $scope.shippings[$index].refnumber = "";
                        //$scope.shippings[$index] = {};
                    },
                    function () {
                    }
                )
            }

            /* pozvati servis za sprmanje */
            
        }

        $scope.cancelShippings = function ($index,order) {
            $scope.novalidInput[$index] = false;
            $scope.shippings[$index] = {};
            $scope.shippings[$index].refnumber = "";
        }

        //$scope.textSearch = "Tražim tekst";
        $scope.isSearched = false;
        $scope.showChanges = function (sr) {
            
            if ( sr !== undefined && sr !== '' && sr !== ' ' ){
                factory.searchOrders(sr).then(
                    function (orders) {
                        $scope.filter2 = '';
                        $scope.filter1 = '';
                        $scope.isSearched = true;
                        $scope.orders = orders;
                    }, function (error) {
                        $scope.error = error;
                    }
                )
            }
            
           // }, 1000);
            
        }
        /* END ORDERS */
    }

})();

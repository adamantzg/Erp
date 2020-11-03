(function () {
    'use strict';
    /** OPIS ZAPOČETOG **/
    /*
     * Započeo sam izradu, paginga zanimljiv pimjer sa weba, zapiso sam link na
     * OneNote - Angular - paging, taj primjer ima i MVC i Angular2
     */
    angular
        .module('app')
        .controller('dealerDetailAlphaCtrl', dealerDetailCtrl);

    dealerDetailCtrl.$inject = ['$scope', '$location', '$stateParams', '$state', 'factory', 'common', '$window', '$uibModal', 'version', 'PagerService', '$filter', 'defaultPaths', '$log', 'types'];

    function dealerDetailCtrl($scope, $location, $stateParams, $state, factory, common, $window, $uibModal, version, PagerService, $filter, defaultPaths, $log, types) {
        $scope.title = "dealerDetailAlphaCtrl.js";
        $scope.ctrlName = "dealerDetailAlphaCtrl.js"

        $scope.types = types;
        $scope.showNewCallLogBtn = true;
        $scope.showAddNewPerson = false;
        $scope.persons = [];
        $scope.Us_call_log = {};
        $scope.search = {};
        $scope.checkBtn = 'log';
        // $scope.showLog = true;
        $scope.dealerDetail = true;
        //$scope.listfeed = "";
        $scope.changeTab = function () {
            $scope.showLog = !$scope.showLog;
        }
        var customer = $stateParams.id;
        $scope.customer = $stateParams.id;
        $scope.isMiscellaneus = !$stateParams.id;
        //$scope.dealerName = $stateParams.name; 

        function TEST() {
        }
        TEST();
        //$scope.tableOptions={
        //    columnDefs: [{
        //        orderable: false,
        //        className:'dt-body-center'
        //    }]
        //}
        //$scope.dtInstance = {};

        var change = 0;



        $scope.showHide = function () {
            if ($scope.Us_call_log.person === "value") {
                $scope.Us_call_log.person = '';
                $scope.showAddNewPerson = !$scope.showAddNewPerson;

            } else {
                //$scope.showAddNewPerson = !$scope.showAddNewPerson;
            }
        }
        $scope.backToList = function () {
            $scope.Us_call_log.person = '';
            $scope.showAddNewPerson = !$scope.showAddNewPerson;
        }

        $scope.radioModel = 'Middle';

        $scope.totalItems = 0;

        function ordersGetTablePage(skip) {


        }
        function getOrdersSize() {
            factory.getOrdersSize($scope.customer)
                .then(
                function (data) {
                    $scope.totalItems = data;
                },
                function (error) { console.error("ERROR: ", error); }
                )
        }
        $scope.setPage = function (pageNo) {
            $scope.currentPage = pageNo;
        };
        $scope.newPaging = false;

        $scope.pageChanged = function (pageNo) {
            $scope.currentPage = pageNo;
            $scope.newPaging = true;
            factory.getDealerOrdersAlpha($scope.customer, pageNo)
                .then(
                function (response) {
                    $scope.items = response.data;
                },
                function (error) { console.error("ERROR: ", error); }
                )

        }

        this.$onInit = function () {

            getCustomers($scope.customer);

            getCurrentuUser();
            if (customer !== '') {
                //setCustomerOrders(1);
                getOrdersSize();
                $scope.pageChanged(1);
                getLogsSize(customer);
                getDealerLogs(customer, 1);
            } else {
                getDealerLogs("misc");
            }
            /*keep for modal window*/
        }


        /** PAGING **/
        var paging = function () {
            $scope.newPaging = false;
            //$scope.dummyItems = _.range(1, $scope.orders.length);
            $scope.dummyItems = $scope.orders;
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
                $scope.pager = PagerService.GetPager($scope.dummyItems.length, page);

                //get current page of items
                $scope.items = $scope.dummyItems.slice($scope.pager.startIndex, $scope.pager.endIndex + 1);
            }
        }
        /** END PAGING **/

        $scope.orderItems = function (order) {
            return _.reduce(order, function (result, item) {
                return result + item.order_qty;
            }, 0);
        }
        $scope.orderValue = function (order) {
            return _.reduce(order, function (result, item) {
                return result + item.value
            }, 0)
        }

        $scope.orderShippingRefNumbers = function (order) {
            if (order != null && order.header != null && order.header.shippings != null && order.header.shippings.length > 0) {
                return _.map(order.header.shippings, 'refnumber').join(', ');
            }
            else
                return '';
        }

        $scope.cancel = function () {
            $scope.Us_call_log = {};
            $scope.showNewCallLogBtn = true;
        }
        /*CHECK FOR REMOVE FROM CODE*/
        $scope.createLog = function () {
            $scope.novalid = !!!$scope.Us_call_log.note;
            $scope.novalid = !!!$scope.Us_call_log.note;

            $scope.novalidName = !!!$scope.Us_call_log.person;

            if (!$scope.novalid && !$scope.novalidName) {
                factory.createLog($scope.Us_call_log).then(
                    function () {

                        $scope.logs.push($scope.Us_call_log);

                        getDealerLogs();
                        $scope.showNewCallLogBtn = true;
                        $scope.Us_call_log = {};


                    },
                    function (err) {
                        $scope.Error = err;
                    }
                )
            }
        }

        /* DIALOG BOX */

        $scope.openNewClaim = function (claim, customer, cprodId) {
            $uibModal.open({
                templateUrl: $scope.error ? defaultPaths.DEALER.URL + defaultPaths.CLAIMS.FOLDER + "/returnError.html?p=" + version.v : defaultPaths.DEALER.URL + defaultPaths.CLAIMS.FOLDER + "/returnFeedbacksView.html?p=" + version.v,
                controller: 'ClaimsPopupCtrl',//claim !== null ? 'ClaimsPopupCtrl' : 'returnFeedbacks',
                size: "lg",
                animation: true,
                resolve: {
                    params: function () {
                        return {
                            claim: claim,
                            customer: customer,
                            cprodId: cprodId,
                            error: $scope.ERROR,
                            dealerView: true
                        };
                    }
                }
            }).result.then(
                function () {
                    $state.go('detail', { id: customer }, { reload: true });
                }
                );
        }

        /****** OPEN EDIT MODAL WINDOW **********/
        $scope.open = function (log, newLog, parentId) {
            /* send data to shared factory */
            common.isNewLog(newLog);

            if (!newLog)
                getDealerPersons(log.dealer);
            common.setLog(log);
            var modalInstance = $uibModal.open({
                templateUrl: "AngularApps/DealerUSA/dealers/myModalContent2.html?p=" + version.v,
                //controller: "editCtrl",
                controller: "dealerDetailEditCtrl",
                size: "lg",
                animation: true,
                backdrop: 'static',
                keyboard: false,
                resolve: {
                    params: function () {
                        return {
                            title: $scope.getDealerDetail(customer),
                            dealer: customer,
                            userid: $scope.currentUser,
                            newLog: newLog,
                            parentId: parentId,
                            dealerId: $scope.filterDealer
                        };
                    }
                }
            });

            modalInstance.result.then(
                /*MODAL WINDOW ACTION BUTTON*/
                function (selected) {
                    /*SAVE*/
                    /*check if we have id */
                    var exist = _.some($scope.logs, function (checkLog) {
                        return checkLog.id === selected.id;
                    });
                    if (!exist && selected.isNewLog) { /**CREATE NEW LOG**/
                        selected.user = {};
                        factory.getUser().then(
                            function (res) {
                                selected.user.userwelcome = res.data.username;
                                selected.user.userid = res.data.userid;
                            }
                        )
                        getDealerLogs(customer, 1)

                    }
                    else { /** EDITED LOG SAVE **/

                        selected.date_edit = Date.now();                        
                        factory.getLog(selected.id).then(
                            function (response) {
                                for (var i = 0; i < $scope.logs.length; i++) {
                                    if ($scope.logs[i].id === response.data[0].id) {
                                        $scope.logs[i] = response.data[0];
                                    }
                                }
                            }
                        );

                        //var index = _.indexOf($scope.logs, _.find(_.find($scope.logs, { id: selected.id })))
                        //$scope.logs.splice(index, 1, selected);
                    }
                },
                function (backLog) {
                    /*CANCEL*/
                    /* backLog is like one step undo */
                    if (!newLog) {
                        for (var i = 0; i < $scope.logs.length; i++) {
                            if ($scope.logs[i].id === backLog.id)
                                $scope.logs[i] = backLog;
                        }
                    }

                })
        }

        function getCustomers(customer) {
            factory.getDealersAlpha(customer)
                .then(
                function (data) {
                    $scope.dealerListAlpha = data;
                    $scope.dealer = data[0];
                    //if (Object.keys(common.getDealer()).length > 0) {
                    //    $scope.dealer = common.getDealer();
                    //    console.log("DEALER", Object.keys(common.getDealer()).length);
                    //} else {
                    //    //getCustomer
                    //    console.log("GET DEALER FROM SERVER: ");
                    //}
                },
                function (error) { console.error("ERROR: ", error); }
                )
        }
        function getCurrentuUser() {
            factory.getUser().then(
                function (response) {
                    $scope.currentUser = response.data.userid;


                }
            )
        }
        function getUser() {
            factory.getUser().then(
                function (response) {

                    $scope.Us_call_log.userwelcome = response.data.userwelcome;
                    $scope.Us_call_log.userid = response.data.userid;
                    $scope.Us_call_log.dealer = customer;

                }
            )
        }


        //save persons to common
        function getDealerPersons(dealer) {
            factory.getDealerPersons(dealer).then(
                function (response) {
                    $scope.persons = response.data;
                    common.setPersons($scope.persons);

                }
            )


        }

        function setCustomerOrders(page) {
            factory.getDealerOrdersAlpha(customer, page).then(
                function (response) {
                    //$scope.responseStatus = response.status;
                    $scope.ordersCopy = response.data;
                    $scope.orders = response.data;
                    $scope.items2 = $scope.orders;
                    $scope.alpha = typeof response.data[0] !== 'undefined' ? response.data[0][0].alpha : "";//response.data[0][0].alpha;
                    // $scope.alpha=reponse.data[0]
                    paging();
                    $scope.newPaging = false;
                });
        }

        //$scope.getCustomerOrdersPage(){
        //    factory.getDealerOrdersAlpha
        //}

        function filterCustomerOrders() {
            factory.getDealerOrders($scope.filterDealer).then(
                function (response) {
                    //$scope.responseStatus = response.status;
                    $scope.ordersCopy = response.data;
                    $scope.orders = response.data;
                    $scope.items2 = $scope.orders;
                    $scope.alpha = "";//response.data != null ? response.data[0][0].alpha:"";
                    // $scope.alpha=reponse.data[0]
                    paging();


                });
        }


        $scope.filterOrderLogs = function () {
            console.log('Filter dealer: ',$scope.filterDealer, $scope.fiterDealer =='undefined');
            if ($scope.filterDealer !== 'undefined' && $scope.filterDealer !== null) {
                console.log('filterDealerEmpty', !!$scope.filterDealer);
                //   setCustomerOrders();
                $scope.pageChanged(1);
                getDealerLogs($scope.filterDealer, 1, true);
                getLogsSize($scope.filterDealer,true);
            }
            else {
                //$scope.logs = _.filter($scope.logsCopy, { 'dealer': $scope.filterDealer })
                getDealerLogs($scope.customer, 1)
                getLogsSize($scope.customer, true);

                $scope.pageChanged(1);
                //filterCustomerOrders();

            }
        }

        /*create list dealers logs and get persons[] for dealer */
        function getLogsSize(cust, filterByCustomer) {
            factory.getLogsSize(cust, filterByCustomer).then(
                function (response) {
                    $scope.totalLogsItems = response;
                }
            )
        }
        $scope.pageLogsChanged = function (page) {

            getDealerLogs($scope.customer, page)

        }
        function getDealerLogs(cust, page, filterByCustomer) {
            factory.getDealerLogsAlpha(cust, page, filterByCustomer).then(
                function (response) {
                    console.log('Izradi kopiju jer je filterByCustomer === undefined => ', filterByCustomer, typeof filterByCustomer == 'undefined');
                    if (filterByCustomer==='undefined') {
                        $scope.logsCopy = response.data;
                    }
                    $scope.logs = response.data;
                    if (response.data.length > 0) {
                        $scope.persons = getDealerPersons(response.data[0].dealer);
                    }
                    else {
                        $scope.persons = [];
                        common.setPersons($scope.persons);
                    }

                }
            )
        }
        $scope.getDealerDetail = function (cust) {


            /*if undefined*/
            if (!!$scope.logs) {
                return cust;
            } else {
                return $scope.logs.length > 0 ? $scope.logs[0].usDealer.name : $scope.orders.length > 0 ? $scope.orders[0][0].alpha : "";

            }
        }


        $scope.showChanges = function () {
            $scope.orders = $filter('filter')($scope.items2, $scope.search.text);
            paging();
        }

    }
})();

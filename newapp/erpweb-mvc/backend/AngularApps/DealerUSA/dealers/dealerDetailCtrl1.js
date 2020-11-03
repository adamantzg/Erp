(function () {
    'use strict';
    /** OPIS ZAPOČETOG **/
    /*
     * Započeo sam izradu, paginga zanimljiv pimjer sa weba, zapiso sam link na
     * OneNote - Angular - paging, taj primjer ima i MVC i Angular2
     */
    angular
        .module('app')
        .controller('dealerDetailCtrl', dealerDetailCtrl);

    dealerDetailCtrl.$inject = ['$scope', '$location', '$stateParams', '$state', 'factory', 'common', '$window', '$uibModal', 'version', 'PagerService', '$filter', 'defaultPaths', 'showTest'];

    function dealerDetailCtrl($scope, $location, $stateParams, $state, factory, common, $window, $uibModal, version, PagerService, $filter, defaultPaths, showTest) {
       
        $scope.showTest = showTest;
        $scope.ctrlName= "DealerDetailCtrl.js"

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
        
        $scope.dealer = common.getDealer();

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

        this.$onInit = function () {
           
            getCurrentuUser();
            if (customer !== '') {
                setCustomerOrders();
                getDealerLogs(customer);
            } else {
                getDealerLogs("misc");
            }
            /*keep for modal window*/
            }


        /** PAGING **/
        var paging = function () {

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

            $scope.novalidName= !!!$scope.Us_call_log.person;

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
            )}
        }

        /* DIALOG BOX */

        $scope.openNewClaim = function (claim, customer, cprodId) {
            $uibModal.open({
                templateUrl: $scope.error ? defaultPaths.DEALER.URL + defaultPaths.CLAIMS.FOLDER + "/returnError.html?p="+version.v : defaultPaths.DEALER.URL + defaultPaths.CLAIMS.FOLDER + "/returnFeedbacksView.html?p="+version.v,
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
                            dealerView:true
                        };
                    }
                }
            }).result.then(
                function () {
                   
                    $state.go('detail', {id:customer}, { reload: true });
                }
            );
        }

        $scope.open = function (log, newLog,parentId) {
            /* send data to shared factory */
            common.isNewLog(newLog);

            if(!newLog)
                getDealerPersons(log.dealer);
            common.setLog(log);
            
            var modalInstance=$uibModal.open({
                templateUrl: "AngularApps/DealerUSA/dealers/myModalContent2.html?p="+version.v,
                //controller: "editCtrl",
                controller:"dealerDetailEditCtrl",
                size: "lg",
                animation: true,
                backdrop: 'static',
                keyboard: false,
                resolve: {
                    params: function () {
                        return {
                            title: $scope.getDealerDetail(customer),
                            dealer:customer,
                            userid: $scope.currentUser,
                            newLog:newLog,
                            parentId:parentId
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
                                selected.user.userid=res.data.userid;
                            }
                            )
                        factory.getDealerLogs(customer).then(
                            function (req) {
                            $scope.logs=req.data
                        });
                        //$scope.logs.push(selected);
                    }
                    else { /** EDITED LOG SAVE **/
                        selected.date_edit = Date.now();

                        for (var i = 0; i < $scope.logs[i]; i++) {
                            if ($scope.logs[i].id === selected.id)
                                $scope.logs[i] = selected;
                        }
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



        /* END DIALOG BOX */

        /**
        * SEARCH
        **/


        function activate() {
            getCurrentuUser();
            if (customer !== '') {
                setCustomerOrders();
                getDealerLogs(customer);
            } else {
                getDealerLogs("misc");
            }


            /*keep for modal window*/



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

        function setCustomerOrders() {
            factory.getDealerOrders(customer).then(
            function (response) {
                //$scope.responseStatus = response.status;
                $scope.orders = response.data;
                $scope.items2 = $scope.orders;
                paging();

            });
        }

        /*create list dealers logs and get persons[] for dealer */
        function getDealerLogs(cust) {
            factory.getDealerLogs(cust).then(
                    function (response) {
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
            return $scope.logs.length > 0 ? $scope.logs[0].usDealer.name: $scope.orders.length>0?$scope.orders[0][0].alpha:"";
        }


        $scope.showChanges = function () {
            $scope.orders = $filter('filter')($scope.items2, $scope.search.text);
            paging();
        }
        
    }
})();

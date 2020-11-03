(function () {
    'use strict';
    angular
        .module('app')
        .controller('listFeedbacks', returnFeedbacks);

    returnFeedbacks.$inject = ['$scope', '$timeout', '$state', '$uibModal', 'factoryClaims', 'defaultPaths','version'];
    
    function returnFeedbacks($scope, $timeout, $state, $uibModal, factoryClaims, defaultPaths,version) {
        $scope.Test = "Ovo je feedback";
        $scope.dealerDetailView = $scope.$parent.dealerDetail;
        $scope.claimType = {
            1:"Return",
            5:"Technical"
        };
        
        $scope.busy = false;
        $scope.error = false;
        $scope.months=0
        $scope.claimSelectedForDelete = null;
        $scope.getDate = function (num) {

            getUser();
            $scope.months = $scope.months + num;
            $scope.date = addMonths(new Date(), $scope.months);

            catchFeedbacks($scope.date);

        }
        
        $scope.delete = function (claim) {
                var deleted = deleteClaim(claim);
                if (deleted) {
                    for (var i = 0; i < $scope.Feedbacks.length; i++) {
                        if ($scope.Feedbacks[i].returnsid === claim.returnsid)
                            $scope.Feedbacks.splice(i, 1);
                    }
                } else {
                    $scope.ERROR = deleted;
                    $scope.deleteConfirm(null,true);
                }
        }
        /** MODAL WINDOW **/
        $scope.open = function (claim) {
            if (claim === null)
                console.log("NULL");
           // console.log("OTVORI MODAL WINDOW");
            $uibModal.open({
                templateUrl: $scope.error ? defaultPaths.DEALER.URL + defaultPaths.CLAIMS.FOLDER + "/returnError.html?v="+version.v : defaultPaths.DEALER.URL + defaultPaths.CLAIMS.FOLDER + "/returnFeedbacksView.html?v="+version.v,
                controller: 'ClaimsPopupCtrl',//claim !== null ? 'ClaimsPopupCtrl' : 'returnFeedbacks',
                size: "lg",
                animation: true,
                resolve: {
                    params: function () {
                        return {
                            claim: claim,
                            error: $scope.ERROR,
                            dealerView:true
                        };
                    }
                }
            })
            .result.then(
                function (data, dealerView) {
                    console.log("Podaci promjenjeni II", data, $scope.dealerDetailView);
                    if ($scope.dealerDetailView) {
                        catchDealerFeedbacks($scope.customer);
                    }else{
                        catchFeedbacks($scope.date);
                    }
                    
                    //for (var i = 0; i < $scope.Feedbacks; i++) {
                    //    if ($scope.Feedbacks[i].returnsid === data.returnsid) {
                    //        console.log("u IF sam");
                    //        $scope.Feedbacks[i] = data;
                    //        break;
                    //    }
                    //}
                    //alert('Podaci promjenjeni');
                    
                },
                function (data) {
                    console.log("ERROR");
                }

            );
        }

        $scope.deleteConfirm = function (claim, error) {
            $scope.claimSelectedForDelete = claim;
            $uibModal.open({
                templateUrl: error && claim===null ? "/AngularApps/ClaimsUSA/feedbacks/returnError.html?v="+version.v : "/AngularApps/ClaimsUSA/feedbacks/messageConfirm.html?v=version.v",
                controller: function ($uibModalInstance,$scope,params) {
                    $scope.return_no=params.return_no
                    $scope.cancel = function () {
                        $uibModalInstance.dismiss();
                    };
                    $scope.proceed = function () {
                        $uibModalInstance.close(true);
                    }
                    $scope.closeError = function () {
                        $uibModalInstance.dismiss();
                        $state.go('create', {}, { reload: true });

                    }
                },
                size: "sm",
                animation: true,
                resolve: {
                    params: function () {
                        return {
                            return_no: claim.return_no,
                            error: $scope.ERROR
                        };
                    }
                }
            })
            .result.then(
                /** IF CONFIRMED IN MODAL DIALOG WINDOW THEN PROCEED DELETING **/
                function () {
                    $scope.delete(claim);
                    $scope.claimSelectedForDelete = null;
                },
                function () {
                    $scope.claimSelectedForDelete = null;
                }
            );

            //return false;
        }

        $scope.change = function () {
            $scope.Product = '';
            catchProducts();
        }

        activate();

        function activate() {
            $scope.user=getUser();
            $scope.date = addMonths(new Date(), $scope.months);
            if ($scope.dealerDetailView)
            {
               
                catchDealerFeedbacks($scope.customer);
            }
            else {
                catchFeedbacks($scope.date);

            }

        }
        function catchDealerFeedbacks(customer) {
            $scope.Feedbacks = [];
            $scope.busy = true;
            factoryClaims.getDealerFeedbacks(customer).then(
                    function (feedbacks) {
                        $scope.busy = false;

                        $scope.Feedbacks = feedbacks;
                    }
                )
        }

        function catchFeedbacks(date) {
            $scope.Feedbacks = [];
            $scope.busy = true;
            factoryClaims.getFeedbacks(date).then(
                    function (feedbacks) {
                        $scope.busy = false;

                        $scope.Feedbacks = feedbacks;
                    }
                )
        }
        function addMonths(date, months) {
            date.setMonth(date.getMonth() + months);
            return date;
        }

        function getUser() {
            factoryClaims.getUser().then(
                    function (user) {
                        return user;
                    }
                )
        }
        function deleteClaim(claim) {
            factoryClaims.deleteClaim(claim).then(
                    function (data) {
                       return true;
                    },
                    function (error) {
                        return error;
                    }
                )
            return true;
        }



    }
})();

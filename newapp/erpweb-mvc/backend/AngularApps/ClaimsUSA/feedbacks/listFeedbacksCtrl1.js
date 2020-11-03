(function () {
    'use strict';

   

    angular
        .module('app')
        .controller('listFeedbacks', returnFeedbacks);

    returnFeedbacks.$inject = ['$scope','$timeout','$state','$uibModal' ,'factory'];
    
    function returnFeedbacks($scope, $timeout,$state,$uibModal, factory) {
        $scope.claimType = {
            1:"Return",
            5:"Technical"
        };
        $scope.title = "Naslov";
        $scope.listfeed = "FEEDBACK";
        
        
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

        $scope.open = function (claim) {
            $uibModal.open({
                templateUrl: $scope.error ? "/AngularApps/ClaimsUSA/feedbacks/returnError.html" : "/AngularApps/ClaimsUSA/feedbacks/returnFeedbacksView.html",
                controller: function ($uibModalInstance, $scope, params, factory) {
                    //$cope.modalWindow = true;
                    $scope.model = {};
                    $scope.hideBackToList = true;
                    $scope.model = params.claim;
                    $scope.model.Images = [];
                    
                    $scope.return_no = params.return_no;
                    $scope.disableInEditMode = true;

                    $scope.optAmaraOrCross = claim.factory_id === 406 ? true : false; /* true === Ammara false === Crosswater */
                    $scope.optDealerConsumer = $scope.model.dealer_id !== null;

                    init();         
                    
                    $scope.change = function () {
                        $scope.Product = '';
                        catchProducts();
                    }
                    $scope.ProductSelected = function ($item, $model, $label, $event) {
                        $scope.model.cprod_id = $model.cprod_id;
                        $scope.model.client_id = $model.cprod_user;
                    }
                    $scope.DealerSelected = function myfunction(item, model, $label, event) {
                        $scope.model.dealer_id = item.customer;
                        //$label = "(" + item.customer + ")" + item.customer;
                        // $scope.item = item;
                        // $scope.label = label;
                        // $scope.event = event;
                    }

                    $scope.error = params.error;

                    $scope.cancel = function () {
                        $uibModalInstance.dismiss();
                    };

                    $scope.saveForm = function () {
                        $scope.busySave = true;
                        factory.saveFeedback($scope.model).then(
                            function (data) {
                                //$scope.open();
                               
                                $state.go('list', {}, { reload: true });
                                //$scope.Products = data;
                                $scope.busySave = false;
                                $uibModalInstance.dismiss();
                                ////return data;
                            },
                            function (err) {
                                $scope.error = true;
                                $scope.ERROR = err;
                                console.log("ERROR");
                                console.log(err);
                                console.log(err.headers);
                                //$scope.open();
                            });
                    }


                    $scope.closeError = function () {
                        $uibModalInstance.dismiss();
                        $state.go('create', {}, { reload: true });

                    }


                    $scope.uploadedFiles = [];
                    $scope.deleteTempImage=function($index){
                        $scope.model.Images.splice($index,1);
                    }

                    $scope.deleteImage = function (image, $index) {
                        factory.deleteImage(image).then(
                            function (data) {
                                $scope.model.images.splice($index, 1);
                            },
                            function (error) {
                                return error;
                            }
                        )
                        
                    }
                    $scope.fileUpload = {

                        url: '/ClaimsUsa/Files',
                        options: {
                            multi_selection: true,
                            max_file_size: '32mb',
                            resize: { width: 800, height: 600, quality: 90 },
                            filters: [{
                                extensions: 'jpg,png,pdf'
                            }]
                        },
                        callbacks: {
                            filesAdded: function (uploader, files) {
                                $scope.uploadInProgress = true;
                                files.forEach(function (elem) {
                                    $scope.uploadedFiles[elem.id] = elem;
                                });
                                $timeout(function () {
                                    uploader.start();
                                }, 1);
                            },
                            uploadProgress: function (uploader, file) {
                                //$scope.loading = file.percent/100.0;
                                $scope.uploadedFiles[file.id].percent = file.percent;
                            },
                            fileUploaded: function (uploader, file, response) {
                               // $scope.model.Images = [];
                                $scope.model.Images.push(new File(file.id, file.name));
                            }
                        }
                    } //END $scope.fileUpload
                    $scope.GetFileImage = function (img) {
                        if (img !== null) {
                            var id = parseInt(img.image_unique);
                            var isPic = $scope.IsPicture(img.return_image);
                            if (isNaN(id))
                                return !isPic ? $scope.GetDocIcon(img) : 'GetTempFile?file=' + img.return_image;
                            else {
                                return !isPic ? $scope.GetDocIcon(img) : 'GetFile?filename=' + img.return_image;
                            }
                        }
                    }
                    $scope.IsPicture = function (image_name) {
                        var ext = GetExtension(image_name).toLowerCase();
                        return ext === "jpg" || ext === "jpeg" || ext === "gif" || ext === "png" || ext === "bmp";
                    };



                    function init() {
                        catchProducts("", $scope.optAmaraOrCross);
                        if ($scope.model.dealer_id !== null) {
                            catchDealers();

                        }
                    }
                    function catchProducts() {
                        $scope.busy = true;
                        $scope.Products = [];
                        factory.getProducts("", $scope.optAmaraOrCross).then(
                            function (data) {
                                $scope.busy = false;
                                $scope.Products = data;
                                for (var i = 0; i < $scope.Products.length; i++) {
                                    if($scope.Products[i].cprod_id === $scope.model.cprod_id){
                                        $scope.Product = $scope.Products[i];
                                        break;
                                    }
        
                                }
                            },
                            function (err) {
                                $scope.ERROR = err;
                            });
                    }
                    function catchDealers() {
                        factory.getDealers().then(
                                function (dealers) {
                                    $scope.Dealers = dealers;
                                    for (var i = 0; i < $scope.Dealers.length; i++) {
                                        //console.log($scope.Dealers[i].dealer_id + " - " + $scope.model.dealer_id)/AngularApps/DealerUSA/claims/
                                        if ($scope.Dealers[i].customer === $scope.model.dealer_id) {
                                            $scope.dealer = $scope.Dealers[i]
                                            break;
                                        }
                                    }
                                }
                            )
                    }
                    function File(id, filename) {
                        this.file_id = id;
                        this.filename = filename;
                        this.image_unique = null;
                        this.return_comment_file_id = null;
                        this.return_image = filename;
                        this.image_name = filename;
                        this.Progress = 0;
                        this.added_date = new Date();
                        var self = this;
                        this.DisplayProgress = function () {
                            return $.validator.format('{0} %', self.Progress);
                        }
                    }
                },
                size: "lg",
                animation: true,
                resolve: {
                    params: function () {
                        return {
                            claim: claim,

                            error: $scope.ERROR
                        };
                    }
                }
            });
        }

        $scope.deleteConfirm = function (claim, error) {
            $scope.claimSelectedForDelete = claim;
            $uibModal.open({
                templateUrl: error && claim===null ? "/AngularApps/ClaimsUSA/feedbacks/returnError.html" : "/AngularApps/ClaimsUSA/feedbacks/messageConfirm.html",
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
                function (paramProceed) {
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
            getUser();
            $scope.date = addMonths(new Date(), $scope.months);
            catchFeedbacks($scope.date);
            console.log("DEALER DETAIL: ", $scope.$parent.dealerDetail);

        }

        function catchFeedbacks(date) {
            $scope.Feedbacks = [];
            $scope.busy = true;
            factory.getFeedbacks(date).then(
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
            factory.getUser().then(
                    function (user) {
                        $scope.user = user;
                    }
                )
        }
        function deleteClaim(claim) {
            factory.deleteClaim(claim).then(
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

(function () {
    'use strict';

   

    angular
        .module('app')
        .controller('returnFeedbacks', returnFeedbacks);

    returnFeedbacks.$inject = ['$scope','$timeout','$state','$uibModal' ,'factory'];
    
    function returnFeedbacks($scope, $timeout,$state,$uibModal, factory) {
       
        $scope.model = {};
        $scope.model.Images = [];
        $scope.model.claim_type = 1;
        $scope.optAmaraOrCross = true;
        $scope.optDealerConsumer = true;
        $scope.title = 'returnFeedbacks';
        $scope.Products = [];
        $scope.busySave = false;

       
        activate();
        /*ZA SAD NIJE U UPOTREBI KASNIJE  METNUT U UPOTREBU*/
        $scope.GetProducts = function (term) {
            $scope.busy = true;
            $scope.Products = [];
            factory.getProducts(term,$scope.optAmaraOrCross).then(
                function (data) {
                    $scope.Products = data;

                    $scope.busy = false;
                    return data;
                },
                function (err) {
                    $scope.ERROR = err;
                });
            //return $http()

        }
        $scope.error = false;
        $scope.open = function () {
            $uibModal.open({
                templateUrl: $scope.error ? "/AngularApps/ClaimsUSA/feedbacks/returnError.html" : "/AngularApps/ClaimsUSA/feedbacks/returnSaveSuccessMessage.html",
                controller: function ($uibModalInstance, $scope, params) {
                    $scope.return_no = params.return_no;
                    $scope.error = params.error;
                    $scope.cancel = function () {
                        $state.go('list', {}, {reload:true});
                        $uibModalInstance.dismiss();
                    };
                    $scope.closeError = function () {
                        $uibModalInstance.dismiss();
                        $state.go('create', {}, { reload: true });

                    }
                },
                //size: "lg",
                animation: true,
                resolve: {
                    params: function () {
                        return {
                            return_no: $scope.model.return_no,
                            error:$scope.ERROR
                        };
                    }
                }
            });
        }
        $scope.change = function () {
            $scope.Product = '';
            catchProducts();
        }

        $scope.changeOptionType = function () {
            getRefCode($scope.model.claim_type);
        }
        $scope.checkDealerConsumer = function () {
            if (!$scope.optDealerConsumer) {
                $scope.dealer = "";
                $scope.model.dealer_id = null;
            } else {
                $scope.model.name = "";
                $scope.model.phone = "";
                $scope.model.mail = "";

            }
        }
        $scope.dealer = "";
        $scope.DealerSelected = function myfunction(item, model, $label,event) {
            $scope.model.dealer_id = item.customer;
          //$label = "(" + item.customer + ")" + item.customer;
           // $scope.item = item;
           // $scope.label = label;
           // $scope.event = event;
        }
        $scope.ProductSelected = function ($item, $model, $label, $event) {
            $scope.model.cprod_id = $model.cprod_id;
            $scope.model.client_id = $model.cprod_user;
        }
        $scope.uploadedFiles = [];
        $scope.fileUpload = {
            
            url: '/ClaimsUsa/Files',
            options: {
                multi_selection: true,
                max_file_size: '32mb',
                resize: { width: 800, height: 600, quality: 90 },
                filters: [{
                    extensions:'jpg,png,pdf'
                }]
            },
            callbacks: {
                filesAdded: function (uploader,files) {
                    $scope.uploadInProgress = true;
                    files.forEach(function (elem) {
                        $scope.uploadedFiles[elem.id] = elem;
                    });
                    $timeout(function () {
                        uploader.start();
                    }, 1);
                },
                uploadProgress: function(uploader, file)
               {
                    //$scope.loading = file.percent/100.0;
                    $scope.uploadedFiles[file.id].percent = file.percent;
               },
               fileUploaded: function(uploader, file, response)
               {
                    
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
       
        $scope.deleteTempImage = function (index) {
            //for (var i = 0; i < $scope.model.Images.length; i++) {
            //    if($scope.model.Images)
            //}
            $scope.model.Images.splice(index,1);
        };

        $scope.cancel = function () {

           $state.go('list', {}, { reload: true });
        }
        $scope.saveForm = function () {
                $scope.busySave = true;
                factory.saveFeedback($scope.model).then(
                    function (data) {
                        $scope.open();
                        $state.go('create', {}, { reload: true });
                        //$scope.Products = data;
                        $scope.busySave = false;
                        ////return data;
                    },
                    function (err) {
                        $scope.error = true;
                        $scope.ERROR = err;
                        console.log("ERROR");
                        console.log(err);
                        console.log(err.headers);
                        $scope.open();
                    });
        }
        function activate() {
            /*prvi parametar koji je "" to je therm za ajax koji predstavlja tekst koji je upisan u search box*/
            catchProducts("", $scope.optAmaraOrCross);
            catchDealers();
            /** REF ID **/
            getRefCode(1);
        }
       
        function catchDealers() {
            factory.getDealers().then(
                    function (dealers) {
                        $scope.Dealers=dealers;
                    }
                )
        }

        function catchProducts() {
            $scope.busy = true;
            $scope.Products = [];
            factory.getProducts("", $scope.optAmaraOrCross).then(
                function (data) {
                    $scope.busy = false;
                    $scope.Products = data;
                    //console.log("PRODUCTS", $scope.optAmaraOrCross, $scope.Products.length);

                    
                    //return data;
                },
                function (err) {
                    $scope.ERROR = err;
                });
        }
        
        function getRefCode(refId) {
            factory.getReference(refId).then(
                function (data) {
                    $scope.model.return_no = data;
                })
        }

        function removeImage(name) {
            factory.getReference(name).then(
               function (data) {
                   $scope.model.return_no = data;
               })
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
    }
})();

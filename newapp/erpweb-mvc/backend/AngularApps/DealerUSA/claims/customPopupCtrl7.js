(function () {
    'use strict'
    angular
        .module('app')
        .controller('ClaimsPopupCtrl', 
    function  ($uibModalInstance, $scope, params, factoryClaims,$state,$timeout,types) {
        //$cope.modalWindow = true;
        $scope.ctrlName = 'customPopupCtrl.js (ClaimsPopupCtrl)';
        $scope.types = types;
        $scope.model = {};
        $scope.hideBackToList = true;
        $scope.disableInEditMode = true;
        /**CREATE NEW**/
        if (params.claim === null) {
            $scope.modalTitle = "Create New Claim";
            $scope.model.claim_type = 1;
            $scope.optAmaraOrCross = true;
            $scope.optDealerConsumer = true;
            $scope.model.Images = [];

            var ret = getRefCode(1);
            $scope.model.return_no = getRefCode(1);
            $scope.return_no = ret;
        
            $scope.disableInEditMode = false;
            $scope.changeOptionType = function () {
                getRefCode($scope.model.claim_type);
            }
            //console.log("typeof params.customer", typeof params.cutomer === 'undefined',params.customer);
            //if(typeof params.cutomer !== 'undefined')
            $scope.model.dealer_id = params.customer;
            $scope.model.cprod_id = params.cprodId;

           
        /**EDIT**/
        } else {
            $scope.modalTitle = "Edit Claim";

            $scope.model = params.claim;
            $scope.model.Images = [];

            $scope.return_no = params.return_no;

            $scope.optAmaraOrCross = params.claim.factory_id === 406 ? true : false; /* true === Ammara false === Crosswater */
            $scope.optDealerConsumer = $scope.model.dealer_id !== null;
        }
        


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
            factoryClaims.saveFeedback($scope.model).then(
                function (data) {
                    //$scope.open();
                               
                    //$state.go('list', {}, { reload: true });
                    //$scope.Products = data;
                    $scope.busySave = false;
                    $uibModalInstance.close($scope.model,params.dealerView);
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
            $state.go('claims', {}, { reload: true });

        }


        $scope.uploadedFiles = [];
        $scope.deleteTempImage=function($index){
            $scope.model.Images.splice($index,1);
        }

        $scope.deleteImage = function (image, $index) {
            factoryClaims.deleteImage(image).then(
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
                    return !isPic ? $scope.GetDocIcon(img) : 'ClaimsUsa/GetTempFile?file=' + img.return_image;
                else {
                    return !isPic ? $scope.GetDocIcon(img) : 'ClaimsUsa/GetFile?filename=' + img.return_image;
                }
            }
        }
        $scope.IsPicture = function (image_name) {
            var ext = GetExtension(image_name).toLowerCase();
            return ext === "jpg" || ext === "jpeg" || ext === "gif" || ext === "png" || ext === "bmp";
        };



        function init() {
            //alert($scope.model.cprod_id);
            catchProducts("", $scope.optAmaraOrCross);
            if ($scope.model.dealer_id !== null) {
                catchDealers();

            }

           
        }

        function getRefCode(refId) {
            factoryClaims.getReference(refId).then(
                function (data) {
                    $scope.model.return_no = data;
                    //return data;
                })
        }

        function catchProducts() {
            $scope.busy = true;
            $scope.Products = [];
            factoryClaims.getProducts("", $scope.optAmaraOrCross).then(
                function (data) {
                    $scope.busy = false;
                    $scope.Products = data;
                    for (var i = 0; i < $scope.Products.length; i++) {
                        if ($scope.Products[i].cprod_id === $scope.model.cprod_id) {
                            
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
            factoryClaims.getDealers().then(
                    function (dealers) {
                        $scope.Dealers = dealers;
                        for (var i = 0; i < $scope.Dealers.length; i++) {
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
    })
})();
(function () {
    'use strict';

    angular
        .module('app')
        .controller('editRecheckController', editRecheckController);

    editRecheckController.$inject = ['$location', '$state', 'factoryStorage', 'factoryInspectionCASample', '$timeout', 'FILE_CATEGORY','RECHECK_STATUS'];

    function editRecheckController($location, $state, factoryStorage, factoryInspectionCASample, $timeout, FILE_CATEGORY, RECHECK_STATUS) {
        /* jshint validthis:true */
        window.onscroll = function () {
            console.info("Window is scrolling")
        }

        var vm = this;
        vm.title = 'editRecheckController';
        vm.param = $state.params.id;
        vm.recheckStatus = RECHECK_STATUS;
        vm.recheck = {};
        vm.isProcessing = false;

        if (factoryStorage.getRecheck().returnsid == null) {
            factoryInspectionCASample.getRecheck(vm.param).then(
                function (data) {
                    vm.recheck.returnsid = data.returnsid;
                    vm.recheck.client_comments2 = data.client_comments2;
                    vm.recheck.recheck_date = new Date(data.recheck_date);
                }, function ()
                {
                    
                }
            );
        }
        else {
            vm.recheck.returnsid = factoryStorage.getRecheck().returnsid;
            vm.recheck.client_comments2 = factoryStorage.getRecheck().client_comments2;

            if (factoryStorage.getRecheck().recheck_date !== null) {
                vm.recheck.recheck_date = new Date(factoryStorage.getRecheck().recheck_date); //!= null? new Date(vm.recheck.recheck_date):'';
            }
        }

        console.log("RECHECK STATUS: ", vm.recheck.recheck_status, vm.recheck.recheck_status === RECHECK_STATUS.NA  );

        if (vm.recheck.recheck_status === RECHECK_STATUS.NA)
            vm.recheck.recheck_status = RECHECK_STATUS.OK;
        

        //vm.param is returnsid (in case of refresh)
        factoryInspectionCASample.getFeedbackImages(vm.param, FILE_CATEGORY.RECHEK_PHOTOS).then(
            function (data) {
                vm.recheck.Images = data;
            }
            , function (ERROR) {
                console.error(ERROR)
            }
        );

        activate();

        function activate() {

        }
        vm.uploadedFiles = [];
        vm.imagesBlob = [];

        vm.saveRecheck = function () {
            if (!vm.isProcessing) {
                vm.isProcessing = true;
                factoryInspectionCASample.saveRecheck(vm.recheck).then(
                    function (data) {
                        vm.isProcessing = false;
                        $state.go('rechecks');
                    }, function () {
                        vm.isProcessing = false;
                    }
                )
            }
        }
        /** UPLOAD FILES */
        vm.fileUpload = {
            url: factoryInspectionCASample.defaultUrl + 'filesReturns',
            options: {
                multi_selection: true,
                max_file_size: '32mb',
                resize: { width: 800, height: 600, quality: 90 },
                filters: [
                    { extensions: 'jpg,png,pdf' }
                ]
            },
            
            callbacks: {
                filesAdded: function (uploader, files) {
                    vm.uploadInProgress = true;

                    console.log("UPLOADER", uploader);
                   
                    files.forEach(function (elem) {

                        var pr = new mOxie.Image();
                       
                        pr.onload = function () {
                            console.log("PRIJE ONLOAD PR: ", pr.name);
                            console.log("ONLOAD");
                            pr.downsize(300, 300);
                            vm.imagesBlob.push({ "url": pr.getAsDataURL(), "name": pr.name });
                            
                        }
                        pr.load(elem.getSource());
                                                
                        vm.uploadedFiles[elem.id] = elem;

                    });
                    $timeout(function () {
                        uploader.start();
                    }, 1);
                },

                uploadProgress: function (uploader, file) {
                    console.log("UploadinProgres");
                    //vm.loading = file.percent/100.0;
                    vm.uploadedFiles[file.id].percent = file.percent;
                },
                beforeUpload: function (uploader, file) {
                    uploader.settings.multipart_params = { id: file.id };
                },
                fileUploaded: function (uploader, file, response) {

                    vm.recheck.Images.push(new File(file.id,vm.recheck.returnsid, file.name, FILE_CATEGORY.RECHEK_PHOTOS));
                    //vm.Feedback.Images.push(new File(file.id, file.name));
                }
            }
        }

        function File(id,return_id, return_image,file_category) {
            this.file_id = id;
            this.return_id = return_id;
            this.return_image = return_image;
            this.file_category = file_category;
            this.filename = return_image;
            this.image_unique = null;
            this.return_comment_file_id = null;
            this.image_name = return_image;
            this.Progress = 0;
            this.added_date = new Date();
            var self = this;
        }

        vm.deletePhoto = function (index, name) {
            for (var i = 0; i < vm.recheck.Images.length; i++) {
                if (vm.recheck.Images[i].return_image === name) {
                    vm.recheck.Images.splice(i, 1);
                }
            }
            vm.imagesBlob.splice(index, 1);
        } 

        vm.DeleteFileNewCA = function (name, index) {
            factoryInspectionCASample.deleteTempFile(name).then(
                function () {
                    vm.Feedback.Images.splice(index, 1);
                },
                function (err) {
                    console.log("Error", err);
                }
            )
        }

        vm.GetFileImage = function (data) {

            if (data != null) {
                var id = parseInt(data.image_unique);
                return factoryInspectionCASample.defaultUrl + 'getTempUrl?file_id=' + data.return_image;
            }
            return '';
        };
        vm.IsPicture = function (image_name) {
            var ext = GetExtension(image_name).toLowerCase();
            return ext == "jpg" || ext == "jpeg" || ext == "gif" || ext == "png" || ext == "bmp";
        };

    }
})();

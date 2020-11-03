(function () {
    'use strict';

    angular
        .module('app')
        .controller('itFeedbackCreateController', itFeedbackCreateController);

    itFeedbackCreateController.$inject = ['itFactory', '$timeout'];

    function itFeedbackCreateController(itFactory, $timeout) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'itFeedbackCreateController';
        vm.showSaved = false;
        vm.showCreate = true;
        vm.images = [];
        vm.feedback = {};
        activate();

        function activate() {

            //itFactory.clearTempFiles.getFeedbackModel().then(
            //    function () {
            //        console.log("Cleared");
            //    },
            //    function (error) {
            //        console.error("Clearing: ", error);
            //    }
            //)

            

            itFactory.getFeedbackModel().then(
                function (response) {
                    vm.feedback = response;
                    vm.feedback.images = [];
                    vm.feedback.subscriptions = [];
                },
                function (err) {
                    console.error("IT FEEDBACK CONTOLLER! Feedbeck Model");
                    console.error(err);
                }
            )
            itFactory.getImportances().then(
                function (response) {
                    vm.importances = response;
                    vm.feedback.importance_id = 4;

                },
                function (err) {
                    console.error("IT FEEDBACK CONTOLLER! Importances");
                    console.error(err);
                }
            );
            itFactory.getCategories().then(
                function (response) {
                    vm.categories = response;
                    vm.feedback.feedback_category_id = 3;
                },
                function (err) {
                    console.error("IT FEEDBACK CONTROLLER! Categories");
                    console.error(err);
                }
            );
            itFactory.getIssueTypes().then(
                function (response) {
                    vm.issueTypes = response;
                    vm.feedback.issue_type_id = 1;
                },
                function (err) {
                    console.error("IT FEEDBACK CONTROLLER! Categories");
                    console.error(err);
                }
            );

        }
        //vm.feedback.images = [];
        vm.DeleteFileNew = function (data, _index) {
            //var file= $scope.FindFile(data.file_id, $scope.Feedback.Images);
            //if (file != null) {
            //    self.Feedback.Images.remove(file.selectedIndex);
            //    DeleteTempFileNew(file.filename);
            //}


            //if (data.file_id == $scope.NewComment.Files[_index == null ? this.$index : _index].file_id) {
               
            //}
            console.log("Delete image ctrl: ", data);
            itFactory.deleteTempFileNew(data.return_image).then(
                function(){
                    vm.images.splice(_index, 1);
                },
                function (error) {
                    console.error("DELETE FILE: ",error)
                }
            );

        };

        vm.fileUpload = {
            url: '/api/claims/uploadFile',
            options: {
                multi_selection: false,
                max_file_size: '32mb',
                resize: { width: 800, height: 600, quality: 90 },
                filters: [
                    {
                        extensions: 'jpg,png'
                    }

                ]

            },
            callbacks: {
                filesAdded: function (uploader, files) {
                    vm.productListuploader = uploader;
                    vm.productListProgress = 0;
                    $timeout(function () {
                        uploader.start();
                    }, 1);
                },
                uploadProgress: function (uploader, file) {
                    console.log("UPLOAD PROGRES: ", file.percent)
                    vm.productListProgress = file.percent;
                },
                fileUploaded: function (uploader, file, response) {
                    console.log("File uploaded test", file);
                    vm.images.push(new File(file.id, file.name));
                    //  vm.feedback.images.push(new File(file.id, file.name));
                }
            }
        }

        vm.GetFileUrl = function (data) {
            console.log("DATA: ", data);
            if (data != null) {
                var id = parseInt(data.image_unique);
                if (isNaN(id))
                    return '/api/claims/getTempFileUrl/?file=' + escape(data.return_image)
                //return '@Url.Action("GetTempFile")/?file=' + escape(data.return_image);
                else {
                    return '@Url.Action("GetFile")/?filename=' + escape(data.return_image);
                }
            }
            return '';
        };
        vm.GetUsers = function (term) {
            console.log("GET USERS: ", term);
            vm.searching = true;
            return itFactory.getSubscribers(term).then(
                function (data) {

                    return data;
                }
            )

        }

        function File(id, return_image, return_id, file_category) {
            this.file_id = id;
            this.return_image = return_image;
            this.return_id = return_id;

            this.file_category = file_category;
        }
        vm.DeleteSelected = function (s) {


            for (var i = 0; i < vm.feedback.subscriptions.length; i++) {

                if (vm.feedback.subscriptions[i].subs_useruserid === s.subs_useruserid) {

                    vm.feedback.subscriptions.splice(i, 1);
                }
            }
        }

        vm.UserSelected = function ($item, $model, $label, $even) {
            console.log("Selected user");
            var selectedSubscriber = { subs_useruserid: $item.userid, userwelcome: $item.userwelcome, User: { userwelcome: $item.userwelcome } };
            vm.selectedSub = "";
            vm.feedback.subscriptions.push(selectedSubscriber)
            //$('#selectedSub').val('');
            //var existing = $.grep(vm.feedback.subsbcriptions, function (elem) {
            //     return elem.userid==$scope.selectedSubQC.userid
            //})
            // vm.feedback.
        }

        vm.SaveFeedback = function () {
            itFactory.createFeedback(vm.feedback).then(
                function (response) {
                    //console.log("Spremljeno");
                    vm.showSaved = true;
                    vm.showCreate = false;
                    vm.return_no = response.return_no;
                    //console.log(response);

                },
                function (err) {
                    console.error("IT FEEDBACK CONTOLLER! Create IT feedback");
                    console.error(err);
                }
            );
        }

    }
})();

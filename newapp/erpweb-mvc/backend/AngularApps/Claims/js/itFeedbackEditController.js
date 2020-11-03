(function () {
    'use strict';

    angular
        .module('app')
        .controller('itFeedbackEditController', editITFeedbackController);

    editITFeedbackController.$inject = ['$state', '$stateParams', 'itFactory', 'importance', 'feedbackId', '$http', '$timeout'];

    function editITFeedbackController($state, $stateParams, itFactory, importance, feedbackId, $http, $timeout) {
        /* jshint validthis:true */
        var vm = this;
        vm.internal = true;
        vm.external = false;

        vm.title = 'IT Feedback Edit Controller: ' + $stateParams.id;
        vm.setActive = function () {
            vm.internal = !vm.internal;
            vm.external = !vm.external;
        }
        vm.NewComment = {};
        vm.NewComment.Files = [];
        vm.NewCommentInt = {};
        vm.NewCommentInt.Files = [];

        itFactory.getFeedback($stateParams.id).then(
            function (data) {
                vm.feedback = data;
            }, function (err) {
                console.error("ERROR: ", err);
            }
        );

        //activate();

        //function activate() { }
        function IsPicture(image_name) {
            var ext = GetExtension(image_name).toLowerCase();
            return ext == "jpg" || ext == "jpeg" || ext == "gif" || ext == "png" || ext == "bmp";
        };

        vm.openClose = function () {

            var oc= vm.feedback.openclosed == 0 ? 1 : 0;
            
            itFactory.openCloseFeedback(vm.feedback.returnsid,oc).then(
                function (data) {
                    vm.feedback.openclosed = data;
                    window.location = '/claims/ItFeedbacks';

                }, function (err) {
                    console.error("ERROR: ", err);
                }
            );
            console.log("Open closed - 0/1",oc);
            
        }


        vm.GetUsers = function (term) {
            console.log("GET USERS: ", term);
            vm.searching = true;
            return itFactory.getSubscribers(term).then(
                function (data) {
                    vm.SEARCH = data.data;
                    return data.data;
                }
            )

        }

        vm.GetFileUrl = function (data) {

            if (data != null) {
                //var id = parseInt(data.image_unique());
                // if (isNaN(id))
                //      return '@Url.Action("GetTempFile")/?file=' + escape(data.return_image());
                //  else {
                //return '@Url.Action("GetFile")/?filename=' + escape(data.return_image());
                var imageName = data.split('/')
                console.log("FILE NAME!: ", imageName[imageName.length - 1]);
                //    return '/images/upload/feedback/'+data
                //}
            }
            return '';
        };
        vm.GetImportance = function (imp) {

            if (imp === importance.high)
                return 'label-danger';
            if (imp === importance.medium)
                return 'label-warning'

            return 'label-success' //importance.low
        }

        /**COMMENTS**/
        vm.CreateComment = function (typeComment) {

            console.log("TYPE: ", typeof typeComment === 'undefined');

            var result;
            var isExternal = typeComment === 'undefined';
            vm.busy = true;

             /* external comments*/
            if (isExternal ) {
                vm.NewComment.comments_to = 1;
                vm.NewComment.return_id = vm.feedback.returnsid;

            }
            else { /* Internal */
                vm.NewCommentInt.comments_type = 1;
                vm.NewCommentInt.return_id = vm.feedback.returnsid;
            }

            $http.post('/Claims/CreateComment', { comment: isExternal ? vm.NewComment :vm.NewCommentInt, closeTicket: vm.response_type == 1 }).
                success(function (data, status, headers, config) {
                    result = data;
                    vm.newCommentRow = false;
                    vm.busy = false;

                    result.comments_date = fromJSONDate(result.comments_date);

                    //if (result.Files != null && result.Files.length > 0) {
                    //    for (var i = 0; i < result.Files.length; i++) {
                    //        result.Files[i].Progress = 100;
                    //        result.Files[i].DisplayProgress = '';
                    //    }
                    //}
                    vm.feedback.comments.push(result);
                    //vm.NewComment, vm.NewCommentInt = {};
                    //vm.NewComment.Files,vm.NewCommentInt.Files = [];
                    //uploaderExt.refresh();
                    //uploaderInt.refresh();
                    if (isExternal)
                        vm.CancelComment(isExternal);
                    else
                        vm.CancelComment(isExternal);
                }).
                error(function (data, status, headers, config) {
                });

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
        vm.UserSelected = function ($item, $model, $label, $even) {
            console.log("Selected user");
            var selectedSubscriber = { subs_useruserid: $item.userid, userwelcome: $item.userwelcome, User: { userwelcome: $item.userwelcome } };
            vm.selectedSub = "";
           

            itFactory.addNewSubscriber($item.userid, vm.feedback.returnsid).then(
                function (data) {
                    vm.addNewSubscriber = false;
                    vm.feedback.subscriptions.push(selectedSubscriber);

                },
                function (error) {
                    console.error("SUBSCRIPTION UPDATE!")
                    console.error(error)

                }
                
            )
        }
        vm.DeleteSelected = function (s) {

            console.log("DElete", s.subs_id);
            if (typeof s.subs_id !== 'undefined') {
                itFactory.deleteSub(s.subs_id).then(
                    function (data) {

                        for (var i = 0; i < vm.feedback.subscriptions.length; i++) {

                            if (vm.feedback.subscriptions[i].subs_useruserid === s.subs_useruserid) {

                                vm.feedback.subscriptions.splice(i, 1);
                            }
                        }
                    },
                    function (error) {
                        console.error("SUBSCRIPTION UPDATE!")
                        console.error(error)

                    }

                )
            } else {
                for (var i = 0; i < vm.feedback.subscriptions.length; i++) {

                    if (vm.feedback.subscriptions[i].subs_useruserid === s.subs_useruserid) {

                        vm.feedback.subscriptions.splice(i, 1);
                    }
                }
            }
        }
        vm.CancelComment = function (isExternal) {
            if (isExternal){ /* EXTERNAL */
                for (var i = 0; i < vm.NewComment.Files.length; i++) {
                    vm.DeleteFileNew(vm.NewComment.Files[i], i);
                }
                vm.NewComment = {};
                vm.NewComment.Files = [];
                vm.newCommentRow = false;
            } else { /* INTERNAL */
                for (var i = 0; i < vm.NewCommentInt.Files.length; i++) {
                    vm.DeleteFileInt(vm.NewCommentInt.Files[i], i);
                }
                vm.NewCommentInt = {};
                vm.NewCommentInt.Files = [];
                vm.newCommentRowInt = false;
            }

        }

        vm.DeleteFileNew = function (data, _index) {
           
            if (data.file_id == vm.NewComment.Files[_index == null ? this.$index : _index].file_id) {
                vm.NewComment.Files.splice(this.$index, 1);
            }
            itFactory.deleteTempFile(data.filename).then(
                function () {
                    
                },
                function (err) {
                    console.log("ERRROR DELETE TEMP FILE");
                }
            
            );

        };
        vm.DeleteFileInt = function (data, _index) {
            if (vm.file_id == vm.NewCommentInt.Files[_index == null ? this.$index : _index].file_id) {
                vm.NewCommentInt.Files.splice(this.$index, 1);
            }
            itFactory.deleteTempFile(data.filename).then(
                function () {

                },
                function (err) {
                    console.log("ERRROR DELETE TEMP FILE");
                }

            );

        };
        vm.DeleteCommentFile = function (comment, file, _index) {

            comment.Files.splice(this.$index, 1)

           // itFactory.DeleteCommentFile(file.return_comment_file_id, file.return_comment_id, file.image_id);
        }
        vm.CancelCommentInt = function () {
            vm.NewCommentInt.comments = "";
            for (var i = 0; i < vm.NewCommentInt.Files.length; i++) {
                vm.DeleteFileInt(vm.NewCommentInt.Files[i], i);
            }
            vm.newCommentRowInt = false;
        }

        vm.ExternalComments = function () {
            var result = [];
            for (var i = 0; i < vm.Feedback.Comments.length; i++) {
                var comment = vm.Feedback.Comments[i];
                if (comment.comments_to == 1)
                    result.push(comment);
            }
            return result.slice(0).sort(function (l, r) { return l.comments_id > r.comments_id ? -1 : 1; });

        };
        vm.InternalComments = function () {
            var result = [];
            for (var i = 0; i < vm.Feedback.Comments.length; i++) {
                var comment = vm.Feedback.Comments[i];
                if (comment.comments_to == 0)
                    result.push(comment);
            }
            return result.slice(0).sort(function (l, r) { return l.comments_id > r.comments_id ? -1 : 1; });

        };

        vm.GetCommentFileImage = function (data) {
            if (data != null) {
                var id = parseInt(data.return_comment_file_id);
                var isPic = vm.IsPicture(data.image_name);
                if (isNaN(id))
                    return !isPic ? vm.GetCommentDocIcon(data) : '@Url.Action("GetCommentTempFile")/?file=' + data.image_name;
                else {
                    return !isPic ? vm.GetCommentDocIcon(data) : '@Url.Action("GetFile")/?filename=' + data.image_name;
                }
            }
            return '';
        };

        vm.GetCommentFileUrl = function (data) {
            if (data != null) {

                var id = parseInt(data.return_comment_file_id);
                console.log("GET FILE : ID ", id)

                if (isNaN(id)) {                    
                    return '/Claims/GetCommentTempFile/?file=' + escape(typeof data.image_name==='undefined'?data:data.image_name);
                }
                else {
                    return '/Claims/GetFile/?filename=' + escape(data.image_name);
                    //return '/Images/Upload/feedback/' + escape(data.image_name);
                    //return '@Url.Action("GetFile")/?filename=' + escape(data.image_name);
                }
            }
            return '';
        };

        vm.GetCommentDocIcon = function (data) {
            var ext = GetExtension(data.image_name).toLowerCase();
            var image = 'document_50px.png';
            if (ext == 'doc' || ext == 'docx')
                image = 'word_50px.png';
            if (ext == 'xls' || ext == 'xlsx')
                image = 'excel_50px.png';
            if (ext == 'pdf')
                image = 'pdf_50px.png';

            return $.validator.format('/images/doctypes/{0}', image);
        };
        vm.uploadedFiles = [];

       
        vm.fileUploadExt = {
            url: '/Claims/CommentFiles',
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
                    vm.uploadInProgress = true;
                    files.forEach(function (elem) {
                        vm.uploadedFiles.push(elem);
                    });
                    $timeout(function () {
                        uploader.start();
                    }, 1);
                },
                uploadProgress: function (uploader, file) {
                    
                    vm.fileExt = file.percent;
                },
                fileUploaded: function (uploader, file, response) {
                    vm.NewComment.Files.push(new File(file.id, file.name));
                }
            }
        };
        //vm.fileUploadInt = {
        //    url: '/Claims/CommentFiles',
        //    options: {
        //        multi_selection: false,
        //        max_file_size: '32mb',
        //        resize: { width: 800, height: 600, quality: 90 },
        //        filters: [
        //            {
        //                extensions: 'jpg,png'
        //            }
        //        ]
                
        //    },
        //    callbacks: {
        //        filesAdded: function (uploader, files) {
        //            vm.uploadInProgress = true;
        //            files.forEach(function (elem) {
        //                vm.uploadedFiles.push(elem);
        //            });
        //            $timeout(function () {
        //                uploader.start();
        //            }, 1);
        //        },
        //        uploadProgress: function (uploader, file) {
                   
        //            vm.fileExt = file.percent;
        //        },
        //        fileUploaded: function (uploader, file, response) {
        //            vm.NewCommentInt.Files.push(new File(file.id, file.name));
        //        }
        //    }
        //};
        function File(id, return_image, return_id, file_category) {
            this.file_id = id;
            this.return_image = return_image;
            this.return_id = return_id;

            this.file_category = file_category;
        }

    }
})();

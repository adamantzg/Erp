(function() {
    'use strict';
    var subscribers;
    angular
        .module('app')
        .controller('controller', controller);

    controller.$inject = ['$scope', '$state', '$stateParams', '$timeout', '$compile','$http', 'factory', 'Lightbox'];

    function controller($scope, $state, $stateParams, $timeout, $compile, $http,factory,Lightbox) {
        $scope.state = $state.current.name;
        $scope.isCollapsed = true;
        $scope.factory_id = null;
        $scope.showSubscribers = false;

        if ($scope.state != 'home') {
            factory.getModel($stateParams.id, claim_type).then(function (reponse) {
                $scope.model = factory.model;
                
                _.remove($scope.model.clients, function (c) {
                    return c.customer_code.length == 0;
                });
                
                $scope.claim = factory.model.claim;
                $scope.categories = $scope.model.categories;
                $scope.returnCategories = $scope.model.returnCategories;
                $scope.canAuthorize = $scope.model.canAuthorizeFeedback;
                $scope.showAuthorizeButton = $scope.canAuthorize && $scope.claim.status1 == 0;
                $scope.canClose = $scope.model.canCloseFeedback;

                if ($scope.productTableInstance != null)
                    $timeout(function () {
                        $scope.productTableInstance.DataTable.columns.adjust().draw();
                    }, 200);

                if($scope.state == 'create')
                    generateReturnNo();

            });
            //    .finally(function () {
            //    factory.getDefaultSubscribers(claim_type).then(
            //        function (defSubscribers) {
            //            $scope.DEF = defSubscribers;  
            //            console.log("Finally", factory.model)
            //            Array.prototype.push.apply($scope.claim.subscriptions, defSubscribers)
            //        }
            //    )
            //});;
            
        }
        else
            factory.get(claim_type).then(function(response) {
                $scope.claims = response.data;
            });

        
        $scope.download = function () {
            downloadURL("QaExport");
        }
        $scope.defaultSubscribers = function () {
           $scope.readOnlySubscribers = [];

           factory.getDefaultSubscribers(claim_type).then(
               
               function (subscribers) {
                   $scope.readOnlySubscribers = subscribers;
               }
            );
        }
        $scope.defaultSubscribers();

        $scope.edit = function(id) {
            $state.go('edit', { id: id });
        }

        $scope.dateOptions = {
            formatYear: 'yyyy'
        };

        $scope.formatDate = function(d) {
            if (d == null)
                return '';
            return fromDateFormatted(d.toString());
        }

        $scope.openPopup = function(which) {
            $scope.popupOpened[which] = !$scope.popupOpened[which];
        };

        $scope.popupOpened = { readyDate: false };

                

        $scope.update = function () {
            var addedProducts = _.filter($scope.products, { selected: true });
            if ($scope.claim.products == null)
                $scope.claim.products = [];
            addedProducts.forEach(function (p) {
                if (_.find($scope.claim.products, { cprod_id: p.cprod_id }) == null)
                    $scope.claim.products.push(clone(p));
            });
            if ($scope.claim.returnsid == null || $scope.claim.returnsid <= 0)
            {   
                factory.create($scope.claim).then(function () {
                    $state.go('home');
                }, function (err) {

                });
            }                
            else
                factory.update($scope.claim).then(function () {
                    $state.go('home');
                });
        };

        
        /*$scope.getProducts = function(text) {
            return factory.searchProducts(text).then(function(data) {
                $scope.products = data;
                return data;
            });
        };*/

        $scope.client_id ='';
        $scope.factory_id = '';


        /*function replaceChar(indexStart, numbChars, text, chars) {
            return text.substr(0, indexStart) + chars + text.substr(indexStart + numbChars);
        }*/

        function generateReturnNo() {
            var arr = $scope.model.return_no_parts;
            $scope.claim.return_no = arr.join('-');
        };

        $scope.userIdChanged = function (item) {
            if ($scope.state == 'create')
            {
                $scope.model.client_id = item;

                var client = _.find($scope.model.clients, { user_id: item });
                if (client != null) {
                    $scope.model.return_no_parts[4] = client.customer_code;
                    generateReturnNo();
                    //$scope.claim.return_no = replaceChar(18, 2, $scope.claim.return_no, client.customer_code);
                }
            }
            
        }

        $scope.factoryIdChanged = function (item) {
            if ($scope.state == 'create') {
                $scope.factory_id = item;

                var factory = _.find($scope.model.factories, { user_id: item });
                if (factory != null) {
                    $scope.model.return_no_parts[0] = factory.factory_code;
                    generateReturnNo();
                    //$scope.claim.return_no = replaceChar(0, 2, $scope.claim.return_no, factory.factory_code);
                }
            }            
        }

        $scope.reasonChanged = function (item) {
            if ($scope.state == 'create') {
                $scope.claim.reason = item;

                var reason = _.find($scope.model.returnCategories, { category_code: item })
                if (reason != null) {
                    //$scope.claim.return_no = replaceChar(7, 1, $scope.claim.return_no, reason.category_code);
                    $scope.model.return_no_parts[2] = reason.category_code;
                    generateReturnNo();
                }
            }    
        }
        
        $scope.searchProducts = function () {
            console.log("SEARCH PARAMS: ", $scope.claim.client_id, $scope.claim.factory_id);
            factory.searchProductsByCriteria($scope.claim.client_id, $scope.claim.factory_id).then(function (response) {
               
                $scope.products = response.data;
            });
        };

        /*$scope.productSelected = function($item, $model, $label, $event) {
            $scope.productMissing = false;
            $scope.addProduct();
        }

        $scope.addProduct = function() {
            if ($scope.claim.products == null)
                $scope.claim.products = [];
            $scope.claim.products.push(clone($scope.product));
            //$scope.GetFCS($scope.Product.cprod_id);
            $scope.product = null;
        }*/

        $scope.removeProduct = function(index) {
            $scope.claim.products.splice(index, 1);
        };

        $scope.getUsers = function(text) {
            return factory.searchUsers(text).then(function(data) {
                $scope.users = data;
                return data;
            });
        };

        $scope.addSubscription = function(submit) {
            if ($scope.claim.subscriptions == null)
                $scope.claim.subscriptions = [];
            var userId = $scope.selectedUser.userid;
            $scope.claim.subscriptions.push({ subs_useruserid: userId , user: {userid: $scope.selectedUser.userid, userwelcome: $scope.selectedUser.userwelcome}});
            //$scope.GetFCS($scope.Product.cprod_id);
            $scope.selectedUser = null;

            if (submit == true)
                factory.saveSubscription({ subs_useruserid: userId, subs_returnid: $scope.claim.returnsid });
        }

        $scope.removeSubscription = function (index, submit) {
            var sub = angular.copy($scope.claim.subscriptions[index]);
            $scope.claim.subscriptions.splice(index, 1);
            if (submit == true)
            {
                sub.user = null;
                sub.subs_returnid = $scope.claim.returnsid;
                factory.removeSubscription(sub);
            }
                
        };

        $scope.getImageUrl = function(obj, type) {
            if (type == 1)
                //return image
                return factory.getFileUrl(obj, 'return_image');
            else if (type == 2)
                //comment image
                return factory.getFileUrl(obj, 'image_name');
            
        };

        $scope.getFileName = function(path)
        {
            return GetFileName(path);
        }

        $scope.removeImage = function(collection,index) {
            collection.splice(index, 1);
        };

        $scope.isPicture = function(image) {
            return isPicture(image);
        };

        $scope.productTableOptions = {
            scrollY: 200,
            scrollCollapse: true,
            paging: false
        };

        $scope.productTableInstance = null;
        

        $scope.productsToggleAll = { checked: false };

        $scope.toggleAll = function() {
            $scope.products.forEach(function(p) {
                p.selected = $scope.productsToggleAll.checked;
            });
        };

        /*$scope.fileUpload = {
            url: factory.getUploadUrl(),
            options: {
                multi_selection: true,
                max_file_size: '32mb',
                resize: { width: 1280, height: 1024, quality: 80 },
                filters: [
                    {
                        extensions: 'jpg,png'
                    }
                ]
            },
            callbacks: {
                filesAdded: function(uploader, files) {
                    $scope.uploadInProgress = true;

                    files.forEach(function(elem) {
                        var file = { return_image: elem.name, return_id: $scope.claim.returnsid, file_id: elem.id };
                        if ($scope.uploadedFiles == null)
                            $scope.uploadedFiles = [];
                        $scope.uploadedFiles.push(file);                        
                    });
                    $timeout(function() {
                        uploader.start();
                    }, 1);
                },
                beforeUpload: function(uploader, file) {
                    uploader.settings.multipart_params = { id: file.id };
                },
                uploadProgress: function(uploader, file) {
                    //$scope.loading = file.percent/100.0;
                    var f = _.find($scope.uploadedFiles, { file_id: file.id });
                    if (f != null)
                        f.percent = file.percent;
                },

                fileUploaded: function(uploader, file, response) {
                    var f = _.find($scope.uploadedFiles, { file_id: file.id });
                    if (f != null) {
                        f.percent = 100;
                        f.url = factory.getFileUrl(f, null);
                        if ($scope.claim.images == null)
                            $scope.claim.images = [];
                        $scope.claim.images.push(f);
                    }
                    if (_.every($scope.uploadedFiles, { percent: 100 }))
                        $scope.uploadedFiles = [];

                },
                error: function(uploader, error) {
                    $scope.loading = false;
                    alert(error.message);
                }
            }
        };*/

        $scope.fileUpload = {
            url: factory.getUploadUrl(),
            options: {
                multi_selection: true,
                max_file_size: '32mb',
                resize: { width: 1280, height: 1024, quality: 80 },
                filters: [
                    {
                        extensions: 'jpg,png,gif,xls,xlsx,doc,docx,pdf'
                    }
                ]
            },
            callbacks: {
                filesAdded: function (uploader, files) {
                    $scope.uploadInProgress = true;

                    files.forEach(function (elem) {
                        var file = { return_image: elem.name, return_id: $scope.claim.returnsid, file_id: elem.id };
                        if ($scope.uploadedFiles == null)
                            $scope.uploadedFiles = [];
                        $scope.uploadedFiles.push(file);
                    });
                    $timeout(function () {
                        uploader.start();
                    }, 1);
                },
                beforeUpload: function (uploader, file) {
                    uploader.settings.multipart_params = { id: file.id };
                },
                uploadProgress: function (uploader, file) {
                    //$scope.loading = file.percent/100.0;
                    var f = _.find($scope.uploadedFiles, { file_id: file.id });
                    if (f != null)
                        f.percent = file.percent;
                },

                fileUploaded: function (uploader, file, response) {
                    var f = _.find($scope.uploadedFiles, { file_id: file.id });
                    if (f != null) {
                        f.percent = 100;
                        f.url = factory.getFileUrl(f, null);
                        if ($scope.claim.images == null)
                            $scope.claim.images = [];
                        $scope.claim.images.push(f);
                    }
                    if (_.every($scope.uploadedFiles, { percent: 100 }))
                        $scope.uploadedFiles = [];

                },
                error: function (uploader, error) {
                    $scope.loading = false;
                    alert(error.message);
                }
            },
            commentCallbacks: {}
        };

        //$scope.commentTypes = [
        //    { id: 'exterrnal', filter: 1, newComment: { comments: '', files: [], comments_to: 1 }, newCommentRow: false, uploadedFiles: [] },
        //    { id: 'internal', filter: 0, newComment: { comments: '', files: [], comments_to: 0 }, newCommentRow: false, uploadedFiles: []}
        //]

        $scope.commentTypes = [
            { id: 'comments', filter: 1, newComment: { comments: '', files: [], comments_to: 1 }, newCommentRow: false, uploadedFiles: [] }
        ]

        $scope.commentTypes.forEach(function (t) {
            $scope.fileUpload.commentCallbacks[t.id] = 
                {
                    filesAdded: function (uploader, files) {
                        $scope.uploadInProgress = true;

                        files.forEach(function (elem) {
                            var file = { image_name: elem.name, file_id: elem.id };
                            if (t.uploadedFiles == null)
                                t.uploadedFiles = [];
                            t.uploadedFiles.push(file);
                        });
                        $timeout(function () {
                            uploader.start();
                        }, 1);
                    },
                    beforeUpload: function (uploader, file) {
                        uploader.settings.multipart_params = { id: file.id };
                    },
                    uploadProgress: function (uploader, file) {
                        //$scope.loading = file.percent/100.0;
                        var f = _.find(t.uploadedFiles, { file_id: file.id });
                        if (f != null)
                            f.percent = file.percent;
                    },

                    fileUploaded: function (uploader, file, response) {
                        var f = _.find(t.uploadedFiles, { file_id: file.id });
                        if (f != null) {
                            f.percent = 100;
                            f.url = factory.getFileUrl(f, null);
                            t.newComment.files.push(f);
                        }
                        if (_.every(t.uploadedFiles, { percent: 100 }))
                            t.uploadedFiles = [];

                    },
                    error: function (uploader, error) {
                        $scope.loading = false;
                        alert(error.message);
                    }
                }
            
        });

        $scope.createComment = function (t) {
            factory.createComment($scope.claim.returnsid, t.newComment).then(function (response) {
                $scope.claim.comments.push(response.data);
                t.newComment.comments = '';
                t.newComment.files = [];
            });
        };

        $scope.openLightboxModal = function (images, index) {
            var image = images[index];
            var filteredImages = _.filter(images, function (im) {
                return isPicture(im.return_image) || isPicture(im.image_name);
            });
            var adjustedIndex = _.findIndex(filteredImages, image);
            Lightbox.openModal(filteredImages, adjustedIndex);
            //Lightbox.openModal($scope.images, 0);
        };

        $scope.openClose = function () {
            var value = $scope.claim.openclosed == 0 ? 1 : 0;
            factory.openClose($scope.claim.returnsid, value).then(function () {
                $scope.claim.openclosed = value;
            });
        };

        $scope.authorize = function () {
            factory.authorize($scope.claim.returnsid).then(function (response) {
                $scope.showAuthorizeButton = response.data;
            });
        };

        $scope.getOpenCloseCaption = function () {
            if($scope.claim != null)
                return $scope.claim.openclosed == 0 || $scope.claim.openclosed == null ? 'Close' : 'Open';
            return '';
        };

        $scope.cancelComment = function (t) {
            var c = t.newComment;
            c.comments = '';
            c.files = [];
            $scope.newCommentRow = !$scope.newCommentRow;
        };

        var pastedCounter = 0;

        $scope.paste = function (event) {
            var clipData = event.originalEvent.clipboardData;
            angular.forEach(clipData.items, function (item, key) {
                if (clipData.items[key]['type'].match(/image.*/)) {
                    // if it is a image
                    var img = clipData.items[key].getAsFile();
                    
                    var fd = new FormData();
                    fd.append('file', img);
                    var file_id = 'pastedimage_' + pastedCounter;
                    pastedCounter++;
                    // CHANGE /post/paste TO YOUR OWN FILE RECEIVER
                    $http.post(factory.getUploadUrl() + '?id=' + file_id, fd, {
                        transformRequest: angular.identity,
                        headers: {
                            'Content-Type': undefined
                        }
                    }).success(function (url) {
                        
                        if ($scope.claim.returnsid > 0)
                        {
                            var f = { image_name: file_id + '.png', file_id: file_id };
                            f.url = factory.getFileUrl(f, null);
                            $scope.commentTypes[0].newComment.files.push(f);
                        }                            
                        else
                        {
                            var f = { return_image: file_id + '.png', file_id: file_id };
                            f.url = factory.getFileUrl(f, null);
                            if ($scope.claim.images == null)
                                $scope.claim.images = [];
                            $scope.claim.images.push(f);
                        }
                            
                        // the url returns
                    }).error(function (data) {
                        
                    });
                };
            });
        };
    }

})();
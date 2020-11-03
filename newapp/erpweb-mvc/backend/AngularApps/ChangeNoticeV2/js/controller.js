(function () {
    'use strict';

    angular
        .module('app')
        .controller('controller', controller);

    controller.$inject = ['$scope', '$state', '$stateParams', '$timeout', '$compile', 'factory'];

    function controller($scope, $state, $stateParams, $timeout, $compile, factory) {
        $scope.state = $state.current.name;

        var noticeOrders = null;

        if ($scope.state == "create" || $scope.state == "edit")
        {
            factory.getModel($scope.state, $stateParams.id).then(function (reponse) {
                $scope.model = factory.model;
                $scope.categories = $scope.model.categories;
                $scope.clients = $scope.model.clients;
                $scope.factories = $scope.model.factories;
                $scope.statuses = $scope.model.statuses;
                $scope.model.statuses.push({ id: null, value: 'N/A' });
                $scope.changeNoticeCategories = $scope.model.changeNoticeCategories;
                $scope.changeNoticeReasons = $scope.model.changeNoticeReasons;
                $scope.notice = $scope.model.notice;
                $scope.document = $scope.model.notice.document;

                $scope.tmpProducts = [];

                if ($scope.model.product != null)
                    $scope.product = $scope.model.product;

                if (factory.model.product != null)
                    $scope.product = factory.model.product;

                $scope.baseDocUrl = '/images/upload/changenotices/';

                $scope.getDocUrl = function (filename) {
                    return $scope.baseDocUrl + filename;
                }

                $scope.datecreated = $scope.model.datecreated;

                $scope.uploadedImages = [];

                if ($scope.model.notice != null && $scope.model.notice.expectedReadyDate) {
                    $scope.model.notice.expectedReadyDate = new Date(factory.model.notice.expectedReadyDate);
                }



                if ($scope.model.notice == null)
                    $scope.model.notice = {};
                else {
                    if ($scope.model.notice.allocations != null)
                        noticeOrders = _.union(_.flatMap($scope.model.notice.allocations, function (a) {
                            return _.flatMap(a.orders, function (o) { return { orderid: o.orderid, userid1: o.userid1, cprod_id: a.cprod_id }; })
                        }));
                }

                if ($scope.model.notice.images == null)
                    $scope.model.notice.images = [];

                if ($scope.product != null) {
                    $scope.searchParams = { client_id: $scope.product.brand_userid, category1_id: $scope.product.mastProduct.category1, factory_id: factory.model.factory_id };
                    $scope.searchProducts();

                    $scope.addProducts();
                }
                else {
                    $scope.searchParams = { client_id: null, category1_id: null, factory_id: factory.model.factory_id };
                }

                $scope.model.factories.splice(0, 0, { user_id: null, factory_code: '(Not set)' });
                $scope.model.clients.splice(0, 0, { user_id: null, customer_code: '(Any)' });
                $scope.model.categories.splice(0, 0, { category1_id: null, cat1_name: '(Any)' });
                

                $scope.uploadedImages = [];

                $scope.rows = [0, 1];
                $scope.columns = [0, 1, 2];
                $scope.imageCount = 6;

                //$scope.PrepareImages($scope.AllImages);

                $scope.model.notice.images = $scope.PrepareImages($scope.model.notice.images);
                $scope.SetFileUploadCallbacks($scope.model.notice.images);

                if ($scope.model.orders != null) {
                    addOrdersToAllocations($scope.model.orders);

                } else {
                    $scope.loadOrders();
                }

                $scope.GetImageIndex = function (row, col) {
                    return row * $scope.rows.length + col;
                }

            });
        }
        else if ($scope.state == "home")
        {
            $scope.dtListOptions = {
                pageLength: 50,
                order: [2, 'desc'],
            };
            
            $.fn.dataTable.moment('DD/MM/YYYY');

            if (factory.listModel == null) {
                factory.getListModel().then(function () {
                    $scope.model = factory.listModel;
                    $scope.model.statuses.splice(0, 0, { id: null, value: '<Any>' });
                    $scope.model.factories.splice(0, 0, { user_id: null, factory_code: '<Any>' });
                    $scope.model.clients.splice(0, 0, { user_id: null, customer_code: '<Any>' });

                    $scope.resetFilter();
                    $scope.showList();
                });
            }
            else
                $scope.model = factory.listModel;

            
        }

        $scope.showList = function () {
            factory.getNotices($scope.model.factory_id, $scope.model.client_id, $scope.model.status, $scope.model.product_code).then(function(response) {
                $scope.model.notices = response.data;
            });
        };

        $scope.resetFilter = function () {
            $scope.model.factory_id = null;
            $scope.model.client_id = null;
            $scope.model.status = 0;
            $scope.model.product_code = '';
        };
        

        function addOrdersToAllocations(orders)
        {
            if ($scope.model.notice.allocations != null) {
                $scope.model.notice.allocations.forEach(function (a) {

                    if (orders != null) {
                        if (a.cprod_id in orders) {
                            a.orders = orders[a.cprod_id];
                            a.orders[0].orderList.splice(0, 0, { orderid: null, custpo: 'SELECT' });
                        }

                        //o.orderList.splice(0, 0, { orderid: null, custpo: 'SELECT' });
                    }
                });
            }
        }

        $scope.edit = function (id) {
            $state.go('edit', { id: id });
        }

        $scope.dateOptions = {
            formatYear: 'yyyy',
            startingDay: 1
        };

        $scope.FormatDate = function (d) {
            if (d == null)
                return '';
            return fromDateFormatted(d.toString());
        }

        $scope.openPopup = function (which) {
            $scope.popupOpened[which] = !$scope.popupOpened[which];
        };

        $scope.popupOpened = {readyDate: false};

        $scope.filterProducts = function (p) {
            return p.allocated == null || !p.allocated;
        };

        $scope.showAddProducts = function () {
            return _.findIndex($scope.products, { selected: true }) >= 0;
        };

        $scope.searchProducts = function () {
            factory.searchProducts($scope.searchParams.client_id, $scope.searchParams.factory_id, $scope.searchParams.category1_id).then(function () {
                $scope.products = factory.products;

                if ($scope.product != null) {
                    var prod = _.find($scope.products, { cprod_id: $scope.product.cprod_id });

                    if (prod != null)
                        prod.selected = true;

                    $scope.addProducts();
                }

            }); 
        };

        $scope.addProducts = function () {
            _.filter($scope.products, { selected: true }).forEach(function (p) {
                if (_.find($scope.model.notice.allocations, { cprod_id: p.cprod_id }) == null)
                {
                    var a = { cprod_id: p.cprod_id, product: p, orders: [] };
                    if ($scope.model.notice.allocations == null)
                        $scope.model.notice.allocations = [];
                    $scope.model.notice.allocations.push(a);
                    p.allocated = true;
                    loadClientsOrders(a);

                    //push to temp 
                    $scope.tmpProducts.push(p);
                    _.remove($scope.products, { cprod_id: p.cprod_id });

                }                
            });
        };

        $scope.loadOrders = function () {
            var ids = _.map($scope.model.notice.allocations, 'cprod_id');
            if (ids.length > 0)
            {
                factory.loadClientsOrdersForProducts(ids, $scope.model.notice.expectedReadyDate, noticeOrders).then(function (response) {
                    addOrdersToAllocations(response.data);
                });
            }   
        };

        
        function loadClientsOrders(a)
        {
            factory.loadClientsOrdersForProduct(a.product.cprod_id, $scope.model.notice.expectedReadyDate).then(function (response) {
                a.orders = response.data;
                a.orders.forEach(function (o) {
                    if (o.orderList.length > 0) {
                        o.orderid = o.orderList[0].orderid;
                        o.orderList.splice(0, 0, { orderid: null, custpo: 'SELECT' });
                    }
                });
            });
        }

        $scope.removeAllocation = function (a) {
            var product = _.find($scope.products, { cprod_id: a.cprod_id });

            if (product != null)
            {
                product.allocated = false;
                product.selected = false;
            }

            var tmpProduct = _.find($scope.tmpProducts, { cprod_id: a.cprod_id });

            if (tmpProduct != null) {
                tmpProduct.selected = false;
                tmpProduct.allocated = false;
                $scope.products.push(tmpProduct);
            }
            _.remove($scope.tmpProducts, { cprod_id: a.cprod_id })
            _.remove($scope.model.notice.allocations, { cprod_id: a.cprod_id });
        };

        $scope.dtOptions = {
            
        };

        //dom:'<"row"f><"pull-right"l>t<p>'

        $scope.update = function () {

            _.remove($scope.notice.images, function (im) {
                return im.notice_image.length == 0;
                });
            
            if ($scope.model.notice.id == null || $scope.model.notice.id <= 0) {
                factory.create($scope.model.notice);
            }
            else
                factory.update($scope.model.notice);
        };

        $scope.deleteChangeNoticeDocument = function () {
            factory.deleteChangeNoticeDocument($scope.model.notice);
        };

        $scope.uploader = null;

        $scope.startUpload = function () {

            if ($scope.uploader != null)
                $scope.uploader.start();
        };

        $scope.fileUploadDocument = {
            url: factory.getUploadUrl(),
            options: {
                multi_selection: false,
                max_file_size: '32mb',
                resize: { width: 1280, height: 1024, quality: 80 },
                filters: [
                    {
                        extensions: 'jpg,jpeg,png,pdf'
                    }
                ]
            },
            callbacks: {
                filesAdded: function (uploader, files) {
                    $scope.uploader = uploader;

                    files.forEach(function (elem) {
                        $scope.model.notice.document.formatted_change_doc_id = elem.id;
                        $scope.model.notice.document.formatted_change_doc = elem.name;                    
                    });
                    $timeout(function () {
                        uploader.start();
                    }, 1);
                },
                uploadProgress: function (uploader, file) {
                    var up = $scope.model.notice.document;
                    if (up != null)
                        up.progress = file.percent;
                },
                beforeUpload: function (uploader, file) {
                    uploader.settings.multipart_params = { id: file.id };
                },
                fileUploaded: function (uploader, file, response) {
                    var up = $scope.model.notice.document;
                    if (up != null)
                        up.progress = 100;
                },
                error: function (uploader, error) {

                    alert(error.message);
                }
            }
        };

        $scope.fileUpload = {
            url: factory.getUploadUrl(),
            options: {
                multi_selection: false,
                max_file_size: '32mb',
                resize: { width: 1280, height: 1024, quality: 80 },
                filters: [
                    {
                        extensions: 'jpg,jpeg,png,pdf'
                    }
                ]
            },
            callbacks:[]
        };
        
        $scope.fileUpload.callbacks = {};

        $scope.SetFileUploadCallbacks = function (images) {
            images.forEach(function (im) {
                im.new = false;
                var index = im.order;
                $scope.fileUpload.callbacks[index] =
                    {
                        filesAdded: function (uploader, files) {
                            $scope.uploadInProgress = true;

                            files.forEach(function (elem) {

                                //first row before, second row after images
                                var type = index < 3 ? 0 : 1;

                                //initial progress value
                                var file = { id: elem.id, notice_image: elem.name, percent: 0,size: elem.size, type: elem.type, order: index, notice_id: $scope.model.notice.id, type_id: type };
                                $scope.uploadedImages.push(file);
                            });
                            $timeout(function () {
                                uploader.start();
                            }, 1);
                        },
                        uploadProgress: function (uploader, file) {
                            var f = _.find($scope.uploadedImages, { id: file.id });
                            if (f != null)
                                f.percent = file.percent;
                        },
                        beforeUpload: function (uploader, file) {
                            uploader.settings.multipart_params = { id: file.id };
                        },
                        fileUploaded: function (uploader, file, response) {
                            var f = _.find($scope.uploadedImages, { id: file.id });

                            

                            if (f != null) {
                                f.percent = 100;
                            }

                            im.id = 0;
                            im.comments = f.id
                            im.new = true;
                            im.order = f.order;
                            im.type_id = f.type_id;
                            im.notice_id = f.notice_id;
                            im.notice_image = f.notice_image
                            

                            $scope.model.notice.images[im.order] = im;
                            $scope.uploadedImages = [];
                        },
                        error: function (uploader, error) {
                            $scope.loading = false;
                            alert(error.message);
                        }
                    };
            });
        };
        
        $scope.fileUploadMultiBefore = {
            url: factory.getUploadUrl(),
            options: {
                multi_selection: true,
                max_file_size: '32mb',
                resize: { width: 1280, height: 1024, quality: 80 },
                filters: [
                    {
                        extensions: 'jpg,jpeg,png'
                    }
                ]
            },
            callbacks: {
                filesAdded: function (uploader, files) {
                    $scope.uploadInProgress = true;
                    $scope.uploadedImages = [];

                    var order = 0;
                    files.forEach(function (elem) {

                        //initial progress value
                        var type = order < 3 ? 0 : 1;

                        //initial progress value
                        var file = { id: elem.id, notice_image: elem.name, percent: 0, size: elem.size, type: elem.type, order: order, notice_id: $scope.model.notice.id, type_id: type };
                        $scope.uploadedImages.push(file);
                        order++;

                    });
                    $timeout(function () {
                        uploader.start();
                    }, 1);
                },
                uploadProgress: function (uploader, file) {
                    //$scope.loading = file.percent/100.0;
                    var f = _.find($scope.uploadedImages, { id: file.id });
                    if (f != null)
                        f.percent = file.percent;
                },
                beforeUpload: function (uploader, file) {
                    uploader.settings.multipart_params = { id: file.id };
                },
                fileUploaded: function (uploader, file, response) {
                    var f = _.find($scope.uploadedImages, { id: file.id });

                    if (f != null) {
                        f.percent = 100;
                    }

                    var im = $scope.model.notice.images[f.order];

                    if (im != null && f.order < $scope.imageCount) {
                        im.id = 0;
                        im.comments = f.id
                        im.new = true;
                        im.order = f.order;
                        im.type_id = f.type_id;
                        im.notice_id = f.notice_id;
                        im.notice_image = f.notice_image

                        $scope.model.notice.images[im.order] = im;
                        
                    }
                },
                error: function (uploader, error) {
                    $scope.loading = false;
                    alert(error.message);
                }
            }
        };

        $scope.fileUploadMultiAfter = {
            url: factory.getUploadUrl(),
            options: {
                multi_selection: true,
                max_file_size: '32mb',
                resize: { width: 1280, height: 1024, quality: 80 },
                filters: [
                    {
                        extensions: 'jpg,jpeg,png,pdf'
                    }
                ]
            },
            callbacks: {
                filesAdded: function (uploader, files) {
                    $scope.uploadInProgress = true;
                    $scope.uploadedImages = [];

                    var order = 3;
                    files.forEach(function (elem) {

                        //lower 3 pictures are after pictures (type 1)
                        var type =  1;

                        //initial progress value
                        var file = { id: elem.id, notice_image: elem.name, percent: 0, size: elem.size, type: elem.type, order: order, notice_id: $scope.model.notice.id, type_id: type };
                        $scope.uploadedImages.push(file);
                        order++;

                    });
                    $timeout(function () {
                        uploader.start();
                    }, 1);
                },
                uploadProgress: function (uploader, file) {
                    //$scope.loading = file.percent/100.0;
                    var f = _.find($scope.uploadedImages, { id: file.id });
                    if (f != null)
                        f.percent = file.percent;
                },
                beforeUpload: function (uploader, file) {
                    uploader.settings.multipart_params = { id: file.id };
                },
                fileUploaded: function (uploader, file, response) {
                    var f = _.find($scope.uploadedImages, { id: file.id });

                    if (f != null) {
                        f.percent = 100;
                    }

                    var im = $scope.model.notice.images[f.order];

                    if (im != null && f.order < $scope.imageCount) {
                        im.id = 0;
                        im.comments = f.id
                        im.new = true;
                        im.order = f.order;
                        im.type_id = f.type_id;
                        im.notice_id = f.notice_id;
                        im.notice_image = f.notice_image

                        $scope.model.notice.images[im.order] = im;

                    }
                },
                error: function (uploader, error) {
                    $scope.loading = false;
                    alert(error.message);
                }
            }
        };

        $scope.getImageUrl = function (row, col) {

            if ($scope.model != null && $scope.model.notice != null && $scope.model.notice.images != null) {
                var image = _.find($scope.model.notice.images, { order: row * $scope.columns.length + col });

                if (image != null && image.notice_image.length > 0) {

                    if (image.new) {
                        return factory.getTempFileUrl(image.notice_image, image.comments);
                    }
                    else
                        return CombineUrls($scope.model.imageRootFolder, image.notice_image);
                }
                return '';
            }
            else
                return '';
        };

        $scope.PrepareImages = function (images) {

            var tmpImages = [];

            for (var i = 0; i < $scope.imageCount; i++) {
                var img = _.find(images, { order: i })

                if (img != null) {
                    tmpImages.push(img);
                }else{
                        var type = i < 3 ? 0 : 1;
                        tmpImages.push({ id: -1, notice_image: '', comments: '', percent: 0, size: 0, type: 0, order: i, notice_id: $scope.model.notice.id, type_id: type });
                }
                
            }
            
            return tmpImages;

        }

        $scope.getOrderDropDownClass = function (o) {
            if (_.some(o.orderList, function (ord) {
                return moment(ord.po_req_etd).isBefore($scope.notice.expectedReadyDate);
            }))
            {
                return 'orders_before_ready_date';
            }
            return '';
        };

        $scope.DeleteImage = function (row, col) {
            var image = _.find($scope.model.notice.images, { order: row * $scope.columns.length + col });
            if (image != null) {
                image.comments = '';
                image.notice_image = '';
            }
        }

        $scope.MoveDown = function (row, col) {
            var order =  row * $scope.columns.length + col;

            var im = _.find($scope.model.notice.images, { order: order });

            if (im != null && order < 6) {

                var newIm = _.find($scope.model.notice.images, { order: order + 1 });

                if (newIm != null) {
                    var oldData = JSON.parse(JSON.stringify(newIm));

                    newIm.notice_image = im.notice_image;
                    newIm.notice_id = im.notice_id;
                    newIm.new = im.new;
                    newIm.comments = im.comments;
                    newIm.id = im.id;
                    newIm.type_id = im.type_id;

                    im.notice_image = oldData.notice_image;
                    im.new = oldData.new;
                    im.comments = oldData.comments;
                    im.id = oldData.id;
                    im.type_id = oldData.type_id;
                }
            }
        }

        $scope.MoveUp = function (row, col) {
            var order = row * $scope.columns.length + col;
            var im = _.find($scope.model.notice.images, { order: order });
            if (im != null && order > 0) {

                var newIm = _.find($scope.model.notice.images, { order: order - 1 });

                if (newIm != null) {
                    var oldData = JSON.parse(JSON.stringify(newIm));

                    newIm.notice_image = im.notice_image;
                    newIm.notice_id = im.notice_id;
                    newIm.new = im.new;
                    newIm.comments = im.comments;
                    newIm.id = im.id;
                    newIm.type_id = im.type_id;

                    im.notice_image = oldData.notice_image;
                    im.notice_id = oldData.notice_id;
                    im.new = oldData.new;
                    im.comments = oldData.comments;
                    im.id = oldData.id;
                    im.type_id = oldData.type_id;
                }
            }
        }

        $scope.GetBootstrapDocIcon = function (row, col, frame) {
            if ($scope.model != null && $scope.model.notice != null && $scope.model.notice.images != null) {
                var image = _.find($scope.model.notice.images, { order: row * $scope.columns.length + col });

                if (image != null && image.notice_image.length > 0) {

                    var ext = GetExtension(image.notice_image).toLowerCase();

                    var icon = frame ? "sm-st-icon st-gray" : "fa fa-picture-o";

                    if (ext == 'pdf')
                        icon = frame ? "sm-st-icon st-red" : "fa fa-file-pdf-o";
                    if (ext == 'doc' || ext == 'docx')
                        icon = frame ? "sm-st-icon st-word-blue" : "fa fa-file-word-o";
                    if (ext == 'xls' || ext == 'xlsx')
                        icon = frame ? "sm-st-icon st-excel-green" : "fa fa-file-excel-o";

                    return icon;
                }
                else
                    return '';
            }
        }

        $scope.IsPicture = function (row, col) {
            if ($scope.model != null && $scope.model.notice != null && $scope.model.notice.images != null) {
                var image = _.find($scope.model.notice.images, { order: row * $scope.columns.length + col });

                if (image != null && image.notice_image.length > 0) {
                    var ext = GetExtension(image.notice_image).toLowerCase();

                    return ext == "jpg" || ext == "jpeg" || ext == "gif" || ext == "png" || ext == "bmp";
                }
            }
            else {
                return false;
            }
        };

        $scope.GetFileImage = function (data) {
            if (data != null) {
                var id = parseInt(data.image_unique);

                var isPic = $scope.IsPicture(data.return_image);
                if (isNaN(id))
                    return !isPic ? $scope.GetDocIcon(data) : '@Url.Action("GetTempFile")/?file=' + data.return_image;
                else {
                    return !isPic ? $scope.GetDocIcon(data) : '@Url.Action("GetFile")/?filename=' + data.return_image;
                }
            }
            return '';
        };

        $scope.backToList = function () {
            $state.go('home');
        };

    }

    function File(id, name, percent, size, type, order) {
        this.id = id;
        this.name = name;
        this.percent = percent;
        this.size = size;
        this.type = type;
        this.order = order;
    }

})();
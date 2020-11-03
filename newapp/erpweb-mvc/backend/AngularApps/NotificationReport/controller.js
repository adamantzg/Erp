function getMastProductField(l,field, defaultField) {
    if (l.orderLine != null && l.orderLine.custProduct != null && l.orderLine.custProduct.mastProduct != null)
        return l.orderLine.custProduct.mastProduct[field];
    return l[defaultField];
}

function getMastProductFactory(l) {
    if (l.orderLine != null && l.orderLine.custProduct != null && l.orderLine.custProduct.mastProduct != null && l.orderLine.custProduct.mastProduct.factory != null)
        return l.orderLine.custProduct.mastProduct.factory.factory_code;
    return '';
}

function getCustProductField(l, field, defaultField) {
    if (l.orderLine != null && l.orderLine.custProduct != null)
        return l.orderLine.custProduct[field];
    return l[defaultField];
}

function getQty(l) {
    if (l.orderLine != null)
        return l.orderLine.orderqty;
    return l.qty;
}

function getInspectioV2LineQty(l) {
    if (l.orderLine != null)
        return l.orderLine.orderqty;
    if (l.inspectionV2Line != null) {
        if (l.inspectionV2Line.qty)
            return l.inspectionV2Line.qty;
        if (l.inspectionV2Line.orderLine)
            return l.inspectionV2Line.orderLine.orderqty;
    }
        
    return l.qty;
}

function getContainers(l,insp) {
    var result = [];
    insp.lines.forEach(function (elem) {
        if (elem.loadings != null && elem.orderlines_id == l.orderlines_id) {
            elem.loadings.forEach(function (lo) {
                if (lo.container != null &&
                    lo.container.container_no != null &&
                    lo.container.container_no.length > 0)
                    result.push(lo.container.container_no);
            });
        }

    });
    return _.uniq(result).join(',');
}


(function () {
    'use strict';

    angular
        .module('app')
        .controller('controller', controller);

    controller.$inject = ['$scope','$uibModal','$location','factory']; 

    function controller($scope,$modal,$location, factory) {

        $scope.lineType = 1;
        $scope.showLineSelectionLink = true;
        $scope.selectAll = false;
        

        var queryString = loadQueryString();
        if ('id' in queryString) {
            factory.getEditModel(loadQueryString()['id'])
            .then(function () {
                $scope.model = factory.model;
                $scope.inspection = $scope.model.inspection;
                $scope.inspection.lines.forEach(function (l) {
                    l.selected = false;
                });
                $scope.showLineSelection = $scope.model.nrHeader.lines == null || $scope.model.nrHeader.lines.length == 0;
            });
        } else {
            factory.getCreateModel(loadQueryString()['insp_id'])
            .then(function () {
                $scope.model = factory.model;
                $scope.inspection = $scope.model.inspection;
                $scope.inspection.lines.forEach(function (l) {
                    l.selected = false;
                });
                $scope.showLineSelection = $scope.model.nrHeader.lines == null || $scope.model.nrHeader.lines.length == 0;
            });
        }

        
        $scope.toggleLineSelection = function() {
            $scope.showLineSelection = !$scope.showLineSelection;
        };

        $scope.getMastProductField = function(l, field, defaultField) {
            return getMastProductField(l, field, defaultField);
        };

        $scope.getMastProductFactory = function(l) {
            return getMastProductFactory(l);
        };

        $scope.getCustProductField = function(l, field, defaultField) {
            return getCustProductField(l, field, defaultField);
        };

        $scope.getQty = function(l) {
            return getQty(l);
        };

        $scope.getContainers = function(l) {
            return getContainers(l,$scope.model.inspection);
        };

        $scope.getImageCount = function(l) {
            if (l.images != null)
                return l.images.length;
            return 0;
        };

        $scope.filterLinesQTY = function (l) {
            if (getInspectioV2LineQty(l) <= 0)
                return false;

            return true;
        }

        $scope.filterLines = function (l) {

            if (getQty(l) <= 0)
                return false;

            if ($scope.inspection.type != l.inspection.type)
                return false;

            if ($scope.model.nrHeader.lines != null) {
                if (_.find($scope.model.nrHeader.lines,
                        function (elem) {
                            return elem.inspection_lines_v2_id == l.id || elem.inspection_lines_tested_id == l.id;
                }) !=
                    null)
                    return false;
            }
            

            if ($scope.lineType == 1 && (l.orderLine != null && l.orderLine.custProduct != null && l.orderLine.custProduct.mastProduct != null && l.orderLine.custProduct.mastProduct.category1 != 13))
                return false;
            return true;

        };

        $scope.addLines = function() {
            $scope.inspection.lines.forEach(function(l) {
                if (l.selected)
                {
                    $scope.model.nrHeader.lines.push({ inspection_lines_v2_id: l.id, inspectionV2Line: l });
                    l.selected = false;
                    $scope.selectAll = false;
                }
            });
        };

        $scope.removeLine = function (l) {
            _.remove($scope.model.nrHeader.lines, { inspection_lines_v2_id: l.inspection_lines_v2_id });
        };

        $scope.update = function () {
            factory.update($scope.model.nrHeader, $scope.model.isFromOldRecord)
                .then(function () {
                    
                    //if new record, update ids
                    if ($scope.model.nrHeader.id <= 0)
                        $scope.model.nrHeader.id = factory.nrHeader.id;
                    $scope.model.nrHeader.lines.forEach(function (l) {
                        if (l.id <= 0) {
                            var line = _.find(factory.nrHeader.lines, { inspection_lines_v2_id: l.inspection_lines_v2_id });
                            if (line != null)
                                l.id = line.id;
                        }

                    });
                    $modal.open({
                        animation: false,
                        backdrop: 'static',
                        size: 'sm',
                        templateUrl: 'standardModalContent.html',
                        controller: 'standardDialogController',
                        resolve: {
                            message: function () {
                                return 'The notification report has been successfully updated.';
                            }
                        }

                    });
                });
        };

        $scope.editReport = function () {
            location.href = '/InspectionV2/EditNrReport/?id=' + $scope.model.nrHeader.id;
        };

        $scope.generatePdf = function() {
            location.href = '/InspectionV2/NrPdfReport/?id=' + $scope.model.nrHeader.id;
        };

        $scope.toggleAll = function() {
            $scope.inspection.lines.filter(l => $scope.filterLines(l)).forEach(l => l.selected = $scope.selectAll);
        }
        
    }
})();


(function () {
    'use strict';

    angular
        .module('app')
        .controller('editReportController', controller);

    controller.$inject = ['$scope', '$uibModal','$timeout', 'factory'];

    function controller($scope, $modal,$timeout, factory) {
        factory.getEditReportModel(loadQueryString()['id'])
            .then(function () {
                $scope.model = factory.model;
                $scope.inspection = $scope.model.inspection;
                $scope.imageTdStyle =
                {
                    'height': '240px',
                    'text-align': 'center',
                    /*'width': (100 / $scope.model.imageTypes.length).toFixed(0).toString() + '%',*/
                    padding: '2px'
                };
                $scope.initializeUploaders();

            });

        $scope.getPoNumbers = function () {
            if($scope.model != null)
                return _.map($scope.model.orders, 'custpo').join(', ');
            return '';
        };

        $scope.getControllers = function () {
            if ($scope.inspection != null) {
                return _.map($scope.inspection.controllers, function (c) {
                    return c.controller.user_initials;
                }).join(',');
            }
            return '';
        };

        $scope.getETD = function() {
            var o = $scope.getMainOrder();
            
            if (o != null && o.po_req_etd != null)
                return fromDateFormatted(o.po_req_etd);
            return '';
        };

        $scope.getETA = function() {
            var o = $scope.getMainOrder();
            if (o != null && o.req_eta != null)
                return fromDateFormatted(o.req_eta);
            return '';
        };

        $scope.getMainOrder = function () {
            if ($scope.model != null) {
                var o = _.find($scope.model.orders, function (ord) {
                    return ord.combined_order == null;
                });
                if (o == null && $scope.model.orders.length > 0)
                    o = $scope.model.orders[0];
                return o;
            }
            return null;

        };

        $scope.getMastProductField = function (l, field, defaultField) {
            return getMastProductField(l, field, defaultField);
        };

        $scope.getMastProductFactory = function (l) {
            return getMastProductFactory(l);
        };

        $scope.getCustProductField = function (l, field, defaultField) {
            return getCustProductField(l, field, defaultField);
        };

        $scope.getQty = function (l) {
            return getQty(l);
        };

        $scope.getInspectioV2LineQty = function (l) {
            return getInspectioV2LineQty(l);
        }

        $scope.getContainers = function (l) {
            return getContainers(l,$scope.model.inspection);
        };

        $scope.getImageUrl = function (carton, type_id) {
            var image = _.find($scope.model.nrHeader.images, { carton_no: carton, image_type: type_id });
            if (image != null) {
                if (image.new) {
                    return factory.getTempFileUrl(image.image_name, image.file_id);
                }
                else
                    return CombineUrls($scope.model.imagesRootUrl, image.image_name);
            }
            return '';
        };

        $scope.deleteImage = function(c, type_id) {
            _.remove($scope.model.nrHeader.images, {carton_no: c, image_type: type_id });
        }

        $scope.getImageUrlNt = function (image) {            
            if (image.new) {
                return factory.getTempFileUrl(image.image_name, image.file_id);
            }
            else
                return CombineUrls($scope.model.imagesRootUrl, image.image_name);            
            return '';
        };

        $scope.deleteImageNt = function (image) {
            if(image.id != null)
                _.remove($scope.model.nrHeader.images, { id: image.id });
            else
                _.remove($scope.model.nrHeader.images, { file_id: image.file_id });
        }

        $scope.moveDown = function (line, type_id) {
            var image = _.find(line.images, { image_type: type_id });
            if (image != null) {
                var nextImage = _.find(line.images, { image_type: type_id + 1 });
                var tempData = null;
                if (nextImage != null) {
                    tempData = nextImage.image_name;
                    nextImage.image_name = image.image_name;
                    image.image_name = tempData;
                    tempData = nextImage.file_id;
                    nextImage.file_id = image.file_id;
                    image.file_id = tempData;
                } else {
                    image.image_type = type_id + 1;
                }
                    
            }
        };

        $scope.moveUp = function (line, type_id) {
            var image = _.find(line.images, { image_type: type_id });
            if (image != null) {
                var prevImage = _.find(line.images, { image_type: type_id - 1 });
                var tempData = null;
                if (prevImage != null) {
                    tempData = prevImage.image_name;
                    prevImage.image_name = image.image_name;
                    image.image_name = tempData;
                    tempData = prevImage.file_id;
                    prevImage.file_id = image.file_id;
                    image.file_id = tempData;
                } else {
                    image.image_type = type_id - 1;
                }

            }
        };

        $scope.update = function (submit) {
            factory.updateReport($scope.model.nrHeader,submit)
                .then(function () {

                    if (submit)
                        $scope.model.nrHeader.status = 1;

                    //if new record, update ids
                    
                    $scope.model.nrHeader.images.forEach(function (im) {                    
                        if (im.id == null || im.id <= 0) {                            
                            var image = _.find(factory.nrHeader.images, { carton_no: im.carton_no, image_type: im.type_id });
                            if (image != null)
                                im.id = image.id;                                                                
                        }
                    });
                    $modal.open({
                        animation: false,
                        backdrop: 'static',
                        size: 'sm',
                        templateUrl: 'standardModalContent.html',
                        controller: 'standardDialogController',
                        resolve: {
                            message: function () {
                                return 'The report has been successfully ' + (submit ? 'submitted.' : 'updated.');
                            }
                        }

                    });
                });
        };

        $scope.initializeUploaders = function() {
            $scope.fileUpload = {
                url: factory.getUploadUrl(),
                options: {
                    multi_selection: false,
                    max_file_size: '32mb',
                    resize: { width: 1280, height: 1024, quality: 80 },
                    filters: [
                        {
                            extensions: 'jpg,png'
                        }
                    ]
                },
                callbacks: {}
            };

            /*$scope.fileUploadMulti = {
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
                callbacks: {}
            };*/

            if ($scope.model.nrHeader.nr_type_id == $scope.model.headerTypes.ns)
            {
                if ($scope.model != null) {
                    $scope.cartonRange().forEach(function (c) {
                        addUploaders(c);
                    });
                }
            }
            else
            {
                $scope.fileUpload.options.multi_selection = true;
                //nt
                addUploadersNt();
            }
            
            
            
        };

        function addUploaders(c)
        {
            $scope.fileUpload.callbacks[c] = {};
            $scope.model.imageTypes.forEach(function (t) {
                $scope.fileUpload.callbacks[c][t.id] = {
                    filesAdded: function (uploader, files) {
                        $scope.uploadInProgress = true;

                        files.forEach(function (elem) {
                            var file = _.find($scope.model.nrHeader.images, { carton_no: c, image_type: t.id });
                            if (file == null)
                            {
                                var file = { image_name: elem.name, nr_id: $scope.model.nrHeader.id, carton_no: c, image_type: t.id, file_id: elem.id };
                                $scope.model.nrHeader.images.push(file);
                            }
                            else
                            {
                                file.image_name = elem.name;
                                file.file_id = elem.id;
                            }

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
                        var f = _.find($scope.model.nrHeader.images, { file_id: file.id });
                        if (f != null)
                            f.percent = file.percent;
                    },

                    fileUploaded: function (uploader, file, response) {
                        var f = _.find($scope.model.nrHeader.images, { file_id: file.id });
                        if (f != null) {
                            f.new = true;
                            f.percent = 100;
                        }

                    },
                    error: function (uploader, error) {
                        $scope.loading = false;
                        alert(error.message);
                    }
                };      
            });  

            
        };

        function addUploadersNt() {
            $scope.fileUpload.callbacks = {};
            $scope.model.imageTypes.forEach(function (t) {
                $scope.fileUpload.callbacks[t.id] = {
                    filesAdded: function (uploader, files) {
                        $scope.uploadInProgress = true;

                        $timeout(function () {
                            uploader.start();
                        }, 1);
                    },
                    beforeUpload: function (uploader, file) {
                        uploader.settings.multipart_params = { id: file.id };
                    },
                    uploadProgress: function (uploader, file) {
                        
                        
                    },

                    fileUploaded: function (uploader, file, response) {
                        if ($scope.model.nrHeader.images == null)
                            $scope.model.nrHeader.images = [];
                        $scope.model.nrHeader.images.push({ image_type: t.id, image_name: file.name, file_id: file.id, new: true });

                    },
                    error: function (uploader, error) {
                        $scope.loading = false;
                        alert(error.message);
                    }
                };
            });


        };

        $scope.cartonRange = function () {
            var result = [];
            if ($scope.model != null && $scope.model.nrHeader != null)
            {
                for (var i = 0; i < $scope.model.nrHeader.no_of_cartons; i++) {
                    result.push(i + 1);
                }
            }            
            return result;
        };

        $scope.formatDate = function (date) {
            if (date != null)
                return fromDateFormatted(date);
            return '';
        };

        $scope.getMinCartons = function () {
            if ($scope.model != null && $scope.model.nrHeader != null)
            {
                if ($scope.model.nrHeader.images != null)
                {
                    var obj = _.maxBy(_.filter($scope.model.nrHeader.images, function (im) {
                        return im.id > 0;
                    }), 'carton_no');
                    if (obj != null)
                        return obj.carton_no;
                }                   
                    
            }
            return 0;
        };

        $scope.no_of_cartons_changed = function () {
            if ($scope.model != null && $scope.model.nrHeader != null)
            {
                for (var i = 1; i <= $scope.model.nrHeader.no_of_cartons; i++) {
                    if (!(i in $scope.fileUpload.callbacks)) {
                        addUploaders(i);
                    }
                }
            }
            
        };

        $scope.preview = function () {
            window.open('/InspectionV2/NrPdfReport/?id=' + $scope.model.nrHeader.id);
        };
    };
})();

function File(id, name, percent, size, image_type) {
    this.file_id = id;
    this.name = name;
    this.percent = percent;
    this.size = size;
    this.image_type = image_type;
}

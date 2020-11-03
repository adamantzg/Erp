(function () {
    'use strict';

    angular
        .module('app')
        .controller('controllerView', controllerView);

    controllerView.$inject = ['$scope','$state','$stateParams','factory','STATUS','IMAGES','$location','$window'];

    function controllerView($scope, $state, $stateParams, factory,STATUS,IMAGES,$location,$window) {
        /* jshint validthis:true */
        var vm = this;
        vm.state=$state.current.name
        vm.title = 'controllerView';
        vm.rows = [];
        vm.columns = [];


        activate();

        function activate() { 
        //console.log("ACTIVATE ROWS: " + IMAGES.ROWS + " COLUMNS "+ IMAGES.COLUMNS)
            vm.rows = initArray(IMAGES.ROWS);
            vm.columns = initArray(IMAGES.COLUMNS);
            
           setModel();
        }
        function setModel() {
            factory.getModel(vm.state, $stateParams.id).then(function (response) {

                //console.log(factory.model);
                vm.model = factory.model;
                vm.factoryCode = getFactoryCode(factory.model.factory_id);
                vm.categoryName = getCategoryName(factory.model.notice.categoryId);
                vm.reason = getReason(factory.model.notice.reason_id);
                //$scope.categories = $scope.model.categories;
                //$scope.clients = $scope.model.clients;
                //$scope.factories = $scope.model.factories;
                vm.status = STATUS.NOTICE[vm.model.notice.status].name;
                vm.description = vm.model.notice.description;
                //$scope.document = $scope.model.notice.document;   
                //$scope.baseDocUrl = '/images/upload/changenotices/';
                //$scope.getDocUrl = function (filename) {
                //    return $scope.baseDocUrl + filename;
                //}
            });
        }

        function initArray(size) {
            var dim = [];
            for (var i = 0; i < size; i++) {
                dim[i] = i;
            }          
            return dim;
        }

        function getFactoryCode(id){
            var f = _.find(vm.model.factories, { user_id: id });
            return f.factory_code;
        }
        function getCategoryName(id) {
            var c = _.find(vm.model.changeNoticeCategories, { returncategory_id: id });
           
            return c.category_name;
        }
        function getReason(id) {
            var r = _.find(vm.model.changeNoticeReasons, { id: id });
            //console.log(r);
            return r.description;
        }
        vm.openPdf = function (id) {
            var tempUrl = "http://newapp.bathroomsourcing.com/changenoticev2/ViewPdf/" + id;
            $window.open(tempUrl, '_blank')
           // var protocol = $location.protocol();
           // var host = $location.host();
           // var absUrl = $location.absUrl();

           // console.info("Protocol: " ,protocol);
           // console.info("Host: ", host);
           // console.info("AbsUrl: ", absUrl);

        }
        vm.FormatDate = function (d) {
            if (d == null)
                return '';
            return fromDateFormatted(d.toString());
        }
        vm.getImageUrl = function (row, col) {
            //console.log("GET IMAGE URL")
            if (vm.model != null && vm.model.notice != null && vm.model.notice.images != null) {
                var image = _.find(vm.model.notice.images, { order: row * vm.columns.length + col });

                if (image != null && image.notice_image.length > 0) {

                    if (image.new) {
                        return factory.getTempFileUrl(image.notice_image, image.comments);
                    }
                    else {
                        //console.log("Slika", image.notice_image);
                        return CombineUrls(vm.model.imageRootFolder, image.notice_image);

                    }
                }
                return '';
            }
            else
                return '';
        };
        vm.IsPicture = function (row, col) {
            if (vm.model != null && vm.model.notice != null && vm.model.notice.images != null) {
                var image = _.find(vm.model.notice.images, { order: row * vm.columns.length + col });

                if (image != null && image.notice_image.length > 0) {
                    var ext = GetExtension(image.notice_image).toLowerCase();
                    var m = ext == "jpg" || ext == "jpeg" || ext == "gif" || ext == "png" || ext == "bmp";
                    //console.log("EXTENZIJA: ", ext, m );
                    return ext == "jpg" || ext == "jpeg" || ext == "gif" || ext == "png" || ext == "bmp";
                }
              
            }
            else {
                
                return false;
            }
        };
        vm.GetBootstrapDocIcon = function (row, col, frame) {
            if (vm.model != null && vm.model.notice != null && vm.model.notice.images != null) {
                var image = _.find(vm.model.notice.images, { order: row * vm.columns.length + col });

                if (image != null && image.notice_image.length > 0) {

                    var ext = GetExtension(image.notice_image).toLowerCase();

                    var icon = frame ? "sm-st-icon st-gray" : "fa fa-picture-o";

                    if (ext == 'pdf')
                        icon = frame ? "lg-view-icon st-red" : "fa fa-file-pdf-o";
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
    }
})();

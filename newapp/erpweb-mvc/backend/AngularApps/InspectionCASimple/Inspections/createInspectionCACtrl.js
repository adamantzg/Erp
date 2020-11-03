(function () {
    'use strict';

    angular
        .module('app')
        .controller('createCACtrl', ['factoryInspectionCASample','factoryStorage', 'MAX', 'MIN', 'FEEDBACK_CATEGORY', '$timeout', '$scope', '$uibModal','DEFAULT_PATH', controller])
        .controller('modalInstanceCtrl', ['$uibModalInstance','params','factoryStorage' ,modalController]);
    //controller.$inject = ['factoryInspectionCASample', 'MAX', 'MIN','FEEDBACK_CATEGORY'];
    
    function controller(factoryInspectionCASample, factoryStorage, MAX, MIN, FEEDBACK_CATEGORY, $timeout, $scope, $uibModal, DEFAULT_PATH) {
        /* jshint validthis:true */
        var vm = this;

        vm.title = factoryStorage.test;
        vm.selected = [];
        vm.Feedback = {};
        vm.Feedback.Images = [];
        vm.Feedback.Products = [];

        vm.ApiModelObject = {};

        vm.Feedback.recheck_required = true;    
        vm.Feedback.feedback_category_id = 0;
        //vm.Feedback.inspection = 0;
        vm.Feedback.inspection_qty = 0;
        vm.Feedback.status1 = 1;
        vm.product = {};
        vm.product.inspection_qty = 0;
        vm.returnsCategories = [];
        vm.resonTemplates = {};

        vm.orders = [];
        vm.categories = [];
        vm.view = 0;
        vm.max = MAX;
        vm.succes = true;
        vm.showCreateNewBtn = false;
        vm.showLoadingDialog = false;
        vm.Order = null;
        vm.returnCategory={};

        /* FOR FORM VALIDATION */
        vm.viewSubmited = new Array(MAX);
        vm.productValid = false;

        vm.busyOrders = true;
        vm.busyProducts = false;
        vm.busyReturnCategory = true;
        vm.busyCreate = false;

        vm.createdCA = false;
        activate();
        /**
         * Def Init
         */
        function activate() {
        
            getReturnNo();
            
            getUser();

            initImportances();
            //setViewSubmited();
            factoryInspectionCASample.getClientOrders().then(
                function (data) {
                    vm.busyOrders = false;
                    vm.orders = data;
                }
            );

            /* FILL DROPDOWNLIST REASON TEMPLATES*/
            factoryInspectionCASample.getReasonTemplates().then(
                function (data) {                    
                    vm.resonTemplates = data;
                },
                function(data) {

                }
            );

            /* FILL DROPDOWN LIST */
            getReturnCategories();

            factoryInspectionCASample.getCategories().then(
                function (data) {
                    vm.categories = data;
                    vm.Feedback.feedback_category_id = setToFeedbackCategoryId(FEEDBACK_CATEGORY.INSPECTION_FINDINGS);
                }
            );  
        }

        function initImportances() {
            factoryInspectionCASample.getImportances().then(
                function (data) {
                    vm.importances = data;
                },
                function (err) {
                    console.error(err);
                }
            )
        }

        function setViewSubmited() {
            for (var i = 0; i < vm.viewSubmited.length; i++) {
                vm.viewSubmited[i] = false;
            }
        }
        function getUser() {
            console.log("get user: ");
            factoryInspectionCASample.getUser().then(
                function(user) {
                    console.log("user: ", user);
                    vm.user = user;
                },
                function (error) {
                    console.log(error)
                }
            )
        }

        function getReturnNo() {           
            factoryInspectionCASample.getReturnNo().then(
                function (data) {                   
                     vm.Feedback.return_no = data;
                }
            );
        }

        function setToFeedbackCategoryId(feedbackId) {
           // console.log("POSTAVLJAM", data[0].feedback_cat_id);
            console.log("CATEGORIES "+ feedbackId + " _ ",vm.categories[0]);
            for (var i = 0; i < vm.categories.length; i++) {
                if (vm.categories[i].feedback_cat_id === feedbackId) 
                    return vm.categories[i].feedback_category_id = feedbackId
            }
            return null;
        }

        function getReturnCategories() {
            factoryInspectionCASample.getReturnCategories().then(
                function (data) {
                    vm.busyReturnCategory = false;
                    vm.returnsCategories = data;
                     
                    
                },
                function (err) {
                    console.log(err);
                }
            )  
        }

        
        /**
         * FOR CREATING REF NUMBER - REPLACE SPECIFIED X FROM - XX-CA01X-20170608-XX
         * @param {number} indexStart
         * @param {number} numbChars
         * @param {string} text
         * @param {string} chars
         */
        function replaceChar(indexStart,numbChars, text, chars){
            return text.substr(0, indexStart) + chars + text.substr(indexStart + numbChars);
        }
        /**
         * FOR CREATING REF NUMBER - REPLACE SPECIFIED X FROM - XX-CA01X-20170608-XX
         * @param {number} index
         * @param {string} code
         * @param {string} chars
         */
        function replaceCharNew(index, code, chars) {
            var arrCode = code.split("-");

            if (index === 1) {
                //arrCode[index] = arrCode[index].replace("X", chars);
                arrCode[index] =arrCode[index].substring(0, 4) + chars
                return arrCode.join('-');
            }
            
            arrCode[index] = chars;
            return arrCode.join('-');
        }
        function replaceNr( nr,code ){
            console.log(code.split("-"));
            var arrCode= code.split("-");
            arrCode[1]= arrCode[1].substring(0,2) + nr + arrCode[1].substring(4,5);
            return arrCode.join('-');
        }
        vm.uploadedFiles = [];
        //vm.uploadedFiles[0].id = "";
        //vm.uploadedFiles[0].name = "";
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

                    //console.log(files);

                    files.forEach(function (elem) {
                        console.log("OOOOOO", elem.id);
                        vm.uploadedFiles[elem.id] = elem;

                        // vm.uploadedFiles.push(elem);
                        // vm.uploadedFiles[elem.id] = elem;
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
                beforeUpload: function(uploader, file) {
                    uploader.settings.multipart_params = { id: file.id };
                },
                fileUploaded: function (uploader, file, response) {
                   
                   
                    vm.Feedback.Images.push(new File(file.id, file.name, file.name));
                }
            }
        }
        function File(id, return_image, filename) {
            this.return_id = id;
            this.return_image = return_image;
            this.file_id = id;
            this.filename = filename;
            this.image_unique = null;
            this.return_comment_file_id = null;
            this.return_image = filename;
            this.image_name = filename;
            this.Progress = 0;
            this.added_date = new Date();
            var self = this;
        }
        vm.DeleteFileNewCA = function (name,index) {
            console.log("ZA BRISANJE: ", name, index);
            console.log(vm.Feedback.Images[index]);
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
                return factoryInspectionCASample.defaultUrl + 'getTempUrl?file_id=' + data.return_id; //this is not return_id. it is guid ffs.
                //var isPic = vm.IsPicture(data.return_image);
                //console.log("Dali je slika: "+ isPic);
                //if (isNaN(id))
                //    return !isPic ? vm.GetDocIcon(data) : factoryInspectionCASample.defaultUrl + 'getTempUrl?' + data.return_image;
                //else {
                //    return !isPic ? vm.GetDocIcon(data) : factoryInspectionCASample.defaultUrl + 'getTempUrl?' + data.return_image;
                //}
            }
            return '';
        };
        vm.IsPicture = function (image_name) {
            var ext = GetExtension(image_name).toLowerCase();
            return ext == "jpg" || ext == "jpeg" || ext == "gif" || ext == "png" || ext == "bmp";
        };
        //CATEGORY
        vm.returnCateogrySelected = function(){
            console.log("CATEGORY CODE: ", vm.returnCategory);
            //vm.Feedback.return_no = replaceChar(7, 1, vm.Feedback.return_no, vm.returnCategory.category_code); 
           
            vm.Feedback.return_no = replaceCharNew(1, vm.Feedback.return_no, vm.returnCategory.category_code);            
            vm.Feedback.reason = vm.returnCategory.category_code;

        }

        vm.productSelected = function () {         

            if (vm.product != null && vm.product.inspection_qty != null)
                vm.Feedback.inspection_qty = vm.product.insp_qty;

            if (vm.product.id == null)
                vm.productValid = false;
            else
                vm.productValid = true;
           
        }
        /**
         * Set validation,
         * Fill Feedback.Products
         * Sum inspection_qty
         * @param {[]} productsSelected
         */
        function productsSelected(productsSelected) {
            var sumInspQty = 0;
            if (productsSelected.length > 0) {
                for (var i = 0; i < productsSelected.length; i++) {
                    sumInspQty += productsSelected[i].insp_qty;
                }
                vm.Feedback.inspection_qty = sumInspQty;
                vm.Feedback.Products=productsSelected
                vm.productValid = true;
            }
            else {
                vm.productValid = false;
            }
        }

        /* Order selected get produtct for order */
        vm.getProducts = function () {
            vm.busyProducts = true;
            vm.product = {};
           
            /** Clear selected products*/
            vm.Feedback.Products = [];

            if (vm.Order != null) {

                //vm.Feedback.return_no = replaceChar(18, 2, vm.Feedback.return_no, vm.Order.customer_code);
                //vm.Feedback.return_no = replaceChar(0, 2, vm.Feedback.return_no, vm.Order.factory_code);
                /*factoryInspectionCASample.getRefSequence().then(
                    function(data){
                    }
                )*/
                
                               // vm.Feedback.return_no = replaceNr( 99,vm.Feedback.return_no);

                
               factoryInspectionCASample.getRefSequence(vm.Order.factory_code).then(
                function(data){
                    vm.Feedback.return_no=replaceNr(data,vm.Feedback.return_no);
                    vm.Feedback.return_no = replaceCharNew(3, vm.Feedback.return_no, vm.Order.customer_code);
                     vm.Feedback.return_no = replaceCharNew(0, vm.Feedback.return_no, vm.Order.factory_code);
                }, 
                function(){
                }

                );   


                vm.Feedback.insp_id = vm.Order.insp_unique;
                
                factoryInspectionCASample.getProducts(vm.Order.new_insp_id===null? vm.Order.insp_unique : null, vm.Order.new_insp_id).then(
                    function (data) {
                        vm.products = data;
                        vm.busyProducts = false;
                    }, function () {
                        vm.busyProducts = false;
                    }
                );
            }
        }
        
        vm.right = function () {

            /* FORM VALIDATION  (change submit validation, set when 1st, 2nd, etc... view checked)*/
            vm.viewSubmited[vm.view] = true;
            console.log("VIEW IS SUBMITED", vm.viewSubmited[vm.view]);
            console.log("Is FORM valid: ", vm.CAform.$valid);

            if (vm.CAform.$valid && vm.view < MAX && vm.productValid)
                vm.view += 1;
            
        }
        vm.left = function () {
            if(vm.view > MIN)
            vm.view -= 1;
        }
        vm.getClass = function (index) {
           
            if (index === MAX && index === vm.view) {
                if (vm.succes)
                    vm.showCreateNewBtn = true;
                return vm.succes ? "prev-view-success" : "prev-view-danger";
            }
                
            return index === vm.view ? "prev-view-active" : "prev-view";
        }
        vm.getButtonStyle = function () {
            return vm.CAform.$valid ? "btn-success":"btn-default"
        }
        vm.hideBottoFlex = function(){
            return vm.view === 1 || vm.view === 2 ? true : false;
        }
        vm.getPanelType = function () {
            return vm.view === 4 ? vm.succes ? "panel-success" :"panel-danger" : "panel-default"
        }
        vm.reset = function () {
            
            vm.Order = null;
            vm.product = {};
            vm.product.inspection_qty = 0;
             vm.returnCategory={};
            getReturnNo();

            vm.Feedback = {};    
            vm.Feedback.Images = [];
            vm.Feedback.recheck_required = true;
            vm.Feedback.feedback_category_id = setToFeedbackCategoryId(FEEDBACK_CATEGORY.INSPECTION_FINDINGS);
            vm.Feedback.status1 = 1;

            if (vm.view === MAX)
                vm.showCreateNewBtn = false;

            vm.view = 0;
            for (var i = 0; i < vm.viewSubmited.length; i++) {
                vm.viewSubmited[i] = false;
            }
        }


        vm.saveNewCA = function () {      
            console.log("SAVE PRODUCT_CODE: ", vm.product.cprod_code)
            /*var isReturnTypeEmpty= vm.Feedback.return_no.split("-")[1].indexOf('X') === 4;
            if(isReturnTypeEmpty)
            {
                vm.ErrorSave=true;
            }*/

            if (!(vm.product.cprod_code === undefined)) {
                vm.busyCreate = true;
                vm.createdCA = true;

                factoryInspectionCASample.getCprod(vm.product.cprod_code).then(
                    function (cprodId) {
                        vm.product.cprod_id = cprodId;
                        vm.Feedback.cprod_id = cprodId;
                        vm.Feedback.request_userid = vm.user.user;
                        vm.Feedback.client_id = vm.user.client;
                        vm.Feedback.request_user = vm.user.name;
                        vm.Feedback.recheck_required = vm.Feedback.recheck_required ? 1 : 0;
                        vm.showLoadingDialog = true;
                       

                        vm.ApiModelObject.CA = vm.Feedback;
                        vm.ApiModelObject.Inspection = vm.Order;

                        factoryInspectionCASample.saveNewCA(vm.ApiModelObject).then(
                            function (data) {
                                //console.log("Vraćeno: ", data);
                                //vm.vraceno = data;
                                
                                vm.view = MAX;
                                setViewSubmited();
                                vm.busyCreate = false;
                                vm.showCreateNewBtn = true;
                                vm.showLoadingDialog = false;
                                vm.returnCategory={};
                            },
                            function (error) {
                                vm.error = error;
                                console.log(error);
                                vm.view = MAX;
                                setViewSubmited();
                                vm.succes = false;
                                vm.busyCreate = false;
                                vm.showLoadingDialog = false;

                            }
                        );

                    },                   
                    function (error) {
                        console.log("GET CPRODiD ERROR");
                        console.log(error);
                    }

                );
            }
            else {
                console.log("from else");
                console.log("Feedback: ", vm.Feedback);
                

                vm.Feedback.request_userid = vm.user.user;
                vm.Feedback.client_id = vm.user.client;
                vm.Feedback.request_user = vm.user.name;
                vm.Feedback.recheck_required = vm.Feedback.recheck_required ? 1 : 0;
                vm.showLoadingDialog = true;

                vm.ApiModelObject.CA = vm.Feedback;
                vm.ApiModelObject.CASimpleProducts = vm.Feedback.Products.map(
                    function (prod) {                        
                        return { "CprodCode": prod['cprod_code'], "OrderLineNum": prod['order_linenum'], "cprod_id":prod['cprod_id'] }
                           
                        //return prod['cprod_code'];
                    });
                vm.ApiModelObject.Inspection = vm.Order;
                console.log("API OBJECT: ", vm.ApiModelObject);

                factoryInspectionCASample.saveNewCA(vm.ApiModelObject).then(
                    function (data) {
                        //console.log("Vraćeno: ", data);
                        //vm.vraceno = data;
                        vm.view = MAX;
                        setViewSubmited();
                        vm.busyCreate = false;
                        vm.showCreateNewBtn = true;
                        vm.showLoadingDialog = false;
                    },
                    function (error) {
                        vm.error = error;
                        console.log(error);
                        vm.view = MAX;
                        setViewSubmited();
                        vm.succes = false;
                        vm.busyCreate = false;
                        vm.showLoadingDialog = false;

                    }
                );
            }
        };

        /**
           ** MODAL WINDOW - for Products dropdown list
           *    <- products list
           *    -> selected products
        */
        vm.openModal = openModal;
        
        function openModal() {
           console.log("OPEN MODAL", vm.Feedback.Products.length > 0 ? true : false);
        
                var modalInstance = $uibModal.open({
                    templateUrl: DEFAULT_PATH + 'Inspections/modal.html',
                    size: 'lg',
                    animation: true,
                    controller: 'modalInstanceCtrl',
                    controllerAs: 'vm',
                    resolve: {
                        params: function () {
                            return {
                                products: vm.products,
                                haveSelectedProds: vm.Feedback.Products.length > 0 ? true : false
                            }
                        }
                    }

                })
                /** Cach after closing modal window */
                modalInstance.result.then(
                    function (selected) {      
                        console.log("SELECTED", selected);
                        productsSelected(selected);
                       
                        /**
                            TEST MOŽE SE IZBRISAT AKO SAM ZABORAVIO
                        */
                        vm.TestPrducs = vm.Feedback.Products.map(
                            function (prod) {
                                return { "code":prod['cprod_code'],"line": prod['order_linenum']}
                            });
                            /** END TEST MOŽE SE IZBRISA AKO SAM ZABORAVIO */
                    }
                )
            }
         
        /**
        * END MODAL WINDOW
        */


    }
    /**
     * MODAL CONTROLER - DROPDOWN LIST CONTROLLER
     * @param {any} $uibModalInstance
     * @param {any} params
     * @param {any} factoryStorage
     */
    function modalController($uibModalInstance, params, factoryStorage) {
        var vm = this;
        vm.title = factoryStorage.test;
        vm.products = angular.copy(params.products);
        /** show hide button (OK) */
        vm.selectedEmpty = true;    
        /** Initialize default empty list for dropdown */
        vm.selectedItems = Array(vm.products.length).fill(false);

        vm.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        }
        vm.clear = function () {

            for (var i = 0; i < vm.selectedItems.length; i++) {
                vm.selectedItems[i] = false;
            }
            vm.selectedEmpty = true;
            
        }
        vm.save = function () {
            factoryStorage.setModelProducts(angular.copy(vm.selectedItems));
            $uibModalInstance.close(getSelectedValues());
        }

        /** Set to vertical center and make content scrollabe */
        $uibModalInstance.opened.then(function () {
            if (params.haveSelectedProds) {
                vm.selectedItems = factoryStorage.getModelProducts();
            }
            //setTimeout(function () {
                if ($('.modal').length != 0) {
                    setModalMaxHeight($('.modal'));
                } else {
                }


            //}, 700)
        })

       
        /** Check if we have any selected item in dropdown list */
        vm.checkSelections = function () {
            var selectedProds = getSelectedValues()
            vm.selectedEmpty = selectedProds.length > 0 ? false : true;
        }
        /**
         * Get only selected Products
         */                
        function getSelectedValues() {
            var res = [];
            for (var i = 0; i < vm.selectedItems.length; i++) {
                if (vm.selectedItems[i] !== false) {
                    res.push(vm.selectedItems[i]);
                }
            }
            return res;

        }

        function setModalMaxHeight(element) {


            setTimeout(function () {

            this.$element = $(element);
            this.$content = this.$element.find('.modal-content');
            var borderWidth = this.$content.outerHeight() - this.$content.innerHeight();

            var dialogMargin = $(window).width() < 768 ? 20 : 60;
            var contentHeight = $(window).height() - (dialogMargin + borderWidth);
            var headerHeight = this.$element.find('.modal-header').outerHeight() || 0;
            var footerHeight = this.$element.find('.modal-footer').outerHeight() || 0;
            var maxHeight = contentHeight - (headerHeight + footerHeight);

            this.$content.css({
                'overflow': 'hidden'
            });

            this.$element
                .find('.modal-body').css({
                    'max-height': maxHeight,
                    'overflow-y': 'auto'
                });


            }, 100);


        }
    }

  
})();

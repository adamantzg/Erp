(function () {
    'use strict'
    angular.module('app').controller('dealerDetailEditCtrl',
        function (
            $scope, $uibModalInstance, $timeout, common, factory, types, params) {

            $scope.ctrlName = 'dealerDetailEditCtrl.js';
            $scope.title = "";
            $scope.params = params;
            $scope.category = {};
            $scope.Images = [];
            $scope.uploadedFiles = [];
            $scope.deleteTempImage = function ($index) {
                $scope.Images.splice($index, 1);
            }

            $scope.deleteImage = function (image, $index) {
                $scope.editLog.images.splice($index, 1);
                //factory.deleteImage(image).then(
                //    //function (data) {
                //    //    $scope.Images.splice($index, 1);
                //    //},
                //    //function (error) {
                //    //    return error;
                //    //}
                //)

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
                        $scope.Images.push(new File(file.id, file.name));
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



            $scope.test = "Modal Ctrl V2"

            $scope.example1model = [];
            $scope.translation = {
                buttonDefaultText: "Please select categories..."
            }
            $scope.settings = {
                displayProp: "name",
                smartButtonMaxItems: 5,
                scrollable: false

            }
            $scope.onItemSelect = function () {
                alert();
            }
            $scope.getDDLtext = function (txt) {


                var t = '';
                if (txt.address1 === '')
                    return txt.customer
                //cat.customer + ' - ' + cat.address1 + ', ' + cat.town_city
                //var t = txt.customer+' ' + txt.address1 !== '' && txt.town_city !== '' ? '-' : '';
                //t += ' '+ txt.address1;
                //t += ' '+txt.address1 !== '' ? ', ' : '';
                //t += ' ' + txt.town_city 
                //;

                return txt.name + ' - ' + txt.address1 + ', ' + txt.town_city;
            }
            /*ADD NEW PERSON if missing name then create new one*/

            $scope.showAddNewPerson = false;
            $scope.persons = [];
            $scope.types = types;
            var log = {};
            init();

            $scope.showSelected = function (item) {
                alert(item);
            }

            $scope.cancel = function () {
                if (!params.newLog) {
                    $scope.editLog = log;
                    $uibModalInstance.dismiss(log);

                } else {
                    $uibModalInstance.dismiss();
                }
                //common.setLog(log);
            }

            $scope.save = function () {
                factory.updateLog($scope.editLog).then(

                    function (data) {
                        for (var i = 0; i < $scope.logCategories.length; i++) {
                            if ($scope.logCategories[i].id === $scope.editLog.category_id) {
                                $scope.editLog.category = {};
                                $scope.editLog.category.name = $scope.logCategories[i].name;
                            }

                        }
                        //$scope.returnModel = {};
                        //$scope.returnModel.editLog = {};

                        $scope.editLog.isNewLog = params.newLog;
                        $uibModalInstance.close($scope.editLog);
                    },
                    function (error) {
                        $scope.ERROR = error
                    }
                )

            }

            /* - MANAGE DDL AND INPUT BOX*/
            $scope.ddlVisible = $scope.persons.length > 0;

            $scope.setView = function (backToList) {
                if ($scope.editLog.person === 'new') {
                    $scope.ddlVisible = false;
                    $scope.editLog.person = '';
                } else if (backToList === 'back') {
                    $scope.editLog.person = '';
                    $scope.ddlVisible = true;
                } else {

                }
            }
            /* - END MANAGE DDL AND INPUT*/
            /*END NEW PERSON*/
            $scope.isTrue = function () {
                return true;
            }
            $scope.dealer = params.dealer;

            $scope.updateShowHideNames = function () {
                
                factory.updateShowHideNames($scope.personsObject ).then(
                    function () {
                        $scope.persons = [];
                        for (var i = 0; i < $scope.personsObject.length; i++) {
                            if ($scope.personsObject[i].hide_person === null || $scope.personsObject[i].hide_person === 0) {
                            $scope.persons.push($scope.personsObject[i].person)
                            }
                        }
                        $scope.showNamesList = false;
                    }
                );
            }

            function init() {
                var date = Date.now();
                $scope.editLog = common.getLog();
                $scope.parentId = params.parentId;
                $scope.persons = [];              

                factory.getDealerPersons(params.dealer).then(
                    function (response) {
                        $scope.persons = response.data;
                       
                        $scope.ddlVisible = $scope.persons.length > 0;
                    });

                factory.getDealerPersonsObj(params.dealer).then(
                    function (response) {
                        $scope.personsObject = response.data;
                       
                        for (var i = 0; i < $scope.personsObject.length; i++) {
                            if ($scope.personsObject[i].hide_person ===  null) {
                                $scope.personsObject[i].hide_person = 0;
                            }
                        }
                    }
                )

                /** DEALERS FOR DDL **/
                factory.getDealersAlpha(params.dealer).then(
                    function (data) {
                        $scope.dealerListAlpha = data;
                        $scope.title = $scope.dealerListAlpha[0].name;

                        if (params.newLog) {
                            $scope.editLog.dealer = params.dealerId;
                        }
                    },
                    function () {
                    }
                );

                if ($scope.editLog !== null && $scope.editLog.category_id === 0)
                    $scope.editLog.category_id = '';

                categories();

                if (common.getLog() === null || params.newLog) { /* CREATE NEW */
                    $scope.editLog = {};
                    $scope.editLog.category_ids = [];
                    $scope.editLog.usaDate = Date.now();
                    $scope.editLog.userid = params.userid;
                    $scope.editLog.type = types.calls.in; /** FROM APP.CONSTANT **/
                    $scope.editLog.in_out = types.chat.phone; /** FROM APP.CONSTANT**/
                    $scope.editLog.status = types.status.open; /** FROM APP.CONSTANT**/
                    $scope.editLog.categories = [];
                    $scope.editLog.parent_id = params.parentId;
                    $scope.editLog.customer = '';


                    if (params.newLog) {
                        $scope.editLog.dealer = params.dealer;
                    }
                   
                } else {/* EDIT LOG */
                    //$scope.editLog.date_edit = Date.now();
                   
                    $scope.editLog.userid = params.userid;
                    $scope.editLog.category_ids = [];
                    //$scope.editLog.customer=params.dealer;

                }

                log = angular.copy($scope.editLog);
            }

            //function getDealerPersons() {
            //    $scope.persons = common.getPersons();
            //}
            function categories() {
                return factory.getLogCategories().then(
                    function (response) {
                        $scope.logCategories = response.data;

                    },
                    function (error) {
                        console.log(error);
                    }
                );
            }
        });
})();
(function () {
    'use strict';

    angular
        .module('app')
        .controller('controller', controller)

    
    controller.$inject = ['$scope', '$state', '$stateParams', '$timeout', '$compile', 'factory','modalService','Lightbox'];

    function controller($scope, $state, $stateParams, $timeout,$compile, factory, modalService,Lightbox) {
        $scope.state = $state.current.name;
        $scope.format = 'dd/MM/yyyy';
        $scope.productSearchTerm = '';
        $scope.productListProgress = 0;
        $scope.dtInstance = {};

        $scope.tableOptions = {};

        

        factory.getModel($scope.state, $stateParams.id).then(function (response) {
            $scope.model = factory.model;
            $scope.role = $scope.model.role;
            $scope.isApprover = $scope.model.isApprover;
            $scope.insp = $scope.model.insp;
            $scope.factories = $scope.model.factories;
            $scope.clients = $scope.model.clients;
            $scope.subjects = $scope.model.subjects;
            $scope.imageTypes = $scope.model.imageTypes;
            $scope.inspectionStatus = $scope.model.inspectionStatus;
            if ($scope.state == 'create') {
                $scope.heading = 'Create new sample inspection';
                
                //$scope.insp = factory.createInspection();
                if ($scope.role != 'qc')
                    $scope.insp.lines.push(factory.createLine());
            }
            else if ($scope.state == 'edit') {
                $scope.heading = 'Edit sample inspection';
                $scope.insp.startdate = moment($scope.insp.startdate).toDate();//format('DD/MM/YYYY');
                if ($scope.role != 'qc')
                    $scope.insp.lines.push(factory.createLine());
                if ($scope.role == 'qc')
                {
                    $scope.addUploaders($scope.insp);                    
                }
                $scope.setSiDetailDataForTypes($scope.imageTypes, $scope.insp);
                
                   
            }
            else if ($scope.state == 'home') {
                $scope.start_date = moment().add(-12, 'M').toDate();
                $scope.end_date = moment().toDate();
                $scope.factory_id = null;
                $scope.client_id = null;

                $scope.filterInspections();
            }
        });

        $scope.iconTexts = ['drawing', 'det. drawing', 'label', 'instr.', 'pack.'];

        $scope.filterInspections = function()
        {
            factory.getInspections($scope.start_date, $scope.end_date, $scope.factory_id, $scope.client_id).then(function (response) {
                $scope.inspections = factory.inspections;
            });
        }

        $scope.create = function () {
            $state.go('create');
        };

        $scope.edit = function (i) {
            $state.go('edit', { id: i.id });
        };
        
        $scope.delete = function (i) {
            var modalOptions = {
                closeButtonText: 'Cancel',
                actionButtonText: 'Delete sample inspection',
                headerText: 'Delete ' + i.computedCode + '?',
                bodyText: 'Are you sure?'
            };

            modalService.showModal({}, modalOptions)
                .then(function (result) {
                    if (result == "ok") {
                        factory.delete(i.id);
                        _.remove($scope.inspections, { id: i.id});
                    }
                });
        };
        
        factory.getFactories().then(function (response) {
            $scope.factories = factory.factories;
        });

        factory.getClients().then(function (response) {
            $scope.clients = factory.clients;
        });

        factory.getSubjects().then(function (response) {
            $scope.subjects = factory.subjects;
        });

        $scope.FormatDate = function(d)
        {
            if (d == null)
                return '';
            return fromDateFormatted(d.toString());
        }

        $scope.displayDate = function (d) {
            if (d != null)
                return moment(d).format('DD/MM/YYYY');
            return '';
        }

                    
        $scope.datepickers = {};
        $scope.openpicker = function (which) {
            $scope.datepickers[which] = true;
        }

        $scope.getStatusText = function () {
            var text = '';
            if ($scope.insp != null)
            {
                text = $scope.getStatusTextFromCode ($scope.insp.insp_status);
            }
            
            return text;
        };

        $scope.getStatusTextFromCode = function (code) {

            var text = '';
            var status = $scope.inspectionStatus;
            switch (code) {
                case status.new:
                    text = 'new';
                    break;
                case status.awaitingApproval:
                    text = 'awaiting aproval';
                    break;
                case status.approved:
                    text = 'approved';
                    break;
                case status.reportSubmitted:
                    text = 'report submitted';
                    break;
            }

            return text;
        };

        $scope.getControllers = function (prefixTerm) {
            return factory.getControllers(prefixTerm).then(function (response) {
                $scope.controllers = factory.controllers;
                return $scope.controllers;
            });
        };

        $scope.qcSelected = function (u) {
            var qc = _.find($scope.controllers, function (elem)
            {
                return elem.controller_id == u.userid;
            });
            if(qc == null)
            {
                if ($scope.insp.controllers == null)
                    $scope.insp.controllers = [];
                $scope.insp.controllers.push({startdate: $scope.insp.startdate, duration: $scope.insp.days, controller_id: u.userid, controller: u });                                
            }
            $scope.qcSearchText = '';
        };

        $scope.showChangeStatusButton = function () {
            var status = $scope.inspectionStatus;
            //ReportSubmitted = 2, AwaitingApproval = 5, Accepted = 4
            return $scope.insp != null && $scope.insp.insp_status != status.reportSubmitted && ($scope.insp.insp_status != status.awaitingApproval || $scope.isApprover)
                && ($scope.insp.insp_status != status.accepted || $scope.role == 'qc');
        };
        $scope.ChangeStatusText = function () {
            return $scope.insp != null ? $scope.insp.insp_status != $scope.inspectionStatus.awaitingApproval ? 'Submit' : 'Approve' : '';
        };

        $scope.changeStatus = function () {
            factory.changeStatus($scope.insp.id).then(function (response) {
                $scope.insp.insp_status = factory.status;
            });
        };

        $scope.removeController = function (c) {
            _.remove($scope.insp.controllers, { controller_id: c.controller_id });
        };

        $scope.Update = function () {
            //remove empty lines
            var inspData = clone($scope.insp);
            _.remove(inspData.lines, function (l) {
                return l.id < 0 && l.cprod_id == null && l.insp_custproduct_code.length == 0 && l.insp_mastproduct_code.length == 0;
            });            
            
            if ($scope.state == 'create')
                factory.create(inspData).then(function (response) {
                    $scope.postUpdate();                    
                });
            else
            {
                $scope.processSiDetails(inspData);                
                factory.update(inspData).then(function (response) {
                    $scope.postUpdate();
                });
            }
                
        };

        $scope.postUpdate = function () {
            $scope.insp = factory.insp;
            $scope.insp.startdate = moment($scope.insp.startdate).toDate();

            $scope.setSiDetailDataForTypes($scope.imageTypes, $scope.insp);

            if ($scope.role != 'qc')
                $scope.insp.lines.push(factory.createLine());
            if($scope.state == 'create')
                $state.go('edit', { id: $scope.insp.id });
        };

        $scope.processSiDetails = function (i) {
            i.lines.forEach(function (l) {
                if (l.sidetails == null)
                    l.sidetails = [];
                $scope.imageTypes.forEach(function (t) {
                    var lineType = l.imageTypes[t.id];
                    if (lineType.requirement.length == 0 && lineType.comments.length == 0)
                        _.remove(l.sidetails, { type_id: t.id });
                    else
                    {
                        var sidetail = _.find(l.sidetails, { type_id: t.id });
                        if (sidetail == null)
                            l.sidetails.push({ type_id: t.id, requirement: lineType.requirement, comments: lineType.comments });
                        else
                        {
                            sidetail.requirement = lineType.requirement;
                            sidetail.comments = lineType.comments;
                        }
                    }
                });
            });
        };

        $scope.getProducts = function (line, term) {
            
            if ($scope.productSearchTerm.length > 0 && term.substring(0, $scope.productSearchTerm.length) == $scope.productSearchTerm) {
                term = term.toUpperCase();
               
                return _.filter($scope.products, function (p) {
                    return p.cprod_code1.substring(0, term.length).toUpperCase() == term || p.cprod_name.substring(0, term.length).toUpperCase() == term || p.factory_ref.substring(0, term.length).toUpperCase() == term || p.factory_name.substring(0, term.length).toUpperCase() == term;
                });
            }
            $scope.productSearchTerm = term;
            return factory.getProducts(term).
                then(function (response) {
                    $scope.products = factory.products;
                    return $scope.products;
                });

        }

        $scope.productSelected = function (line, $item, $model, $label, $event) {
            line.product = $item;
            line.factory_code = $item.factory_code;
            line.insp_mastproduct_code = $item.factory_ref;
            line.insp_custproduct_name = $item.cprod_name;
            line.insp_custproduct_code = $item.cprod_code1;
            line.cprod_id = $item.cprod_id;
            line.qty = 1;
            line.type_id = 1;
            $scope.imageTypes.forEach(function (t) {
                line.imageTypes[t.id] = { id: t.id, description: t.description, requirement: '', comments: '' };
            });
            line.icons = $item.icons;
            if (line.id <= 0 && $scope.getEmptyLines().length == 0)
                $scope.insp.lines.push(factory.createLine());
        }

        $scope.getEmptyLines = function()
        {
            return _.filter($scope.insp.lines, function(l) {
                return l.cprod_id == null && l.insp_custproduct_code.length == 0 && l.insp_mastproduct_code.length == 0;
            });
        }

        $scope.showRemoveLine = function(l)
        {
            return l.id > 0 || l.id != _.minBy($scope.insp.lines,'id').id;
        }

        $scope.removeLine = function(l)
        {
            _.remove($scope.insp.lines, { id: l.id });
        }

        $scope.expand = function (l,index) {
            
            l.expanded = l.expanded == null ? true : !l.expanded;
            var dtRow = $scope.dtInstance.DataTable.row(index);
            if (l.expanded) {
                var scope = $scope.$new(true);
                scope.l = l;
                scope.iconTexts = $scope.iconTexts;
                scope.role = $scope.role;
                scope.removeImage = $scope.removeImage;
                scope.getImageUrl = $scope.getImageUrl;
                scope.getImages = $scope.getImages;
                scope.lineUpload = $scope.lineUpload;
                scope.openLightboxModal = $scope.openLightboxModal;
                dtRow.child($compile('<div childline></div>')(scope)).show();
            }
            else {
                dtRow.child().hide();
            }
            
        };

        $scope.setSiDetailDataForTypes = function (imageTypes, insp) {

            insp.lines.forEach(function (l) {
                l.imageTypes = {};
                if (l.sidetails != null) {
                    imageTypes.forEach(function (t) {
                        var d = _.find(l.sidetails, { type_id: t.id });
                        if (d != null) {
                            l.imageTypes[t.id] = { id: t.id, description: t.description, requirement: d.requirement, comments: d.comments };
                        }
                        else {
                            l.imageTypes[t.id] = { id: t.id, description: t.description, requirement: '', comments: '' };
                        }
                    });
                }
                else {
                    imageTypes.forEach(function (t) {
                        l.imageTypes[t.id] = { id: t.id, description: t.description, requirement: '', comments: '' };
                    });
                }
            });            
            
        };

        $scope.showExpand = function (l) {
            return $scope.showRemoveLine(l);
        };

        /*$scope.formatChildRow = function (l) {
            var iconTexts = ['drawing', 'det. drawing', 'label', 'instr.', 'pack.'];
            var html;
            //html  = '<section class="panel">';
            html =  '<div class="form-horizontal" style="margin-left:40px">';
            html += '  <div class="form-group">';
            html += '    <label class="col-md-2"></label>';
            html += '    <div class="col-md-6">';
            l.icons.forEach(function (i,index) {
                html += $scope.getProductTextAndImage(i,iconTexts[index]);
            });
            html += '    </div>'
            html += '  </div>'
            html += '  <div class="form-group">';
            html += '    <label class="col-md-2">Requirement</label>';
            html += '    <div class="col-md-6">';
            if ($scope.role == 'fc') {
                html += '<textarea ng-model="l.si_requirement" class="form-control"/>'
            }
            else
                html += '<span ng-model="l.si_requirement"></span>';
            html += '    </div>'
            html += '  </div>'
            html += '</div>'
            //html += '</section>';
            return html;
        };*/

        $scope.getProductTextAndImage = function (icon, text) {
            return ' <small>' + text + '</small>   ' + (icon != null && icon.length > 0 ? '<a href="' + icon + '" target=_blank><img src="/images/small/pdf_icon.gif"/></a>' : '<img src="/images/small/pdf_icon2.gif"/>');
        };

        $scope.productListUpload = {
            url: '/api/inspection/uploadproductlist',
            options:
                {
                    multi_selection: false,
                    max_file_size: '50mb',
                },
            callbacks:
            {
                filesAdded: function (uploader, files) {
                    $scope.productListuploader = uploader;
                    $scope.productListProgress = 0;
                    $timeout(function () {
                        uploader.start();
                    }, 1);
                },
                uploadProgress: function (uploader, file) {
                    $scope.productListProgress = file.percent;
                },
                fileUploaded: function (uploader, file, response) {
                    $scope.productListProgress = 0;
                    var objResponse = JSON.parse(response.response);
                    objResponse.lines.forEach(function (l) {
                        l.imageTypes = {};
                        $scope.imageTypes.forEach(function (t) {
                            l.imageTypes[t.id] = { id: t.id, description: t.description, requirement: '', comments: '' };
                        });
                        $scope.insp.lines.push(l);
                    });
                },
                error: function (uploader, error) {

                    alert(error.message);
                }
            }
        }

        $scope.lineUpload = {
            url: '/api/inspection/uploadimage',
            options:
                {
                    multi_selection: true,
                    max_file_size: '50mb',
                }
        };

        $scope.addUploaders = function (insp) {
            insp.lines.forEach(function (l) {
                l.newimgid = -1;
                l.type_id = 1;
                l.callbacks = {};
                $scope.imageTypes.forEach(function (t) {
                    l.callbacks[t.id] =
                    {
                        filesAdded: function (uploader, files) {
                            if (t.tempImages == null)
                                t.tempImages = [];
                            files.forEach(function (f) {
                                t.tempImages.push({ id: l.newimgid--, file_id: f.id, insp_line: l.id, uploader: uploader, progress: 0, insp_image: f.name, type_id: t.id });
                            });

                            $timeout(function () {
                                uploader.start();
                            }, 1);
                        },
                        uploadProgress: function (uploader, file) {
                            var img = _.find(t.tempImages, { file_id: file.id });
                            if (img != null)
                                img.progress = file.percent;
                        },
                        beforeUpload: function (uploader, file) {
                            uploader.settings.multipart_params = { id: file.id };
                        },
                        fileUploaded: function (uploader, file, response) {
                            var img = _.find(t.tempImages, { file_id: file.id });
                            if (img != null) {
                                img.progress = 100;
                                img.url = $scope.getImageUrl(img);
                                if (l.images == null)
                                    l.images = [];
                                l.images.push(img);
                            }
                            if (_.every(t.tempImages, { progress: 100 }))
                                t.tempImages = [];

                        },
                        error: function (uploader, error) {

                            alert(error.message);
                        }
                    }
                    });
                
            });
            
        };

        $scope.removeImage = function (l,i) {
            _.remove(l.images, { id: i.id });
        };

        $scope.getImageUrl = function (i) {
            if (i.id < 0)
                return '/api/inspection/getTempUrl?file_id=' + i.file_id;
            else
                return $scope.model.imagesRoot + moment($scope.insp.startdate).format('YYYY-MM') + '/' + i.insp_image;
        };

        $scope.getReportLink = function (i) {

            return '/Inspectionv2/SampleInspectionReportPdf/' + i.id;
        };

        $scope.getImages = function(l,t)
        {
            return _.filter(l.images, { type_id: t.id });
        }

        $scope.images = ["http://localhost:1535/images/upload/qc/2017-01/IMG_2236_1.JPG", ""];

        $scope.openLightboxModal = function (images,index) {
            Lightbox.openModal(images, index);
            //Lightbox.openModal($scope.images, 0);
        };

    }


})();
(function () {
    'use strict';

    angular
        .module('app')
        .controller('controller', controller)
        .filter('userTypeFilter', function() {
            return function (users, role) {
                if (users == null)
                    return false;
                return _.filter(users, function(u) {
                    return u.roles.indexOf(role) >= 0;
                });
            };
    });

    controller.$inject = ['$scope', '$state', '$stateParams', '$timeout', 'factory'];

    function controller($scope, $state, $stateParams, $timeout, factory) {

        $scope.selectedDocumentUsers = false;
        $scope.documents = [];
        $scope.timeline = [];
        $scope.baseDocUrl = '/images/upload/onlinetraining/';
        $scope.titles = { 'upload': 'Upload documents', 'edit': 'Edit document', 'publish': 'Publish new version' };

        $scope.state = $state.current.name;
        $scope.all = { 'fc': false, 'cc': false, 'qc': false, 'qa': false };
        
        for (var i = 1; i <= 30; i++){
            $scope.timeline.push(i);
        }

        if($scope.state == 'home')
        {
            factory.getDocuments().then(function (response) {
                $scope.documents = factory.documents;                
            });
        }
        else if ($scope.state == 'upload' || $scope.state == 'edit' || $scope.state == 'publish')
        {
            $scope.title = $scope.titles[$scope.state];
            factory.getUsers().then(function (response) {
                $scope.users = factory.users;
                if ($scope.state != 'upload')
                {
                    $scope.commonOptions.commonInput = false;
                    var id = $state.params.id;
                    //load document, select users
                    factory.getDocument(id).then(function (response) {
                        $scope.documents = _.map(factory.documents, 'current');
                        if ($scope.state == 'publish')
                        {
                            $scope.documents.forEach(function (d) {
                                if(d.original_id == null)
                                    d.original_id = d.id;
                                d.modifiedBy = null;
                                d.dateLastModified = null;
                                d.version = d.version + 1;
                                d.id = 0;
                            });
                        }
                        var selectedUsers = {};
                        
                        $scope.documents.forEach(function (d) {
                            if (d.users != null)
                            {
                                d.users.forEach(function (u) {
                                    selectedUsers[u.userid] = true;
                                });
                            }
                            if (d.acknowledgePeriod != null)
                            {
                                $scope.commonOptions.acknowledge = true;
                                $scope.commonOptions.timeline = d.acknowledgePeriod;
                            }
                            else
                            {
                                $scope.commonOptions.acknowledge = false;
                                $scope.commonOptions.timeline = '';
                            }
                            d.progress = 0;
                            d.file_id = '';
                        });
                        for(var id in selectedUsers)
                        {
                            var index = _.findIndex($scope.users, { userid: parseInt(id) });
                            if (index >= 0)
                                $scope.users[index].selected = true;
                        }
                        
                    });
                }
            });
        }

        $scope.getUserDescription = function (u) {
            if (u != null)
                return u.userwelcome != null ? u.userwelcome : u.userusername;
            return '';
        };

        $scope.displayDate = function(d)
        {
            if(d != null)
                return moment(d).format('DD/MM/YYYY');
            return '';
        }

        $scope.openUpload = function () {
            $state.go('upload');
        };

        $scope.tableOptions = {
            columnDefs: [
            {
                orderable: false,
                searchable: false,
                targets: ['_all'],
                className: 'dt-body-left'
            }],
            order: 1,
            dom:'<"row"f><"pull-right"l>t<p>'
        };
        $scope.dtInstance = {};
        //$scope.uploadedFiles = [];

        $scope.getExpandIcon = function (d) {
            return d.previousVersions.length > 0 ? d.expanded != true ? 'fa  fa-expand fa-2x' : 'fa  fa-expand fa-2x' : 'glyphicon';
        };

        $scope.formatChildRow = function(d)
        {
            var html = '<table class="table table-striped">';
            html += '<thead><tr>';
            html += '<th>Name</th>';
            html += '<th>Version</th>';
            html += '<th>Subject</th>';
            html += '<th>Creator</th>';
            html += '<th>Created</th>';
            html += '<th>Last Modified by</th>';
            html += '<th>Modified</th>';
            html += '</tr></thead><tbody>';
            d.previousVersions.forEach(function (v) {
                html += '<tr>';
                html += '<td><a href="' + $scope.getDocUrl(v) + '">' + '<i class="fa fa-download" aria-hidden="true"></i> ' + v.filename + '</a></td>';
                html += '<td>' + v.version + '</td>';
                html += '<td>' + v.subject + '</td>';
                html += '<td>' + $scope.getUserDescription(v.creator) + '</td>';
                html += '<td>' + $scope.displayDate(v.dateCreated) + '</td>';
                html += '<td>' + $scope.getUserDescription(v.editor) + '</td>';
                html += '<td>' + $scope.displayDate(v.dateLastModified) + '</td>';
                html += '</tr>';
            });
            html += '</tbody></table>';
            return html;

        }

        $scope.getDocUrl = function(d)
        {
            return $scope.baseDocUrl + d.filename;
        }

        $scope.expand = function (d, index) {
            if (d.previousVersions.length > 0)
            {
                d.expanded = d.expanded == null ? true : !d.expanded;
                var dtRow = $scope.dtInstance.DataTable.row(index);
                if (d.expanded) {
                    dtRow.child($scope.formatChildRow(d)).show();
                }
                else {
                    dtRow.child().hide();
                }
            }            
        };

        $scope.checkAllUploaded = function () {
            return _.every($scope.documents, { progress: 100 });
        };



        $scope.removeDocument = function (d) {
            _.remove($scope.documents, { file_id: d.file_id });
        };

        $scope.commonOptions = {
            commonInput: true,
            document: { version: 1, subject: '', note: '',users: []},            
            acknowledge: false,
            timeline: null
        };

        $scope.updateDocuments = function () {
            var common = $scope.commonOptions.document;
            $scope.documents.forEach(function (d) {
                d.version = common.version;
                d.subject = common.subject;
                d.note = common.note;                
            });
        };

        $scope.uploader = null;

        $scope.startUpload = function () {
            
            if ($scope.uploader != null)
                $scope.uploader.start();
        };

        $scope.fileUpload = {
            url: '/api/tcdocument/upload',
            options:
                {
                    multi_selection: false,
                    max_file_size: '50mb'
                },
            editOptions: {
                multi_selection: false,
                max_file_size: '50mb'
            },
            callbacks:
            {
                filesAdded: function (uploader, files) {
                    $scope.uploader = uploader;
                    var isCommon = $scope.commonOptions.commonInput;
                    var commonDoc = $scope.commonOptions.document;
                    files.forEach(function (elem) {
                        $scope.documents.length = 0;
                        //initial progress value
                        var doc = { file_id: elem.id, filename: elem.name, progress: 0, size: elem.size, version: isCommon ? commonDoc.version : 1, subject: isCommon ? commonDoc.subject : '', note: isCommon ? commonDoc.note : '' };
                        $scope.documents.push(doc);
                    });                    
                },
                uploadProgress: function (uploader, file) {
                    var up = _.find($scope.documents, { file_id: file.id });
                    if (up != null)
                        up.progress = file.percent;
                },
                beforeUpload: function (uploader, file) {
                    uploader.settings.multipart_params = { id: file.id };
                },
                fileUploaded: function (uploader, file, response) {
                    var up = _.find($scope.documents, { file_id: file.id });
                    if (up != null)
                        up.progress = 100;
                },
                error: function (uploader, error) {
                    
                    alert(error.message);
                }
            },
            editCallbacks:
            {
                filesAdded: function (uploader, files) {
                    $scope.uploader = uploader;
                    
                    files.forEach(function (elem) {
                        //initial progress value
                        
                        $scope.documents[0].filename = elem.name;
                        $scope.documents[0].progress = 1;
                        $scope.documents[0].file_id = elem.id;
                    });
                    $timeout(function () {
                        uploader.start();
                    }, 1);
                },
                uploadProgress: function (uploader, file) {
                    var up = _.find($scope.documents, { file_id: file.id });
                    if (up != null)
                        up.progress = file.percent;
                },
                beforeUpload: function (uploader, file) {
                    uploader.settings.multipart_params = { id: file.id };
                },
                fileUploaded: function (uploader, file, response) {
                    var up = _.find($scope.documents, { file_id: file.id });
                    if (up != null)
                        up.progress = 100;
                },
                error: function (uploader, error) {

                    alert(error.message);
                }
            }
        };

        $scope.create = function () {
            $scope.beforeSaveDocuments();
            factory.create($scope.documents).then(function (response) {
                $state.go('home');
            });
        };

        $scope.edit = function(id)
        {
            $state.go('edit', { id: id });
        }

        $scope.beforeSaveDocuments = function () {
            $scope.documents.forEach(function (d) {
                d.users = _.filter($scope.users, { selected: true });
                if ($scope.commonOptions.acknowledge == true)
                    d.acknowledgePeriod = $scope.commonOptions.timeline;
                else
                    d.acknowledgePeriod = null;
            });
        };

        $scope.update = function () {
            $scope.beforeSaveDocuments();
            if ($scope.state == 'edit')
            {
                factory.update($scope.documents).then(function (response) {
                    $state.go('home');
                });
            }
            else if($scope.state == 'publish')
            {
                factory.create($scope.documents).then(function (response) {
                    $state.go('home');
                });
            }
        };

        $scope.publish = function (id) {
            $state.go('publish', { id: id });
        };

        $scope.showSave = function () {
            var show = $scope.documents.length > 0  && ($scope.documents[0].progress == 0 || $scope.documents[0].progress == 100) ;
            if ($scope.state == 'publish')
                show = show && $scope.selectedDocumentUsers && _.every($scope.documents, function (d) {
                    return d.file_id.length > 0 && d.progress == 100;
                });
            return show;
        };

        $scope.delete = function (d) {
            if (confirm('Delete the document?'))
            {
                factory.delete(d.current.id).then(function (response) {
                    if (d.previousVersions == null || d.previousVersions.length == 0)
                    {
                        //no previous versions
                        _.remove($scope.documents, function (doc) {
                            return doc.current.id == d.current.id;
                        });
                    }
                    else
                    {
                        //previous versions
                        var lastVersion = _.maxBy(d.previousVersions, function (doc) { return doc.version; } );
                        if (lastVersion != null)
                        {
                            d.current = lastVersion;
                            _.remove(d.previousVersions, { id: lastVersion.id });
                        }                        
                    }
                });
            }
            
        };

        $scope.selectAll = function(role)
        {
            var select = $scope.all[role];
            $scope.users.forEach( function(u) {
                if (u.roles.indexOf(role) >= 0)
                    u.selected = select;
            });

        }

        $scope.usersSelected = function()
        {
            $scope.documents.forEach(function (d) {
                d.users = _.filter($scope.users, { selected: true });

                if (d.users.length > 0)
                    $scope.selectedDocumentUsers = true;
                else
                    $scope.selectedDocumentUsers = false;
            });

        }
    }
})();

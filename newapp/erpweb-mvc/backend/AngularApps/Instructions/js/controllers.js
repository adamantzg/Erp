angular.module('app').controller('homeCtrl', ['$scope','factory', function ($scope,factory) {
    factory.getAll().then(function (response) {
        $scope.model = response.data;
    });

    $scope.dtSettings = {
        pageLength: 50
    };

    $scope.getFileUrl = function (i) {
        return factory.getFileUrl(i);        
    };
}]);

angular.module('app').controller('editCtrl', ['$scope', '$state', '$stateParams', '$timeout', '$q','$compile','factory', 'DTOptionsBuilder', 'DTColumnBuilder', function ($scope, $state, $stateParams, $timeout,$q,$compile,factory, DTOptionsBuilder, DTColumnBuilder) {

    var id = $state == 'create' ? null : $stateParams.id;
    $scope.title = id == null ? 'create new instructions' : 'Edit instruction';
    $scope.saveButtonText = id == null ? 'Create' : 'Update';

    $scope.dt = {};
    $scope.selected = {};
    var loadFromServer = false;


    $scope.dtOptions = DTOptionsBuilder.fromFnPromise(function () {
        var defer = $q.defer();
        var factory_id = $scope.searchParams.factory_id, client_id = $scope.searchParams.client_id;
        if (factory_id != null || client_id != null) {
            if (loadFromServer) {
                factory.searchProducts(factory_id, client_id).then(function (response) {
                    //$scope.selected = {};
                    //response.data.forEach(function (d) {
                    //    $scope.selected[d.cprod_id] = false;
                    //});
                    $scope.products = response.data;
                    defer.resolve(response.data);
                });
            }
            else
                defer.resolve(_.filter($scope.products, $scope.filterProducts));
        }
        else
            defer.resolve([]);
        
        return defer.promise;
    }).withOption('createdRow', function (row, data, dataIndex) {        
        $compile(angular.element(row).contents())($scope);
    }).withPaginationType('full_numbers');

    $scope.dtColumns = [
        DTColumnBuilder.newColumn('cprod_code1').withTitle('Code'),
        DTColumnBuilder.newColumn('cprod_name').withTitle('Description'),
        DTColumnBuilder.newColumn('selected').withTitle('Add').notSortable()            
           .renderWith(function (data, type, full, meta) {
               $scope.selected[full.cprod_id] = false;
               return '<input type="checkbox" ng-model="selected[' + full.cprod_id + ']">';
           })
    ];

    factory.getModel(id).then(function (response) {
        $scope.model = response.data;
    });

    $scope.getFileUrl = function (i) {
        return factory.getFileUrl(i);
    };

    $scope.removeProduct = function (p) {
        _.remove($scope.model.instruction.products, { mast_id: p.mast_id });
        var prod = _.find($scope.products, { cprod_mast: p.mast_id });
        if (prod != null)
            prod.allocated = false;
        loadFromServer = false;
        $scope.dt.reloadData(null, true);
    };

    $scope.searchParams = {};

    $scope.searchProducts = function () {
        //factory.searchProducts($scope.searchParams.factory_id, $scope.searchParams.client_id).then(function (response) {
        //    $scope.products = response.data;
        //});
        loadFromServer = true;
        $scope.dt.reloadData(null, false);
    };

    $scope.showAddProducts = function () {
        return _.values($scope.selected).indexOf(true) >= 0;
    };


    $scope.addProducts = function () {
        
        _.filter(_.map($scope.selected, function(value, key, collection) {
            return {cprod_id: parseInt(key), selected: value};
        }), { selected: true }).forEach(function (prod) {
            var p = _.find($scope.products, { cprod_id: prod.cprod_id });

            if (p != null && _.find($scope.model.instruction.products, { mast_id: p.cprod_mast }) == null) {
                var prodcopy = angular.copy(p);
                var mast = angular.copy(prodcopy.mastProduct);
                mast.custProducts = [prodcopy];
                if ($scope.model.instruction.products == null)
                    $scope.model.instruction.products = [];
                $scope.model.instruction.products.push(mast);
                p.allocated = true;
                $scope.selected[prod.cprod_id] = false;
            }
        });
        loadFromServer = false;
        
        $scope.dt.reloadData(null, true);
        //if ($scope.dt != null)
        //{
        //    $timeout(function () {
        //        $scope.dt.DataTable.draw();
        //    }, 500);
        //}
    
    };

    $scope.filterProducts = function (p) {
        return p.allocated == null || !p.allocated;
    };

    $scope.update = function () {
        var data = angular.copy($scope.model.instruction);
        if (data.products != null)
            data.products.forEach(function (p) {
                p.custProducts = null;
                p.factory = null;
            });
        factory.update(data).then(function (response) {
            $state.go('home');
        }, function (errResponse) {
            $scope.errorMessage = getErrorFromResponse(errResponse);
        });
    };

    $scope.fileUpload = {
        url: factory.getUploadUrl(),
        options: {
            multi_selection: false,
            max_file_size: '32mb',
            filters: [
                
            ]
        },
        callbacks: {
            filesAdded: function (uploader, files) {
                $scope.uploader = uploader;

                files.forEach(function (elem) {
                    $scope.uploadedFile = { file_id: elem.id, name: elem.name };                    
                });
                $timeout(function () {
                    uploader.start();
                }, 1);
            },
            uploadProgress: function (uploader, file) {
                $scope.uploadedFile.progress = file.percent;
            },
            beforeUpload: function (uploader, file) {
                uploader.settings.multipart_params = { id: file.id };
            },
            fileUploaded: function (uploader, file, response) {
                $scope.uploadedFile.progress = 100;
                $scope.model.instruction.file_id = $scope.uploadedFile.file_id;
                $scope.model.instruction.filename = $scope.uploadedFile.name;
            },
            error: function (uploader, error) {

                alert(error.message);
            }
        }
    };
}]);
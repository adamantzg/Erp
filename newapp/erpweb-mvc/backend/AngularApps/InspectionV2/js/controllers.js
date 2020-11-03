angular.module('app').controller('uploadCtrl', ['$scope','$stateParams','$timeout', 'factory', function ($scope, $stateParams,$timeout,factory) {

    $scope.uploadedImages = [];

    $scope.insp = {};

    if ($stateParams.id)
        factory.getEditModel($stateParams.id).then(function (response) {
            $scope.insp = response.data.insp;
            $scope.rootFolder = response.data.rootFolder;
            //$scope.drawingFile = response.data.drawingFile;
        });

    $scope.getFileUrl = function () {
        return factory.getFileUrl($scope.insp,'drawingFile');
    };

    $scope.getFileName = function () {
        return GetFileName($scope.insp.drawingFile);
    };

    $scope.fileUpload = {
        url: factory.getDrawingUploadUrl(),
        options: {
            multi_selection: false,
            max_file_size: '32mb'
        },
        callbacks: {
            filesAdded: function (uploader, files) {
                $scope.uploadInProgress = true;
                files.forEach(function (elem) {
                    //initial progress value
                    var file = { id: elem.id, name: elem.name, percent: 0, size: elem.size };
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
                //im.id = 0;
                $scope.insp.file_id = f.id;
                $scope.insp.drawingFile = f.name;
                $scope.uploadedImages = [];
                $scope.uploaded = true;
                //im.id = "";

            },
            error: function (uploader, error) {
                $scope.loading = false;
                alert(error.message);
            }
        }
    }

    $scope.update = function () {
        
        factory.updateDrawing($scope.insp).then(function (response) {
            $scope.success = true;
            $scope.insp.file_id = null;
            $scope.insp.drawingFile = response.data.drawingFile;
            $scope.uploaded = false;
            $timeout(function () {
                $scope.success = false;
            }, 1000);
        }, function (errResponse) {
            $scope.errorMessage = errResponse.data.ExceptionMessage;
        });
    };
}]);
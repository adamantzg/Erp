(function () {
    'use strict';

    angular
        .module('app')
        .controller('controller', controller);

    controller.$inject = ['$scope','$location','$timeout','factory']; 

    function controller($scope, $location,$timeout, factory) {
        var queryString = loadQueryString();
        $scope.uploading = false;
        $scope.loading = false;
        $scope.saving = false;
        if('id' in queryString)
        {
            $scope.id = queryString['id'];
            $scope.loading = true;
            factory.getTrackingNumbersData(queryString['id']).then(function () {
                $scope.trackingNumbers = factory.trackingNumbersData.trackingNumbers;
                $scope.lines = factory.trackingNumbersData.lines;
                $scope.oldInspId = factory.trackingNumbersData.oldInspId;
                $scope.loading = false;
            });
        }

        $scope.getTrackingCount = function(l)
        {
            return _.filter($scope.trackingNumbers, function (t) {
                return t.mastid == l.mast_id;
            }).length;
        }

        $scope.filterNumbers = function(trackNumber,line)
        {
            if(trackNumber != null)
                return trackNumber.mastid == line.mast_id;
            return false;
        }

        $scope.Save = function () {
            $scope.saving = true;            
            factory.saveTrackingNumbers($scope.trackingNumbers).then(function () {
                $scope.trackingNumbers = factory.trackingNumbersData.trackingNumbers;
                $scope.saving = false;
                factory.sendAutoEmail($scope.oldInspId);
            });
        };

        $scope.removeNumber = function(t)
        {
            _.remove($scope.trackingNumbers, { producttrack_id: t.producttrack_id });
        }

        $scope.fileUpload = {
            url: '/InspectionV2/TrackingNumberFile',
            options: 
                { 
                    multi_selection: false,
                    max_file_size: '10mb',                            
                    filters: [
                    {
                        extensions: 'xls,xlsx'
                    }]
                },
            callbacks: 
            {
                filesAdded: function(uploader, files) {
                    $scope.uploading = true;
                    
                    files.forEach(function(elem) {
                        //initial progress value
                        var file = new File(elem.id,elem.name, 0, elem.size);
                        
                    });
                    $timeout(function() {
                        uploader.start();
                    }, 1);
                },
                uploadProgress: function(uploader, file) {
                    //$scope.loading = file.percent/100.0;
                    
                },
                beforeUpload: function(uploader, file) {
                    
                },
                fileUploaded: function(uploader, file, response) {
                    $scope.trackingNumbers = JSON.parse(response.response).trackingNumbers;
                    var id = -1;
                    $scope.trackingNumbers.forEach(function (t) {
                        t.insp_id = $scope.oldInspId;
                        t.producttrack_id = id;
                        id--;
                    });
                    $scope.uploading = false;
                },
                error: function(uploader, error) {
                    $scope.uploading = false;
                    alert(error.message);
                }
            }
        };
        
    }
    

    
})();

function File(id, name, percent, size) {
    this.id = id;
    this.name = name;
    this.percent = percent;
    this.size = size;
}

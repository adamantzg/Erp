(function () {
    'use strict';

    angular
        .module('app')
        .controller('detailResolvedController', detailResolvedController);

    detailResolvedController.$inject = ['$location', 'factoryStorage', 'RECHECK_STATUS', 'factoryInspectionCASample','FILE_CATEGORY','$window'];

    function detailResolvedController($location, factoryStorage, RECHECK_STATUS, factoryInspectionCASample, FILE_CATEGORY,$window) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'detailResolvedController';
        vm.resolved = factoryStorage.getResolved()
        vm.images = [];
        activate();
        
        function activate() {
            factoryInspectionCASample.getFeedbackImages(vm.resolved.returnsid, FILE_CATEGORY.RECHEK_PHOTOS).then(
                function (data) {
                    vm.images = data;
                }
                , function (ERROR) {
                    console.error(ERROR)
                }
            );
        }
       
        vm.status = function () {
            if (vm.resolved.recheck_status === RECHECK_STATUS.OK)
                return "OK";
            else if (vm.resolved.recheck_status == RECHECK_STATUS.OK)
                return "NO";
            else
                return "N/A";
        }
        vm.openPdf = function (name) {
            var tempUrl = "http://newapp.bathroomsourcing.com/" + name.return_image;
            $window.open(tempUrl, '_blank')
            // var protocol = $location.protocol();
            // var host = $location.host();
            // var absUrl = $location.absUrl();

            // console.info("Protocol: " ,protocol);
            // console.info("Host: ", host);
            // console.info("AbsUrl: ", absUrl);

        }
        vm.isPdf = function (name) {
            console.log(name);
            var ext = GetExtension(name.return_image);
            console.log("extension",ext);
            return ext == '.pdf';
        }

        function GetExtension(filename) {
            console.log("File name: ", filename);
            var point = filename.lastIndexOf(".");
            if (point >= 0)
                return filename.substr(point, filename.length);
            else {
                return '';
            }
        }
    }
})();

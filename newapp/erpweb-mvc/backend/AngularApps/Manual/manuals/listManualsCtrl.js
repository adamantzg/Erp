(function () {
    'use strict';

    angular
        .module('app')
        .controller('listManualsCtrl', ['factoryManual','$timeout','$scope','$uibModal','$state', listManualsCtrl]);

    listManualsCtrl.$inject = ['$state'];
    
    function listManualsCtrl(factoryManual, $timeout, $scope, $uibModal, $state) {

        var vm = this;

        vm.busyManuals = false;
        vm.manuals = [];
        vm.dtInstance = {};
 
        vm.tableOptions = {
            columnDefs: [
                {
                    orderable: false,
                    searchable: false,
                    targets: ['_all'],
                    className: 'dt-body-left'
                }],
            order: 1,
            dom: '<"row"f><"pull-right"l>t<p>'
        };

        activate();

        function activate() {

            vm.busyManuals = true;

            factoryManual.getManuals().then(
                function (response) {
                    vm.manuals = response;
                    vm.busyManuals = false;
                })
        }

        vm.GetCreatorUserName = function (manual) {
            if (manual != null && manual.creator != null && manual.creator.userwelcome != null)
                return manual.creator.userwelcome;
            else
                return '';
        }

        vm.GetCreatedDate = function (manual) {
            if (manual != null && manual.date_created != null)
                return moment(manual.date_created).format('DD/MM/YYYY');
            return '';
        }

        vm.GetLastModifiedDate = function (manual) {
            if (manual != null && manual.lastEdit != null && manual.lastEdit.editUser != null && manual.lastEdit.edit_timestamp != null)
                return moment(manual.lastEdit.edit_timestamp).format('DD/MM/YYYY');
            else
                return '';
        }

        vm.GetLastEditorUserName = function (manual) {
            if (manual != null && manual.lastEdit != null && manual.lastEdit.editUser != null && manual.lastEdit.editUser.userwelcome != null)
                return manual.lastEdit.editUser.userwelcome;
            else
                return '';
        }

        vm.EditManual = function (manual) {
            $state.go('edit', { id: manual.manual_id });
        }

        vm.DeleteManual = function (manual) {
            factoryManual.deleteManual(manual).then(function(response){
                _.remove(vm.manuals, function (man) {
                return manual.manual_id == man.manual_id;
                });
            });
        }

        vm.CopyManual = function (manual) {
            factoryManual.copyManual(manual).then(function (response) {
                manual = factoryManual.manual;
                if (manual != null) {
                    vm.manuals.push(manual);
                }
            });
        }  

        vm.OpenNewManual = function () {
            $state.go('create');
        }
    }
})();
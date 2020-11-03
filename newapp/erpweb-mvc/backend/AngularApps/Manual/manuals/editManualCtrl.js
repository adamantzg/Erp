(function () {
    'use strict';

    angular
        .module('app')
        .controller('editManualCtrl', editManualCtrl);

    editManualCtrl.$inject = ['$location', '$state','factoryManual'];

    function editManualCtrl($location, $state, factoryManual) {
        var vm = this;

        vm.state = $state.current.name;
        
        if (vm.state == 'edit' || vm.state == 'create') {
            var id = $state.params.id;

            factoryManual.getModel(vm.state, id).then(function (response) {
                vm.model = factoryManual.model;
                vm.manual = vm.model.manual;
                vm.node = {};
            });
        }

        vm.update = function () {
            if (vm.manual.manual_id == null || vm.manual.manual_id <= 0) {
                factoryManual.createManual(vm.manual).then(function(response){
                    vm.manual = factoryManual.manual;
                });
            } else {
                factoryManual.updateManual(vm.manual).then(function (respose) {
                    vm.manual = factoryManual.manual;
                });
            }
        };

        vm.addNode = function (manual) {
            if (vm.manual != null) {
                vm.manual.nodes.push(new Node(vm.manual.manual_id, '','','','',0,new Date()));
            }
        };
    }

    function Node(manual_id,title,header,content,footer,order,date_created) {
        this.manual_id = manual_id;
        this.title = title;
        this.header = header;
        this.content = content;
        this.footer = footer;
        this.order = order;
        this.date_created = date_created;
    }
})();
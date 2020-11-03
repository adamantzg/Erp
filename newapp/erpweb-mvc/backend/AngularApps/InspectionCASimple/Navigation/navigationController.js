(function () {
    'use strict';

    angular
        .module('app')
        .controller('navigationController', navigationController);

    navigationController.$inject = ['$location', '$state','factoryStorage'];

    function navigationController($location, $state, factoryStorage) {
        /* jshint validthis:true */
        var nav = this;
        nav.state = $state.current.name;

        nav.return_no = factoryStorage.getRecheck().return_no;
        nav.return_id = factoryStorage.getRecheck().returnsid;
        

        nav.parentTab = function () {
            return nav.state === 'rechecks' || nav.state === 'create' || nav.state == 'resolved';
            /*child tab is edit recheks*/
        }
        activate();

        function activate() { }
    }
})();

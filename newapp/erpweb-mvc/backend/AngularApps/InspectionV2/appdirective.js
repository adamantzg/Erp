(function () {
    'use strict';

    angular
        .module('app')
        .directive('appdirective', appdirective);

    appdirective.$inject = ['$window'];

    function appdirective($window) {
        // Usage:
        //     <appdirective></appdirective>
        // Creates:
        // 
        var directive = {
            link: link,
            restrict: 'A'
        };
        return directive;

        function link(scope, element, attrs) {
            
                var href = element.href;
                if (true) {  // replace with your condition
                    element.attr("target", "_blank");
                }
            
        }
    }

})();
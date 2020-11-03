app.directive('myTarget', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var href = element.href;
            if (true) {  // replace with your condition
                element.attr("target", "_blank");
            }
        }
    };
});
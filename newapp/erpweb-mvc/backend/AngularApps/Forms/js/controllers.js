angular.module('app').controller('FillCtrl', ['$scope','$state','$stateParams', 'factory', function ($scope,$state,$stateParams, factory) {
    if ($stateParams.id != null)
        factory.getForm($stateParams.id).then(function (response) {
            $scope.form = response.data.form;
            $scope.types = response.data.types;
            $scope.renderMethods = response.data.renderMethods;
        });

    $scope.submitForm = function () {
        factory.submitForm($scope.form).then(function () {
            $state.go('home');
        });
    };
}]);


angular.module('app').controller('HomeCtrl', ['$scope', '$state', '$stateParams', 'factory', function ($scope, $state, $stateParams, factory) {
    factory.getAll().then(function (response) {
        $scope.forms = response.data;
    });
}]);

angular.module('app').controller('ResultCtrl', ['$scope', '$state', '$stateParams', 'factory', function ($scope, $state, $stateParams, factory) {
    var state = $state.current.name;
    $scope.form = $stateParams.name;
    if(state == 'results')
    {
        factory.getResults($stateParams.id).then(function (response) {
            $scope.submissions = response.data;            
        });
    }
    else
    {
        factory.getResult($stateParams.id).then(function (response) {
            $scope.form = response.data;
        });
    }
    $scope.id = $stateParams.id;
}]);
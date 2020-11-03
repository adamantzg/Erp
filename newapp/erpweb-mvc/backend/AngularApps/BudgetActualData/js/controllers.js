angular.module('app').controller('homeCtrl', ['$scope','$state', 'factory', function ($scope,$state,factory) {
    if ($state.current.name == 'home')
    {
        factory.getAll().then(function (response) {
            $scope.data = response.data;
        });
    }
    else if ($state.current.name == 'distributors')
    {
        factory.getDistributors().then(function (response) {
            $scope.distributors = response.data;
        });
    }

    $scope.updateDistributor = function (d) {
        factory.updateDistributor(d.user_id, d.showForBudgetActual);
    };

    $scope.dtSettings = {
        pageLength: 50
    };

    
}]);

angular.module('app').controller('editCtrl', ['$scope', '$state', '$stateParams', '$timeout', '$q','$compile','factory', function ($scope, $state, $stateParams, $timeout,$q,$compile,factory) {

    var month21 = $state == 'create' ? null : $stateParams.month21;
    $scope.title = month21 == null ? 'create new budget/actual data' : 'Edit budget/actual data';
    $scope.saveButtonText = month21 == null ? 'Create' : 'Update';

    $scope.isNew = $state == 'create';
       
    
    factory.getModel(month21).then(function (response) {
        $scope.model = response.data;
        
        $scope.model.brands.forEach(function (b) {
            b.ukActual = getFromDictionary(response.data.ukbrandActualData, b.brand_id);
            b.nonUkActual = getFromDictionary(response.data.nonUkbrandActualData, b.brand_id);
            b.ukBudget = getFromDictionary(response.data.ukbrandBudgetData, b.brand_id);
            b.nonUkBudget = getFromDictionary(response.data.nonUkbrandBudgetData, b.brand_id);
        });

        $scope.model.distributors.forEach(function (d) {
            d.actual = getFromDictionary(response.data.distributorActualData, d.user_id);
            d.budget = getFromDictionary(response.data.distributorBudgetData, d.user_id);
        });

        $scope.ukDistributors = _.sortBy( _.filter(response.data.distributors, function (d) { return d.user_country == 'UK' || d.user_country == 'GB' || (d.user_country == 'IE' && d.user_country2 == 'GB'); }),
            function (d) { return d.user_name.toLowerCase();});
        $scope.nonUkDistributors = _.sortBy(_.filter(response.data.distributors, function (d) { return d.user_country != 'UK' && d.user_country != 'GB' && d.user_country != 'IE'; }),
            function (d) { return d.user_name.toLowerCase(); });
        
    });

    function getFromDictionary(dict, key)
    {
        if (key in dict)
            return dict[key];
        return null;
    }
        
    

    $scope.update = function () {
        var data = $scope.model.data;
        $scope.model.brands.forEach(function (b) {
            var brand_id = b.brand_id == 0 ? null : b.brand_id;
            if (b.ukActual != null)
            {
                var r = _.find(data, { brand_id: brand_id, record_type: 'A', ukflag: 1 })
                if (r == null) {
                    r = { brand_id: brand_id, record_type: 'A', month21: month21, ukflag: 1, value: b.ukActual };
                    data.push(r);
                }
                else
                    r.value = b.ukActual;
            }
            if (b.nonUkActual != null) {
                
                var r = _.find(data, { brand_id: brand_id, record_type: 'A', ukflag: 0 })
                if (r == null) {
                    r = { brand_id: brand_id, record_type: 'A', month21: month21, ukflag: 0, value: b.nonUkActual };
                    data.push(r);
                }
                else
                    r.value = b.nonUkActual;
            }
            if (b.ukBudget != null) {
                var r = _.find(data, { brand_id: brand_id, record_type: 'B', ukflag: 1 })
                if (r == null) {
                    r = { brand_id: brand_id, record_type: 'B', month21: month21, ukflag: 1, value: b.ukBudget };
                    data.push(r);
                }
                else
                    r.value = b.ukBudget;
            }
            if (b.nonUkBudget != null) {

                var r = _.find(data, { brand_id: brand_id, record_type: 'B', ukflag: 0 })
                if (r == null) {
                    r = { brand_id: brand_id, record_type: 'B', month21: month21, ukflag: 0, value: b.nonUkBudget };
                    data.push(r);
                }
                else
                    r.value = b.nonUkBudget;
            }
        });

        $scope.model.distributors.forEach(function (d) {

            if (d.actual != null) {
                var r = _.find(data, { distributor_id: d.user_id, record_type: 'A', ukflag: 1 })
                if (r == null) {
                    r = { distributor_id: d.user_id, record_type: 'A', month21: month21, ukflag: 1, value: d.actual };
                    data.push(r);
                }
                else
                    r.value = d.actual;
            }
            if (d.budget != null) {
                var r = _.find(data, { distributor_id: d.user_id, record_type: 'B', ukflag: 1 })
                if (r == null) {
                    r = { distributor_id: d.user_id, record_type: 'B', month21: month21, ukflag: 1, value: d.budget };
                    data.push(r);
                }
                else
                    r.value = d.budget;
            }            
        });

        factory.update(data).then(function (response) {
            $state.go('home');
        });
                
    };

    $scope.paste = function (event, collection, index, field) {
        var clipData = event.originalEvent.clipboardData;
        event.preventDefault();
        angular.forEach(clipData.items, function (item, key) {
            if (clipData.items[key]['type'].match(/text*/)) {
                clipData.items[key].getAsString(function (s) {
                    var parts = convertToPlain(s).split('\n');
                    if (s.substring(0, 1) == '\n')
                        parts.splice(0, 0, '');
                    var i = 0;
                    while(i+index<collection.length && i< parts.length)
                    {
                        var num = null;
                        var sNum = _.replace(parts[i], /,/g, '');
                        if (sNum.length > 0)
                        {
                            num = parseFloat(sNum);
                            if (isNaN(num))
                                num = null;
                        }
                        collection[i + index][field] = num;
                        i++;
                    }
                });
            }
        });
        $timeout(function () {
            $scope.$apply();
        }, 500);
    };

    function convertToPlain(rtf) {
        rtf = rtf.replace(/\\par[d]?/g, "");
        return rtf.replace(/\{\*?\\[^{}]+}|[{}]|\\\n?[A-Za-z]+\n?(?:-?\d+)?[ ]?/g, "").trim();
    }

    

    
}]);
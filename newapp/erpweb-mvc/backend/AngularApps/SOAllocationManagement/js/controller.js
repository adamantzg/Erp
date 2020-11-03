function ClearObject(obj) {
    for (var variableKey in obj) {
        if (obj.hasOwnProperty(variableKey)) {
            delete obj[variableKey];
        }
    }
}


soApp.controller('SoController', [
    '$scope', '$http', function($scope, $http) {
        $scope.Products = model.Products;
        $scope.Product = null;
        $scope.productSearchTerm = '';
        $scope.COOrders = [];
        $scope.SelectedCOModel = [];
        $scope.multiSelectOptions = { showCheckAll: false, showUncheckAll: false, displayProp: 'custpo', idProp: 'orderid'};
        $scope.COLines = [];
        $scope.Allocations = {};
        $scope.AvailableSOLines = [];
        $scope.loadingOrders = false;
        $scope.loadingAllocations = false;
        $scope.loadingProducts = false;
        $scope.updating = false;
        $scope.GetProducts = function (term) {
            if ($scope.productSearchTerm.length > 0 && term.substring(0, $scope.productSearchTerm.length) === $scope.productSearchTerm) {
                term = term.toUpperCase();
                return _.filter($scope.Products, function (p) {
                    return p.cprod_code1.toUpperCase().indexOf(term.toUpperCase()) >= 0 || p.cprod_name.toUpperCase().indexOf(term.toUpperCase()) >= 0;
                });
            }
            $scope.productSearchTerm = term;
            $scope.loadingProducts = true;
            return $http.post(productsUrl, { prefixText: term }).
                then(function (response) {
                    $scope.Products = response.data;
                    $scope.loadingProducts = false;
                    return response.data;
                });
        }

        $scope.ProductSelected = function ($item, $model, $label, $event) {
            $scope.Product = $item;
            $scope.LoadCOOrders($item.cprod_id);
            $scope.SelectedCOModel = [];
        };

        $scope.LoadCOOrders = function (cprod_id) {
            $scope.loadingOrders = true;
            $http.post(coOrdersUrl, { cprod_id: cprod_id }).then(function(response) {
                $scope.COOrders = response.data;
                $scope.loadingOrders = false;
            });
        };

        

        $scope.ShowAllocations = function () {
            //ClearObject($scope.CurrentAllocations);
            $scope.loadingAllocations = true;
            $http.post(coLinesUrl, { cprod_id: $scope.Product.cprod_id, ids: _.map($scope.SelectedCOModel, 'id')}).then(function (response) {
                $scope.COLines = response.data.COLines;
                $scope.AvailableSOLines = response.data.AvailableSOLines;
                //$scope.BuildCOrders(response.data);
                $scope.Allocations = $scope.GetAllocations();
                $scope.loadingAllocations = false;
            });
        }

        /*$scope.BuildCOrders = function (lines) {
            $scope.COOrders = [];
            var grouped = _.groupBy(lines, 'orderid');
            for (var key in grouped) {
                var oLines = grouped[key];
                $scope.COOrders.push({ orderid: key, custpo: oLines[0].custpo, COLines: grouped, orderqty: _.sumBy(grouped, 'orderqty') });
            }
        }*/

        $scope.GetAllocations = function () {

            var result = {};
            $scope.COLines.forEach(function (coLine) {
                coLine.allocationError = false;
                coLine.SOAllocations.forEach(function (alloc) {
                    //var include = (fullyAllocated && alloc.balance == 0) || (!fullyAllocated && alloc.balance > 0);
                    if (!(alloc.st_line in result)) {
                        result[alloc.st_line] = { st_line: alloc.st_line, custpo: alloc.custpo, start_balance: alloc.balance, balance: alloc.balance, COAllocations: [], po_req_etd: alloc.po_req_etd, orderqty: alloc.orderqty };
                    }
                    //if (include)
                    result[alloc.st_line].COAllocations.push({ id: alloc.id, co_line: coLine.linenum, start_alloc_qty: alloc.alloc_qty, alloc_qty: alloc.alloc_qty, modified: false });
                });
            });

            var allZeroStockAllocations = [];

            for (var key in result) {
                var so = result[key];
                so.start_alloc_qty = _.sumBy(so.COAllocations, 'start_alloc_qty');
                if (so.balance === 0 && so.start_alloc_qty === 0)
                    allZeroStockAllocations.push(key);
                $scope.COLines.forEach(function (coLine) {
                    if (_.find(so.COAllocations, { co_line: coLine.linenum }) == null)
                        so.COAllocations.push({ id: 0, st_line: so.st_line, co_line: coLine.linenum, start_alloc_qty: 0, alloc_qty: 0, modified: false });
                });
            }
            //remove so lines that cannot be modified
            for (var i = 0; i < allZeroStockAllocations.length; i++) {
                delete result[allZeroStockAllocations[i]];
            }

            //Add available stock lines not on COrders
            $scope.AvailableSOLines.forEach(function(so_line) {
                result[so_line.linenum] = { st_line: so_line.linenum, custpo: so_line.custpo, start_balance: so_line.balance, balance: so_line.balance, COAllocations: [], po_req_etd: so_line.po_req_etd, orderqty: so_line.orderqty, start_alloc_qty: 0 };
                $scope.COLines.forEach(function(coLine) {
                    result[so_line.linenum].COAllocations.push({ id: 0,co_line: coLine.linenum, start_alloc_qty: 0, alloc_qty: 0, modified: false });
                });
            });

            return result;
        }

        $scope.Recalculate = function(sa, ca) {
            var sumAlloc = _.sumBy(sa.COAllocations, 'alloc_qty');
            
            //Check balance in row for SOrder
            var diff = sumAlloc - sa.start_alloc_qty;
            if (sa.start_balance >= diff)
                sa.balance = sa.start_balance - diff;
            else {
                ca.alloc_qty -= diff;
                sa.balance = sa.start_balance;
            }
            if (sa.balance > sa.orderqty)
                sa.balance = sa.orderqty;

            //Check balance in column for COrder
            var coLineQty = _.find($scope.COLines, { linenum: ca.co_line }).orderqty;
            var sumAllocCo = $scope.GetSumAllocationsCO(ca.co_line);

            diff = sumAllocCo - coLineQty;
            if (diff > 0) {
                ca.alloc_qty -= diff;
                sa.balance += diff;
            }
            ca.modified = true;
        };

        $scope.GetSumAllocationsCO = function (co_line) {
            var sum = 0;
            for (var key in $scope.Allocations) {
                var sa = $scope.Allocations[key];
                var ca = _.find(sa.COAllocations, { co_line: co_line });
                if (ca != null)
                    sum += ca.alloc_qty;
            }
            return sum;
        }

        $scope.formatDate = function(d) {
            return fromJSONDateFormatted(d);
        }

        $scope.AvailableBalance = function(elem) {
            return elem.balance > 0;
        }

        $scope.CurrentBalance = function (elem) {
            return elem.balance == 0;
        }

        $scope.GetTotal = function (cl) {
            var sum = 0;
            for (var key in $scope.Allocations) {
                var sa = $scope.Allocations[key];
                sum += _.filter(sa.COAllocations, { co_line: cl.linenum })[0].alloc_qty;
            }
            return sum;
        }

        $scope.GetTotalStyle = function(cl) {
            if (cl.orderqty != $scope.GetTotal(cl)) {
                cl.allocationError = true;
                return 'wrongAllocations';
            }
            cl.allocationError = false;
            return '';
        }

        $scope.Update = function () {
            var error = _.find($scope.COLines, { allocationError: true });
            if (error != null)
                alert('Please correct allocations before updating.');
            else {
                $scope.updating = true;
                var stock_order_allocations = [];
                for (var key in $scope.Allocations) {
                    var so = $scope.Allocations[key];
                    _.filter(so.COAllocations, { modified: true }).forEach(function(a) {
                        if (a.id > 0)
                            stock_order_allocations.push({ unique_link_ref: a.id, alloc_qty: a.alloc_qty });
                        else {
                            if (a.alloc_qty > 0)
                                stock_order_allocations.push({unique_line_ref: 0, st_line: so.st_line, so_line: a.co_line, alloc_qty: a.alloc_qty });
                        }
                    });
                };
                $http.post(updateUrl, { allocations: stock_order_allocations }).
                    then(function(response) {

                        var updatedAllocations = response.data;
                        //update structure with ids of new records
                        for (var key in $scope.Allocations) {
                            var so = $scope.Allocations[key];
                            _.filter(so.COAllocations, { modified: true }).forEach(function (a) {
                                if (a.id == 0) {
                                    var updatedAlloc = _.find(updatedAllocations, { st_line: a.st_line, so_line: a.co_line });
                                    if (updatedAlloc != null)
                                        a.id = updatedAlloc.unique_link_ref;
                                }
                                a.modified = false;
                            });
                        };
                        $scope.updating = false;
                    });
            }
        }
    }
]);
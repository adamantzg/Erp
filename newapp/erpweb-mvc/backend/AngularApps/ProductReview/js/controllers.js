
prApp.controller('PrController', [
    '$scope', '$http',
    function($scope, $http) {
        $scope.Brands = model.Brands;
        $scope.AnalyticsSubCatRows = model.AnalyticsSubCatRows;
        $scope.BrandOptions = model.BrandOptions;
        $scope.Tabs = _.union($scope.Brands, [{ user_id: null, brandname: 'Universal' }, { user_id: 81, brandname: 'OEM' }]);

        $scope.BrandData = _.groupBy($scope.AnalyticsSubCatRows, function(row) {
            return row.Category.category_type;
        });
        $scope.BrandQty = 0;
        $scope.BrandGBP = 0;
        $scope.DiscQty = 0;
        $scope.DiscGBP = 0;
        $scope.ActiveSubCatId = null;

        $scope.BrandCatData = {};
        for (k in $scope.BrandData)
        {
            var newObj = {};
            newObj.Data = $scope.BrandData[k];
            var categories = _.groupBy(newObj.Data, function (d) {
                return d.Category.category_name;
            });
            newObj.Categories = {};
            var sumPrev=0, sumLast=0, sumDisplayQty=0;
            var hasOptions = false;
            var optionsNum = k in $scope.BrandOptions ? $scope.BrandOptions[k].length : 0;
            
            for (c in categories)
            {
                var optionsTotalsPrev = [], optionsTotalsLast = [];
                for (var i = 0; i < optionsNum; i++) {
                    optionsTotalsPrev.push({TotalList: 0});
                    optionsTotalsLast.push({TotalList: 0 });
                }
                if (optionsNum > 0)
                {
                    categories[c].forEach(function (item) {
                        for (var i = 0; i < optionsNum; i++) {
                            optionsTotalsPrev[i].TotalList += item.STotalsOpPrevious6m[i].TotalList;
                            optionsTotalsLast[i].TotalList += item.STotalsOpLast6m[i].TotalList;
                        }
                    });
                }                
                newObj.Categories[c] = {
                    SumPrev: _.sumBy(categories[c], 'TotalPrevious6m'),
                    SumLast: _.sumBy(categories[c], 'TotalLast6m'),
                    SumDisplayQty: _.sumBy(categories[c], 'DisplayQty'),
                    Data: categories[c],
                    OptionsTotalsPrev: optionsTotalsPrev,
                    OptionsTotalsLast: optionsTotalsLast
                };
            }
            
            $scope.BrandCatData[k] = newObj;
        }

        $scope.Tabs.forEach(function (tab) {
            tab.Qty = 0;
            tab.GBP = 0;
            tab.DiscQty = 0;
            tab.DiscGBP = 0;
            $scope.BrandData[tab.user_id].forEach(function (a) {
                tab.Qty += a.ProductCount;
                tab.GBP += a.TotalGBPLast12m;
            });
            if (tab.user_id != 81)
            {
                $scope.BrandQty += tab.Qty;
                $scope.BrandGBP += tab.GBP;
            }
            
        });

        $scope.DiscontQty = function(tab)
        {
            tab.DiscQty = _.sumBy($scope.BrandData[tab.user_id], function (item) {
                return _.sumBy(item.Products, function (p) {
                    return p.proposed_discontinuation ? 1 : 0;
                })
            });
            
            if (tab.DiscQty == null)
                tab.DiscQty = 0;
            return tab.DiscQty;
        }

        $scope.DiscontQtyBrands = function () {
            $scope.DiscQty = _.sumBy($scope.AnalyticsSubCatRows, function (item) {
                return _.sumBy(item.Products, function (p) {
                    return p.proposed_discontinuation ? 1 : 0;
                })
            })
                        
            if ($scope.DiscQty == null)
                $scope.DiscQty = 0;
            return $scope.DiscQty;
        }

        $scope.DiscontGBP = function(tab)
        {
            
            tab.DiscGBP = _.sumBy($scope.BrandData[tab.user_id], function (item) {
                return _.sumBy(item.Products, function (p) {
                    return p.proposed_discontinuation ? p.TotalGBPLast12m : 0;
                })
            });

            return tab.DiscGBP;
        }

        $scope.DiscontGBPBrands = function () {
            $scope.DiscGBP = _.sumBy($scope.AnalyticsSubCatRows, function (item) {
                return _.sumBy(item.Products, function (p) {
                    return p.proposed_discontinuation ? p.TotalGBPLast12m : 0;
                })
            });
            if ($scope.DiscGBP == null)
                $scope.DiscGBP = 0;
            return $scope.DiscGBP;
        }

        $scope.RemainingQty = function(tab)
        {
            return tab.Qty - tab.DiscQty;
        }

        $scope.RemainingQtyBrands = function () {
            return $scope.BrandQty - $scope.DiscQty;
        }

        $scope.RemainingGBP = function(tab)
        {
            return tab.GBP - tab.DiscGBP;
        }

        $scope.RemainingGBPBrands = function () {
            return $scope.BrandGBP - $scope.DiscGBP;
        }

        $scope.GetCategories = function(tab)
        {

        }

        $scope.GetRowSpan = function(user_id)
        {
            return user_id in $scope.BrandOptions && $scope.BrandOptions[user_id].length > 0 ? 2 : 1;                
        }

        $scope.HasOptions = function(user_id)
        {
            return user_id in $scope.BrandOptions;
        }

        $scope.GetColSpan = function (user_id) {
            if(user_id in $scope.BrandOptions)
                return $scope.BrandOptions[user_id].length+1;
            return 1;
        }

        $scope.FormatDate = function(date)
        {
            if(date != null)
            {
                return moment(new Date(date)).format('d/MM/YYYY');
            }
            return '';
        }
        $scope.FormatPercent = function (num) {
            return (num*100).toFixed(1);
        }

        $scope.FormatNumber = function(num)
        {
            return number_format(num, 2);
        }

        $scope.BrandName = function(brand_user_id)
        {
            return _.find($scope.Tabs, { user_id: brand_user_id }).brandname;
        }

        $scope.GetImageUrl = function(subcat)
        {
            var filtered = _.filter(subcat.Products, { proposed_discontinuation: true });
            if (filtered.length == subcat.Products.length)
                return "/images/icons/checkbox-checked.png";
            if(filtered.length == 0)
                return "/images/icons/checkbox-unchecked.png";
            return "/images/icons/checkbox-partial.png";
        }

        $scope.OpenClosePopup = function(subcat, open)
        {
            $scope.ActiveSubCatId = open ? subcat.Id : null;
        }

        $scope.TogglePopup = function(subcat)
        {
            if(subcat.Products.length > 1)
                $scope.ActiveSubCatId = $scope.ActiveSubCatId != null ? null : subcat.Id;
            else
            {
                var prod = subcat.Products[0];
                prod.proposed_discontinuation = !prod.proposed_discontinuation;
                $scope.SaveProduct(prod);
            }
        }

        $scope.SaveProduct = function(p)
        {
            $http.post('/Product/ProductUpdate', p);
        }

        $scope.HasProposedDiscontinuations = function(subcat)
        {
            return _.filter(subcat.Products, { proposed_discontinuation: true }).length > 0;
        }

        $scope.GetDiscontinued = function()
        {
            var result = [];
            $scope.AnalyticsSubCatRows.forEach(function (r) {
                result = _.union(result, _.filter(r.Products, { proposed_discontinuation: true }));
            });
            return result;
        }
    }
]);
(function (undefined) {

    'use strict'
    angular.module('app').factory('PagerService', function () {


        var service = { GetPager: GetPager };

        return service;


        function GetPager(totalItems, currentPage, pageSize) {
            /** DEFAULT TO FIRST PAGE **/
            currentPage = currentPage || 1;

            /** DEFAULT PAGE SIZE **/
            pageSize = pageSize || 10;

            /** CALCULATE TOTAL PAGES **/
            var totalPages = Math.ceil(totalItems / pageSize);
            var startPage, endPage;


            if (totalPages < 10) { /** LESS THAN 10 TOTAL PAGES SO SHOW ALL **/
                //console.log("INSIDE IF");
                startPage = 1;
                endPage = totalPages;
            } else {
                if (currentPage <= 6) {
                    startPage = 1;
                    endPage = 10;
                }
                else if (currentPage + 4 >= totalPages) {
                    startPage = totalPages - 9;
                    endPage = totalPages;
                } else {
                    startPage = currentPage - 5;
                    endPage = currentPage + 4;
                }
            }

            /** CALCULATE START AND END ITEM INDEXES **/
            var startIndex = (currentPage - 1) * pageSize;
            var endIndex = Math.min(startIndex + pageSize - 1, totalItems - 1);

            var pages = _.range(startPage, endPage + 1);

            return {
                totalItems: totalItems,
                currentPage: currentPage,
                pageSize: pageSize,
                totalPages: totalPages,
                startPage: startPage,
                endPage: endPage,
                startIndex: startIndex,
                endIndex: endIndex,
                pages: pages
            }

        } /** END GetPager function **/

    }); /*** END factory function ***/
})();
(function () {
    'use strict';

    angular
        .module('app')
        .factory('itFactory', itFactory);

    itFactory.$inject = ['$http', 'baseUrl', 'feedbackTypeIt'];

    function itFactory($http, baseUrl, feedbackTypeIt) {
        var service = {
            getFeedbackModel: getFeedbackModel,
            getFeedback: getFeedback,
            getSubscribers: getSubscribers,
            getImportances: getImportances,
            getCategories: getCategories,
            getIssueTypes : getIssueTypes,
            createFeedback: createFeedback,
            deleteTempFile: deleteTempFile,
            openCloseFeedback: openCloseFeedback,
            addNewSubscriber: addNewSubscriber,
            deleteSub: deleteSub,
            deleteTempFileNew: deleteTempFileNew
        };

        return service;

        function getFeedbackModel() {
            return $http.get(baseUrl + 'getFeedbackModel').then(
                function (response) {
                    return response.data
                }
            );
        }
        function getFeedback(id) {
            return $http.get(baseUrl + 'getFeedback', { params: { id: id } }).then(
                function (response) {
                    return response.data
                }
            );
        }
        function getSubscribers(term) {
            return $http.post(baseUrl + 'getUsers?prefixText='+term ).then(
                function (response) {
                    return response.data
                }
            );
        }
        function getImportances() {
            return $http.get(baseUrl + 'getImportances').then(
                function (response) {
                    return response.data
                }
            );
        }
        function getCategories() {
            return $http.get(baseUrl + 'getCategories').then(
                function (response) {
                    return response.data
                }
            );
        }

        function getIssueTypes() {
            return $http.get(baseUrl + 'getIssueTypes', { params: { feedback_type_id: feedbackTypeIt } }).then(
                function (response) {
                    return response.data;
                }
            );
        }

        function createFeedback(r) {
        
           // return $http.post(baseUrl + 'createItFeedback',r).then(
            return $http.post('api/claims/create',r).then(
                function (response) {
                    return response.data
                }
            );
        }
        function deleteTempFile (filename) {
            return $http.post('/Claims/DeleteCommentTempFile', { name: filename });
        };
        function openCloseFeedback(id, openClose) {
            return $http.post('/Claims/openCloseFeedback', { id : id,openClose: openClose });
        };

        //var DeleteImageFile = function (image_unique, return_id) {
        //    return $http.post('@Url.Action("DeleteImageFile")', { image_unique: image_unique, return_id: return_id });
        //};
        function deleteTempFileNew( filename ) {
            console.log("NAME FILE FOR DELETE"+ filename );
            return $http.post('/Claims/DeleteTempFile', { name: filename });
        };
        //var DeleteCommentFile = function (return_comment_file_id, return_comment_id, image_id) {
        //    return $http.post('@Url.Action("DeleteCommentFile")', { return_comment_file_id: return_comment_file_id, return_comment_id: return_comment_id, image_id: image_id });
        //};
         function addNewSubscriber(subs_userid, returnsid) {
            console.log("subs_userid");

            if (returnsid < 1)
                return $http.post('/Claims/UpdateSubscription', { user_id: subs_userid });
            return $http.post('/Claims/UpdateSubscription', { user_id: subs_userid, returnsid: returnsid })
        }
        function deleteSub(subs_userid) {

            return $http.post('/Claims/DeleteSubscription', { subs_id: subs_userid })
        }
    }
})();
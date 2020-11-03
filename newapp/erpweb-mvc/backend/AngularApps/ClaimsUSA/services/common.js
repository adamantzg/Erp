(function () {
    'use strict';

    angular
        .module('app')
        .factory('common', common);

    common.$inject = ['$http'];

    function common($http) {
        var da = "testData";
        var _log = {};
        var _persons = [];
        var _isNewLog = false;
        var _currentUser = {};
        var _products = [];

        var service = {
            setProducts: setProducts,
            /**OLD FOR DELETING**/
            currentUserId:currentUserId,
            getData: getData,
            setLog: setLog,
            getLog: getLog,
            setPersons: setPersons,
            getPersons: getPersons,
            isNewLog: isNewLog,
            nav:da
        };

        return service;

        function setProducts() {

        }

        /** OLD FOR DELETE**/
        function getData() { }
        function currentUserId() {
            return currentUserId;
        }
        function setCurrentUser(user) {
            _currentUser = user;
        }
        /*1st.use in dealerDetailEditCtr.js for modal dialog*/
        function setLog(log) {
            _log = log
        };
        function getLog() {
            return _log
        };
        function setPersons(persons) {
            _persons = persons;
        };
        function getPersons() {
            return _persons;
        }
        function isNewLog() {
            return _isNewLog;
        }

        /*1st END*/
    }
})();
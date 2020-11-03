    (function () {
    'use strict';

    angular
        .module('app')
        .factory('common', common);

    common.$inject = ['$http'];

    function common($http) {
        var da = "empty";
        var title = "DEALER MANAGEMENT";
        var _log = {};
        var _persons = [];
        var _isNewLog = false;
        var _currentUser = {};
        var _claims = [];
        var _dealer = {};
        var _dealers = [];

        var service = {
            currentUserId:currentUserId,
            getData: getData,
            setLog: setLog,
            getLog: getLog,
            setPersons: setPersons,
            getPersons: getPersons,
            isNewLog: isNewLog,
            nav: da,
            title: title,
            setClaims: setClaims,
            getClaims: getClaims,
            setDealer: setDealer,
            getDealer: getDealer, 
            setDealers: setDealers,
            getDealers: getDealers
        };

        return service;
        function setDealers(dealers) {
        _dealers = dealers
        }
        function getDealers() {
            return _dealers;
        }
        function setClaims(claim) {
            _claims.push(claim);
        }
        function getClaims() {
            return _claims;
        }
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
        function setDealer(dealer) {
            console.log("SET DEALER", dealer);
            _dealer = dealer;
        }
        function getDealer() {
            return _dealer;
        }

        /*1st END*/
    }
})();
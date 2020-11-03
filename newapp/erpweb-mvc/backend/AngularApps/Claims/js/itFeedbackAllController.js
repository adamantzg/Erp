/** Za sada samo navigacija kasnije prebaciti MVC View ITFeedbacks.cshtml ovdje **/
(function () {
    'use strict';

    angular
        .module('app')
        .controller('itFeedbackAllController', itFeedbackAllController);

    itFeedbackAllController.$inject = ['feedbackId', '$state', '$stateParams'];

    function itFeedbackAllController(feedbackId, $state, $stateParams) {
        /* jshint validthis:true */
        var _id = null;
        console.log("Unutar controllera");
        var vm = this;

        //vm.title = 'All Feedbacks injected ' + feedbackId;
        if (feedbackId !== null) {

           // $state.go('editIT', {id:_id});
        }
        else {

        }
        console.log("Param: ", paramId);

        activate();

        function activate() { }
    }
})();

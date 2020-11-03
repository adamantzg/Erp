(function () {
    'use strict';

    var app = angular.module('app', [
        // Angular modules 
        'ui.bootstrap', 'datatables', 'angular-plupload', 'ui.router', 'bootstrapLightbox', 'logToServer'

        // Custom modules 

        // 3rd Party Modules

    ]);
    app.config(function (pluploadOptionProvider) {
        /* Global settings*/
        pluploadOptionProvider.setOptions({
            flash_swf_url: '/Scripts/plupload-2.0.0/js/Moxie.swf',
            silverlight_xap_url: '/Scripts/plupload-2.0.0/js//Moxie.xap',
            max_file_size: '50mb'
        });
    });
    


})();
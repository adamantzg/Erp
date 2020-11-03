angular.module('app', ['ui.bootstrap', 'angular-plupload','ui.router', 'logToServer'])
.config(function (pluploadOptionProvider) {
    /* Global settings*/
    pluploadOptionProvider.setOptions({
        flash_swf_url: '/Scripts/plupload-2.0.0/js/Moxie.swf',
        silverlight_xap_url: '/Scripts/plupload-2.0.0/js//Moxie.xap',
        max_file_size: '10mb'
    });
});
;
'use strict';

var EverReaderJS = (function ($) {

    var self = {};

    self.Notify = function (message, type, delay) {

        $.notify({
            message: message
        }, {
            type: type || 'success',
            placement: {
                from: "bottom",
                align: "center"
            },
            delay: delay || 2500,
            offset: { y: 50 },
            z_index: 1000,
            animate: {
                enter: 'animated fadeInDown',
                exit: 'animated fadeOutUp'
            }
        });
    };

    self.CopyToClipboard = function (text) {
        var tempInput = $("<input>");
        $("body").append(tempInput);
        tempInput.val(text).select();
        document.execCommand("copy");
        tempInput.remove();
    };

    return self;
})(jQuery);

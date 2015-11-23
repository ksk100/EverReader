function everReaderNotify(message, type) {

    $.notify({
        message : message
    }, {
        type: type || 'success',
        placement: {
            from: "bottom",
            align: "center"
        },
        delay: 2500,
        animate: {
            enter: 'animated fadeInDown',
            exit: 'animated fadeOutUp'
        }
    });

}

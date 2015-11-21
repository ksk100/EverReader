function everReaderNotify(message) {

    $.notify({
        message : message
    }, {
        type: 'success',
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

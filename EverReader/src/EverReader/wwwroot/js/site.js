function everReaderNotify(message, type, delay) {

    $.notify({
        message : message
    }, {
        type: type || 'success',
        placement: {
            from: "bottom",
            align: "center"
        },
        delay: delay || 2500,
        offset: { y: 50 },
        z_index : 1000,
        animate: {
            enter: 'animated fadeInDown',
            exit: 'animated fadeOutUp'
        }
    });

}

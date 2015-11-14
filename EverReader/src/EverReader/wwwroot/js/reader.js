$(document).ready(function () {

    $(window).scroll(null, function () {

        // save the three positions
        var savedDocHeight = $(document).height();
        var savedWinHeight = $(window).height();
        var savedWinScrollTop = $(window).scrollTop();

        // set timeout for 0.7 seconds
        setTimeout(function () {
            var docHeight = $(document).height();
            var winHeight = $(window).height();
            var winScrollTop = $(window).scrollTop();

            if (docHeight == savedDocHeight && winHeight == savedWinHeight && winScrollTop == savedWinScrollTop) {
                // var d = moment().format("h:mm:ss.SS");
                // console.log("firing save event: " + d);
                // console.log("docHeight: " + savedDocHeight + ", winHeight: " + savedWinHeight + ", winScrollTop: " + savedWinScrollTop);
                // TODO: make API call to save position
            }
        }, 700);

        var documentScrollPercent = ((savedWinScrollTop) / (savedDocHeight - savedWinHeight)) * 100;

        $("#positionReport").text(documentScrollPercent.toPrecision(4) + "% through document");
    });
    
});
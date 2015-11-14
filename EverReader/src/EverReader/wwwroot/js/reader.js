function navigateToPosition(percentage) {
    var docHeight = $(document).height();
    var winHeight = $(window).height();

    var targetScrollTop = (percentage / 100) * (docHeight - winHeight);
    $(window).scrollTop(targetScrollTop);
}

$(document).ready(function () {

    // attach the scroll handler
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
                var documentScrollPercent = ((savedWinScrollTop) / (savedDocHeight - savedWinHeight)) * 100;

                $.ajax("/api/reading/" + noteGuid, {
                    "method": "PUT",
                    "data": { "percentageRead": documentScrollPercent },
                    "success": function () {
                        // TODO: alert user that postion has been saved?
                    },
                    "error": function () {
                        // TODO: alert the user their position wasn't saved?
                    }
                });
            }
        }, 700);

        var documentScrollPercent = ((savedWinScrollTop) / (savedDocHeight - savedWinHeight)) * 100;

        $("#positionReport").text(documentScrollPercent.toPrecision(4) + "% through document");
    });

    // zoom to currently read position
    navigateToPosition(percentageRead);
    
});
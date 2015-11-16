function navigateToPosition(percentage) {
    var docHeight = $(document).height();
    var winHeight = $(window).height();

    var targetScrollTop = (percentage / 100) * (docHeight - winHeight);
    $(window).scrollTop(targetScrollTop);
}

var lastSavedScrollTop = 0;
var lastSavedScrollTopResetTimerSet = 0;

$(document).ready(function () {

    // attach the scroll handler
    $(window).scroll(null, function () {

        // save the three positions
        var savedDocHeight = $(document).height();
        var savedWinHeight = $(window).height();
        var savedWinScrollTop = $(window).scrollTop();
        var savedTime = moment();

        // set timeout for 0.7 seconds
        setTimeout(function () {
            var docHeight = $(document).height();
            var winHeight = $(window).height();
            var winScrollTop = $(window).scrollTop();

            // if we are still at the same position (after 0.7 seconds), and position is after last saved, then save the bookmark
            if (docHeight == savedDocHeight && winHeight == savedWinHeight && winScrollTop == savedWinScrollTop && winScrollTop > lastSavedScrollTop) {
                // var d = moment().format("h:mm:ss.SS");
                // console.log("firing save event: " + d);
                // console.log("docHeight: " + savedDocHeight + ", winHeight: " + savedWinHeight + ", winScrollTop: " + savedWinScrollTop);

                var documentScrollPercent = ((savedWinScrollTop) / (savedDocHeight - savedWinHeight)) * 100;

                $.ajax("/api/reading/" + noteGuid, {
                    "method": "PUT",
                    "data": { "percentageRead": documentScrollPercent },
                    "success" : function () {
                        lastSavedScrollTop = savedWinScrollTop;
                    },
                    "error" : function () {
                        // TODO: alert the user their position wasn't saved?
                    }
                });
            }

            // if we would save, but the scroll position is higher up the page (user is scrolling up), then set a timeout to reset the lastSavedScrollTop
            if (docHeight == savedDocHeight && winHeight == savedWinHeight && winScrollTop == savedWinScrollTop &&
                lastSavedScrollTop > winScrollTop && lastSavedScrollTopResetTimerSet == 0) {
                // console.log("not saving now, but setting timer to reset scroll position.  saving will resume in 5 seconds");
                lastSavedScrollTopResetTimerSet = 1;
                setTimeout(function () {
                    // console.log("resetting lastSavedScrollTop, so saving will start working again");
                    lastSavedScrollTop = 0;
                    lastSavedScrollTopResetTimerSet = 0;
                }, 5000);
            }

        }, 700);

        var documentScrollPercent = ((savedWinScrollTop) / (savedDocHeight - savedWinHeight)) * 100;

        $("#positionReport").text(documentScrollPercent.toPrecision(4) + "% through document");
    });

    // zoom to currently read position
    navigateToPosition(percentageRead);
    
});
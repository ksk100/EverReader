'use strict';

var lastSavedScrollTop = 0;
var lastSavedScrollTopResetTimerSet = 0;

var reader = (function ($) {
    var self = { readerViewModel : null };
    var lastSelection = "";
    var readerViewModel;

    function calculateDocumentReadPosition() {
        var docHeight = $(document).height();
        var winHeight = $(window).height();
        var winScrollTop = $(window).scrollTop();

        return ((winScrollTop) / (docHeight - winHeight)) * 100;
    }

    function navigateToPosition(percentage) {
        var docHeight = $(document).height();
        var winHeight = $(window).height();

        var targetScrollTop = (percentage / 100) * (docHeight - winHeight);
        $(window).scrollTop(targetScrollTop);
    }

    function saveSelection() {
        var selection = window.getSelection().toString();
        if (selection != "") {
            lastSelection = selection;
        }
    }

    function getSelection() {
        return lastSelection;
    }

    function addBookmark() {
        // - get document percentage read
        var documentPercentageRead = reader.calculateDocumentReadPosition();
        // - get selection
        var lastSelectedText = reader.getSelection() || "[ untitled ]";

        // TODO: 
        // - save the bookmark
        $.ajax("/api/bookmarks/" + noteGuid, {
            "method": "POST",
            "data": { "percentageRead": documentPercentageRead, "bookmarkTitle" : lastSelectedText },
            "success": function () {
                self.readerViewModel.addBookmark(documentPercentageRead, lastSelectedText);
                everReaderNotify("Bookmark successfully added");
            },
            "error": function () {
                // TODO: alert the user their position wasn't saved?
            }
        });

        return false;
    }

    // the external interface exposed
    self.calculateDocumentReadPosition = calculateDocumentReadPosition;
    self.getSelection = getSelection;
    self.navigateToPosition = navigateToPosition;
    self.saveSelection = saveSelection;
    self.addBookmark = addBookmark;

    return self;

})($);


$(document).ready(function () {

    // attach the selection handler
    $("body").click(reader.saveSelection);

    $("#addBookmark").click(reader.addBookmark);

    // attach the scroll handler
    $(window).scroll(null, function () {

        // save the three positions
        var savedDocHeight = $(document).height();
        var savedWinHeight = $(window).height();
        var savedWinScrollTop = $(window).scrollTop();
        // var savedTime = moment();

        // set timeout for 0.7 seconds
        setTimeout(function () {
            var docHeight = $(document).height();
            var winHeight = $(window).height();
            var winScrollTop = $(window).scrollTop();

            // if we are still at the same position (after 0.7 seconds), and position is after last saved, then save the bookmark
            if (docHeight == savedDocHeight && winHeight == savedWinHeight && winScrollTop == savedWinScrollTop && winScrollTop > lastSavedScrollTop) {
                //var d = moment().format("h:mm:ss.SS");
                //console.log("firing save event: " + d);
                //console.log("docHeight: " + savedDocHeight + ", winHeight: " + savedWinHeight + ", winScrollTop: " + savedWinScrollTop);

                var documentScrollPercent = ((savedWinScrollTop) / (savedDocHeight - savedWinHeight)) * 100;

                $.ajax("/api/bookmarks/" + noteGuid, {
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
                //console.log("not saving now, but setting timer to reset scroll position.  saving will resume in 4 seconds");
                lastSavedScrollTopResetTimerSet = 1;
                setTimeout(function () {
                    //console.log("resetting lastSavedScrollTop, so saving will start working again");
                    lastSavedScrollTop = 0;
                    lastSavedScrollTopResetTimerSet = 0;
                }, 4000);
            }

        }, 700);

        var documentScrollPercent = ((savedWinScrollTop) / (savedDocHeight - savedWinHeight)) * 100;

        $("#positionReport").text(documentScrollPercent.toPrecision(4) + "%");
    });

    // zoom to currently read position
    reader.navigateToPosition(percentageRead);

    // setup the view model for KO
    reader.readerViewModel = (function () {
        var bookmarks = ko.observableArray([]);

        function formatPercentageRead(documentPercentageRead) {
            return documentPercentageRead.toPrecision(4) + "%";
        }

        function addBookmark(documentPercentageRead, bookmarkTitle)  {
            console.log("ViewModel.addBookmark fired");
            bookmarks.push({
                action: function () { reader.navigateToPosition(documentPercentageRead) },
                text: "[" + formatPercentageRead(documentPercentageRead) + "] " + bookmarkTitle
            });
        }

        return {
            bookmarks: bookmarks,
            addBookmark: addBookmark
        };
    })();

    ko.applyBindings(reader.readerViewModel);

    $.ajax("/api/bookmarks/" + noteGuid, {
        "method": "GET",
        "data": { },
        "success": function (data) {
            data.forEach(function (bookmark) {
                reader.readerViewModel.addBookmark(bookmark.PercentageRead, bookmark.BookmarkTitle);
            });
        },
        "error": function () {
            // TODO: alert the user their position wasn't saved?
        }
    });
    
});
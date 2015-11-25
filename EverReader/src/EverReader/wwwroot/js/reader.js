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
        reader.documentScrollPercent = percentage;
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

        if (lastSelectedText.length > 30) {
            lastSelectedText = lastSelectedText.slice(0, lastSelectedText.indexOf(' ', 30) + 1) + "...";
        }

        // - save the bookmark
        $.ajax("/api/bookmarks/" + noteGuid, {
            "method": "POST",
            "data": { "percentageRead": documentPercentageRead, "bookmarkTitle" : lastSelectedText },
            "success": function (data) {
                self.readerViewModel.addBookmark(data.id, documentPercentageRead, lastSelectedText);
                everReaderNotify("Bookmark added");
            },
            "error": function () {
                // TODO: alert the user their position wasn't saved?
            }
        });

        return false;
    }

    function deleteBookmark(event, id) {
        // delete the bookmark
        $.ajax("/api/bookmarks/" + noteGuid, {
            "method": "DELETE",
            "data": { "id": id },
            "success": function (data) {
                if (!data.error) {
                    self.readerViewModel.deleteBookmark(id);
                    everReaderNotify("Bookmark deleted");
                } else {
                    everReaderNotify("There was a problem deleting your bookmark", "danger");
                }
            },
            "error": function () {
                everReaderNotify("There was a problem deleting your bookmark", "danger");
            }
        });

        event.stopPropagation();
        $("#koContextMenu").remove();
        return false;
    }


    // the external interface exposed
    self.calculateDocumentReadPosition = calculateDocumentReadPosition;
    self.getSelection = getSelection;
    self.navigateToPosition = navigateToPosition;
    self.saveSelection = saveSelection;
    self.addBookmark = addBookmark;
    self.deleteBookmark = deleteBookmark;

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

        reader.readerViewModel.documentScrollPercent(((savedWinScrollTop) / (savedDocHeight - savedWinHeight)) * 100);
    });

    // zoom to currently read position
    reader.navigateToPosition(percentageRead);

    // setup the view model for KO
    reader.readerViewModel = (function () {
        var bookmarks = ko.observableArray([]);
        var documentScrollPercent = ko.observable(0);

        function documentScrollPercentFormatted() {
            return formatPercentageRead(documentScrollPercent);
        }

        function formatPercentageRead(documentPercentageRead) {
            return (ko.isObservable(documentPercentageRead) ? documentPercentageRead() : documentPercentageRead).toFixed(2) + "%";
        }

        function addBookmark(id, documentPercentageRead, bookmarkTitle)  {
            var insertIndex = (bookmarks().length == 0) ? 0 : bookmarks().length;
            for (var i = 0; i < bookmarks().length; i++) {
                if (bookmarks()[i].percentageRead > documentPercentageRead) {
                    insertIndex = i;
                    break;
                }
            }

            bookmarks().splice(insertIndex, 0, {
                id : id,
                action: function () { reader.navigateToPosition(documentPercentageRead) },
                text: "[" + formatPercentageRead(documentPercentageRead) + "] " +
                            bookmarkTitle +
                            "&nbsp;<span class='glyphicon glyphicon-remove pull-right' onclick='return reader.deleteBookmark(event, " + id + ");' style='top: 3px'></span>",
                percentageRead: documentPercentageRead
            });
        }

        function deleteBookmark(id) {
            for (var i = 0; i < bookmarks().length; i++) {
                if (bookmarks()[i].id == id) {
                    bookmarks().splice(i, 1);
                    break;
                }
            }
        }

        return {
            bookmarks: bookmarks,
            addBookmark: addBookmark,
            deleteBookmark: deleteBookmark,
            documentScrollPercent: documentScrollPercent,
            documentScrollPercentFormatted: documentScrollPercentFormatted
        };
    })();

    ko.applyBindings(reader.readerViewModel);

    $.ajax("/api/bookmarks/" + noteGuid, {
        "method": "GET",
        "data": { },
        "success": function (data) {
            data.forEach(function (bookmark) {
                reader.readerViewModel.addBookmark(bookmark.Id, bookmark.PercentageRead, bookmark.BookmarkTitle);
            });
        },
        "error": function () {
            // TODO: alert the user their position wasn't saved?
        }
    });
    
});
'use strict';

var reader = (function ($, readerInDummyMode) {
    var self = { readerViewModel : null };
    var lastSelection = "";
    var lastSavedScrollTop = 0;
    var lastSavedScrollTopResetTimerSet = 0;
    var completedReadingDocument = false;

    self.calculateDocumentReadPosition = function () {
        var docHeight = $(document).height();
        var winHeight = $(window).height();
        var winScrollTop = $(window).scrollTop();

        return ((winScrollTop) / (docHeight - winHeight)) * 100;
    };

    self.navigateToPosition = function (percentage) {
        reader.documentScrollPercent = percentage;
        var docHeight = $(document).height();
        var winHeight = $(window).height();

        var targetScrollTop = (percentage / 100) * (docHeight - winHeight);
        $(window).scrollTop(targetScrollTop);

        self.readerViewModel.documentScrollPercent(percentage);
    };

    self.getSelection = function () {
        return window.getSelection().toString();
    };

    function addBookmark() {
        // - get document percentage read
        var documentPercentageRead = self.calculateDocumentReadPosition();
        // - get selection
        var selectedText = self.getSelection() || "[ untitled ]";

        if (selectedText.length > 30) {
            selectedText = selectedText.slice(0,
                                             ((selectedText.indexOf(' ', 30) > -1 ? selectedText.indexOf(' ', 30) :
                                              (selectedText.lastIndexOf(' ') > -1 ? selectedText.lastIndexOf(' ') : 30)) + 1)) + "...";
        }

        if (!readerInDummyMode) {
            // - save the bookmark
            $.ajax("/api/bookmarks/" + noteGuid, {
                "method": "POST",
                "data": { "percentageRead": documentPercentageRead, "bookmarkTitle": selectedText },
                "success": function (data) {
                    self.readerViewModel.addBookmark(data.id, documentPercentageRead, selectedText);
                    EverReaderJS.Notify("Bookmark added");
                },
                "error": function () {
                    // TODO: alert the user their position wasn't saved?
                }
            });
        } else {
            self.readerViewModel.addBookmark(reader.readerViewModel.bookmarks().length, documentPercentageRead, selectedText);
            EverReaderJS.Notify("Bookmark added");
        }

        return false;
    }

    function deleteBookmark(event, id) {
        // delete the bookmark
        if (!readerInDummyMode) {
            $.ajax("/api/bookmarks/" + noteGuid, {
                "method": "DELETE",
                "data": { "id": id },
                "success": function (data) {
                    if (!data.error) {
                        self.readerViewModel.deleteBookmark(id);
                        EverReaderJS.Notify("Bookmark deleted");
                    } else {
                        EverReaderJS.Notify("There was a problem deleting your bookmark", "danger");
                    }
                },
                "error": function () {
                    EverReaderJS.Notify("There was a problem deleting your bookmark", "danger");
                }
            });
        } else {
            self.readerViewModel.deleteBookmark(id);
            EverReaderJS.Notify("Bookmark deleted");
        }

        event.stopPropagation();
        $("#koContextMenu").remove();
        return false;
    }

    function scrollHandler() {
        $("#positionReportBlock").removeClass("saved");

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

                if (completedReadingDocument) {
                    // console.log("New scroll position: " + documentScrollPercent + ", but reading completed, ignoring");
                }

                if (!readerInDummyMode && !completedReadingDocument) {
                    $.ajax("/api/bookmarks/" + noteGuid, {
                        "method": "PUT",
                        "data": { "percentageRead": documentScrollPercent },
                        "success": function () {
                            lastSavedScrollTop = savedWinScrollTop;
                            $("#positionReportBlock").addClass("saved");
                        },
                        "error": function () {
                            // TODO: alert the user their position wasn't saved?
                        }
                    });
                }

                if (percentageRead != 100 && documentScrollPercent == 100) {
                    completedReadingDocument = true;
                    EverReaderJS.Notify("<strong>Reading completed!</strong><br />Auto-removing document from 'Recently Read' list.  <strong>Click here to undo</strong>.",
                                        "info",
                                        25000,
                                       function (event) {
                                           // TODO: fire AJAX call to cancel the "mark as read" call
                                       });
                    // TODO: fire AJAX "mark as read" call
                }
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

        self.readerViewModel.documentScrollPercent(((savedWinScrollTop) / (savedDocHeight - savedWinHeight)) * 100);
    }

    // the external interface exposed
    self.addBookmark = addBookmark;
    self.deleteBookmark = deleteBookmark;
    self.scrollHandler = scrollHandler;

    return self;

})($, readerInDummyMode);


$(document).ready(function () {

    $("#addBookmark").click(reader.addBookmark);

    // attach the scroll handler
    $(window).scroll(null, reader.scrollHandler);

    // setup the view model for KO
    reader.readerViewModel = (function (documentTitle) {
        var bookmarks = ko.observableArray([]);
        var documentScrollPercent = ko.observable(0);
        var copyMenu = [{
            text: "Citation",
            action: function () {
                var citation = documentTitle + ' (' + documentScrollPercent().toFixed(2) + '%)';
                EverReaderJS.CopyToClipboard(citation);
                EverReaderJS.Notify("Citation copied to clipboard");
            }
        },
        {
            text: "URL permalink",
            action: function () {
                var url = window.location.toString() + '#' + documentScrollPercent().toFixed(2);
                EverReaderJS.CopyToClipboard(url);
                EverReaderJS.Notify("Permalink URL copied to clipboard");
            }
        }];

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
            copyMenu : copyMenu,
            addBookmark: addBookmark,
            deleteBookmark: deleteBookmark,
            documentScrollPercent: documentScrollPercent,
            documentScrollPercentFormatted: documentScrollPercentFormatted
        };
    })((typeof documentTitle === 'undefined') ? "Untitled Note" : documentTitle);

    ko.applyBindings(reader.readerViewModel);

    // zoom to currently read position
    reader.navigateToPosition(percentageRead);
    
    if (!readerInDummyMode) {
        $.ajax("/api/bookmarks/" + noteGuid, {
            "method": "GET",
            "data": {},
            "success": function (data) {
                data.forEach(function (bookmark) {
                    reader.readerViewModel.addBookmark(bookmark.Id, bookmark.PercentageRead, bookmark.BookmarkTitle);
                });
            },
            "error": function () {
                // TODO: alert the user their position wasn't saved?
            }
        });
    }
});
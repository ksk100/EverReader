using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using EverReader.Models;
using EverReader.DataAccess;
using Microsoft.AspNet.Identity;
using System.Security.Claims;

using EverReader.Controllers;
using EverReader.Services;
using Microsoft.AspNet.Authorization;

namespace EverReader
{
    [Authorize]
    [Route("api/bookmarks")]
    public class BookmarksAPIController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEverReaderDataAccess _dataAccess;

        public BookmarksAPIController(UserManager<ApplicationUser> userManager,
            IEverReaderDataAccess dataAccess)
        {
            _userManager = userManager;
            _dataAccess = dataAccess;
        }

        // GET /api/bookmarks/{guid}
        [HttpGet("{guid:guid}")]
        public async Task<JsonResult> Get(string guid)
        {
            string currentUserId = HttpContext.User.GetUserId();
            Dictionary<string, object> response = new Dictionary<string, object>();

            ApplicationUser user = await ControllerHelpers.GetCurrentUserAsync(_userManager, _dataAccess, currentUserId);

            if (user.EvernoteCredentials == null)
            {
                response["error"] = "You must authenticate with Evernote";
                return Json(response);
            }

            IEvernoteService evernoteService = new EvernoteServiceSDK1(user.EvernoteCredentials);

            List<Bookmark> bookmarks = _dataAccess.GetManualBookmarks(currentUserId, guid);

            var bookmarksForClient = from bookmark in bookmarks
                                     select new { bookmark.Id, bookmark.BookmarkTitle, bookmark.PercentageRead };

            return Json(bookmarksForClient);
        }

        // PUT /api/bookmarks/{guid}
        [HttpPut("{guid:guid}")]
        public async Task<JsonResult> Put(string guid, decimal percentageRead)
        {
            string currentUserId = HttpContext.User.GetUserId();
            Dictionary<string, object> response = new Dictionary<string, object>();

            ApplicationUser user = await ControllerHelpers.GetCurrentUserAsync(_userManager, _dataAccess, currentUserId);

            if (user.EvernoteCredentials == null)
            {
                response["error"] = "You must authenticate with Evernote";
                return Json(response);
            }

            IEvernoteService evernoteService = new EvernoteServiceSDK1(user.EvernoteCredentials);

            Bookmark bookmark = _dataAccess.GetAutomaticBookmark(currentUserId, guid);

            if (bookmark == null)
            {
                bookmark = new Bookmark()
                {
                    NoteGuid = guid,
                    Type = BookmarkType.Automatic,
                    UserId = currentUserId,
                    Updated = DateTime.Now
                };
            }

            bookmark.PercentageRead = percentageRead;
            bookmark.Updated = DateTime.Now;

            _dataAccess.SaveBookmark(bookmark);

            return Json(response);
        }

        // POST /api/bookmarks/{guid}
        [HttpPost("{guid:guid}")]
        public async Task<JsonResult> Post(string guid, string bookmarkTitle, decimal percentageRead)
        {
            string currentUserId = HttpContext.User.GetUserId();
            Dictionary<string, object> response = new Dictionary<string, object>();

            if (bookmarkTitle == null || bookmarkTitle.Length > 64)
            {
                response["error"] = "Bookmark title cannot be empty, and must be no more than 64 characters.";
                return Json(response);
            }

            ApplicationUser user = await ControllerHelpers.GetCurrentUserAsync(_userManager, _dataAccess, currentUserId);

            if (user.EvernoteCredentials == null)
            {
                response["error"] = "You must authenticate with Evernote";
                return Json(response);
            }

            IEvernoteService evernoteService = new EvernoteServiceSDK1(user.EvernoteCredentials);

            // TODO: check for valid note id here?  it won't matter, because bookmarks always retrieved using userId + noteGuid
            // if (evernoteService.GetNote(guid) == null)
            // {
            // }

            Bookmark bookmark =  new Bookmark()
            {
                NoteGuid = guid,
                Type = BookmarkType.Manual,
                BookmarkTitle = bookmarkTitle,
                PercentageRead = percentageRead,
                UserId = currentUserId,
                Updated = DateTime.Now
            };

            _dataAccess.SaveBookmark(bookmark);

            response["id"] = bookmark.Id;

            return Json(response);
        }

        // DELETE /api/bookmarks/{guid}
        [HttpDelete("{guid:guid}")]
        public async Task<JsonResult> Delete(int id)
        {
            string currentUserId = HttpContext.User.GetUserId();
            Dictionary<string, object> response = new Dictionary<string, object>();

            ApplicationUser user = await ControllerHelpers.GetCurrentUserAsync(_userManager, _dataAccess, currentUserId);

            if (user.EvernoteCredentials == null)
            {
                response["error"] = "You must authenticate with Evernote";
                return Json(response);
            }

            IEvernoteService evernoteService = new EvernoteServiceSDK1(user.EvernoteCredentials);

            Bookmark bookmark = _dataAccess.GetBookmarkById(id);

            if (bookmark == null)
            {
                response["error"] = "Unable to delete bookmark: no such bookmark";
                return Json(response);
            }

            if (bookmark.UserId != currentUserId)
            {
                response["error"] = "Unable to delete bookmark: user not authorised";
                return Json(response);
            }

            _dataAccess.DeleteBookmark(bookmark);

            return Json(response);
        }

    }
}

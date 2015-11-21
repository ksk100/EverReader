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

        // PUT api/bookmarks/{guid}
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

            return Json(bookmarks);
        }

        // PUT api/bookmarks/{guid}
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

        // PUT api/bookmarks/{guid}
        [HttpPost("{guid:guid}")]
        public async Task<JsonResult> Post(string guid, string bookmarkTitle, decimal percentageRead)
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

            return Json(response);
        }
    }
}

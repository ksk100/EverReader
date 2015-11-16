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
    [Route("api/reading")]
    public class ReadingAPIController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEverReaderDataAccess _dataAccess;

        public ReadingAPIController(UserManager<ApplicationUser> userManager,
            IEverReaderDataAccess dataAccess)
        {
            _userManager = userManager;
            _dataAccess = dataAccess;
        }


        // POST api/reading/{guid}
        [HttpPost("{guid}")]
        public void Post(string guid, [FromBody]string value)
        {

        }

        // PUT api/reading/{guid}
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
    }
}

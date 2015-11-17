using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using EverReader.ViewModels.Reader;
using Microsoft.AspNet.Authorization;
using EverReader.Models;
using Microsoft.AspNet.Identity;
using EverReader.DataAccess;
using System.Security.Claims;
using EverReader.Services;
using Evernote.EDAM.Type;
using System.Net;
using EverReader.Utility;

namespace EverReader.Controllers
{
    [Authorize]
    public class ReaderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EverReaderContext _applicationDbContext;
        private readonly IEverReaderDataAccess _dataAccess;

        public ReaderController(UserManager<ApplicationUser> userManager,
            EverReaderContext applicationDbContext,
            IEverReaderDataAccess dataAccess)
        {
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
            _dataAccess = dataAccess;
        }


        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await ControllerHelpers.GetCurrentUserAsync(_userManager, _dataAccess, HttpContext.User.GetUserId());          
            if (user.EvernoteCredentials == null)
            {
                return View("MustAuthoriseEvernote");
            }
            return View("FindNotes", new FindNotesViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(FindNotesViewModel findNotesViewModel)
        {
            ApplicationUser user = await ControllerHelpers.GetCurrentUserAsync(_userManager, _dataAccess, HttpContext.User.GetUserId());
            if (user.EvernoteCredentials == null)
            {
                return View("MustAuthoriseEvernote");
            }
            if (ModelState.IsValid)
            {
                IEvernoteService evernoteService = new EvernoteServiceSDK1(user.EvernoteCredentials);
                try
                {
                    findNotesViewModel.SearchResults = evernoteService.GetNotesMetaList(findNotesViewModel.SearchField);
                }
                catch (EvernoteServiceSDK1AuthorisationException)
                {
                    return View("EvernoteAuthorisationError");
                }
            }
            return View("FindNotes", findNotesViewModel);
        }

        [HttpGet]
        [Route("Reader/Read/{guid:guid}")]
        public async Task<IActionResult> Read(string guid)
        {
            string currentUserId = HttpContext.User.GetUserId();
            ApplicationUser user = await ControllerHelpers.GetCurrentUserAsync(_userManager, _dataAccess, currentUserId);
            if (user.EvernoteCredentials == null)
            {
                return View("MustAuthoriseEvernote");
            }
            IEvernoteService evernoteService = new EvernoteServiceSDK1(user.EvernoteCredentials);

            Note note;
            try {
                note = evernoteService.GetNote(guid);
            }
            catch (EvernoteServiceSDK1AuthorisationException)
            {
                // thrown if the user's credentials are no longer valid
                return View("EvernoteAuthorisationError");
            }
            catch (EvernoteServiceSDK1NoteNotFoundException)
            {
                return View("NoteNotFoundError");
            }

            Bookmark bookmark = _dataAccess.GetAutomaticBookmark(currentUserId, guid);

            if (bookmark == null)
            {
                bookmark = new Bookmark() { NoteGuid = guid,
                    PercentageRead = 0,
                    UserId = currentUserId,
                    Type = BookmarkType.Automatic,
                    Updated = DateTime.Now
                };
            }

            // update all the note metadata we store
            bookmark.NoteTitle = note.Title;
            bookmark.NoteLength = note.ContentLength;
            bookmark.NoteCreated = EvernoteSDKHelper.ConvertEvernoteDateToDateTime(note.Created);
            bookmark.NoteUpdated = EvernoteSDKHelper.ConvertEvernoteDateToDateTime(note.Updated);
            _dataAccess.SaveBookmark(bookmark);

            // Create view model for page
            ReaderViewModel readerViewModel = new ReaderViewModel()
            {
                Title = note.Title,
                Content = WebUtility.HtmlDecode(note.Content),
                NoteGuid = note.Guid,
                PercentageRead = bookmark == null ? 0 : bookmark.PercentageRead
            };

            return View(readerViewModel);
        }

        public async Task<IActionResult> RecentlyRead()
        {
            string currentUserId = HttpContext.User.GetUserId();
            ApplicationUser user = await ControllerHelpers.GetCurrentUserAsync(_userManager, _dataAccess, currentUserId);
            if (user.EvernoteCredentials == null)
            {
                return View("MustAuthoriseEvernote");
            }

            List<Bookmark> bookmarks = _dataAccess.GetRecentlyRead(currentUserId);

            return View(new RecentlyReadViewModel() { RecentlyReadNotes = bookmarks });
        }

        [HttpPost]
        [Route("Reader/DeleteBookmark/{id:int}")]
        public async Task<IActionResult> DeleteBookmark(int id)
        {
            string currentUserId = HttpContext.User.GetUserId();
            ApplicationUser user = await ControllerHelpers.GetCurrentUserAsync(_userManager, _dataAccess, currentUserId);
            if (user.EvernoteCredentials == null)
            {
                return View("MustAuthoriseEvernote");
            }

            // checks
            Bookmark bookmark = _dataAccess.GetBookmarkById(id);
            if (bookmark == null)
            {
                return View("BookmarkNotFoundError");
            }
            if (bookmark.UserId != currentUserId)
            {
                return HttpBadRequest();
            }

            // delete bookmark
            _dataAccess.DeleteBookmark(bookmark);

            return RedirectToAction("RecentlyRead");
        }
    }
}

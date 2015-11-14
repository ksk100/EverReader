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
        public IActionResult Index()
        {
            return View("FindNotes", new FindNotesViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(FindNotesViewModel findNotesViewModel)
        {
            ApplicationUser user = await ControllerHelpers.GetCurrentUserAsync(_userManager, _dataAccess, HttpContext.User.GetUserId());
            IEvernoteService evernoteService = new EvernoteServiceSDK1(user.EvernoteCredentials);
            findNotesViewModel.SearchResults = evernoteService.GetNotesMetaList(findNotesViewModel.SearchField);
            return View("FindNotes", findNotesViewModel);
        }

        [HttpGet]
        [Route("Reader/Read/{guid}")]
        public async Task<IActionResult> Read(string guid)
        {
            // TODO: check that the user has authorised Evernote
            string currentUserId = HttpContext.User.GetUserId();
            ApplicationUser user = await ControllerHelpers.GetCurrentUserAsync(_userManager, _dataAccess, currentUserId);
            IEvernoteService evernoteService = new EvernoteServiceSDK1(user.EvernoteCredentials);

            // TODO: check that this note exists.
            Note note = evernoteService.GetNote(guid);

            Bookmark bookmark = _dataAccess.GetAutomaticBookmark(currentUserId, guid);

            ReaderViewModel readerViewModel = new ReaderViewModel()
            {
                Title = note.Title,
                Content = WebUtility.HtmlDecode(note.Content),
                NoteGuid = note.Guid,
                PercentageRead = bookmark == null ? 0 : bookmark.PercentageRead
            };

            return View(readerViewModel);
        }
    }
}

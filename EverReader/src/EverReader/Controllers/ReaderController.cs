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

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

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
            ApplicationUser user = await GetCurrentUserAsync();
            IEvernoteService evernoteService = new EvernoteServiceSDK1(user.EvernoteCredentials);
            findNotesViewModel.SearchResults = evernoteService.GetNotesMetaList(findNotesViewModel.SearchField);
            return View("FindNotes", findNotesViewModel);
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            ApplicationUser user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
            user.EvernoteCredentials = _dataAccess.EFDbEvernoteCredentials.SingleOrDefault(cred => cred.Id == user.EvernoteCredentialsId); ;
            return user;
        }

        [HttpGet]
        [Route("Reader/Read/{guid}")]
        public IActionResult Read(string guid)
        {
            return View();
        }
    }
}

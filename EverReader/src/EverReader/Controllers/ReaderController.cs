﻿using System;
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
using System.Text.RegularExpressions;

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

        [Route("Reader/Search/{exclude:bool}/{sortOrder:range(1,5)}/{sortAscending:bool}/{search}/{page:min(1)?}")]
        public async Task<IActionResult> Search(bool exclude, int sortOrder, bool sortAscending, string search, int page = 1)
        {
            return await PerformSearch(new FindNotesViewModel() {
                ExcludeShortNotes = exclude,
                SortOrder = sortOrder == 4 ? 1 : sortOrder, // we don't allow 4, because there is no sort order we want to use for that
                SortAscending = sortAscending,
                SearchField = WebUtility.UrlDecode(search),
                CurrentResultsPage = page
            });
        }

        [HttpPost]
        public async Task<IActionResult> Index(FindNotesViewModel findNotesViewModel)
        {
            return await PerformSearch(findNotesViewModel);
        }

        private async Task<IActionResult> PerformSearch(FindNotesViewModel findNotesViewModel)
        {
            ApplicationUser user = await ControllerHelpers.GetCurrentUserAsync(_userManager, _dataAccess, HttpContext.User.GetUserId());
            if (user.EvernoteCredentials == null)
            {
                return View("MustAuthoriseEvernote");
            }
            if (ModelState.IsValid)
            {
                findNotesViewModel.SearchPerformed = true;

                IEvernoteService evernoteService = new EvernoteServiceSDK1(user.EvernoteCredentials);
                try
                {
                    ISearchResults searchResults = evernoteService.GetNotesMetaList(
                        findNotesViewModel.SearchField, 
                        (Evernote.EDAM.Type.NoteSortOrder)findNotesViewModel.SortOrder, 
                        findNotesViewModel.SortAscending,
                        findNotesViewModel.CurrentResultsPage,
                        findNotesViewModel.PageSize);

                    findNotesViewModel.NumberUnfilteredResults = searchResults.NotesMetadata.Count;

                    if (findNotesViewModel.ExcludeShortNotes)
                    {
                        searchResults.NotesMetadata.RemoveAll(metadata => metadata.ContentLength < (1024 * 3));
                    }

                    // now we populate with tags and notebook name
                    foreach (INoteMetadata noteMetadata in searchResults.NotesMetadata)
                    {
                        if (noteMetadata.TagGuids != null)
                        {
                            List<TagData> tags = _dataAccess.GetCachedTagData(user.Id, noteMetadata.TagGuids);

                            noteMetadata.TagNames = new List<string>();

                            foreach (string tagGuid in noteMetadata.TagGuids)
                            {
                                TagData tag = tags.FirstOrDefault(t => t.Guid == tagGuid);

                                if (tag == null)
                                {
                                    // save tag
                                    Tag evernoteTag = evernoteService.GetTag(tagGuid);
                                    tag = new TagData { Guid = tagGuid, Name = evernoteTag.Name, UserId = user.Id };
                                    _dataAccess.SaveTag(tag);
                                }
                                noteMetadata.TagNames.Add(tag.Name);
                            }
                        }

                        noteMetadata.NotebookName = _dataAccess.GetCachedNotebookName(user.Id, noteMetadata.NotebookGuid);
                        if (noteMetadata.NotebookName == null)
                        {
                            Notebook notebook = evernoteService.GetNotebook(noteMetadata.NotebookGuid);
                            noteMetadata.NotebookName = notebook.Name;
                            NotebookData notebookData = new NotebookData() { UserId = user.Id, Guid = noteMetadata.NotebookGuid, Name = notebook.Name };
                            _dataAccess.SaveNotebook(notebookData);
                        }
                    }

                    findNotesViewModel.SearchResults = searchResults.NotesMetadata.ConvertAll(noteMeta => new EverReaderNodeMetadataFormatter(noteMeta));
                    findNotesViewModel.TotalResultsForSearch = searchResults.TotalResults;
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

            string decodedContent = WebUtility.HtmlDecode(note.Content);

            // ensure that links to other Evernote notes are directed through EverReader
            decodedContent = Regex.Replace(decodedContent, 
                "<a href=\"evernote:///view/[^/]+/[^/]+/[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}/([0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12})/", 
                "<a href=\"/Reader/Read/$1");

            // convert en-media tags to img tags
            // get a list of image resources, indexed to the MD5SUM hash
            if (note.Resources != null)
            {
                Dictionary<string, string> hashGuidMapping = new Dictionary<string, string>();
                foreach (Resource resource in note.Resources)
                {
                    if (resource.Mime.StartsWith("image") && resource.Data.Size < 1024 * 512)
                    {
                        hashGuidMapping.Add(resource.Data.BodyHash.ToHexString(), resource.Guid);
                    }
                }
                // search and replace each resource in the document
                foreach (string hash in hashGuidMapping.Keys)
                {
                    decodedContent = Regex.Replace(decodedContent,
                        "en-media( [^>]*)? hash=[\"'](" + hash + ")[\"']",
                        "img $1 src=\"/Reader/NoteResource/" + hashGuidMapping[hash] + "\"");

                    decodedContent = Regex.Replace(decodedContent,
                        "<img([^>]*);height:auto;([^>]*)>",
                        "<img $1;$2>");
                }
            }

            ENNoteINoteMetadataAdapter erNoteMetaData = new ENNoteINoteMetadataAdapter(note);

            

            // Create view model for page
            ReaderViewModel readerViewModel = new ReaderViewModel()
            {
                FormattedNoteMetadata = new EverReaderNodeMetadataFormatter(erNoteMetaData),
                Content = decodedContent,
                PercentageRead = bookmark == null ? 0 : bookmark.PercentageRead
            };

            return View(readerViewModel);
        }

        [HttpGet]
        [Route("Reader/NoteResource/{resourceGuid:guid}")]
        public async Task<IActionResult> NoteResource(string resourceGuid)
        {
            string currentUserId = HttpContext.User.GetUserId();
            ApplicationUser user = await ControllerHelpers.GetCurrentUserAsync(_userManager, _dataAccess, currentUserId);
            if (user.EvernoteCredentials == null)
            {
                return View("MustAuthoriseEvernote");
            }
            IEvernoteService evernoteService = new EvernoteServiceSDK1(user.EvernoteCredentials);

            Resource resource;
            try
            {
                resource = evernoteService.GetResource(resourceGuid);
            }
            catch (EvernoteServiceSDK1AuthorisationException)
            {
                // thrown if the user's credentials are no longer valid
                return View("EvernoteAuthorisationError");
            }

            if (!resource.Mime.StartsWith("image"))
            {
                return View("UnknownResourceTypeError");
            }
            if (resource.Data.Size > 1024 * 512)
            {
                return new HttpNotFoundResult();
            }

            return File(resource.Data.Body, resource.Mime);
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

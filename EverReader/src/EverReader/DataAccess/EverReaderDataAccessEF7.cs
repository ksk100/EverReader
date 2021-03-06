﻿using EverReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace EverReader.DataAccess
{
    public class EverReaderDataAccessEF7 : IEverReaderDataAccess
    {
        private readonly EverReaderContext _dbContext;

        public IEnumerable<EFDbEvernoteCredentials> EFDbEvernoteCredentials
        {
            get
            {
                return _dbContext.EvernoteCredentials;
            }
        }

        public EverReaderDataAccessEF7(EverReaderContext dbContext)
        {
            _dbContext = dbContext;
        }

        public EFDbEvernoteCredentials GetEvernoteCredentials(int? credentialsId)
        {
            if (credentialsId == null) throw new ArgumentNullException();
            return _dbContext.EvernoteCredentials.SingleOrDefault(a => a.Id == credentialsId);
        }

        public void UpdateEvernoteCredentials(EFDbEvernoteCredentials evernoteCredentials)
        {
            _dbContext.EvernoteCredentials.Add(evernoteCredentials);
            _dbContext.SaveChanges();
        }

        public Bookmark GetBookmarkById(int id)
        {
            return _dbContext.Bookmarks.SingleOrDefault(b => (b.Id == id));
        }

        public Bookmark GetAutomaticBookmark(string userId, string guid)
        {
            return _dbContext.Bookmarks.SingleOrDefault(b => (b.UserId == userId) && (b.NoteGuid == guid) && b.Type == BookmarkType.Automatic);
        }

        public List<Bookmark> GetManualBookmarks(string userId, string guid)
        {
            return _dbContext.Bookmarks.Where(b => (b.UserId == userId) && (b.NoteGuid == guid) && b.Type == BookmarkType.Manual).OrderBy(b => b.PercentageRead).ToList();
        }

        public void SaveBookmark(Bookmark bookmark)
        {
            _dbContext.Bookmarks.Update(bookmark);
            _dbContext.SaveChanges();
        }

        public void DeleteBookmark(Bookmark bookmark)
        {
            _dbContext.Bookmarks.Remove(bookmark);
            _dbContext.SaveChanges();
        }

        public List<Bookmark> GetRecentlyRead(string userId)
        {
            return _dbContext.Bookmarks.Where(b => (b.UserId == userId) && b.Type == BookmarkType.Automatic).OrderByDescending(b => b.Updated).ToList();
        }

        public List<TagData> GetCachedTagData(string userId, List<string> tagGuids)
        {
            return _dbContext.Tags.Where(t => t.UserId == userId && tagGuids.Contains(t.Guid)).ToList();
        }

        public string GetCachedNotebookName(string userId, string notebookGuid)
        {
            NotebookData notebookData = _dbContext.Notebooks.Where(n => n.UserId == userId && n.Guid == notebookGuid).FirstOrDefault();
            return (notebookData == null) ? null : notebookData.Name;
        }

        public TagData GetTag(string userId, string tagGuid)
        {
            return _dbContext.Tags.Where(t => t.UserId == userId && t.Guid == tagGuid).SingleOrDefault();
        }

        public void SaveTag(TagData tag)
        {
            _dbContext.Tags.Add(tag);
            _dbContext.SaveChanges();
        }

        public void SaveNotebook(NotebookData notebook)
        {
            _dbContext.Notebooks.Add(notebook);
            _dbContext.SaveChanges();
        }

    }
}

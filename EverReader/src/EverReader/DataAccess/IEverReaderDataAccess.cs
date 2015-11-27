using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EverReader.Models;

namespace EverReader.DataAccess
{
    public interface IEverReaderDataAccess
    {
        IEnumerable<EFDbEvernoteCredentials> EFDbEvernoteCredentials { get; }

        EFDbEvernoteCredentials GetEvernoteCredentials(int? credentialsId);

        void UpdateEvernoteCredentials(EFDbEvernoteCredentials evernoteCredentials);

        Bookmark GetBookmarkById(int id);

        Bookmark GetAutomaticBookmark(string userId, string guid);

        List<TagData> GetCachedTagData(string userId, List<string> tagGuids);

        TagData GetTag(string userId, string tagGuid);

        void SaveTag(TagData tag);

        void SaveBookmark(Bookmark bookmark);

        void DeleteBookmark(Bookmark bookmark);

        List<Bookmark> GetRecentlyRead(string userId);

        List<Bookmark> GetManualBookmarks(string userId, string guid);

        string GetCachedNotebookName(string userId, string notebookGuid);

        void SaveNotebook(NotebookData notebook);
    }
}

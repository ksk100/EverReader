using Evernote.EDAM.NoteStore;
using Evernote.EDAM.Type;
using EverReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EverReader.Services
{
    public interface IEvernoteService
    {
        ISearchResults GetNotesMetaList(string searchString, NoteSortOrder sortOrder, bool ascending, int resultsPage, int pageSize);
        Note GetNote(string guid);
        Resource GetResource(string resourceGuid);
        Tag GetTag(string tagGuid);
        Notebook GetNotebook(string notebookGuid);
    }
}

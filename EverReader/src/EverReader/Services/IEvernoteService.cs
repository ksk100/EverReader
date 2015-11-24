using Evernote.EDAM.NoteStore;
using Evernote.EDAM.Type;
using EverReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EverReader.Services
{
    interface IEvernoteService
    {
        List<EvernoteNodeMetadataDecorator> GetNotesMetaList(string searchString);
        Note GetNote(string guid);
        Resource GetResource(string resourceGuid);
    }
}

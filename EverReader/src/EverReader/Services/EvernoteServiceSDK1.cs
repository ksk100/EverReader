using EverReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evernote.EDAM.NoteStore;
using Thrift.Transport;
using Thrift.Protocol;
using Evernote.EDAM.Type;

namespace EverReader.Services
{
    public class EvernoteServiceSDK1 : IEvernoteService
    {
        private NoteStore.Client noteStore;
        private EFDbEvernoteCredentials credentials;

        public EvernoteServiceSDK1(EFDbEvernoteCredentials credentials)
        {
            this.credentials = credentials;

            THttpClient noteStoreTransport = new THttpClient(new Uri(credentials.NotebookUrl));
            TBinaryProtocol noteStoreProtocol = new TBinaryProtocol(noteStoreTransport);
            noteStore = new NoteStore.Client(noteStoreProtocol);
        }

        public List<EvernoteNodeMetadataDecorator> GetNotesMetaList(string searchString)
        {
            NoteFilter noteFilter = new NoteFilter();
            noteFilter.Words = searchString;

            NotesMetadataResultSpec resultsSpec = new NotesMetadataResultSpec();
            resultsSpec.IncludeTitle = true;
            resultsSpec.IncludeCreated = true;
            resultsSpec.IncludeNotebookGuid = true;
            resultsSpec.IncludeUpdated = true;
            resultsSpec.IncludeAttributes = true;
            resultsSpec.IncludeTagGuids = true;
            resultsSpec.IncludeContentLength = true;

            NotesMetadataList noteMetadataList = noteStore.findNotesMetadata(credentials.AuthToken, noteFilter, 0, 100, resultsSpec);

            List<EvernoteNodeMetadataDecorator> notesMetaWrapperList = noteMetadataList.Notes.ConvertAll(noteMeta => new EvernoteNodeMetadataDecorator(noteMeta));

            return notesMetaWrapperList;
        }

        public Note GetNote(string guid)
        {           
            return noteStore.getNote(credentials.AuthToken, guid, true, false, false, false);
        }
    }
}

using EverReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evernote.EDAM.NoteStore;
using Thrift.Transport;
using Thrift.Protocol;
using Evernote.EDAM.Type;
using Evernote.EDAM.Error;

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

        public List<INoteMetadata> GetNotesMetaList(string searchString)
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

            NotesMetadataList noteMetadataList;

            try {
                noteMetadataList = noteStore.findNotesMetadata(credentials.AuthToken, noteFilter, 0, 100, resultsSpec);
            }
            catch (EDAMUserException)
            {
                throw new EvernoteServiceSDK1AuthorisationException();
            }

            List<ENNoteMetadataINoteMetadataAdapter> notesMetaWrapperList = 
                noteMetadataList.Notes.ConvertAll(noteMeta => new ENNoteMetadataINoteMetadataAdapter(noteMeta));

            return notesMetaWrapperList.ToList<INoteMetadata>();
        }

        public Note GetNote(string guid)
        {
            Note note;

            try
            {
                note = noteStore.getNote(credentials.AuthToken, guid, true, false, false, false);
            }
            catch (EDAMUserException)
            {
                throw new EvernoteServiceSDK1AuthorisationException();
            }
            catch (EDAMNotFoundException)
            {
                throw new EvernoteServiceSDK1NoteNotFoundException();
            }
            return note;
        }

        public Resource GetResource(string resourceGuid)
        {
            Resource resource;

            try
            {
                resource = noteStore.getResource(credentials.AuthToken, resourceGuid, true, false, false, false);
            }
            catch (EDAMUserException)
            {
                throw new EvernoteServiceSDK1AuthorisationException();
            }
            catch (EDAMNotFoundException)
            {
                throw new EvernoteServiceSDK1NoteNotFoundException();
            }
            return resource;
        }

        public Tag GetTag(string tagGuid)
        {
            Tag tag;

            try
            {
                tag = noteStore.getTag(credentials.AuthToken, tagGuid);
            }
            catch (EDAMUserException)
            {
                throw new EvernoteServiceSDK1AuthorisationException();
            }
            catch (EDAMNotFoundException)
            {
                throw new EvernoteServiceSDK1NoteNotFoundException();
            }

            return tag;
        }
    }
}

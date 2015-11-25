using Evernote.EDAM.Type;
using System.Collections.Generic;

namespace EverReader.Models
{
    public class ENNoteINoteMetadataAdapter : INoteMetadata
    {
        private Note _enNote;

        public List<string> TagNames { get; set; }
        public List<string> TagGuids { get { return _enNote.TagGuids; } }

        public string Title { get { return _enNote.Title; } }
        public string Guid { get { return _enNote.Guid; } }
        public int ContentLength { get { return _enNote.ContentLength; } }
        public long Created { get { return _enNote.Created; } }
        public long Updated { get { return _enNote.Updated; } }

        public string Source { get { return _enNote.Attributes.Source; } }
        public string SourceUrl { get { return _enNote.Attributes.SourceURL; } }
        public string SourceApplication { get { return _enNote.Attributes.SourceApplication; } }

        public ENNoteINoteMetadataAdapter(Note enNote)
        {
            _enNote = enNote;
        }
    }
}

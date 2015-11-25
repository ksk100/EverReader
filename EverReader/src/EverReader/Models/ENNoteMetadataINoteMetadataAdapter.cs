using Evernote.EDAM.NoteStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EverReader.Models
{
    public class ENNoteMetadataINoteMetadataAdapter : INoteMetadata
    {
        private NoteMetadata _enNoteMetadata;

        public List<string> TagNames { get; set; }
        public List<string> TagGuids { get { return _enNoteMetadata.TagGuids; } }

        public string Title { get { return _enNoteMetadata.Title; } }
        public string Guid { get { return _enNoteMetadata.Guid; } }
        public int ContentLength { get { return _enNoteMetadata.ContentLength; } }
        public long Created { get { return _enNoteMetadata.Created; } }
        public long Updated { get { return _enNoteMetadata.Updated; } }

        public string Source { get { return _enNoteMetadata.Attributes.Source; } }
        public string SourceUrl { get { return _enNoteMetadata.Attributes.SourceURL; } }
        public string SourceApplication { get { return _enNoteMetadata.Attributes.SourceApplication; } }

        public ENNoteMetadataINoteMetadataAdapter(NoteMetadata enNoteMetadata)
        {
            _enNoteMetadata = enNoteMetadata;
        }
    }
}

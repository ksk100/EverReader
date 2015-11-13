using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evernote.EDAM.NoteStore;

namespace EverReader.Models
{
    public class EvernoteNodeMetadataDecorator
    {
        public NoteMetadata BaseNoteMetadata { get; set; }

        public string Title
        {
            get
            {
                return BaseNoteMetadata.Title;
            }
        }

        public DateTime CreatedDateTime
        {
            get
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return epoch.AddMilliseconds(BaseNoteMetadata.Created);
            }
        }

        public DateTime UpdatedDateTime
        {
            get
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return epoch.AddMilliseconds(BaseNoteMetadata.Updated);
            }
        }

        public EvernoteNodeMetadataDecorator(NoteMetadata noteMetadata)
        {
            BaseNoteMetadata = noteMetadata;
        }
    }
}

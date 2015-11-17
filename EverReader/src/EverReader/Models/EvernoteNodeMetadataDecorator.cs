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

        public string Source
        {
            get
            {
                if (String.IsNullOrWhiteSpace(BaseNoteMetadata.Attributes.Source))
                {
                    return null;
                } else
                {
                    switch (BaseNoteMetadata.Attributes.Source)
                    {
                        case "mobile.android":
                            return "Android";
                        case "mail.clip":
                            return "Clipped from email";
                        case "mail.smtp":
                            return "Emailed in";
                        case "web.clip":
                            return "Web clipping";
                        default:
                            return BaseNoteMetadata.Attributes.Source;
                    }
                }
            }
        }

        public string ContentLength
        {
            get
            {
                if (BaseNoteMetadata.ContentLength > (1024 * 1024))
                {
                    decimal sizeInMb = (decimal)BaseNoteMetadata.ContentLength / (1024 * 1024);
                    return sizeInMb.ToString(".#") + " Mb";
                }
                if (BaseNoteMetadata.ContentLength > 1024 * 2)
                {
                    int sizeInKb = BaseNoteMetadata.ContentLength / 1024;
                    return sizeInKb + " Kb";
                }
                return BaseNoteMetadata.ContentLength + " bytes";
            }
        }

        public EvernoteNodeMetadataDecorator(NoteMetadata noteMetadata)
        {
            BaseNoteMetadata = noteMetadata;
        }
    }
}

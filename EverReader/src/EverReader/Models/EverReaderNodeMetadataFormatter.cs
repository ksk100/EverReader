using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evernote.EDAM.NoteStore;

namespace EverReader.Models
{
    public class EverReaderNodeMetadataFormatter
    {
        public INoteMetadata BaseNoteMetadata { get; set; }

        public string Title { get { return BaseNoteMetadata.Title; } }
        public string Guid { get { return BaseNoteMetadata.Guid; } }
        public string SourceUrl { get { return BaseNoteMetadata.SourceUrl; } }
        public string SourceApplication { get { return BaseNoteMetadata.SourceApplication; } }
        public List<string> TagNames { get { return BaseNoteMetadata.TagNames; } set { BaseNoteMetadata.TagNames = value; } }
        public List<string> TagGuids { get { return BaseNoteMetadata.TagGuids; } }

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
                if (String.IsNullOrWhiteSpace(BaseNoteMetadata.Source))
                {
                    return null;
                } else
                {
                    switch (BaseNoteMetadata.Source)
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
                            return BaseNoteMetadata.Source;
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

        public EverReaderNodeMetadataFormatter(INoteMetadata noteMetadata)
        {
            BaseNoteMetadata = noteMetadata;
        }
    }
}

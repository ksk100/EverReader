using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EverReader.Models
{
    public class Bookmark
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string NoteGuid { get; set; }

        public decimal PercentageRead { get; set; }

        public BookmarkType Type { get; set; }

        public string BookmarkTitle { get; set; }

        public DateTime Updated { get; set; }

        public string NoteTitle { get; set; }

        public DateTime NoteCreated { get; set; }

        public DateTime NoteUpdated { get; set; }

        public int NoteLength { get; set; }

        public string NoteLengthPretty
        {
            get
            {
                if (NoteLength > (1024 * 1024))
                {
                    decimal sizeInMb = (decimal)NoteLength / (1024 * 1024);
                    return sizeInMb.ToString(".#") + " Mb";
                }
                if (NoteLength > 1024 * 2)
                {
                    int sizeInKb = NoteLength / 1024;
                    return sizeInKb + " Kb";
                }
                return NoteLength + " bytes";
            }
        }
    }
}

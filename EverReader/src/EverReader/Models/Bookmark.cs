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

        public DateTime Updated { get; set; }

        public string NoteTitle { get; set; }

        public DateTime NoteCreated { get; set; }

        public DateTime NoteUpdated { get; set; }
    }
}

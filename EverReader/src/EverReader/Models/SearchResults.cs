using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EverReader.Models
{
    public class SearchResults : ISearchResults
    {
        public List<INoteMetadata> NotesMetadata { get; set; }
        public int TotalResults { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EverReader.Models
{
    public interface ISearchResults
    {
        List<INoteMetadata> NotesMetadata { get; set; }
        int TotalResults { get; set; }
    }
}

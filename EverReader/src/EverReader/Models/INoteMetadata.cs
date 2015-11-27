using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EverReader.Models
{
    public interface INoteMetadata
    {
        List<string> TagNames { get; set; }
        List<string> TagGuids { get; }
        string Guid { get; }
        string Title { get; }
        long Created { get; }
        long Updated { get; }
        string Source { get; }
        string SourceUrl { get; }
        string SourceApplication { get; }
        int ContentLength { get; }
        string NotebookGuid { get; }
        string NotebookName { get; set; }
    }
}

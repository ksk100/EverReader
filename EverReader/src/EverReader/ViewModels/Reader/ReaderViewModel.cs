using EverReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EverReader.ViewModels.Reader
{
    public class ReaderViewModel
    {
        public EverReaderNodeMetadataFormatter FormattedNoteMetadata { get; set; }
        public string Content { get; set; }
        public decimal PercentageRead { get; set; }
    }
}

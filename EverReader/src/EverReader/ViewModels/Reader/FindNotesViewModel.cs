using EverReader.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace EverReader.ViewModels.Reader
{
    public class FindNotesViewModel
    {
        [Display(Name="Search")]
        [Required(ErrorMessage = "Please enter a search string")]
        public string SearchField { get; set; }

        [Display(Name="Exclude short notes?")]
        public bool ExcludeShortNotes { get; set; } = true;

        public List<EverReaderNodeMetadataFormatter> SearchResults { get; set; }
    }
}

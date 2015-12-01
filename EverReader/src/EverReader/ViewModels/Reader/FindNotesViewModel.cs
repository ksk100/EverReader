using Evernote.EDAM.Type;
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

        [Display(Name="Sort order")]
        public int SortOrder { get; set; }

        public List<NoteSortOrderFormatted> SortOrderOptions { get; set; }

        [Display(Name = "Sort ascending?")]
        public bool SortAscending { get; set; }

        [Display(Name="Exclude short notes?")]
        public bool ExcludeShortNotes { get; set; } = true;

        public List<EverReaderNodeMetadataFormatter> SearchResults { get; set; }

        public int NumberUnfilteredResults { get; set; }

        public int TotalResultsForSearch { get; set; }

        public FindNotesViewModel()
        {
            SortOrderOptions = new List<NoteSortOrderFormatted>()
            {
                new NoteSortOrderFormatted () { Name = "Created", Value = 1 },
                new NoteSortOrderFormatted () { Name = "Updated", Value = 2 },
                new NoteSortOrderFormatted () { Name = "Relevance", Value = 3 },
                new NoteSortOrderFormatted () { Name = "Title", Value = 5 }
            };
        }

        public class NoteSortOrderFormatted
        {
            public int Value { get; set; }
            public string Name { get; set; }
            public bool Checked {get; set; }
        }

    }
}

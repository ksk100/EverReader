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
        public string SearchField { get; set; }

        public List<EvernoteNodeMetadataDecorator> SearchResults { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EverReader.ViewModels.Manage
{
    public class EvernoteCallbackViewModel
    {
        public bool UserAuthorisedEvernote { get; set; }

        public string SuccessMessage { get; set; }

        public string ErrorMessage { get; set; }
    }
}

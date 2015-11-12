using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using EverReader.ViewModels.Reader;
using Microsoft.AspNet.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace EverReader.Controllers
{
    [Authorize]
    public class ReaderController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View("FindNotes");
        }

        [HttpPost]
        public IActionResult Index(FindNotesViewModel findNotesViewModel)
        {
            return View("FindNotes", findNotesViewModel);
        }
    }
}

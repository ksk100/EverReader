using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using AsyncOAuth.Evernote.Simple;

namespace EverReader.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public bool HasAuthorisedEvernote { get; set; }

        public DateTime? EvernoteAuthorisedUntilDate { get; set; }

        public EvernoteCredentials EvernoteCredentials { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using AsyncOAuth.Evernote.Simple;

namespace EverReader.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public bool HasAuthorisedEvernote { get; set; } = false;

        public int EvernoteCredentialsId { get; set; } = -1;

        public virtual EFDbEvernoteCredentials EvernoteCredentials { get; set; }
    }
}

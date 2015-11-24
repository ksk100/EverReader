using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;

namespace EverReader.Models
{
    public class EverReaderContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<EFDbEvernoteCredentials> EvernoteCredentials { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<PrivateBetaUser> PrivateBetaUsers { get; set; }
        public DbSet<TagData> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
